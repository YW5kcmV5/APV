using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Language", Namespace = Constants.DictionaryNamespace)]
    public sealed class LanguageInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public int LanguageId { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string Code { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string Name { get; set; }
    }
}