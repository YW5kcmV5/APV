using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APV.EntityFramework;
using APV.Common.Extensions;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class AddressManagement : BaseManagement<AddressEntity, AddressCollection, AddressDataLayerManager>
    {
        public const string PositionChars = @"0123456789QWERTYUIOPASDFGHJKLZXCVBNMЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ";
        public const string PositionSymbols = @"-./\_:";
        public const string AddressSeparators = @" ,;";

        public static readonly HashSet<char> AddressSeparatorsHash = new HashSet<char>(AddressSeparators);
        public static readonly HashSet<char> PositionCharsHash = new HashSet<char>(PositionChars);
        public static readonly HashSet<char> PositionSymbolsHash = new HashSet<char>(PositionSymbols);

        #region Private

        private bool TryExtractPosition(string address, out AddressPositionType positonType, out string position)
        {
            char[] separators = PositionSymbols.ToCharArray();
            string[] items = address.Split(AddressSeparators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = items.Length - 1; i >= 0; i--)
            {
                bool last = (i == items.Length - 1);
                string item = items[i];
                string converted = separators.Aggregate(item, (current, separator) => current.Replace(separator, '.'));
                int index = converted.IndexOf(".");
                string prefix = null;
                position = null;
                if (index != -1)
                {
                    prefix = converted.Substring(0, index + 1);
                    if (index < converted.Length - 1)
                    {
                        position = converted.Substring(index + 1);
                    }
                    else if (!last)
                    {
                        position = items[i + 1];
                    }
                }
                else if (!last)
                {
                    position = items[i + 1];
                }

                if ((prefix != null) && (position != null))
                {
                    position = position.Trim(PositionSymbols.ToCharArray());
                    if (/*(prefix == "к.") || */(prefix == "кв.") || (prefix == "квартира."))
                    {
                        positonType = AddressPositionType.Apartments;
                        return true;
                    }
                    if ((prefix == "ст.") || (prefix == "с.") || (prefix == "строение."))
                    {
                        positonType = AddressPositionType.Building;
                        return true;
                    }
                    if ((prefix == "оф.") || (prefix == "офис."))
                    {
                        positonType = AddressPositionType.Office;
                        return true;
                    }
                    if ((prefix == "пом.") || (prefix == "п.") || (prefix == "помещение."))
                    {
                        positonType = AddressPositionType.Placement;
                        return true;
                    }
                }
            }

            position = null;
            positonType = AddressPositionType.None;
            return true;
        }

        private string ToFormattedAddress(string locationAddress, AddressPositionType positonType, string position, int? floor, int? porch)
        {
            var sb = new StringBuilder(locationAddress);
            if (positonType != AddressPositionType.None)
            {
                sb.AppendFormat(", {0} {1}", positonType.GetPrefix(), position);
            }
            else if (positonType == AddressPositionType.Other)
            {
                sb.Append(position);
            }
            if (floor != null)
            {
                sb.AppendFormat(", этаж {0}", floor);
            }
            if (porch != null)
            {
                sb.AppendFormat(", подъезд {0}", porch);
            }
            return sb.ToString();
        }

        #endregion

        [AnonymousAccess]
        public string FormatPosition(string position)
        {
            if (string.IsNullOrEmpty(position))
            {
                return null;
            }

            position = position.Trim();
            var sb = new StringBuilder();
            for (int i = 0; i < position.Length; i++)
            {
                char ch = position[i];
                char upperCh = char.ToUpperInvariant(ch);
                if (PositionCharsHash.Contains(upperCh))
                {
                    sb.Append(ch);
                }
                else if ((sb.Length > 0) && (PositionSymbolsHash.Contains(upperCh)))
                {
                    char previous = (sb[sb.Length - 1]);
                    if (previous != '/')
                    {
                        if ((i < position.Length - 1) && (char.IsDigit(previous) == char.IsDigit(position[i + 1])))
                        {
                            sb.Append("/");
                        }
                    }
                }
            }
            return sb.ToString();
        }

        [ClientAccess]
        public AddressEntity Create(string address, int? floor = null, int? porch = null, string description = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            AddressPositionType positionType;
            string position;
            if (!TryExtractPosition(address, out positionType, out position))
                throw new ArgumentOutOfRangeException(string.Format("Address \"{0}\" can not be parsed.", address));

            return Create(address, positionType, position, floor, porch, description);
        }

        [ClientAccess]
        public AddressEntity CreateFlat(string address, string flatNumber, int? floor = null, int? porch = null, string description = null)
        {
            return Create(address, AddressPositionType.Apartments, flatNumber, floor, porch, description);
        }

        [ClientAccess]
        public AddressEntity Create(string address, AddressPositionType positionType, string position = null, int? floor = null, int? porch = null, string description = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            string formattedPosition = FormatPosition(position);
            
            if ((string.IsNullOrEmpty(formattedPosition)) && (positionType != AddressPositionType.None))
                throw new ArgumentOutOfRangeException("position", string.Format("Invalid position \"{0}\" (\"{1}\") or position type \"{2}\".", position, formattedPosition, positionType));

            LocationEntity location = LocationManagement.Instance.Create(address);

            AddressEntity entity = DatabaseManager.Find(location.LocationId, positionType, formattedPosition);
            string formattedAddress = ToFormattedAddress(location.Address, positionType, position, floor, porch);

            if (entity != null)
            {
                if ((entity.Floor != floor) || (entity.Porch != porch) || (entity.Description != description) || (entity.Address != formattedAddress))
                {
                    entity.Address = formattedAddress;
                    entity.Floor = floor;
                    entity.Porch = porch;
                    entity.Description = description;
                    entity.Save();
                }
                return entity;
            }

            entity = new AddressEntity
                {
                    Address = formattedAddress,
                    PositionType = positionType,
                    Position = formattedPosition,
                    Floor = floor,
                    Porch = porch,
                    Description = description,
                    Location = location,
                };

            entity.Save();

            return entity;
        }

        public static readonly AddressManagement Instance = (AddressManagement)EntityFrameworkManager.GetManagement<AddressEntity>();
    }
}