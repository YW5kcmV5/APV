using System.Runtime.Serialization;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class ProductOptionInfo : BaseProductOptionInfo
    {
        protected ProductOptionInfo()
        {
        }

        public ProductOptionInfo(ProductOptionType optionType)
            : base(optionType, optionType.ToString())
        {
        }
    }
}