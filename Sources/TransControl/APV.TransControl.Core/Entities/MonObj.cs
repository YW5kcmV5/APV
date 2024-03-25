using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    [XmlType(Namespace = "TransControl")]
    [XmlRootAttribute(Namespace = "TransControl")]
    //[DBObjectAttribute(DBTableName = "MONOBJ", DBUserName = "MONUSER")]
    public class MonObj
    {
        //[DBType(OracleType.NUMERIC, Key = true)]

        //[DBType(OracleType.NVARCHAR2, 20, /*Nullable = false,*/ DBFieldName = "AVTO_NO")]

        //[DBType(OracleType.NUMERIC)]

        //[DBType(OracleType.RAW, 4)]

        //[DBType(OracleType.NUMERIC)]

        //[DBType(OracleType.RAW, 4)]

        //[DBType(OracleType.NVARCHAR2, 32, DBFieldName = "PHONE_SMS")]

        //[DBType(OracleType.NVARCHAR2, 32, DBFieldName = "PHONE_DATA")]

        //[DBType(OracleType.NVARCHAR2, 32, DBFieldName = "PHONE_VOICE")]

        //[DBType(OracleType.NVARCHAR2, 32, DBFieldName = "PHONE_GPRS")]

        //[DBType(OracleType.NVARCHAR2, 100, DBFieldName = "RSVRS_SMS")]

        //[DBType(OracleType.NVARCHAR2, 100, DBFieldName = "RSVRS_DATA")]

        //[DBType(OracleType.NVARCHAR2, 100, DBFieldName = "RSVRS_GPRS")]

        //[DBType(OracleType.NVARCHAR2, 15)]

        //[DBType(OracleType.NVARCHAR2, 50)]

        //[DBType(OracleType.NVARCHAR2, 50)]

        //[DBType(OracleType.NVARCHAR2, 20, DBFieldName = "AVTO_VIN")]

        //[DBType(OracleType.NVARCHAR2, 50, DBFieldName = "AVTO_MODEL")]

        //[DBType(OracleType.CLOB, DBFieldName = "DEVICE_INFO")]

        //[DBType(OracleType.CLOB)]

        //[DBType(OracleType.CLOB)]

        //[DBType(OracleType.CLOB)]

        //[DBType(OracleType.BLOB)]

        //[DBType(OracleType.BLOB)]

        //[DBType(OracleType.BLOB)]

        //[DBType(OracleType.BINARY_FLOAT, DBFieldName = "TEST_INTERVAL")]

        private List<string> _addEquipmentNames = new List<string>();
        private List<AddEquipment> _addEquipments = new List<AddEquipment>();

        public static int Count()
        {
            throw new NotImplementedException();
        }

        public static MonObj[] SelectAll()
        {
            throw new NotImplementedException();
        }

        [XmlElement]
        public int Objid { get; set; }

        [XmlElement]
        public int Clientid { get; set; }

        [XmlElement]
        public string Version { get; set; }

        [XmlElement]
        public int Category { get; set; }

        [XmlElement]
        public string Status { get; set; }

        [XmlElement]
        public string Phone_sms { get; set; }

        [XmlElement]
        public string Phone_data { get; set; }

        [XmlElement]
        public string Phone_voies { get; set; }

        [XmlElement]
        public string Phone_gprs { get; set; }

        [XmlElement]
        public string Rsvrs_sms { get; set; }

        [XmlElement]
        public string Rsvrs_data { get; set; }

        [XmlElement]
        public string Rsvrs_gprs { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Fullname { get; set; }

        [XmlElement]
        public string Owner { get; set; }

        [XmlElement]
        public string Avto_no { get; set; }

        [XmlElement]
        public string Avto_vin { get; set; }

        [XmlElement]
        public string Avto_model { get; set; }

        [XmlElement]
        public string Device_info { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string Instruction { get; set; }

        [XmlElement]
        public string Additional { get; set; }

        [XmlElement]
        public string Config { get; set; }

        [XmlElement]
        public string State { get; set; }

        [XmlElement]
        public string Route { get; set; }

        [XmlElement]
        public float Test_interval { get; set; }

        [XmlElement]
        public List<string> AddEquipmentName
        {
            get { return _addEquipmentNames; }
            set { _addEquipmentNames = value; }
        }

        [XmlElement]
        public List<AddEquipment> AddEquipment
        {
            get { return _addEquipments; }
            set { _addEquipments = value; }
        }
    }
}
