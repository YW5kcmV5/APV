using System;
using System.Collections.Generic;
using System.IO;
using APV.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Export.ConsoleApplication
{
    public class SupplierExporter : BaseExporter
    {
        public const string TagName = "Name";
        public const string TagShortName = "ShortName";
        public const string TagDescription = "Description";
        public const string TagUrl = "Url";
        public const string TagCountry = "Country";
        public const string TagCompany = "Company";

        public const string InfoFileName = "Info.txt";
        public const string Title = "Поставщик";

        public static readonly string[] Tags = new[] { TagName, TagShortName, TagDescription, TagUrl, TagCountry, TagCompany };

        public override string[] GetTags()
        {
            return Tags;
        }

        public override string GetTitle()
        {
            return Title;
        }

        public override bool ExportEntity(string folder)
        {
            string folderName = Path.GetFileName(folder);
            try
            {
                string infoFileName = Path.Combine(folder, InfoFileName);
                SortedList<string, string> tags = ParseTags(infoFileName);

                string name = tags[TagName];
                string countryName = tags[TagCountry] ?? SystemConstants.CountryNameRussia;
                string companyName = tags[TagCompany];
                string url = tags[TagUrl];

                CountryEntity country = CountryManagement.Instance.GetByName(countryName);
                CompanyEntity company = (!string.IsNullOrEmpty(companyName)) ? CompanyManagement.Instance.FindByAnyName(companyName) : null;
                UrlEntity urlEntity = (!string.IsNullOrEmpty(url)) ? UrlManagement.Instance.Create(url) : null;
                DataImageEntity logo = GetLogo(folder);
                DataImageEntity icon = GetIcon(folder);

                SupplierEntity supplier = SupplierManagement.Instance.FindByName(name) ?? new SupplierEntity();

                supplier.Name = name;
                supplier.ShortName = tags[TagShortName];
                supplier.Description = tags[TagDescription];
                supplier.Country = country;
                supplier.Company = company;
                supplier.Url = urlEntity;
                supplier.LogoImage = logo;
                supplier.IconImage = icon;

                supplier.Save();
                WordManagement.Instance.SaveName(supplier);

                Console.WriteLine("\tПоставщик \"{0}\" (\"{1}\") успешно добавлена.", supplier.Name, folderName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t[ERROR]Ошибка при экспорте поставщика из папки \"{0}\", ошибка \"{1}\".", folderName, ex.Message);
                return false;
            }
        }
    }
}