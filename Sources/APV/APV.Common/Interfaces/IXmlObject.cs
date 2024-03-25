using System.Xml;

namespace APV.Common.Interfaces
{
    public interface IXmlObject
    {
        XmlDocument Serialize();

        string OuterXml { get; }
    }
}