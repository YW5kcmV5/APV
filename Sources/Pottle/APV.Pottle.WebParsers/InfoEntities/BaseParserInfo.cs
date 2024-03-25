using System;
using System.Runtime.Serialization;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.Votonia;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    [KnownType(typeof(CatalogContainerInfo))]
    [KnownType(typeof(CatalogPageInfo))]
    [KnownType(typeof(ImageInfo))]
    [KnownType(typeof(SupplierProductInfo))]
    [KnownType(typeof(VotoniaSupplierProductInfo))]
    public abstract class BaseParserInfo
    {
        protected BaseParserInfo()
        {
        }

        protected BaseParserInfo(AbsoluteUri url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            Url = url;
        }

        [DataMember]
        public AbsoluteUri Url { get; protected set; }
    }
}