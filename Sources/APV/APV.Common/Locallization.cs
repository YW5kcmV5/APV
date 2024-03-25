using System.Globalization;
using System.Threading;

namespace APV.Common
{
    public static class Locallization
    {
        public static string LanguageCode
        {
            get
            {
                CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
                string languageCode = culture.TwoLetterISOLanguageName.ToUpperInvariant();
                return languageCode;
            }
        }
    }
}