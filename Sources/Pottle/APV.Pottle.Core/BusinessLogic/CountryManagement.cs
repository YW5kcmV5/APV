using System;
using System.Collections.Generic;
using APV.EntityFramework;
using APV.Common;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class CountryManagement : BaseManagement<CountryEntity, CountryCollection, CountryDataLayerManager>
    {
        private static bool _inited;
        private static CountryCollection _items;
        private static readonly SortedList<string, CountryEntity> Codes = new SortedList<string, CountryEntity>();
        private static readonly SortedList<string, CountryEntity> Names = new SortedList<string, CountryEntity>();

        private static void Init()
        {
            if (!_inited)
            {
                _inited = true;
                var manager = (CountryDataLayerManager)EntityFrameworkManager.GetManager<CountryEntity>();
                _items = (CountryCollection)manager.GetAll();
                foreach (CountryEntity country in _items)
                {
                    Codes.Add(country.Code.ToUpperInvariant(), country);
                    Names.Add(country.Name.ToUpperInvariant(), country);
                }
            }
        }

        CountryManagement()
        {
            Init();
        }

        [AnonymousAccess]
        public override CountryEntity FindByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            int index = Names.IndexOfKey(name.ToUpperInvariant());
            return (index != -1) ? Names.Values[index] : null;
        }

        [AnonymousAccess]
        public override CountryEntity GetByName(string name)
        {
            CountryEntity entity = FindByName(name);

            if (entity == null)
                throw new ArgumentOutOfRangeException("name", string.Format("Country with name \"{0}\" does not exist.", name));

            return entity;
        }

        [AnonymousAccess]
        public CountryEntity FindByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("code");

            int index = Codes.IndexOfKey(code.ToUpperInvariant());
            return (index != -1) ? Codes.Values[index] : null;
        }

        [AnonymousAccess]
        public CountryEntity GetByCode(string code)
        {
            CountryEntity entity = FindByCode(code);

            if (entity == null)
                throw new ArgumentOutOfRangeException("code", string.Format("Country with code \"{0}\" does not exist.", code));

            return entity;
        }

        [AnonymousAccess]
        public override CountryCollection GetAll()
        {
            return _items;
        }

        public static readonly CountryManagement Instance = (CountryManagement)EntityFrameworkManager.GetManagement<CountryEntity>();

        public static readonly CountryEntity Russia = Instance.GetByName(SystemConstants.CountryNameRussia);

        public static readonly CountryEntity Germany = Instance.GetByName(SystemConstants.CountryNameGermany);

        public static readonly CountryEntity Default = Instance.GetByCode(SystemConstants.DefaultCountryCode);

        public static readonly CountryCollection All = Instance.GetAll();
    }
}