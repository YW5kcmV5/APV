using System;
using System.Collections.Generic;
using System.IO;
using APV.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Export.ConsoleApplication
{
    public class TrademarkExporter : BaseExporter
    {
        public const string TagName = "Name";
        public const string TagDescription = "Description";
        public const string TagUrl = "Url";
        public const string TagCountry = "Country";
        public const string TagCompany = "Company";

        public const string InfoFileName = "Info.txt";
        public const string Title = "Торговая марка";

        public static readonly string[] Tags = new[] { TagName, TagDescription, TagUrl, TagCountry, TagCompany };

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

                TrademarkEntity trademark = TrademarkManagement.Instance.FindByName(name) ?? new TrademarkEntity();

                trademark.Name = name;
                trademark.Description = tags[TagDescription];
                trademark.Country = country;
                trademark.Company = company;
                trademark.Url = urlEntity;
                trademark.LogoImage = logo;
                trademark.IconImage = icon;

                trademark.Save();
                WordManagement.Instance.SaveName(trademark);

                Console.WriteLine("\tТорговая марка \"{0}\" (\"{1}\") успешно добавлена.", trademark.Name, folderName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t[ERROR]Ошибка при экспорте торговой марки из папки \"{0}\", ошибка \"{1}\".", folderName, ex.Message);
                return false;
            }
        }
    }
}