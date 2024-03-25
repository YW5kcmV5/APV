using System;
using System.Collections.Generic;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
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