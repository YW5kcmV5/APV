using System;
using System.Reflection;
using APV.EntityFramework;
using APV.Pottle.Toolkit.Linguistics.Interfaces;
using APV.Pottle.Toolkit.Linguistics.KeywordManagers.Solarix;
using APV.Pottle.Toolkit.Linguistics.TranslitManagers;
using APV.Pottle.Toolkit.Navigation.GeoLocators.Google;
using APV.Pottle.Toolkit.Navigation.IPLocators.IpApi;
using APV.Pottle.Toolkit.Navigation.Interfaces;

namespace APV.Pottle.Common.Application
{
    public static class ApplicationManager
    {
        private static IGeoLocator _geoLocator;
        private static IPLocator _ipLocator;
        private static IKeywordManager _keywordManager;
        private static ITranslitManager _translitManager; 

        public static IGeoLocator GetGeoLocator()
        {
            return _geoLocator ?? (_geoLocator = new GoogleGeoLocator());
        }

        public static IIPLocator GetIPLocator()
        {
            return _ipLocator ?? (_ipLocator = new IPLocator());
        }

        public static ITranslitManager GetTranslitManager()
        {
            return _translitManager ?? (_translitManager = new TranslitManager());
        }

        public static IKeywordManager GetKeywordManager()
        {
            return _keywordManager ?? (_keywordManager = new SolarixKeywordManager());
        }

        public static void Register(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            if (instance is IGeoLocator)
            {
                _geoLocator = (IGeoLocator) instance;
            }
            else if (instance is IPLocator)
            {
                _ipLocator = (IPLocator) instance;
            }
            else if (instance is IKeywordManager)
            {
                _keywordManager = (IKeywordManager) instance;
            }
            else if (instance is ITranslitManager)
            {
                _translitManager = (ITranslitManager) instance;
            }
            else if (instance is Assembly)
            {
                EntityFrameworkManager.Register((Assembly) instance);
            }
            else throw new ArgumentOutOfRangeException(string.Format("instance cannot be registered. Unknown instance type \"{0}\".", instance.GetType().FullName));
        }
    }
}