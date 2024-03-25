using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Coord", Namespace = Constants.DictionaryNamespace)]
    public sealed class CoordInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public long CoordId { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string Name { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public string Description { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public CoordType Type { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public CoordValueInfo[] Values { get; set; }
    }
}