using System;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Common.Application;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.Toolkit.Navigation.Entities;
using APV.Pottle.Toolkit.Navigation.Interfaces;

namespace APV.Pottle.Core.BusinessLogic
{
    public class LocationManagement : BaseManagement<LocationEntity, LocationCollection, LocationDataLayerManager>
    {
        [AnonymousAccess]
        public LocationEntity Create(GeoLocation location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            LocationEntity existingEntity = DatabaseManager.Find(location.LAT, location.LON);
            if (existingEntity != null)
            {
                return existingEntity;
            }

            IGeoLocator geoLocator = ApplicationManager.GetGeoLocator();
            GeoAddress geoAddress = geoLocator.GetGeoAddress(location);

            if (geoAddress == null)
                throw new ArgumentOutOfRangeException("location", string.Format("Address can not be defined from following location LAT=\"{0}\", LON=\"{1}\".", location.LAT, location.LON));

            string formattedAddress = geoAddress.FormattedAddress;
            existingEntity = DatabaseManager.Find(formattedAddress);
            if (existingEntity != null)
            {
                return existingEntity;
            }

            var entity = new LocationEntity
            {
                Address = formattedAddress,
                LAT = geoAddress.Location.LAT,
                LON = geoAddress.Location.LON
            };
            entity.Save();

            return entity;
        }

        [ClientAccess]
        public LocationEntity Create(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            LocationEntity existingEntity = DatabaseManager.Find(address);
            if (existingEntity != null)
            {
                return existingEntity;
            }

            IGeoLocator geoLocator = ApplicationManager.GetGeoLocator();
            GeoAddress geoAddress = geoLocator.GetGeoAddress(address);

            if (geoAddress == null)
                throw new ArgumentOutOfRangeException("address", string.Format("Address \"{0}\" could not be parsed.", address));

            string formattedAddress = geoAddress.FormattedAddress;

            existingEntity = DatabaseManager.Find(formattedAddress);
            if (existingEntity == null)
            {
                geoAddress = geoLocator.GetGeoAddress(formattedAddress);
                existingEntity = DatabaseManager.Find(geoAddress.Location.LAT, geoAddress.Location.LON);
            }

            if (existingEntity != null)
            {
                if (existingEntity.Address != formattedAddress)
                {
                    existingEntity.Address = formattedAddress;
                    existingEntity.Save();
                }
                return existingEntity;
            }

            var entity = new LocationEntity
                {
                    Address = formattedAddress,
                    LAT = geoAddress.Location.LAT,
                    LON = geoAddress.Location.LON
                };
            entity.Save();

            return entity;
        }

        [ClientAccess]
        public LocationEntity Find(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            LocationEntity existingEntity = DatabaseManager.Find(address);
            if (existingEntity != null)
            {
                return existingEntity;
            }

            IGeoLocator geoLocator = ApplicationManager.GetGeoLocator();
            GeoAddress geoAddress = geoLocator.GetGeoAddress(address);

            if (geoAddress == null)
                throw new ArgumentOutOfRangeException("address", string.Format("Address \"{0}\" could not be parsed.", address));

            string formattedAddress = geoAddress.FormattedAddress;
            existingEntity = DatabaseManager.Find(formattedAddress);
            if (existingEntity != null)
            {
                return existingEntity;
            }

            return null;
        }

        public static readonly LocationManagement Instance = (LocationManagement)EntityFrameworkManager.GetManagement<LocationEntity>();
    }
}