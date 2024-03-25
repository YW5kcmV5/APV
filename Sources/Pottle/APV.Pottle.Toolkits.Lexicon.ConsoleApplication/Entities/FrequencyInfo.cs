using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Frequency", Namespace = Constants.DictionaryNamespace)]
    public sealed class FrequencyInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public long FrequencyId { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string Name { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public string Url { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string Description { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public double K { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public int WordsCount { get; set; }
    }
}