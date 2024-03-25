using System;
using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class ModelDataLayerManager : BaseDataLayerManager<ModelEntity, ModelCollection>
    {
        public ModelEntity FindByName(long trademarkId, string name, string period = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            const string whereSqlWithPeriod = @"WHERE ([Model].TrademarkId = @TrademarkId) AND ([Model].Name = @Name) AND ([Model].Period = @Period)";
            const string whereSqlWithoutPeriod = @"WHERE ([Model].TrademarkId = @TrademarkId) AND ([Model].Name = @Name) AND ([Model].Period IS NULL)";

            bool hasPeriod = (!string.IsNullOrWhiteSpace(period));
            string whereSql;

            var @params = new Dictionary<string, object>
                    {
                        { "@TrademarkId", trademarkId },
                        { "@Name", name },
                    };

            if (hasPeriod)
            {
                whereSql = whereSqlWithPeriod;
                @params.Add("@Period", period);
            }
            else
            {
                whereSql = whereSqlWithoutPeriod;
            }

            return Find(whereSql, @params);
        }

        public int GetReferenceCount(long modelId)
        {
            const string sql = @"SELECT COUNT([ProductId]) FROM [Product] WHERE [Product].ModelId = @ModelId";
            var @params = new Dictionary<string, object> { { "@ModelId", modelId } };
            return (int)ExecuteScalar(sql, @params);
        }

        public ModelCollection GetList(long trademarkId)
        {
            const string whereSql = @"WHERE [Model].TrademarkId = @TrademarkId";
            var @params = new Dictionary<string, object> { { "@TrademarkId", trademarkId } };
            return GetList(whereSql, @params);
        }
    }
}