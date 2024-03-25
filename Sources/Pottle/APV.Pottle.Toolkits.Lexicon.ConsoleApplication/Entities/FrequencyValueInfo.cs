using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "FrequencyValue", Namespace = Constants.DictionaryNamespace)]
    public sealed class FrequencyValueInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public FrequencyReference Frequency { get; set; }

        /// <summary>
        /// Оригинальное безразмерное значение
        /// </summary>
        [DataMember(IsRequired = true, Order = 1)]
        public double Value { get; set; }

        /// <summary>
        /// IPM (вхождений на миллион слов, Instances Per Million words)
        /// </summary>
        [DataMember(IsRequired = true, Order = 2)]
        public double IPM { get; set; }
    }
}