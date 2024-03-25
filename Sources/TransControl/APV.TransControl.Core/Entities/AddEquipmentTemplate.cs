using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    public class AddEquipmentTemplate
    {
        private AddEquipment[] _addEquipment = new AddEquipment[0];

        [XmlElement]
        public string TemplateName { get; set; }

        [XmlElement]
        public AddEquipment[] Equipment
        {
            get { return _addEquipment; }
            set { _addEquipment = value; }
        }

        [XmlElement]
        public string[] AddEquipmentName
        {
            get { return _addEquipment.Select(t => t.Name).ToArray(); }
        }

        public void AddEquipment(AddEquipment equipment)
        {
            if (equipment != null)
            {
                for (int i = 0; i < _addEquipment.Length; i++)
                {
                    if (_addEquipment[i].Name == equipment.Name)
                    {
                        _addEquipment[i] = equipment;
                        return;
                    }
                }
                List<AddEquipment> list = new List<AddEquipment>(_addEquipment);
                list.Add(equipment);
                _addEquipment = list.ToArray();
            }
        }

        public void DelEquipment(AddEquipment equipment)
        {
            if (equipment != null)
            {
                for (int i = 0; i < _addEquipment.Length; i++)
                {
                    if (_addEquipment[i].Name == equipment.Name)
                    {
                        var list = new List<AddEquipment>(_addEquipment);
                        list.RemoveAt(i);
                        _addEquipment = list.ToArray();
                        return;
                    }
                }
            }
        }

        public static AddEquipmentTemplate[] CollectTemplates(AddEquipment[] equipments, AddEquipmentTemplateItem[] templates)
        {
            var templateList = new SortedList<string, AddEquipmentTemplate>();
            for (int i = 0; i < templates.Length; i++)
            {
                if (!templateList.ContainsKey(templates[i].TemplateName))
                {
                    for (int j = 0; j < equipments.Length; j++)
                    {
                        if (equipments[j].Name == templates[i].AddEquimpmentName)
                        {
                            var template = new AddEquipmentTemplate
                                {
                                    TemplateName = templates[i].TemplateName
                                };
                            template.AddEquipment(equipments[j]);
                            templateList.Add(template.TemplateName, template);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < equipments.Length; j++)
                    {
                        if (equipments[j].Name == templates[i].AddEquimpmentName)
                        {
                            AddEquipmentTemplate template = templateList[templates[i].TemplateName];
                            template.AddEquipment(equipments[j]);
                        }
                    }
                }
            }
            var list = new List<AddEquipmentTemplate>(templateList.Values);
            return list.ToArray();
        }
    }
}