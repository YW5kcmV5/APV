using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using APV.Common.Attributes;

namespace APV.Common    
{
    public static class Serializer
    {
        /// <summary>
        /// Тип сериализатора
        /// </summary>
        [Serializable]
        [DataContract]
        public enum Type
        {
            [EnumMember]
            DataContractSerializer,

            [EnumMember]
            XmlSerializer,
        }

        /// <summary>
        /// Serialize the input object using "XmlSerializer" or "DataContractSerializer"
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="source">The serialazable object</param>
        /// <param name="encoding">The output xml encoding</param>
        /// <param name="serializer">Serialization type</param>
        /// <returns>XML document containing the serialized data</returns>
        public static XmlDocument Serialize<T>(T source, Encoding encoding, Type serializer = Type.XmlSerializer)
        {
            if (ReferenceEquals(source, null))
                throw new ArgumentNullException("source");

            var sb = new StringBuilder();
            var doc = new XmlDocument();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                Indent = true
            };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                if (serializer == Type.XmlSerializer)
                {
                    new XmlSerializer(typeof(T)).Serialize(writer, source);
                    writer.Flush();
                    doc.LoadXml(sb.ToString());
                }
                else
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(T));
                    DataContractNamespaceAttribute[] namespaces = DataContractNamespaceAttribute.GetAttirbutes(typeof(T));
                    if ((namespaces != null) && (namespaces.Length > 0))
                    {
                        dataContractSerializer.WriteStartObject(writer, source);
                        foreach (DataContractNamespaceAttribute xmlns in namespaces)
                        {
                            writer.WriteAttributeString("xmlns", xmlns.Prefix, string.Empty, xmlns.Uri);
                        }
                        dataContractSerializer.WriteObjectContent(writer, source);
                        dataContractSerializer.WriteEndObject(writer);
                    }
                    else
                    {
                        dataContractSerializer.WriteObject(writer, source);
                    }

                    writer.Flush();
                    doc.LoadXml(sb.ToString());
                }
                return XmlUtility.CanonizeXmlDocument(doc, encoding);
            }
        }

        public static XmlDocument Serialize<T>(T source, Type serializer = Type.XmlSerializer)
        {
            return Serialize(source, Encoding.UTF8, serializer);
        }

        public static object Deserialize(System.Type type, XmlDocument doc, Type serializer = Type.DataContractSerializer)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));
            if (doc.DocumentElement == null)
                throw new ArgumentOutOfRangeException(nameof(doc), "doc.DocumentElement can not be null");

            using (var reader = new XmlNodeReader(doc.DocumentElement))
            {
                if (serializer == Type.XmlSerializer)
                {
                    return new XmlSerializer(type).Deserialize(reader);
                }
                return new DataContractSerializer(type).ReadObject(reader);
            }
        }

        public static T Deserialize<T>(XmlDocument doc, Type serializer = Type.XmlSerializer)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");
            if (doc.DocumentElement == null)
                throw new ArgumentOutOfRangeException("doc", "doc.DocumentElement can not be null");

            return (T)Deserialize(typeof(T), doc, serializer);
        }

        public static object Deserialize(System.Type type, string xml, Type serializer = Type.XmlSerializer)
        {
            if (xml == null)
                throw new ArgumentNullException(nameof(xml));
            if (string.IsNullOrWhiteSpace("xml"))
                throw new ArgumentOutOfRangeException(nameof(xml), "Xml is empty or whitespace.");

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return Deserialize(type, doc, serializer);
        }

        public static T Deserialize<T>(string xml, Type serializer = Type.XmlSerializer)
        {
            if (string.IsNullOrEmpty("xml"))
                throw new ArgumentNullException("xml");

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return Deserialize<T>(doc, serializer);
        }

        public static T Deserialize<T>(Stream stream, Type serializer = Type.XmlSerializer)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var doc = new XmlDocument();
            doc.Load(stream);
            return Deserialize<T>(doc, serializer);
        }

        public static void SerializeToFile<T>(T source, string filename, Encoding encoding, Type serializer = Type.XmlSerializer)
        {
            if (ReferenceEquals(source, null))
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Indent = true,
                    Encoding = encoding,
                };
            using (var file = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (XmlWriter writer = XmlWriter.Create(file, settings))
                {
                    if (serializer == Type.XmlSerializer)
                    {
                        new XmlSerializer(typeof(T)).Serialize(writer, source);
                        writer.Flush();
                    }
                    else
                    {
                        var dataContractSerializer = new DataContractSerializer(typeof(T));
                        dataContractSerializer.WriteObject(writer, source);
                        writer.Flush();
                    }
                }
            }
        }

        public static void SerializeToFile<T>(T source, string filename, Type serializer = Type.XmlSerializer)
        {
            SerializeToFile(source, filename, Encoding.UTF8, serializer);
        }

        public static T DeserializeFromFile<T>(string filename, Type serializer = Type.XmlSerializer)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");
            if (!File.Exists(filename))
                throw new ArgumentOutOfRangeException("filename", string.Format("File \"{0}\" does not exist.", filename));

            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    if (serializer == Type.XmlSerializer)
                    {
                        return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                    }
                    return (T)new DataContractSerializer(typeof(T)).ReadObject(reader);
                }
            }
        }
    }
}