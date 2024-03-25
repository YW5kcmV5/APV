using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Lemma", Namespace = Constants.DictionaryNamespace)]
    public sealed class LemmaInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string PartOfSpeech { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public FormInfo NormalForm { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public LemmaEntryInfo[] Entries { get; set; }
    }
}