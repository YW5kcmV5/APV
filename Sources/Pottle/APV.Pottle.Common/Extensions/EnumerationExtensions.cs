using System;
using System.Collections.Generic;

namespace APV.Pottle.Common.Extensions
{
    public static class EnumerationExtensions
    {
        #region PartsOfSpeech

        public static bool Has(this PartsOfSpeech partsOfSpeech, PartOfSpeech partOfSpeech)
        {
            return partsOfSpeech.Has((PartsOfSpeech)partOfSpeech);
        }

        public static bool Has(this PartsOfSpeech partsOfSpeech, PartsOfSpeech partOfSpeech)
        {
            return ((partOfSpeech != PartsOfSpeech.Unknown) && (partsOfSpeech.HasFlag(partOfSpeech)));
        }

        public static PartsOfSpeech Not(this PartsOfSpeech partsOfSpeech, PartOfSpeech partOfSpeech)
        {
            return partsOfSpeech.Not((PartsOfSpeech)partOfSpeech);
        }

        public static PartsOfSpeech Not(this PartsOfSpeech partsOfSpeech, PartsOfSpeech partOfSpeech)
        {
            return (partsOfSpeech & ~partOfSpeech);
        }

        public static PartOfSpeech[] ToArray(this PartsOfSpeech partsOfSpeech)
        {
            return partsOfSpeech.ToList().ToArray();
        }

        public static List<PartOfSpeech> ToList(this PartsOfSpeech partsOfSpeech)
        {
            var result = new List<PartOfSpeech>();
            Array values = Enum.GetValues(typeof(PartsOfSpeech));
            foreach (Enum value in values)
            {
                var partsOfSpeechValue = (PartsOfSpeech)value;
                if (partsOfSpeech.Has(partsOfSpeechValue))
                {
                    result.Add((PartOfSpeech)value);
                }
            }
            return result;
        }

        #endregion

        #region ProductCharacteristicModifiers

        public static bool Has(this ProductCharacteristicModifiers partsOfSpeech, ProductCharacteristicModifier partOfSpeech)
        {
            return partsOfSpeech.Has((ProductCharacteristicModifiers)partOfSpeech);
        }

        public static bool Has(this ProductCharacteristicModifiers partsOfSpeech, ProductCharacteristicModifiers partOfSpeech)
        {
            return ((partOfSpeech != ProductCharacteristicModifiers.None) && (partsOfSpeech.HasFlag(partOfSpeech)));
        }

        public static ProductCharacteristicModifiers Not(this ProductCharacteristicModifiers partsOfSpeech, ProductCharacteristicModifier partOfSpeech)
        {
            return partsOfSpeech.Not((ProductCharacteristicModifiers)partOfSpeech);
        }

        public static ProductCharacteristicModifiers Not(this ProductCharacteristicModifiers partsOfSpeech, ProductCharacteristicModifiers partOfSpeech)
        {
            return (partsOfSpeech & ~partOfSpeech);
        }

        public static ProductCharacteristicModifier[] ToArray(this ProductCharacteristicModifiers partsOfSpeech)
        {
            return partsOfSpeech.ToList().ToArray();
        }

        public static List<ProductCharacteristicModifier> ToList(this ProductCharacteristicModifiers partsOfSpeech)
        {
            var result = new List<ProductCharacteristicModifier>();
            Array values = Enum.GetValues(typeof(ProductCharacteristicModifiers));
            foreach (Enum value in values)
            {
                var partsOfSpeechValue = (ProductCharacteristicModifiers)value;
                if (partsOfSpeech.Has(partsOfSpeechValue))
                {
                    result.Add((ProductCharacteristicModifier)value);
                }
            }
            return result;
        }

        #endregion
    }
}