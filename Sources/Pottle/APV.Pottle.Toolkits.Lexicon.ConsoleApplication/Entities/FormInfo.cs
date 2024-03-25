using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Form", Namespace = Constants.DictionaryNamespace)]
    public sealed class FormInfo
    {
        [DataMember(IsRequired = true, Order = 0)]
        public WordCoordValue[] Coords { get; set; }
    }
}