using APV.Pottle.Core.BusinessLogic;

namespace APV.Pottle.UnitTest
{
    public static class TestManager
    {
        private const string ConnectionString = @"Data Source=(local);Initial Catalog=Pottle;Integrated Security=True;Pooling=True;Min Pool Size=3;";

        public const int TestUserId = 1;

        public const int NotExistsId = 999999999;

        public const string NotExistsName = "963E5C23825C4F4DA623EF5037AAF3612E4AFF026ED0481EAC5A8164D580E439606215E1CB46461C817C08B777E85ADC";

        public const int AdminUserId = 1;

        public const string AdminUsername = @"Admin";

        public const string AdminPasswod = @"123";

        public const string AdminEmail = @"ateam@hotbox.ru";

        public const long TestProductId = 8;

        public const string TestProductSearch = "Перчатки детские Margot Bis";

        public const string TestTrademarkName = "Тестовая торговая марка (7BBABE8F)";

        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        public static void Login()
        {
            UserManagement.Instance.Login(AdminUsername, AdminPasswod);
        }
    }
}