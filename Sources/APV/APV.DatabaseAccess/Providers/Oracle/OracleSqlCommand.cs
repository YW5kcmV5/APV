using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using APV.DatabaseAccess.Interfaces;
using Devart.Data.Oracle;

namespace APV.DatabaseAccess.Providers.Oracle
{
    public class OracleSqlCommand : ISqlCommand
    {
        private readonly ISqlParameterCollection _parameters = new DbSqlParameterCollection();
        private readonly OracleCommand _instance;
        private OracleSqlConnection _connection;

        private static readonly SortedList<SqlDbType, OracleDbType> SqlDbTypeTransformations = new SortedList<SqlDbType, OracleDbType>
            {
                {SqlDbType.Int, OracleDbType.Integer},
                {SqlDbType.BigInt, OracleDbType.Int64},
                {SqlDbType.VarChar, OracleDbType.VarChar},
                {SqlDbType.NVarChar, OracleDbType.NVarChar},
                {SqlDbType.Decimal, OracleDbType.Number},
                {SqlDbType.Real, OracleDbType.Float},
                {SqlDbType.Float, OracleDbType.Double},
                {SqlDbType.Timestamp, OracleDbType.TimeStamp},
                {SqlDbType.DateTime, OracleDbType.Date},
                {SqlDbType.Char, OracleDbType.Char},
                {SqlDbType.NChar, OracleDbType.NChar},
                {SqlDbType.Binary, OracleDbType.Blob},
                {SqlDbType.VarBinary, OracleDbType.Raw},
                {SqlDbType.Bit, OracleDbType.Boolean},
            };

        private void SetConnection(ISqlConnection connection)
        {
            if (connection is OracleSqlConnection)
            {
                _connection = (OracleSqlConnection) connection;
                _instance.Connection = ((OracleSqlConnection) connection).Instance;
            }
            else if (connection is DbSqlConnection)
            {
                SetConnection(((DbSqlConnection) connection).Instance);
            }
            else
                throw new NotSupportedException(string.Format("Specified connection type \"{0}\" is not supported for Oracle SQL database.", connection.GetType()));
        }

        private OracleDbType ToOracleDbType(SqlDbType sqlDbType)
        {
            int index = SqlDbTypeTransformations.IndexOfKey(sqlDbType);
            if (index == -1)
                throw new NotSupportedException(string.Format("Specified database type \"{0}\" is not supported.", sqlDbType));

            return SqlDbTypeTransformations.Values[index];
        }

        private OracleParameter ToOracleParameter(SqlParameter parameter)
        {
            OracleDbType oracleDbType = ToOracleDbType(parameter.SqlDbType);
            var oracleParameter = new OracleParameter
                {
                    ParameterName = parameter.ParameterName,
                    Direction = parameter.Direction,
                    Size = parameter.Size,
                    OracleDbType = oracleDbType,
                };
            if ((parameter.Direction == ParameterDirection.Input) ||
                (parameter.Direction == ParameterDirection.InputOutput))
            {
                oracleParameter.OracleValue = parameter.SqlValue;
                oracleParameter.Value = parameter.Value;
            }
            return oracleParameter;
        }

        private void CopyParameters()
        {
            _instance.Parameters.Clear();
            foreach (SqlParameter parameter in _parameters)
            {
                OracleParameter oracleParameter = ToOracleParameter(parameter);
                _instance.Parameters.Add(oracleParameter);
            }
        }

        private void CopyOutputParameterValues()
        {
            foreach (OracleParameter oracleParameter in _instance.Parameters)
            {
                if ((oracleParameter.Direction == ParameterDirection.ReturnValue) ||
                    (oracleParameter.Direction == ParameterDirection.InputOutput) ||
                    (oracleParameter.Direction == ParameterDirection.Output))
                {
                    SqlParameter parameter = Parameters.Single(item => item.ParameterName == oracleParameter.ParameterName);
                    if (parameter.DbType == DbType.Boolean)
                    {
                        parameter.SqlValue = oracleParameter.Value;
                        parameter.Value = oracleParameter.Value;
                    }
                    else
                    {
                        parameter.SqlValue = oracleParameter.OracleValue;
                        parameter.Value = oracleParameter.Value;
                    }
                }
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

        public OracleSqlCommand()
        {
            _instance = new OracleCommand();
        }

        public OracleSqlCommand(OracleSqlConnection connection, OracleCommand instance)
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
            CopyOutputParameterValues();
        }

        public object ExecuteScalar()
        {
            CopyParameters();
            object result = _instance.ExecuteScalar();
            CopyOutputParameterValues();
            return result;
        }

        public ISqlDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            CopyParameters();
            OracleDataReader reader = _instance.ExecuteReader(commandBehavior);
            CopyOutputParameterValues();
            return new OracleSqlDataReader(reader);
        }

        public ISqlParameterCollection Parameters
        {
            get { return _parameters; }
        }
    }
}