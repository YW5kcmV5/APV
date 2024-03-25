using System;
using System.Threading;
using System.Web;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.BusinessLogic.Statistics;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Core.Application
{
    public class ContextManager : IContextManager
    {
        public const int ConnectionTimeoutInSec = 3;

        public const int StatisticsUpdateTimeoutInSeconds = 5;

        public static readonly string ConnectionString = string.Format(@"Data Source=(local);Initial Catalog=Pottle;Integrated Security=True;Pooling=True;Min Pool Size=3;Connection Timeout={0};", ConnectionTimeoutInSec);

        private const long InstanceId = 1;

        private static readonly object Lock = new object();

        private static readonly UserContext StaticContext = new UserContext();

        private static LanguageEntity GetLanguage()
        {
            if ((HttpContext.Current != null) && (HttpContext.Current.Request.UserLanguages != null))
            {
                foreach (string languageCode in HttpContext.Current.Request.UserLanguages)
                {
                    LanguageEntity language = (!string.IsNullOrEmpty(languageCode))
                                                  ? LanguageManagement.Instance.FindByCode(languageCode)
                                                  : null;
                    if (language != null)
                    {
                        return language;
                    }
                }
            }
            return LanguageManagement.Default;
        }

        static ContextManager()
        {
            //Init static controllers
            StatisticsController.Init();
        }

        public int GetManagerId(Type managerType)
        {
            if (managerType == null)
                throw new ArgumentNullException("managerType");

            return 0;
        }

        public int GetOperationId(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException("methodName");

            return 0;
        }

        public IContext GetContext()
        {
            lock (Lock)
            {
                var context = Thread.CurrentPrincipal as IContext;
                if (context == null)
                {
                    context = (SessionManager.SupportsSession) ? new UserContext() : StaticContext;
                    Thread.CurrentPrincipal = context;
                }
                return context;
            }
        }

        public SqlProviderSettings GetSqlSettings()
        {
            return SqlProviderSettings.Default;
        }

        public long GetInstanceId()
        {
            return InstanceId;
        }

        public string GetConnectionString()
        {
            return ConnectionString;
        }

        public int GetStatisticsUpdateTimeoutInSeconds()
        {
            return StatisticsUpdateTimeoutInSeconds;
        }
    }
}