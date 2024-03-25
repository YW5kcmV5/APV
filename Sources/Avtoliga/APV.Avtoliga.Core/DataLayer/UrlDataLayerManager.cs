using System;
using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class UrlDataLayerManager : BaseDataLayerManager<UrlEntity, UrlCollection>
    {
        public UrlEntity Find(byte[] hashCode)
        {
            if (hashCode == null)
                throw new ArgumentNullException("hashCode");

            const string whereSql = @"WHERE [Url].[HashCode] = @HashCode";
            var @params = new Dictionary<string, object> { { "@HashCode", hashCode } };
            return Find(whereSql, @params);
        }

        public UrlCollection GetToVerify(DateTime olderThan)
        {
            olderThan = olderThan.ToUniversalTime();

            const string sql = @"WHERE ([Url].[Alive] IS NULL) OR ([Url].[Alive] <= @OlderThan)";
            var @params = new Dictionary<string, object> { { "@OlderThan", olderThan } };
            return GetList(sql, @params);
        }
    }
}