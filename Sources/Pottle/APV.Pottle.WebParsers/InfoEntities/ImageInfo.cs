using System.Runtime.Serialization;
using APV.Common;
using APV.GraphicsLibrary.Images;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class ImageInfo : BaseParserInfo
    {
        protected ImageInfo()
        {
        }

        public ImageInfo(AbsoluteUri url)
            : base(url)
        {
        }

        [DataMember(Order = 0)]
        public string Title { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public ImageContainer Data { get; set; }
    }
}