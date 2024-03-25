using APV.Avtoliga.Core.Application;

namespace APV.Avtoliga.UnitTest
{
    public static class TestManager
    {
        private const string ConnectionString = @"Data Source=(local)\SQL2012;Initial Catalog=Avtoliga;Integrated Security=True;Pooling=True;Min Pool Size=3;";

        public const string AdminUsername = @"Admin";

        public const string AdminPasswod = @"123";

        public const string AdminEmail = @"ateam@hotbox.ru";

        public const string ProductGroupWithProduct = @"Капот";

        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        public static void Login()
        {
            ContextManager.LoginAsAdmin();
            //UserManagement.Instance.Login(AdminUsername, AdminPasswod);
        }
    }
}