using System;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.TransControl.Core.Application
{
    public class ContextManager : IContextManager
    {
        public const int ConnectionTimeoutInSec = 10;

        public const int StatisticsUpdateTimeoutInSeconds = 5;

        //public static readonly string DefaultConnectionString = string.Format(@"Direct=true;Host=192.168.58.24;Port=1521;SID=tran;uid=synch_wl;Password=123;Unicode=True;Connection Timeout={0};Provider=Oracle;", ConnectionTimeoutInSec);
        //public static readonly string DefaultConnectionString = string.Format(@"Direct=true;Host=192.168.58.24;Port=1521;SID=tran;uid=monuser;Password=monuser9;Unicode=True;Connection Timeout={0};Provider=Oracle;", ConnectionTimeoutInSec);

        private static string _connectionString;

        private static ConnectionSettings _connectionSettings = new ConnectionSettings();

        private const long InstanceId = 1;

        private static readonly object Lock = new object();

        private static readonly AnonymousUserContext StaticContext = new AnonymousUserContext();

        public static ConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _connectionSettings = value;
                _connectionString = value.ConnectionString;
            }
        }

        static ContextManager()
        {
        }

        public IContext GetContext()
        {
            lock (Lock)
            {
                return StaticContext;
            }
        }

        public SqlProviderSettings GetSqlSettings()
        {
            return new SqlProviderSettings
                {
                    SqlCommandSettings = string.Empty,
                };
        }

        public long GetInstanceId()
        {
            return InstanceId;
        }

        public string GetConnectionString()
        {
            lock (Lock)
            {
                return _connectionString ?? (_connectionString = ConnectionSettings.ConnectionString);
            }
        }

        int IContextManager.GetStatisticsUpdateTimeoutInSeconds()
        {
            throw new NotSupportedException();
        }
    }
}