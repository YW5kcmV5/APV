using System;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers
{
    public class DbSqlConnection : ISqlConnection
    {
        private bool _disposed;
        private readonly ISqlConnection _instance;

        #region ISqlConnection

        ISqlCommand ISqlConnection.CreateCommand()
        {
            return CreateCommand();
        }

        #endregion

        internal DbSqlConnection(ISqlConnection instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
        }

        public DbSqlConnection(string connectionString)
        {
            DatabaseType database = DbProvider.GetDatabaseType(ref connectionString);
            _instance = DbProvider.GetConnectionInstance(database);
            _instance.ConnectionString = connectionString;
        }

        public int ConnectionTimeout
        {
            get { return _instance.ConnectionTimeout; }
        }

        public void Open()
        {
            _instance.Open();
        }

        public void Close()
        {
            _instance.Close();
        }

        public DbSqlCommand CreateCommand()
        {
            ISqlCommand command = _instance.CreateCommand();
            return new DbSqlCommand(command);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _instance.Dispose();
            }
        }

        public string ConnectionString
        {
            get { return _instance.ConnectionString; }
            set { _instance.ConnectionString = value; }
        }

        public DatabaseType Database
        {
            get { return _instance.Database; }
        }

        public ISqlConnection Instance
        {
            get { return _instance; }
        }
    }
}