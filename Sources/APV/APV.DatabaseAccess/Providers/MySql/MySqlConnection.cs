using APV.DatabaseAccess.Interfaces;

namespace APV.DatabaseAccess.Providers.MySql
{
    public class MySqlConnection : ISqlConnection
    {
        public void Dispose()
        {
            throw new global::System.NotImplementedException();
        }

        public int ConnectionTimeout { get; private set; }

        public void Open()
        {
            throw new global::System.NotImplementedException();
        }

        public void Close()
        {
            throw new global::System.NotImplementedException();
        }

        public ISqlCommand CreateCommand()
        {
            throw new global::System.NotImplementedException();
        }

        public DatabaseType Database { get; private set; }
        public string ConnectionString { get; set; }
    }
}