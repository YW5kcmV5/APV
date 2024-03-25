using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    [XmlType(Namespace = "TransControl")]
    [XmlRootAttribute(Namespace = "TransControl")]
    //[DBObjectAttribute(DBTableName = "ADD_EQUIPMENT", DBUserName = "SYNCH_WL")]
    public class AddEquipment
    {
        private byte[] _inputMask = new byte[16];
        private byte[] _stateMask = new byte[4];
        private byte[] _algorithmMask = new byte[16];

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public double FuelConsumption { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string SpeedConditionFlag { get; set; }

        [XmlElement]
        public double SpeedCondition { get; set; }

        [XmlElement]
        public bool AddressEnabled { get; set; }

        [XmlElement]
        public bool CountMode { get; set; }

        [XmlElement]
        public byte[] AlgorithmMask
        {
            get { return _algorithmMask; }
            set { _algorithmMask = value; }
        }

        [XmlElement]
        public byte[] StateMask
        {
            get { return _stateMask; }
            set { _stateMask = value; }
        }

        [XmlElementAttribute]
        public byte[] InputMask
        {
            get { return _inputMask; }
            set { _inputMask = value; }
        }
    }
}