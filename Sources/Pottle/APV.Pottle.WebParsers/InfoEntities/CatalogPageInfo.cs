using System.Runtime.Serialization;
using APV.Common;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class CatalogPageInfo : BaseParserInfo
    {
        protected CatalogPageInfo()
        {
        }

        public CatalogPageInfo(AbsoluteUri url)
            : base(url)
        {
        }

        [DataMember(IsRequired = true)]
        public AbsoluteUri[] Links { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public AbsoluteUri NextPageLink { get; set; }

        [DataMember]
        public AbsoluteUri PreviousPageLink { get; set; }
    }
}
