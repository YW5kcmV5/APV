using System.Runtime.Serialization;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Pottle.WebParsers.Votonia
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class VotoniaSupplierProductInfo : SupplierProductInfo
    {
        private VotoniaSupplierProductInfo()
        {
        }

        public VotoniaSupplierProductInfo(AbsoluteUri url, byte[] htmlHashCode)
            : base(url, htmlHashCode)
        {
        }

        public VotoniaSupplierProductInfo(VotoniaSupplierProductInfo from)
            : base(from)
        {
        }
    }
}