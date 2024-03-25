using System.Diagnostics;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DebuggerDisplay("Entry:{PartOfSpeech}<-{Lemma.Name}(\"{Lemma.PartOfSpeech}\")")]
    [DataContract(Name = "Entry", Namespace = Constants.DictionaryNamespace)]
    public sealed class EntryInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string PartOfSpeech { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public WordReference Lemma { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public FormInfo[] Forms { get; set; }
    }
}