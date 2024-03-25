using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class TrademarkDataLayerManager : BaseDataLayerManager<TrademarkEntity, TrademarkCollection>
    {
        public int GetReferenceCount(long trademarkId)
        {
            const string sql = @"SELECT COUNT([Model]) FROM [Model] WHERE [Model].TrademarkId = @TrademarkId";
            var @params = new Dictionary<string, object> { { "@TrademarkId", trademarkId } };
            return (int) ExecuteScalar(sql, @params);
        }
    }
}