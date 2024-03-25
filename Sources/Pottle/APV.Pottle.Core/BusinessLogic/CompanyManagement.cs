using System;
using System.Linq;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class CompanyManagement : BaseManagement<CompanyEntity, CompanyCollection, CompanyDataLayerManager>
    {
        private static string[] _businessTypePrefixes;

        private static string[] GetBusinessTypePrefixes()
        {
            if (_businessTypePrefixes == null)
            {
                BusinessTypeCollection types = BusinessTypeManagement.Instance.GetAll();
                _businessTypePrefixes = types.ToArray().Select(x => x.Name).ToArray();
            }
            return _businessTypePrefixes;
        }

        [AnonymousAccess]
        public string FormatName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            name = name.Trim();
            while (name.Contains("  "))
            {
                name = name.Replace("  ", " ");
            }
            name = name.Replace("'", "\"");

            string[] prefixes = GetBusinessTypePrefixes();
            string prefix = prefixes.FirstOrDefault(
                item => (name.StartsWith(item + " ", StringComparison.InvariantCultureIgnoreCase)) ||
                        (name.StartsWith(item + "\"", StringComparison.InvariantCultureIgnoreCase)));

            string nameWithoutPrefix = name;
            if (prefix != null)
            {
                nameWithoutPrefix = name.Substring(prefix.Length);
                nameWithoutPrefix = nameWithoutPrefix.Trim();
            }
            else
            {
                prefix = string.Empty;
            }

            if ((nameWithoutPrefix.StartsWith("\"")) && (nameWithoutPrefix.EndsWith("\"")))
            {
                nameWithoutPrefix = nameWithoutPrefix.Substring(1, nameWithoutPrefix.Length - 2);
            }
            else if (nameWithoutPrefix.StartsWith("\""))
            {
                nameWithoutPrefix = nameWithoutPrefix.Substring(1, nameWithoutPrefix.Length - 1);
            }

            name = (prefix + " " + nameWithoutPrefix).Trim();
            return name;
        }

        [ClientAccess]
        public CompanyEntity FindByAnyName(string name)
        {
            name = FormatName(name);
            return DatabaseManager.FindByAnyName(name);
        }

        public static readonly CompanyManagement Instance = (CompanyManagement)EntityFrameworkManager.GetManagement<CompanyEntity>();
    }
}