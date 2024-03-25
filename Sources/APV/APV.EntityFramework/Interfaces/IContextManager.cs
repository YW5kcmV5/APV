using APV.EntityFramework.DataLayer;

namespace APV.EntityFramework.Interfaces
{
    public interface IContextManager
    {
        IContext GetContext();

        SqlProviderSettings GetSqlSettings();

        long GetInstanceId();

        string GetConnectionString();

        int GetStatisticsUpdateTimeoutInSeconds();
    }
}