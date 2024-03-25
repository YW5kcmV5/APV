using System.Collections.Generic;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class AddressDataLayerManager : BaseDataLayerManager<AddressEntity, AddressCollection>
    {
        public AddressEntity Find(long locationId, AddressPositionType positionType, string position)
        {
            if (!string.IsNullOrEmpty(position))
            {
                const string whereSql = @"WHERE [Address].[LocationId] = @LocationId AND [Address].[Position] = @Position AND [Address].[PositionType] = @PositionType";
                var @params = new Dictionary<string, object>
                    {
                        {"@LocationId", locationId},
                        {"@Position", position},
                        {"@PositionType", positionType},
                    };

                return Find(whereSql, @params);
            }
            else
            {
                const string whereSql = @"WHERE [Address].[LocationId] = @LocationId AND [Address].[Position] IS NULL AND [Address].[PositionType] = @PositionType";
                var @params = new Dictionary<string, object>
                    {
                        {"@LocationId", locationId},
                        {"@PositionType", positionType},
                    };

                return Find(whereSql, @params);
            }
        }
    }
}