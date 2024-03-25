using System.Collections.Generic;
using System.Data.SqlClient;

namespace APV.DatabaseAccess.Interfaces
{
    public interface ISqlParameterCollection : ICollection<SqlParameter>
    {
        SqlParameter AddWithValue(string parameterName, object value);
    }
}