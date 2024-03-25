using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace APV.TransControl.FuelViewer
{
    /// <summary>
    /// Класс, обеспечивающий Xml сериализацию и десериализацию данных
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serialize the input object using "XmlSerializer" or "DataContractSerializer"
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="source">The serialazable object</param>
        /// <returns>XML document containing the serialized data</returns>
        public static XDocument Serialize<T>(T source)
        {
            if (ReferenceEquals(source, null))
                throw new ArgumentNullException("source");

            var doc = new XDocument();
            using (XmlWriter writer = doc.CreateWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(writer, source);
                return doc;
            }
        }

        public static object Deserialize(XDocument doc, Type type)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            using (XmlReader reader = doc.CreateReader())
            {
                return new XmlSerializer(type).Deserialize(reader);
            }
        }

        public static T Deserialize<T>(XDocument doc)
        {
            return (T)Deserialize(doc, typeof(T));
        }

        public static object Deserialize(string xml, Type type)
        {
            if (string.IsNullOrEmpty("xml"))
                throw new ArgumentNullException("xml");

            var doc = XDocument.Parse(xml);
            return Deserialize(doc, type);
        }

        public static object Deserialize(Stream stream, Type type)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var doc = XDocument.Load(stream);
            return Deserialize(doc, type);
        }

        public static T Deserialize<T>(string xml)
        {
            return (T)Deserialize(xml, typeof(T));
        }

        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        public static byte[] BinarySerialize(object source, bool compress = false)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return (compress)
                           ? Utility.CompressBytes(stream.ToArray())
                           : stream.ToArray();
            }
        }
        
        public static T BinaryDeserialize<T>(byte[] data, bool compressed = false)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream((compressed) ? Utility.DecompressBytes(data) : data))
            {
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}