using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using APV.GraphicsLibrary.Colors;

namespace APV.GraphicsLibrary.Resources
{
    public static class ResourceManager
    {
        private static XmlDocument LoadXmlFromResource(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type type = typeof(ResourceManager);
            Assembly assembly = type.Assembly;
            string resourceNamespace = type.Namespace;
            string resourceName = $"{resourceNamespace}.{name}";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unknown resource name \"{name}\".");

                var doc = new XmlDocument();
                doc.Load(stream);
                return doc;
            }
        }

        private static XslCompiledTransform LoadXslFromResource(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type type = typeof(ResourceManager);
            Assembly assembly = type.Assembly;
            string resourceNamespace = type.Namespace;
            string resourceName = $"{resourceNamespace}.{name}";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unknown resource name \"{name}\".");

                var xsl = new XslCompiledTransform();
                using (var reader = XmlReader.Create(stream))
                {
                    xsl.Load(reader);
                    return xsl;
                }
            }
        }

        /// <summary>
        /// "MnemonicColors.xml"
        /// </summary>
        public const string MnemonicColorsName = @"MnemonicColors.xml";

        public static readonly XmlDocument MnemonicColorsXml = LoadXmlFromResource(MnemonicColorsName);

        public static readonly MnemonicColors MnemonicColors = MnemonicColors.Deserialize(MnemonicColorsXml, true);
    }
}