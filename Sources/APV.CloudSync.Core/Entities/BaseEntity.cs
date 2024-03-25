using System.Runtime.Serialization;
using APV.CloudSync.Common;
using APV.Common;

namespace APV.CloudSync.Core.Entities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public abstract class BaseEntity : XmlObject
    {
    }
}