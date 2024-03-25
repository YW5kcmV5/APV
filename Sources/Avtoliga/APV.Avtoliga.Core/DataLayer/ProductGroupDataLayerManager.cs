using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class ProductGroupDataLayerManager : BaseDataLayerManager<ProductGroupEntity, ProductGroupCollection>
    {
        public int GetReferenceCount(long productGroupId)
        {
            const string sql = @"SELECT COUNT([ProductId]) FROM [Product] WHERE [Product].GroupId = @PoductGroupId";
            var @params = new Dictionary<string, object> {{"@PoductGroupId", productGroupId}};
            return (int) ExecuteScalar(sql, @params);
        }
    }
}