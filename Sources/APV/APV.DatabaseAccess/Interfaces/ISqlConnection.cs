using System;

namespace APV.DatabaseAccess.Interfaces
{
    public interface ISqlConnection : IDisposable
    {
        int ConnectionTimeout { get; }

        void Open();

        void Close();

        ISqlCommand CreateCommand();

        DatabaseType Database { get; }

        string ConnectionString { get; set; }
    }
}