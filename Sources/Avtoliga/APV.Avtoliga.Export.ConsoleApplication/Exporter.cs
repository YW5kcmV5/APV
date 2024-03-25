using System;
using System.IO;
using APV.Common;
using APV.Pottle.WebParsers.Avtoberg;

namespace APV.Avtoliga.Export.ConsoleApplication
{
    public static class Exporter
    {
        public static void ExportTrademarks(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");
            if (!Directory.Exists(folder))
                throw new ArgumentOutOfRangeException(string.Format("Directory \"{0}\" does not exist.", folder));

            try
            {
                string[] files = Directory.GetFiles(folder, "*.xml");
                int length = files.Length;
                int success = 0;
                int errors = 0;
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine("{0}/{1}/{2} ({3:00.00}%)", success, errors, length, (100.0 * i / length));
                    string filename = files[i];
                    try
                    {
                        var trademarkInfo = Serializer.DeserializeFromFile<AvtobergTrademarkInfo>(filename, Serializer.Type.DataContractSerializer);
                        new TrademarkExporter().Export(trademarkInfo);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToTraceString());
                        errors++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Exception!");
                Console.WriteLine(ex.ToTraceString());
            }
        }

        public static void ExportProducts(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");
            if (!Directory.Exists(folder))
                throw new ArgumentOutOfRangeException(string.Format("Directory \"{0}\" does not exist.", folder));

            try
            {
                Console.WriteLine("Export products.");
                var exporter = new ProductExporter();
                string[] files = Directory.GetFiles(folder, "*.xml");
                int length = files.Length;
                int success = 0;
                int errors = 0;
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine("{0}/{1}/{2} ({3:00.00}%)", success, errors, length, (100.0 * i / length));
                    string filename = files[i];
                    try
                    {
                        var productInfo = Serializer.DeserializeFromFile<AvtobergSupplierProductInfo>(filename, Serializer.Type.DataContractSerializer);
                        exporter.Export(productInfo);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToTraceString());
                        errors++;
                    }
                }

                Console.WriteLine("Export buy together product references.");
                length = files.Length;
                success = 0;
                errors = 0;
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine("{0}/{1}/{2} ({3:00.00}%)", success, errors, length, (100.0 * i / length));
                    string filename = files[i];
                    try
                    {
                        var productInfo = Serializer.DeserializeFromFile<AvtobergSupplierProductInfo>(filename, Serializer.Type.DataContractSerializer);
                        exporter.ExportBuyTogetherProductReferences(productInfo);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToTraceString());
                        errors++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Exception!");
                Console.WriteLine(ex.ToTraceString());
            }
        }
    }
}
