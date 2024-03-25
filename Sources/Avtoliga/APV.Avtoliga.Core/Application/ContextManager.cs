using System;
using System.Configuration;
using System.Threading;
using APV.Avtoliga.Core.BusinessLogic;
using APV.EntityFramework;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Application
{
    public class ContextManager : IContextManager
    {
        public const int ConnectionTimeoutInSec = 3;

        public const int StatisticsUpdateTimeoutInSeconds = 5;

        public static readonly string DefaultConnectionString =
            string.Format(
                @"Data Source=(local)\SQL2012;Initial Catalog=Avtoliga;Integrated Security=True;Pooling=True;Min Pool Size=3;Connection Timeout={0};",
                ConnectionTimeoutInSec);

        private static string _connectionString;

        private const long InstanceId = 1;

        private static readonly object Lock = new object();

        private static readonly UserContext StaticContext = new UserContext();

        static ContextManager()
        {
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
            lock (Lock)
            {
                if (_connectionString == null)
                {
                    _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                    if (string.IsNullOrWhiteSpace(_connectionString))
                    {
                        _connectionString = DefaultConnectionString;
                    }
                }
                return _connectionString;
            }
        }

        int IContextManager.GetStatisticsUpdateTimeoutInSeconds()
        {
            throw new NotSupportedException();
        }

        public static void LoginAsAdmin()
        {
            var contextManager = (ContextManager) EntityFrameworkManager.GetContextManager();
            IContext context = contextManager.GetContext();
            var user = new Admin();
            context.Login(user);
        }

        public static bool Login(string username, string password)
        {
            return UserManagement.Instance.Login(username, password);
        }

        public static void Logout()
        {
            IContext context = Instance.GetContext();
            context.Logout();
        }

        public static IUser GetUser()
        {
            IContext context = Instance.GetContext();
            return context.User;
        }

        public static bool IsAdmin
        {
            get
            {
                IUser user = GetUser();
                return (user != null) && (user.UserRole == UserRole.Administrator);
            }
        }

        public static readonly ContextManager Instance = (ContextManager) EntityFrameworkManager.GetContextManager();
    }
}