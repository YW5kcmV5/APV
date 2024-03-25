using System;
using System.Text;
using APV.DatabaseAccess.Interfaces;
using APV.DatabaseAccess.Providers.MsSql;
using APV.DatabaseAccess.Providers.MySql;
using APV.DatabaseAccess.Providers.Oracle;

namespace APV.DatabaseAccess.Providers
{
    public static class DbProvider
    {
        public static ISqlConnection GetConnectionInstance(DatabaseType database)
        {
            switch (database)
            {
                case DatabaseType.MsSql:
                    return new MsSqlConnection();
                case DatabaseType.MySql:
                    return new MySqlConnection();
                case DatabaseType.Oracle:
                    return new OracleSqlConnection();
                default:
                    throw new NotSupportedException($"Database type \"{database}\" is not supported.");
            }
        }

        public static ISqlCommand GetCommandInstance(DatabaseType database)
        {
            switch (database)
            {
                case DatabaseType.MsSql:
                    return new MsSqlCommand();
                case DatabaseType.MySql:
                    return new MySqlCommand();
                case DatabaseType.Oracle:
                    return new OracleSqlCommand();
                default:
                    throw new NotSupportedException($"Database type \"{database}\" is not supported.");
            }
        }

        public static DatabaseType GetDatabaseType(ref string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentOutOfRangeException(nameof(connectionString), "Connection string is empty or whitespace.");

            string[] items = connectionString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            DatabaseType? database = null;
            var sb = new StringBuilder();
            foreach (string item in items)
            {
                if ((item.StartsWith("DatabaseType=", StringComparison.InvariantCultureIgnoreCase) ||
                      item.StartsWith("DatabaseType=", StringComparison.InvariantCultureIgnoreCase) ||
                      item.StartsWith("Provider=", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (database != null)
                        throw new ArgumentOutOfRangeException(nameof(connectionString), $"Invalid connection string format (\"{connectionString}\"). Provider (DatabaseType) section can be only one.");

                    int index = item.IndexOf("=", StringComparison.InvariantCultureIgnoreCase);
                    string databaseTypeToParse = (index < item.Length - 1)
                                                     ? item.Substring(index + 1)
                                                     : null;

                    DatabaseType databaseValue;
                    if ((string.IsNullOrEmpty(databaseTypeToParse)) || (!Enum.TryParse(databaseTypeToParse, true, out databaseValue)))
                        throw new ArgumentOutOfRangeException(nameof(connectionString), $"Invalid connection string format (\"{connectionString}\"). Unknown Provider (DatabaseType) \"{item}\"");

                    database = databaseValue;
                }
                else
                {
                    sb.AppendFormat("{0};", item);
                }
            }

            connectionString = sb.ToString();
            return database ?? DatabaseType.MsSql;
        }
    }
}