using System.Runtime.Serialization;
using APV.Common.Periods;
using APV.Common;
using APV.Pottle.Common;

namespace APV.Pottle.WebParsers.Avtoberg
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public sealed class AvtobergModelInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public AnnualPeriodCollection ModelPeriod { get; set; }

        [DataMember]
        public AbsoluteUri Url { get; set; }
    }
}