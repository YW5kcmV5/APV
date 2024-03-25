using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class ProducerDataLayerManager : BaseDataLayerManager<ProducerEntity, ProducerCollection>
    {
        public int GetReferenceCount(long producerId)
        {
            const string sql = @"SELECT COUNT([ProductId]) FROM [Product] WHERE [Product].ProducerId = @ProducerId";
            var @params = new Dictionary<string, object> {{"@ProducerId", producerId}};
            return (int) ExecuteScalar(sql, @params);
        }
    }
}