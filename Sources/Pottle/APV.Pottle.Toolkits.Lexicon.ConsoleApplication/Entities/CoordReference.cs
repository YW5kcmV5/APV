using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Namespace = Constants.DictionaryNamespace)]
    public sealed class CoordReference
    {
        [DataMember(IsRequired = true, Order = 0)]
        public string Name { get; set; }

        [DataMember(Name = "Reference.Coord.CoordId", IsRequired = true, Order = 1)]
        public long ReferenceId { get; set; }
    }
}