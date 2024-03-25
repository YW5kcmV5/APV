using System.Collections.Generic;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class AddressManagementTests
    {
        [TestMethod]
        public void CreateAddressTest()
        {
            const string address = "г.Санкт-Петербург, ул. Дыбенко, дом 12, корпус 1";
            const string flatNumber1 = "123";
            const string flatNumber2 = "124";
            const int floor = 4;

            AddressEntity entity1 = AddressManagement.Instance.CreateFlat(address, flatNumber1, floor);

            Assert.IsNotNull(entity1);
            Assert.IsFalse(entity1.IsNew);
            Assert.IsNotNull(entity1.Location);
            Assert.IsFalse(entity1.Location.IsNew);

            Assert.AreEqual(flatNumber1, entity1.Position);
            Assert.AreEqual(floor, entity1.Floor);
            Assert.AreEqual(AddressPositionType.Apartments, entity1.PositionType);

            AddressEntity entity2 = AddressManagement.Instance.CreateFlat(address, flatNumber2, floor);

            Assert.IsNotNull(entity2);
            Assert.IsFalse(entity2.IsNew);
            Assert.IsNotNull(entity2.Location);
            Assert.IsFalse(entity2.Location.IsNew);

            Assert.AreEqual(flatNumber2, entity2.Position);
            Assert.AreEqual(floor, entity2.Floor);
            Assert.AreEqual(AddressPositionType.Apartments, entity2.PositionType);

            Assert.AreEqual(entity1.LocationId, entity2.LocationId);
        }

        [TestMethod]
        public void ParseAddresswithPlacementTest()
        {
            const string address = @"198188, г. Санкт-Петербург, ул.Васи Алексеева, д.6, лит.А, пом. 4Н";
            const string formattedAddress = @"198188, Россия, город Санкт-Петербург, улица Васи Алексеева, дом 6, помещение 4Н";

            AddressEntity entity = AddressManagement.Instance.Create(address);

            Assert.IsNotNull(entity);
            Assert.IsFalse(entity.IsNew);
            Assert.IsNotNull(entity.Location);
            Assert.IsFalse(entity.Location.IsNew);

            Assert.AreEqual(formattedAddress, entity.Address);

            Assert.AreEqual(AddressPositionType.Placement, entity.PositionType);
            Assert.AreEqual("4Н", entity.Position);
            Assert.AreEqual(null, entity.Floor);
            Assert.AreEqual(null, entity.Porch);
            Assert.AreEqual(null, entity.Description);
        }

        [TestMethod]
        public void ParseAddressWithOfficeTest()
        {
            const string address = @"634050, г. Томск, ул. Беленца, д. 17, оф. 57.";
            const string formattedAddress = @"634050, Россия, Томская область, город Томск, улица Алексея Беленца, дом 17, офис 57";

            AddressEntity entity = AddressManagement.Instance.Create(address);

            Assert.IsNotNull(entity);
            Assert.IsFalse(entity.IsNew);
            Assert.IsNotNull(entity.Location);
            Assert.IsFalse(entity.Location.IsNew);

            Assert.AreEqual(formattedAddress, entity.Address);

            Assert.AreEqual(AddressPositionType.Office, entity.PositionType);
            Assert.AreEqual("57", entity.Position);
            Assert.AreEqual(null, entity.Floor);
            Assert.AreEqual(null, entity.Porch);
            Assert.AreEqual(null, entity.Description);
        }

        [TestMethod]
        public void ParseLocationTest()
        {
            var addresses = new List<string[]>
                {
                    new[]
                        {
                            @"620010, г Екатеринбург, ул Черняховского, д 86, корп 8",
                            @"620010, Россия, Свердловская область, город Екатеринбург, улица Черняховского, дом 86, корпус 8"
                        },
                    new[]
                        {
                            @"г.Санкт-Петербург, ул. Гельсингфорсская д.3",
                            @"194044, Россия, город Санкт-Петербург, Гельсингфорсская улица, дом 3"
                        },
                    new[]
                        {
                            @"115114 г. Москва, 1-ый Кожевнический пер., д. 8",
                            @"115114, Россия, город Москва, 1-й Кожевнический переулок, дом 8"
                        },
                    new[]
                        {
                            @"улица Губина, 20, Санкт-Петербург, Россия, 198099",
                            @"198099, Россия, город Санкт-Петербург, улица Губина, дом 20"
                        }
                };

            foreach (string[] addressItems in addresses)
            {
                string address = addressItems[0];
                string formattedAddress = addressItems[1];

                AddressEntity entity = AddressManagement.Instance.Create(address);

                Assert.IsNotNull(entity);
                Assert.IsFalse(entity.IsNew);
                Assert.IsNotNull(entity.Location);
                Assert.IsFalse(entity.Location.IsNew);

                Assert.AreEqual(formattedAddress, entity.Address);

                Assert.AreEqual(AddressPositionType.None, entity.PositionType);
                Assert.AreEqual(null, entity.Position);
                Assert.AreEqual(null, entity.Floor);
                Assert.AreEqual(null, entity.Porch);
                Assert.AreEqual(null, entity.Description);
            }
        }
    }
}