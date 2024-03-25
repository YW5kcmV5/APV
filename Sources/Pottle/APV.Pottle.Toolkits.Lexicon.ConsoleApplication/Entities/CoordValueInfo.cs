using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "CoordValue", Namespace = Constants.DictionaryNamespace)]
    public class CoordValueInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string Name { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public int Value { get; set; }
    }
}