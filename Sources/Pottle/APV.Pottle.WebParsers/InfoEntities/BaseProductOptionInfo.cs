using System;
using System.Runtime.Serialization;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    [KnownType(typeof(ProductCharacteristicInfo))]
    [KnownType(typeof(ProductOptionInfo))]
    public abstract class BaseProductOptionInfo
    {
        protected BaseProductOptionInfo()
        {
        }

        protected BaseProductOptionInfo(ProductOptionType optionType, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            OptionType = optionType;
            Name = name;
        }

        [DataMember]
        public virtual ProductOptionType OptionType { get; private set; }

        [DataMember]
        public virtual string Name { get; private set; }

        [DataMember]
        public virtual string Value { get; set; }
    }
}