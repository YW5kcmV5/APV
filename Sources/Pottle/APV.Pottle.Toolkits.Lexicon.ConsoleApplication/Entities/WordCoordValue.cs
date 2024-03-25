using System.Diagnostics;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Namespace = Constants.DictionaryNamespace)]
    [DebuggerDisplay("{Coord.Name}({ValueType}):{StringValue}({Value})")]
    public class WordCoordValue
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 0)]
        public CoordReference Coord { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 1)]
        public CoordValueType ValueType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public int Value { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
        public string StringValue { get; set; }
    }
}