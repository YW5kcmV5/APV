using System.Data;

namespace APV.DatabaseAccess.Interfaces
{
    public interface ISqlCommand
    {
        ISqlConnection Connection { get; set; }

        CommandType CommandType { get; set; }

        string CommandText { get; set; }

        void ExecuteNonQuery();

        object ExecuteScalar();

        ISqlDataReader ExecuteReader(CommandBehavior commandBehavior);

        ISqlParameterCollection Parameters { get; }
    }
}