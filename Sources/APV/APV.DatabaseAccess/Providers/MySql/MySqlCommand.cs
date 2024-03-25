using System.Data;
using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers.MySql
{
    public class MySqlCommand : ISqlCommand
    {
        public ISqlConnection Connection { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public void ExecuteNonQuery()
        {
            throw new global::System.NotImplementedException();
        }

        public object ExecuteScalar()
        {
            throw new global::System.NotImplementedException();
        }

        public ISqlDataReader ExecuteReader(CommandBehavior commandBehavior)
        {
            throw new global::System.NotImplementedException();
        }

        public ISqlParameterCollection Parameters { get; private set; }
    }
}