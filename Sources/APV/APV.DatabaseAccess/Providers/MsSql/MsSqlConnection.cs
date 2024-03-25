using System;
using System.Data.SqlClient;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers.MsSql
{
    public class MsSqlConnection : ISqlConnection
    {
        private bool _disposed;
        private SqlConnection _instance;

        public MsSqlConnection()
        {
            _instance = new SqlConnection();
        }

        public MsSqlConnection(SqlConnection instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _instance.Dispose();
                _instance = null;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");
                
                return _instance.ConnectionTimeout;
            }
        }

        public void Open()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");

            _instance.Open();
        }

        public void Close()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");

            _instance.Close();
        }

        public ISqlCommand CreateCommand()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");

            SqlCommand command = _instance.CreateCommand();
            return new MsSqlCommand(this, command);
        }

        public string ConnectionString
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");

                return _instance.ConnectionString;
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");

                _instance.ConnectionString = value;
            }
        }

        public DatabaseType Database
        {
            get { return DatabaseType.MsSql; }
        }

        public SqlConnection Instance
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().Name, "The connection already disposed.");

                return _instance;
            }
        }
    }
}