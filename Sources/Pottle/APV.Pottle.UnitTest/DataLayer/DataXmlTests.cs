using APV.Pottle.Common;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.DataLayer.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;

namespace APV.Pottle.UnitTest.DataLayer
{
    [TestClass]
    public class DataXmlTests : BaseTests<DataXmlEntity>
    {
        protected override DataXmlEntity CreateEntity()
        {
            return new DataXmlEntity
                {
                    Data = new byte[12],
                    DataStorage = DataStorage.Database,
                    Path = null,
                    SerializerType = Serializer.Type.DataContractSerializer,
                    TypeName = "Test",
                };
        }

        protected override void Modify(DataXmlEntity entity)
        {
            Assert.IsNotNull(entity);
            entity.Data = new byte[13];
            entity.TypeName = "Test-2";
        }
    }
}