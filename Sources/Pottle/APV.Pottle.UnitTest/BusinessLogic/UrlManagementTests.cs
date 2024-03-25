using System;
using APV.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.BusinessLogic.Extensions;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class UrlManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void FormatUrlTest()
        {
            UrlManagement management = UrlManagement.Instance;

            const string url = @"https://translate.GooGle.ru/#en/ru/VerifiedAt";
            string formattedUrl = management.FormatUrl(url);
            Assert.AreEqual(@"https://translate.google.ru/#en/ru/VerifiedAt", formattedUrl);
        }

        [TestMethod]
        public void GetHostTest()
        {
            UrlManagement management = UrlManagement.Instance;

            const string url = @"http://www.mytoys.ru/%D0%A2%D0%BE%D0%BC%D0%B8%D0%BA-%D0%9A%D1%83%D0%B1%D0%B8%D0%BA%D0%B8-%D0%A4%D1%80%D1%83%D0%BA%D1%82%D1%8B-%D1%8F%D0%B3%D0%BE%D0%B4%D1%8B-4-%D1%88%D1%82%D1%83%D0%BA%D0%B8-%D0%A2%D0%BE%D0%BC%D0%B8%D0%BA/%D0%94%D0%B5%D1%80%D0%B5%D0%B2%D1%8F%D0%BD%D0%BD%D1%8B%D0%B5-%D0%B8%D0%B3%D1%80%D1%8B-%D0%B8-%D0%BF%D0%B0%D0%B7%D0%BB%D1%8B/%D0%94%D0%B5%D1%80%D0%B5%D0%B2%D1%8F%D0%BD%D0%BD%D1%8B%D0%B5-%D0%B8%D0%B3%D1%80%D1%83%D1%88%D0%BA%D0%B8/KID/ru-mt.to.ca33.07.03/3650279";

            UrlEntity entity = management.Create(url);

            Assert.IsNotNull(entity);

            UrlEntity host = entity.HostUrl;

            Assert.IsNotNull(host);
            Assert.IsNull(host.HostUrl);

            UrlCollection children = host.Children;

            Assert.IsNotNull(children);
            Assert.IsTrue(children.Count > 0);
            Assert.IsTrue(children.Contains(entity));
        }

        [TestMethod]
        public void CreateTest()
        {
            UrlManagement management = UrlManagement.Instance;

            const string url = @"www.canpolbabies.com";

            UrlEntity entity = management.Create(url);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.UrlId != SystemConstants.UnknownId);
            Assert.IsTrue(entity.HostUrlId == null);

            entity.Verify();

            Assert.IsNotNull(entity.Alive);

            UrlEntity loadedByUrlEntity = management.Find(url);
            UrlEntity loadedByIdEntity = management.Find(entity.UrlId);

            Assert.IsNotNull(loadedByUrlEntity);
            Assert.IsNotNull(loadedByIdEntity);

            Assert.IsTrue(entity.Equals(loadedByUrlEntity));
            Assert.IsTrue(entity.Equals(loadedByIdEntity));
        }

        [TestMethod]
        public void VerifyAllTest()
        {
            UrlManagement management = UrlManagement.Instance;

            DateTime now = DateTime.UtcNow.AddDays(1);
            
            management.Verify(now);
        }
    }
}