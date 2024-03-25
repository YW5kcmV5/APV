using System;
using System.Data;
using System.Data.SqlClient;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers.MsSql
{
    public class MsSqlCommand : ISqlCommand
    {
        private readonly ISqlParameterCollection _parameters = new DbSqlParameterCollection();
        private readonly SqlCommand _instance;
        private MsSqlConnection _connection;

        private void SetConnection(ISqlConnection connection)
        {
            if (connection is MsSqlConnection)
            {
                _connection = (MsSqlConnection) connection;
                _instance.Connection = ((MsSqlConnection) connection).Instance;
            }
            else if (connection is DbSqlConnection)
            {
                SetConnection(((DbSqlConnection) connection).Instance);
            }
            else
                throw new NotSupportedException(string.Format("Specified connection type \"{0}\" is not supported for MS SQL database.", connection.GetType()));
        }

        private void CopyParameters()
        {
            _instance.Parameters.Clear();
            foreach (SqlParameter parameter in _parameters)
            {
                _instance.Parameters.Add(parameter);
            }
        }

        public ISqlConnection Connection
        {
            get { return _connection; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                
                SetConnection(value);
            }
        }

        public CommandType CommandType
        {
            get { return _instance.CommandType; }
            set { _instance.CommandType = value; }
        }

        public string CommandText
        {
            get { return _instance.CommandText; }
            set { _instance.CommandText = value; }
        }

        public MsSqlCommand()
        {
            _instance = new SqlCommand();
        }

        public MsSqlCommand(MsSqlConnection connection, SqlCommand instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _connection = connection;
            _instance = instance;
        }

        public void ExecuteNonQuery()
        {
            CopyParameters();
            _instance.ExecuteNonQuery();
        }

        public object ExecuteScalar()
        {
            CopyParameters();
            return _instance.ExecuteScalar();
        }

        public ISqlDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            CopyParameters();
            SqlDataReader reader = _instance.ExecuteReader(commandBehavior);
            return new MsSqlDataReader(reader);
        }

        public ISqlParameterCollection Parameters
        {
            get { return _parameters; }
        }
    }
}