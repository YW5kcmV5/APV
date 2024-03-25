using System.Diagnostics;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DebuggerDisplay("LemmaEntry:{PartOfSpeech}")]
    [DataContract(Name = "Entry", Namespace = Constants.DictionaryNamespace)]
    public sealed class LemmaEntryInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string PartOfSpeech { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public FormInfo[] Forms { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public WordReference[] Children { get; set; }
    }
}