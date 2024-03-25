using System;
using System.Net;
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
    public class IPAddressManagement : BaseManagement<IPAddressEntity, IPAddressCollection, IPAddressDataLayerManager>
    {
        [AnonymousAccess]
        public IPAddressEntity Find(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");

            string name = ip.ToString();
            IPAddressEntity entity = DatabaseManager.FindByName(name);
            if (entity != null)
            {
                return entity;
            }

            IIPLocator locator = ApplicationManager.GetIPLocator();
            IPLocation ipLocation = locator.GetIPLocation(ip);

            if (ipLocation == null)
            {
                //TODO: log
                //throw new ArgumentOutOfRangeException("ip", string.Format("Locator can not define location for specified ip address \"{0}\".", ip));
                return null;
            }

            CountryEntity country = CountryManagement.Instance.FindByCode(ipLocation.CountryCode);

            if (country == null)
            {
                //TODO: log
                //throw new ArgumentOutOfRangeException("ip", string.Format("Unknown country code \"{0}\" for specified ip address \"{1}\".", ipLocation.CountryCode, ip));
                return null;
            }

            LocationEntity location = LocationManagement.Instance.Create(ipLocation.Location);

            entity = new IPAddressEntity
                {
                    Value = name,
                    Country = country,
                    Location = location,
                    Restricted = false,
                };
            entity.Save();

            return entity;
        }

        public static readonly IPAddressManagement Instance = (IPAddressManagement)EntityFrameworkManager.GetManagement<IPAddressEntity>();
    }
}