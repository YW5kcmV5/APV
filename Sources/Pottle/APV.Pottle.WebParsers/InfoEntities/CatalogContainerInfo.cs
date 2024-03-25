using System.Runtime.Serialization;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.Avtoberg;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class CatalogContainerInfo : BaseParserInfo
    {
        protected CatalogContainerInfo()
        {
        }

        public CatalogContainerInfo(AbsoluteUri url)
            : base(url)
        {
        }

        [DataMember(IsRequired = true)]
        public AbsoluteUri[] Links { get; set; }

        [DataMember(IsRequired = true)]
        public AvtobergTrademarkInfo[] Trademarks { get; set; }
    }
}