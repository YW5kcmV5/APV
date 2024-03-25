using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.WebParsers
{
    [TestClass]
    public class ProductCharacteristicInfoTest
    {
        [TestMethod]
        public void GetModifierTest()
        {
            ProductCharacteristicModifier modifier = ProductCharacteristicInfo.GetModifier(ProductCharacteristicInfo.ColorName);
            Assert.AreEqual(ProductCharacteristicModifier.Color, modifier);

            modifier = ProductCharacteristicInfo.GetModifier(ProductCharacteristicInfo.SizeName);
            Assert.AreEqual(ProductCharacteristicModifier.Size, modifier);

            modifier = ProductCharacteristicInfo.GetModifier(ProductCharacteristicInfo.WeightName);
            Assert.AreEqual(ProductCharacteristicModifier.None, modifier);

            string name = ProductCharacteristicInfo.GetName(ProductCharacteristicModifier.Color);
            Assert.AreEqual(ProductCharacteristicInfo.ColorName, name);

            name = ProductCharacteristicInfo.GetName(ProductCharacteristicModifier.Size);
            Assert.AreEqual(ProductCharacteristicInfo.SizeName, name);

            name = ProductCharacteristicInfo.GetName(ProductCharacteristicModifier.None);
            Assert.IsNull(name);
        }
    }
}