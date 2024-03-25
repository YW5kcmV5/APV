using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using APV.Math.Diagrams;
using APV.Math.Diagrams.Devices;
using APV.TransControl.Core.Entities.Collection;
using APV.TransControl.Core.Entities.Consumption;

namespace APV.TransControl.Core.Entities
{
    [XmlRoot]
    [Serializable]
    [DebuggerDisplay("{AvtoNo}-{ObjId} (ObjRecord)")]
    public class ObjRecord : ICloneable
    {
        private string _configData;

        [NonSerialized]
        private string _parsedConfigData;

        [NonSerialized]
        private float[][] _transformationTable;

        [NonSerialized]
        private Diagram _diagram;

        [NonSerialized]
        private ConsumptionInfo[] _consumptions;

        [NonSerialized]
        private ConsumptionInfo _selectedConsumption;

        [XmlElement]
        public int ObjId { get; set; }

        [XmlElement]
        public string AvtoNo { get; set; }

        [XmlElement]
        public string ConfigData
        {
            get { return _configData; }
            set
            {
                _configData = value;
                _transformationTable = ParseMcfgConfig(value, out _parsedConfigData);
            }
        }

        [XmlIgnore]
        public string ParsedConfigData
        {
            get { return _parsedConfigData; }
        }

        [XmlIgnore]
        public float[][] TransformationTable
        {
            get { return _transformationTable; }
        }

        [XmlIgnore]
        public bool HasFuelSensor
        {
            get { return ((ConfigData != null) && (TransformationTable != null)); }
        }

        [XmlElement]
        public ObjDataCollection Data { get; set; }

        [XmlIgnore]
        public ConsumptionInfo[] Consumptions
        {
            get { return _consumptions; }
            set { _consumptions = value; }
        }

        [XmlIgnore]
        public ConsumptionInfo SelectedConsumption
        {
            get { return _selectedConsumption; }
            set { _selectedConsumption = value; }
        }
        
        public override string ToString()
        {
            return $"{AvtoNo} ({ObjId})";
        }

        public static float[][] ParseMcfgConfig(string base64String, out string parsedConfigData)
        {
            parsedConfigData = null;
            byte[] bin = Convert.FromBase64String(base64String);
            string str = Encoding.ASCII.GetString(bin).Replace("\0", "").Replace(" ", "");
            str = string.Concat(str.ToCharArray().Where(@char => "0123456789(),<>=qwertyuiopasdfghjklzxcvbnm.".Contains(@char)));

            int index1 = str.IndexOf("tab(", StringComparison.InvariantCultureIgnoreCase);

            if (index1 == -1)
            {
                str = Encoding.UTF8.GetString(bin).Replace("\0", "").Replace(" ", "");
                str = string.Concat(str.ToCharArray().Where(@char => "0123456789(),<>=qwertyuiopasdfghjklzxcvbnm.".Contains(@char)));
                index1 = str.IndexOf("tab(", StringComparison.InvariantCultureIgnoreCase);
            }

            if (index1 != -1)
            {
                str = str.Substring(index1);
                index1 = str.IndexOf(",", StringComparison.InvariantCultureIgnoreCase);
                if (index1 != -1)
                {
                    str = str.Substring(index1 + 1);
                    int index2 = str.IndexOf(")", StringComparison.InvariantCultureIgnoreCase);
                    if (index2 != -1)
                    {
                        str = str.Substring(0, index2);
                        parsedConfigData = $"tab({str})";
                        string[] stringValues = str.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        int count = stringValues.Length / 2;
                        //var transform = new float[count][];
                        var transform = new List<float[]>();
                        for (int i = 0; i < count; i++)
                        {
                            float freq = float.Parse(stringValues[2 * i].Replace(".", ","));
                            float v = float.Parse(stringValues[2 * i + 1].Replace(".", ","));
                            transform.Add(new[] { freq, v, 0.0f });
                            if (i > 0)
                            {
                                float x2 = freq;
                                float y2 = v;
                                float x1 = transform[i - 1][0];
                                float y1 = transform[i - 1][1];
                                float k = (y2 - y1) / (x2 - x1);
                                transform[i - 1][2] = k;
                            }
                        }

                        if ((transform[0][0] > 0) && (transform[0][1] > 0))
                        {
                            float x2 = transform[0][0];
                            float y2 = transform[0][1];
                            float k = (y2) / (x2);
                            transform.Insert(0, new[] { 0.0f, 0.0f, k });
                        }

                        return transform.ToArray();
                    }
                }
            }

            return null;
        }

        public object Clone()
        {
            return new ObjRecord
                       {
                           ObjId = ObjId,
                           AvtoNo = AvtoNo,
                           ConfigData = ConfigData,
                           Data = (Data != null) ? (ObjDataCollection) Data.Clone() : null,
                       };
        }

        /// <summary>
        /// Преобразовывает частоту в объем топлива (Гц - Литры)
        /// </summary>
        public float FreqToV(float freq)
        {
            float[][] transformationTable = TransformationTable;
            float minFreq = transformationTable[0][0];
            float maxFreq = transformationTable.Last()[0];
            if (freq <= minFreq)
            {
                return transformationTable[0][1];
            }
            if (freq >= maxFreq)
            {
                return transformationTable.Last()[1];
            }

            int count = transformationTable.Length;
            for (int i = 0; i < count - 1; i++)
            {
                float x1 = transformationTable[i][0];
                float x2 = transformationTable[i + 1][0];
                if ((freq > x1) && (freq <= x2))
                {
                    float k = transformationTable[i][2];
                    return (transformationTable[i][1] + k * (freq - transformationTable[i][0]));
                }
            }

            return -1.0f;
        }

        public Diagram GetDiagram(Control container, out bool isCreated)
        {
            isCreated = false;
            if (_diagram == null)
            {
                float[][] transformationTable = TransformationTable;
                float[] x = transformationTable.Select(item => item[0]).ToArray();
                float[] y = transformationTable.Select(item => item[1]).ToArray();
                _diagram = new Diagram(x, y, new SimpleDevice(container));
                isCreated = true;
            }
            return _diagram;
        }
    }
}