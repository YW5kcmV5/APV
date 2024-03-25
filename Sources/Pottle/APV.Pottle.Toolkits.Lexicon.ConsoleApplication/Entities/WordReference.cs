using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Namespace = Constants.DictionaryNamespace)]
    public sealed class WordReference
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string Name { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string PartOfSpeech { get; set; }

        [DataMember(Name = "Reference.Word.WordId", IsRequired = true, Order = 2)]
        public long ReferenceId { get; set; }
    }
}