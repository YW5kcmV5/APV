using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class ProductCharacteristicInfo : BaseProductOptionInfo
    {
        #region Constants

        /// <summary>
        /// "Color"
        /// </summary>
        public const string ColorName = "Color";

        /// <summary>
        /// "Age"
        /// </summary>
        public const string AgeName = "Age";

        /// <summary>
        /// "Structure"
        /// </summary>
        public const string StructureName = "Structure";

        /// <summary>
        /// "Size"
        /// </summary>
        public const string SizeName = "Size";

        /// <summary>
        /// "PackingSize"
        /// </summary>
        public const string PackingSizeName = "PackingSize";

        /// <summary>
        /// "Weight"
        /// </summary>
        public const string WeightName = "Weight";

        /// <summary>
        /// "Mode"
        /// </summary>
        public const string ModeName = "Mode";

        public static readonly SortedList<ProductCharacteristicModifier, string> ModifierToName = new SortedList<ProductCharacteristicModifier, string>
            {
                { ProductCharacteristicModifier.Color, ColorName },
                { ProductCharacteristicModifier.Size, SizeName },
            };

        #endregion

        protected ProductCharacteristicInfo()
        {
        }

        public ProductCharacteristicInfo(string name)
            : base(ProductOptionType.Characteristic, name)
        {
        }

        public static string GetName(ProductCharacteristicModifier modifier)
        {
            int index = ModifierToName.IndexOfKey(modifier);
            return (index != -1) ? ModifierToName.Values[index] : null;
        }

        public static ProductCharacteristicModifier GetModifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return (from keyValue in ModifierToName where keyValue.Value == name select keyValue.Key).FirstOrDefault();
        }
    }
}