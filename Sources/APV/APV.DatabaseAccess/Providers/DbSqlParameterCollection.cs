using System.Collections.Generic;
using System.Data.SqlClient;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers
{
    public class DbSqlParameterCollection : List<SqlParameter>, ISqlParameterCollection
    {
        public SqlParameter AddWithValue(string parameterName, object value)
        {
            var parameter = new SqlParameter(parameterName, value);
            Add(parameter);
            return parameter;
        }
    }
}