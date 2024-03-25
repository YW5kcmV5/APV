using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Statistics", Namespace = Constants.DictionaryNamespace)]
    public sealed class StatisticsInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public int EntriesCount { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public int WordsCount { get; set; }
    }
}