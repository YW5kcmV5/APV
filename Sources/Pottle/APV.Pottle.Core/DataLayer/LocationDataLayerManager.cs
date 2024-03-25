using System;
using System.Collections.Generic;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class LocationDataLayerManager : BaseDataLayerManager<LocationEntity, LocationCollection>
    {
        public LocationEntity Find(float lat, float lon)
        {
            const string whereSql = @"WHERE [Location].[LAT] = @LAT AND [Location].[LON] = @LON";
            var @params = new Dictionary<string, object> { { "@LAT", lat }, { "@LON", lon } };

            return Find(whereSql, @params);
        }

        public LocationEntity Find(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            const string whereSql = @"WHERE [Location].[Address] = @Address";
            var @params = new Dictionary<string, object> { {"@Address", address} };

            return Find(whereSql, @params);
        }
    }
}