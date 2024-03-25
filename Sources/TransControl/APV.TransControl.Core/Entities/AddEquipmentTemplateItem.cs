using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    [XmlType(Namespace = "TransControl")]
    [XmlRoot(Namespace = "TransControl")]
    //[DBObjectAttribute(DBTableName = "ADD_EQUIPMENT_TEMPLATE", DBUserName = "SYNCH_WL")]
    public class AddEquipmentTemplateItem
    {
        [XmlElement]
        public string TemplateName { get; set; }

        [XmlElement]
        public string AddEquimpmentName { get; set; }
    }
}
