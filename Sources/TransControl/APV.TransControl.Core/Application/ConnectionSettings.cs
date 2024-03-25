namespace APV.TransControl.Core.Application
{
    public class ConnectionSettings
    {
        private string GetDescriptor()
        {
            return $@"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={Host})(PORT={Port})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={DBName})))";
        }

        private string GetConnectionString()
        {
            string dataSource = UseTnsDBName ? DBName : GetDescriptor();
            string connString = (!UseTnsDBName)
                ? $@"Direct=true;Host={Host};Port={Port};SID={DBName};uid={Username};password={Password};Unicode=True;Provider=Oracle;"
                : $@"Data Source={dataSource};uid={Username};password={Password};Unicode=True;Provider=Oracle;";
            return connString;
        }

        public string Username { get; set; } = "SYNCH_WL";

        public string Password { get; set; } = "123";

        public string Host { get; set; } = "192.168.58.24";

        public int Port { get; set; } = 1521;

        public string DBName { get; set; } = "TRAN";

        public bool UseTnsDBName { get; set; }

        public string DataSource
        {
            get { return GetDescriptor(); }
        }

        public string ConnectionString
        {
            get { return GetConnectionString(); }
        }
    }
}
