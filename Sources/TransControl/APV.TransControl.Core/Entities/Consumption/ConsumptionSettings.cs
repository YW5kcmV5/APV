using System;
using System.Xml.Serialization;
using APV.Common;

namespace APV.TransControl.Core.Entities.Consumption
{
    [Serializable]
    [XmlRoot]
    public class ConsumptionSettings : ICloneable, IEquatable<ConsumptionSettings>
    {
        /// <summary>
        /// 5 минут
        /// </summary>
        [XmlElement]
        public int LoadTimeLimit { get; set; }

        /// <summary>
        /// 10 литров
        /// </summary>
        [XmlElement]
        public int LoadFuelLimit { get; set; }

        /// <summary>
        /// 5 минут
        /// </summary>
        [XmlElement]
        public int DrainTimeLimit { get; set; }

        /// <summary>
        /// 5 литров
        /// </summary>
        [XmlElement]
        public int DrainFuelLimit { get; set; }

        /// <summary>
        /// Котроль зажигания включён
        /// </summary>
        [XmlElement]
        public bool DrainEngineControl { get; set; }

        [XmlIgnore]
        public string OuterXml
        {
            get { return Serializer.Serialize(this).OuterXml; }
        }

        public ConsumptionSettings()
        {
        }

        public ConsumptionSettings(ConsumptionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Assign(settings);
        }

        public ConsumptionSettings(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException("xml");

            Assign(Serializer.Deserialize<ConsumptionSettings>(xml));
        }

        public void Assign(ConsumptionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            LoadFuelLimit = settings.LoadFuelLimit;
            DrainFuelLimit = settings.DrainFuelLimit;
            LoadTimeLimit = settings.LoadTimeLimit;
            DrainTimeLimit = settings.DrainTimeLimit;
            DrainEngineControl = settings.DrainEngineControl;
        }

        #region ICloneable

        public object Clone()
        {
            return new ConsumptionSettings(this);
        }

        #endregion

        #region IEquatable

        public bool Equals(ConsumptionSettings other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            bool equals = (LoadFuelLimit == other.LoadFuelLimit);
            equals = equals && (DrainFuelLimit == other.DrainFuelLimit);
            equals = equals && (LoadTimeLimit == other.LoadTimeLimit);
            equals = equals && (DrainTimeLimit == other.DrainTimeLimit);
            equals = equals && (DrainEngineControl == other.DrainEngineControl);

            return equals;
        }

        #endregion

        public static readonly ConsumptionSettings Default = new ConsumptionSettings
                                                                 {
                                                                     LoadFuelLimit = 10,
                                                                     LoadTimeLimit = 25,
                                                                     DrainFuelLimit = 5,
                                                                     DrainTimeLimit = 25,
                                                                     DrainEngineControl = true
                                                                 };
    }
}
