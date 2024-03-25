using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class ConverterTests
    {
        private enum SimpleEnum
        {
            X,
            Y
        }

        private enum XmlEnum
        {
            [EnumMember(Value = "X0")]
            X,

            [EnumMember(Value = "Y0")]
            Y
        }

        [TestMethod]
        public void ConvertNullTest()
        {
            string stringValue = ((object)null).ToXmlString();
            Assert.AreEqual(null, stringValue);

            object restored = Converter.FromXmlString(stringValue, typeof(object));
            Assert.AreEqual(stringValue, restored);
        }

        [TestMethod]
        public void ConvertDateTimeTest()
        {
            var value = new DateTime(2014, 01, 01);
            string stringValue = value.ToXmlString();
            Assert.AreEqual("2014-01-01T00:00:00.0000000", stringValue);

            var restored = (DateTime)Converter.FromXmlString(stringValue, typeof(DateTime));
            Assert.AreEqual(value, restored);
        }

        [TestMethod]
        public void ConvertNullableDateTimeTest()
        {
            DateTime? value = new DateTime(2014, 01, 01);
            string stringValue = value.ToXmlString();
            Assert.AreEqual("2014-01-01T00:00:00.0000000", stringValue);

            var restored = (DateTime?)Converter.FromXmlString(stringValue, typeof(DateTime?));
            Assert.AreEqual(value, restored);
        }

        [TestMethod]
        public void ConvertEnumTest()
        {
            const SimpleEnum value = SimpleEnum.X;

            string stringValue = value.ToXmlString();
            Assert.AreEqual("X", stringValue);

            var restored = (SimpleEnum)Converter.FromXmlString(stringValue, typeof(SimpleEnum));
            Assert.AreEqual(value, restored);

            stringValue = ((object)value).ToXmlString();
            Assert.AreEqual("X", stringValue);
        }

        [TestMethod]
        public void ConvertXmlEnumTest()
        {
            const XmlEnum value = XmlEnum.X;

            string stringValue = value.ToXmlString();
            Assert.AreEqual("X0", stringValue);

            var restored = (XmlEnum)Converter.FromXmlString(stringValue, typeof(XmlEnum));
            Assert.AreEqual(value, restored);

            stringValue = ((object)value).ToXmlString();
            Assert.AreEqual("X0", stringValue);
        }

        [TestMethod]
        public void ConvertByteArrayTest()
        {
            var value = new byte[10];

            string stringValue = value.ToXmlString();
            Assert.AreEqual(Convert.ToBase64String(value), stringValue);

            stringValue = ((object)value).ToXmlString();
            Assert.AreEqual(Convert.ToBase64String(value), stringValue);
        }

        [TestMethod]
        public void ConvertEmptyByteArrayTest()
        {
            var value = new byte[0];

            string stringValue = value.ToXmlString();
            Assert.AreEqual(Convert.ToBase64String(value), stringValue);

            stringValue = ((object)value).ToXmlString();
            Assert.AreEqual(Convert.ToBase64String(value), stringValue);
        }
    }
}
