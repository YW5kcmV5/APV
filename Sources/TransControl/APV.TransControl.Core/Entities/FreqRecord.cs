using System;
using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    [XmlRoot]
    [Serializable]
    public class FreqRecord : ICloneable
    {
        /// <summary>
        /// Дата и время сигнала в локальном времени (в базе храниться в UTC но при чтении из базы преобразуется к локальному формату)
        /// </summary>
        [XmlElement]
        public DateTime GMT { get; set; }

        [XmlElement]
        public float Freq { get; set; }

        //[XmlElement(IsNullable = true)]
        //public byte[] State { get; set; }

        //[XmlIgnore]
        //public StateType StateType
        //{
        //    get
        //    {
        //        return (State != null) ? (StateType)(uint)BitConverter.ToInt32(State, 0) : 0;
        //    }
        //}

        [XmlIgnore]
        public bool IsEngine
        {
            //get { return ((StateType & StateType.Engine) == StateType.Engine); }
            get;
            set;
        }

        public object Clone()
        {
            return new FreqRecord
            {
                GMT = GMT,
                Freq = Freq,
                //State = State,
                IsEngine = IsEngine,
            };
        }
    }
}