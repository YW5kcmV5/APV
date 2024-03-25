using System.Runtime.Serialization;
using APV.Common;
using APV.GraphicsLibrary.Images;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.Avtoberg
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public sealed class AvtobergTrademarkInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public ImageContainer Logo { get; set; }

        [DataMember]
        public AbsoluteUri Url { get; set; }

        [DataMember]
        public AvtobergModelInfo[] Models { get; set; }
    }
}