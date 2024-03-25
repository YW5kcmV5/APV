using System;
using System.Collections.Generic;
using System.IO;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Export.ConsoleApplication
{
    public class CountryExporter : BaseExporter
    {
        public const string TagName = "Name";
        public const string TagLegalName = "LegalName";
        public const string TagAlternativeName = "AlternativeName";
        public const string TagCode = "Code";
        public const string TagLanguage = "Language";

        public const string InfoFileName = "Info.txt";
        public const string Title = "Страна";

        public static readonly string[] Tags = new[] { TagName, TagLegalName, TagAlternativeName, TagCode, TagLanguage };

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
                string languageName = tags[TagLanguage];

                LanguageEntity language = LanguageManagement.Instance.GetByName(languageName);
                DataImageEntity logoImage = GetLogo(folder);
                DataImageEntity iconImage = (logoImage != null) ? ImageManagement.Instance.ResizeHeight(logoImage, Constants.IconSize) : null;

                CountryEntity country = CountryManagement.Instance.FindByName(name) ?? new CountryEntity();

                country.Name = name;
                country.LegalName = tags[TagLegalName];
                country.AlternativeName = tags[TagAlternativeName];
                country.Code = tags[TagCode];
                country.Language = language;
                country.LogoImage = logoImage;
                country.IconImage = iconImage;

                country.Save();
                WordManagement.Instance.SaveName(country);

                Console.WriteLine("\tСтрана \"{0}\" (\"{1}\") успешно добавлена.", country.Name, folderName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t[ERROR]Ошибка при экспорте страны из папки \"{0}\", ошибка \"{1}\".", folderName, ex.Message);
                return false;
            }
        }
    }
}