using System.Net;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class IPAddressManagementTests
    {
        public static readonly IPAddress[] IPAddresses = new[]
            {
                IPAddress.Parse("212.119.177.5"), IPAddress.Parse("92.62.52.247")
            };

        [TestMethod]
        public void Create()
        {
            foreach (IPAddress ip in IPAddresses)
            {
                IPAddressEntity entity = IPAddressManagement.Instance.Find(ip);

                Assert.IsNotNull(entity);
                Assert.IsNotNull(entity.Location);
                Assert.IsNotNull(entity.Country);

                Assert.AreEqual("RU", entity.Country.Code);
                Assert.AreEqual(ip.ToString(), entity.Value);
            }
        }
    }
}
