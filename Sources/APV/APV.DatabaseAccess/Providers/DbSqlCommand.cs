using System;
using System.Data;
using System.Data.SqlClient;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers
{
    public class DbSqlCommand : ISqlCommand
    {
        private ISqlCommand _instance;
        private DbSqlConnection _connection;
        private readonly DbSqlParameterCollection _parameters = new DbSqlParameterCollection();

        private ISqlCommand GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            if (_connection == null)
                throw new InvalidOperationException("Db instance is not defined. Please specify connection first.");

            _instance = DbProvider.GetCommandInstance(_connection.Database);
            return _instance;
        }

        private DbSqlConnection GetConnection()
        {
            if (_connection != null)
            {
                return _connection;
            }

            if (_instance != null)
            {
                SetConnection(_instance.Connection);
            }

            return _connection;
        }

        private void SetConnection(ISqlConnection connection)
        {
            _connection = (connection as DbSqlConnection) ?? new DbSqlConnection(_instance.Connection);
        }

        private void CopyParameters()
        {
            Instance.CommandType = CommandType;
            Instance.CommandText = CommandText;
            Instance.Connection = Connection;
            Instance.Parameters.Clear();
            foreach (SqlParameter parameter in Parameters)
            {
                Instance.Parameters.Add(parameter);
            }
        }

        #region ISqlCommand

        ISqlDataReader ISqlCommand.ExecuteReader(CommandBehavior commandBehavior)
        {
            return ExecuteReader(commandBehavior);
        }

        ISqlConnection ISqlCommand.Connection
        {
            get { return GetConnection(); }
            set { SetConnection(value); }
        }

        #endregion

        internal DbSqlCommand(ISqlCommand instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
        }

        public DbSqlCommand()
        {
        }

        public DbSqlCommand(string sql)
        {
            CommandText = sql;
        }

        public ISqlParameterCollection Parameters
        {
            get { return _parameters; }
        }

        public DbSqlDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            CopyParameters();
            ISqlDataReader reader = Instance.ExecuteReader(commandBehavior);
            return new DbSqlDataReader(reader);
        }

        public void ExecuteNonQuery()
        {
            CopyParameters();
            Instance.ExecuteNonQuery();
        }

        public object ExecuteScalar()
        {
            CopyParameters();
            return Instance.ExecuteScalar();
        }

        public DbSqlConnection Connection
        {
            get { return GetConnection(); }
            set { SetConnection(value); }
        }

        public CommandType CommandType { get; set; }

        public string CommandText { get; set; }

        public ISqlCommand Instance
        {
            get { return GetInstance(); }
        }
    }
}