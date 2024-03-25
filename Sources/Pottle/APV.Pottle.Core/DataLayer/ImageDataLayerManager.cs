using System;
using System.Collections.Generic;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class ImageDataLayerManager : BaseDataLayerManager<DataImageEntity, DataImageCollection>
    {
        public DataImageEntity Find(byte[] hashCode)
        {
            if (hashCode == null)
                throw new ArgumentNullException("hashCode");

            const string whereSql = @"WHERE [Image].[HashCode] = @HashCode";
            var @params = new Dictionary<string, object> { { "@HashCode", hashCode } };
            return Find(whereSql, @params);
        }
    }
}