using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class PriceController
    {
        private class InternalRow
        {
            public string Producer { get; set; }

            public string Trademark { get; set; }

            public string Model { get; set; }

            public string ModelPeriod { get; set; }

            public string ProductPeriod { get; set; }

            public string Name { get; set; }

            public string Article { get; set; }

            public string OutOfStock { get; set; }

            public double Cost { get; set; }

            public int DeliveryTime { get; set; }

            public static string ToHeaderString()
            {
                return "Производитель\tМарка\tМодель\tПериод\tПериод товара\tИмя\tАртикул\tНаличие\tЦена\tДоставка";
            }

            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8:### ###.##} р.\t{9}", Producer, Trademark, Model, ModelPeriod, ProductPeriod, Name, Article, OutOfStock, Cost, DeliveryTime);
            }
        }

        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
        {
            // Add a new worksheet part to the workbook.
            var newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Any())
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Append the new worksheet and associate it with the workbook.
            var sheet = new Sheet { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }

        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Count(r => r.RowIndex == rowIndex) != 0)
            {
                row = sheetData.Elements<Row>().First(r => r.RowIndex == rowIndex);
            }
            else
            {
                row = new Row { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Any(c => c.CellReference.Value == columnName + rowIndex))
            {
                return row.Elements<Cell>().First(c => c.CellReference.Value == cellReference);
            }

            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            Cell refCell = row.Elements<Cell>().FirstOrDefault(cell => string.Compare(cell.CellReference.Value, cellReference, StringComparison.OrdinalIgnoreCase) > 0);
            var newCell = new Cell { CellReference = cellReference };
            row.InsertBefore(newCell, refCell);

            worksheet.Save();
            return newCell;
        }

        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        private static void InsertText(SpreadsheetDocument spreadSheet, WorksheetPart worksheetPart, string columnName, uint rowIndex, string text)
        {
            // Get the SharedStringTablePart. If it does not exist, create a new one.
            SharedStringTablePart shareStringPart =
                spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Any()
                    ? spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First()
                    : spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();

            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(text, shareStringPart);

            // Insert cell A1 into the new worksheet.
            Cell cell = InsertCellInWorksheet(columnName, rowIndex, worksheetPart);

            // Set the value of cell A1.
            cell.CellValue = new CellValue(index.ToString(CultureInfo.InvariantCulture));
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

            // Save the new worksheet.
            worksheetPart.Worksheet.Save();
        }

        private static byte[] CreateCsvData(List<InternalRow> rows)
        {
            using (var stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.Unicode))
                {
                    writer.WriteLine(InternalRow.ToHeaderString());
                    rows.ForEach(row => writer.WriteLine(row.ToString()));
                    writer.Flush();
                    byte[] data = stream.ToArray();
                    return data;
                }
            }
        }

        public static byte[] GetPriceContent()
        {
            ProductCollection products = ProductManagement.Instance.GetPriceList();
            //products = new ProductCollection(products.OfType<ProductEntity>().Take(1000));
            var rows = new List<InternalRow>();
            foreach (ProductEntity product in products)
            {
                var row = new InternalRow
                    {
                        Producer = (product.Producer != null) ? product.Producer.Name : string.Empty,
                        Trademark = ((product.Model != null) && (product.Model.Trademark != null)) ? product.Model.Trademark.Name : string.Empty,
                        Model = (product.Model != null) ? product.Model.Name : string.Empty,
                        ModelPeriod = (product.Model != null) ? product.Model.Period : string.Empty,
                        ProductPeriod = product.Period,
                        Name = product.Name,
                        Article = product.Article,
                        OutOfStock = product.OutOfStock ? "-" : "+",
                        Cost = product.Cost,
                        DeliveryTime = product.DeliveryTime,
                    };
                rows.Add(row);
            }

            //byte[] data = CreateWorkbook(rows);
            byte[] data = CreateCsvData(rows);
            return data;
        }
    }
}