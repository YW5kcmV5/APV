using System;
using APV.DatabaseAccess.Interfaces;
using Devart.Data.Oracle;

namespace APV.DatabaseAccess.Providers.Oracle
{
    public class OracleSqlConnection : ISqlConnection
    {
        private bool _disposed;
        private OracleConnection _instance;

        public OracleSqlConnection()
        {
            _instance = new OracleConnection();
        }

        public OracleSqlConnection(OracleConnection instance)
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

            OracleCommand command = _instance.CreateCommand();
            return new OracleSqlCommand(this, command);
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
            get { return DatabaseType.Oracle; }
        }

        public OracleConnection Instance
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