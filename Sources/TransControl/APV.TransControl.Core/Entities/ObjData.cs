using System;
using System.Linq;
using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    [XmlRoot]
    [Serializable]
    public class ObjData : ICloneable
    {
        [XmlElement]
        public DateTime From { get; set; }

        [XmlElement]
        public DateTime To { get; set; }

        [XmlArray]
        public FreqRecord[] Data { get; set; }

        public bool Completed { get { return ((Data != null) && (Data.Length > 0)); } }

        public object Clone()
        {
            return new ObjData
                       {
                           From = From,
                           To = To,
                           Data = (Completed) ? Data.Select(item => (FreqRecord) item.Clone()).ToArray() : null
                       };
        }
    }
}