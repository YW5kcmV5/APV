using System;
using System.Collections.Generic;
using System.IO;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Export.ConsoleApplication
{
    public class CompanyExporter : BaseExporter
    {
        public const string TagName = "Name";
        public const string TagLegalName = "Legal name";
        public const string TagCompanyName = "Company name";
        public const string TagDescription = "Description";
        public const string TagCountry = "Country";
        public const string TagBusinessType = "Business type";
        public const string TagAddress = "Address";
        public const string TagLegalAddress = "Legal address";

        public const string InfoFileName = "Info.txt";
        public const string Title = "Компания";

        public static readonly string[] Tags = new[] { TagName, TagLegalName, TagCompanyName, TagDescription, TagCountry, TagBusinessType, TagAddress, TagLegalAddress };

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
                string businessTypeName = tags[TagBusinessType] ?? Constants.BusinessTypeOOO;
                string address = tags[TagAddress];
                string legalAddress = tags[TagLegalAddress];

                CountryEntity country = CountryManagement.Instance.GetByName(countryName);
                BusinessTypeEntity businessType = BusinessTypeManagement.Instance.GetByName(businessTypeName);
                AddressEntity addressLocation = (!string.IsNullOrEmpty(address)) ? AddressManagement.Instance.Create(address) : null;
                AddressEntity legalAddressLocation = (!string.IsNullOrEmpty(legalAddress)) ? AddressManagement.Instance.Create(legalAddress) : null;
                DataImageEntity logo = GetLogo(folder);
                DataImageEntity icon = GetIcon(folder);

                CompanyEntity company = CompanyManagement.Instance.FindByName(name) ?? new CompanyEntity();

                company.Name = name;
                company.LegalName = tags[TagLegalName];
                company.CompanyName = tags[TagCompanyName];
                company.Description = tags[TagDescription];
                company.Country = country;
                company.BusinessType = businessType;
                company.Address = addressLocation;
                company.LegalAddress = legalAddressLocation;
                company.LogoImage = logo;
                company.IconImage = icon;

                company.Save();
                WordManagement.Instance.SaveName(company);

                Console.WriteLine("\tКомпания \"{0}\" (\"{1}\") успешно добавлена.", company.Name, folderName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t[ERROR]Ошибка при экспорте компании из папки \"{0}\", ошибка \"{1}\".", folderName, ex.Message);
                return false;
            }
        }
    }
}