namespace APV.EntityFramework.DataLayer
{
    public sealed class SqlProviderSettings
    {
        public string SqlCommandSettings { get; set; }

        public static readonly SqlProviderSettings Default = new SqlProviderSettings
            {
                // -- Stops the message that shows the count of the number of rows affected by a Transact-SQL statement or stored procedure from being returned as part of the result set.
                // SET NOCOUNT ON;
                // -- Specifies whether SQL Server automatically rolls back the current transaction when a Transact-SQL statement raises a run-time error.
                // SET XACT_ABORT ON;
                SqlCommandSettings = "SET NOCOUNT ON;\r\nSET XACT_ABORT ON;\r\n",
            };
    }
}
