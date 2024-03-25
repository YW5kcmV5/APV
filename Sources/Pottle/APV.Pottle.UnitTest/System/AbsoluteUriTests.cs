using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class AbsoluteUriTests
    {
        [TestMethod]
        public void SuccessTest()
        {
            const string url = @"http://translate.google.ru/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual("?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Query);
            Assert.AreEqual("http://translate.google.ru/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsTrue(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(80, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessDomainOnlyTest()
        {
            const string url = @"www.canpolbabies.com";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("www.canpolbabies.com", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual(string.Empty, absoluteUri.Path);
            Assert.AreEqual(string.Empty, absoluteUri.Query);
            Assert.AreEqual("http://www.canpolbabies.com", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsTrue(absoluteUri.IsDefaultSchema);
            Assert.IsTrue(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(80, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessWithoutQueryTest()
        {
            const string url = @"hTtp://trAnslate.google.ru/#en/ru/verifiedat/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual(string.Empty, absoluteUri.Query);
            Assert.AreEqual("http://translate.google.ru/#en/ru/verifiedat", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsTrue(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(80, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessHostTest()
        {
            const string url = @"hTtp://trAnslate.google.ru/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual(string.Empty, absoluteUri.Path);
            Assert.AreEqual(string.Empty, absoluteUri.Query);
            Assert.AreEqual("http://translate.google.ru", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsTrue(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(80, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessSslTest()
        {
            const string url = @"https://translate.google.ru/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("https://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual("?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Query);
            Assert.AreEqual("https://translate.google.ru/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsFalse(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(443, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessWithDefaultPortNumberTest()
        {
            const string url = @"http://translate.google.ru:80/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual("?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Query);
            Assert.AreEqual("http://translate.google.ru/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsTrue(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(80, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessWithDefaultSslPortNumberTest()
        {
            const string url = @"https://translate.google.ru:443/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("https://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(string.Empty, absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual("?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Query);
            Assert.AreEqual("https://translate.google.ru/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Url);

            Assert.IsTrue(absoluteUri.IsDefaultPort);
            Assert.IsFalse(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(443, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessWithPortNumberTest()
        {
            const string url = @"http://translate.google.ru:9090/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(":9090", absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual("?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Query);
            Assert.AreEqual("http://translate.google.ru:9090/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Url);

            Assert.IsFalse(absoluteUri.IsDefaultPort);
            Assert.IsTrue(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(9090, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessSslWithPortNumberTest()
        {
            const string url = @"https://translate.google.ru:9090/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("https://", absoluteUri.Schema);
            Assert.AreEqual("translate.google.ru", absoluteUri.Host);
            Assert.AreEqual(":9090", absoluteUri.Port);
            Assert.AreEqual("#en/ru/verifiedat", absoluteUri.Path);
            Assert.AreEqual("?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Query);
            Assert.AreEqual("https://translate.google.ru:9090/#en/ru/verifiedat/?Address=wedfwehfelwkfhqwle&s=ssss", absoluteUri.Url);

            Assert.IsFalse(absoluteUri.IsDefaultPort);
            Assert.IsFalse(absoluteUri.IsDefaultSchema);
            Assert.IsFalse(absoluteUri.IsDefaultDomain);
            Assert.AreEqual(9090, absoluteUri.PortNumber);
        }

        [TestMethod]
        public void SuccessWithColonTest()
        {
            const string url = @"http://www.votonia.ru/igra-nastolnaya-nordplast-dvustoronniiy-labirint-smeshariki:-sportsostyazaniya/spb/82048/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("www.votonia.ru", absoluteUri.Host);
        }

        [TestMethod]
        public void SuccessWithRussinCharTest()
        {
            const string url = @"http://www.votonia.ru/igrushka-iz-dereva-mdi-labirint-№-7/spb/2100/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("www.votonia.ru", absoluteUri.Host);
        }

        [TestMethod]
        public void CombinerUrl()
        {
            const string domainUrl = @"http://www.votonia.ru/igrushka-dlya-vanni-uf-ribka-plavayushaya-z-fish-podsvetka-led/spb/86952/";
            const string relativeUrl = @"/images/pictures/86952_553ee2a635b52_small.jpg";

            var domainUri = new AbsoluteUri(domainUrl);
            var relativeUri = new AbsoluteUri(domainUri, relativeUrl);

            Assert.AreEqual("http://", relativeUri.Schema);
            Assert.AreEqual("www.votonia.ru", relativeUri.Host);
            Assert.AreEqual("", relativeUri.Port);
            Assert.AreEqual(80, relativeUri.PortNumber);
            Assert.AreEqual("images/pictures/86952_553ee2a635b52_small.jpg", relativeUri.Path);
            Assert.AreEqual("", relativeUri.Query);
            Assert.AreEqual("http://www.votonia.ru/images/pictures/86952_553ee2a635b52_small.jpg", relativeUri.Url);
        }

        [TestMethod]
        public void CombinerWithColonUrl()
        {
            const string domainUrl = @"http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=150";
            const string relativeUrl = @"/igra-nastolnaya-nordplast-dvustoronniiy-labirint-smeshariki:-sportsostyazaniya/spb/82048/";

            var domainUri = new AbsoluteUri(domainUrl);
            var relativeUri = new AbsoluteUri(domainUri, relativeUrl);

            Assert.AreEqual("http://", relativeUri.Schema);
            Assert.AreEqual("www.votonia.ru", relativeUri.Host);
            Assert.AreEqual("", relativeUri.Port);
            Assert.AreEqual(80, relativeUri.PortNumber);
            Assert.AreEqual("igra-nastolnaya-nordplast-dvustoronniiy-labirint-smeshariki:-sportsostyazaniya/spb/82048", relativeUri.Path);
            Assert.AreEqual("", relativeUri.Query);
            Assert.AreEqual("http://www.votonia.ru/igra-nastolnaya-nordplast-dvustoronniiy-labirint-smeshariki:-sportsostyazaniya/spb/82048", relativeUri.Url);
        }

        [TestMethod]
        public void DataContractSerializationText()
        {
            const string url = @"http://www.votonia.ru/igrushka-iz-dereva-mdi-labirint-№-7/spb/2100/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("www.votonia.ru", absoluteUri.Host);

            XmlDocument doc = Serializer.Serialize(absoluteUri, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(doc);

            var restoredUrl = Serializer.Deserialize<AbsoluteUri>(doc, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(restoredUrl);
            Assert.IsTrue(restoredUrl.Equals(absoluteUri));

            const string urlOnly = @"<AbsoluteUri xmlns=""APV.System.Data""><Value>http://www.votonia.ru/igrushka-iz-dereva-mdi-labirint-№-7/spb/2100/</Value></AbsoluteUri>";

            restoredUrl = Serializer.Deserialize<AbsoluteUri>(urlOnly, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(restoredUrl);
            Assert.IsTrue(restoredUrl.Equals(absoluteUri));
            Assert.AreEqual(absoluteUri.SourceUrl, restoredUrl.SourceUrl);
            Assert.AreEqual(absoluteUri.Url, restoredUrl.Url);
        }

        [TestMethod]
        public void XmlSerializationText()
        {
            const string url = @"http://www.votonia.ru/igrushka-iz-dereva-mdi-labirint-№-7/spb/2100/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual("http://", absoluteUri.Schema);
            Assert.AreEqual("www.votonia.ru", absoluteUri.Host);

            XmlDocument doc = Serializer.Serialize(absoluteUri);

            Assert.IsNotNull(doc);

            var restoredUrl = Serializer.Deserialize<AbsoluteUri>(doc);

            Assert.IsNotNull(restoredUrl);
            Assert.IsTrue(restoredUrl.Equals(absoluteUri));

            const string urlOnly = @"<AbsoluteUri xmlns=""APV.System.Data""><Value>http://www.votonia.ru/igrushka-iz-dereva-mdi-labirint-№-7/spb/2100/</Value></AbsoluteUri>";

            restoredUrl = Serializer.Deserialize<AbsoluteUri>(urlOnly);

            Assert.IsNotNull(restoredUrl);
            Assert.IsTrue(restoredUrl.Equals(absoluteUri));
            Assert.AreEqual(absoluteUri.SourceUrl, restoredUrl.SourceUrl);
            Assert.AreEqual(absoluteUri.Url, restoredUrl.Url);
        }
    }
}