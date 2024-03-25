using System.Text;

namespace APV.Common
{
    public static class SystemConstants
    {
        /// <summary>
        /// "APV.System"
        /// </summary>
        public const string Namespace = @"APV.System";

        /// <summary>
        /// "APV.System.Data"
        /// </summary>
        //public const string NamespaceData = @"APV.System.Data";
        public const string NamespaceData = @"Pottle.Data";

        /// <summary>
        /// "APV.System.Service"
        /// </summary>
        public const string NamespaceService = @"APV.System.Service";

        /// <summary>
        /// "RU"
        /// </summary>
        public const string LanguageCodeRussian = "RU";

        /// <summary>
        /// "EN"
        /// </summary>
        public const string LanguageCodeEnglish = "EN";

        /// <summary>
        /// "RU"
        /// </summary>
        public const string DefaultLanguageCode = LanguageCodeRussian;

        /// <summary>
        /// "RU"
        /// </summary>
        public const string DefaultCountryCode = "RU";

        /// <summary>
        /// "Русский"
        /// </summary>
        public const string LanguageNameRussian = "Русский";

        /// <summary>
        /// "Россия"
        /// </summary>
        public const string CountryNameRussia = "Россия";

        /// <summary>
        /// "Германия"
        /// </summary>
        public const string CountryNameGermany = "Германия";

        /// <summary>
        /// "256"
        /// </summary>
        public const int MaxNameLength = 256;

        /// <summary>
        /// "1000"
        /// </summary>
        public const int MaxUrlLength = 1000;

        /// <summary>
        /// "1000"
        /// </summary>
        public const int MaxDescriptionLength = 1000;

        /// <summary>
        /// "4000"
        /// </summary>
        public const int MaxStringLength = 4000;

        /// <summary>
        /// "0"
        /// </summary>
        public const long UnknownId = 0;

        /// <summary>
        /// Universal sortable date/time pattern.
        /// </summary>
        public const string DateTimeFormat = @"u";

        /// <summary>
        /// "Unicode" (UTF-16)
        /// </summary>
        public static readonly Encoding SqlXmlEncoding = Encoding.Unicode;

    }
}