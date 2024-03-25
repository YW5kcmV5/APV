using System;
using System.Linq;
using APV.TransControl.Core.DataLayer;
using APV.TransControl.Core.Entities;
using APV.TransControl.Core.Entities.Consumption;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.TransControl.UnitTest
{
    [TestClass]
    public sealed class DbManagerTests
    {
        public const string TemplateName = "TEST_Template_Name";
        public const string AddEquipmentName = "TEST_AddEquipment_Name";
        public const string AddEquipmentDescription = "TEST_AddEquipment_Description";

        [TestMethod]
        public void InsertUpdateDSTMaskTest()
        {
            var equipment = new AddEquipment
                {
                    Name = AddEquipmentName,
                    Description = AddEquipmentDescription,
                    FuelConsumption = 100.0,
                    StateMask = BitConverter.GetBytes(0),
                };
            equipment.AlgorithmMask[0] = 1;
            equipment.InputMask[0] = 1;
            equipment.StateMask[0] = 1;

            int res = DbManager.InsertUpdateDSTMask(equipment);

            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void DeleteDSTMaskTest()
        {
            InsertUpdateDSTMaskTest();

            int res = DbManager.DeleteDSTMask(AddEquipmentName);

            Assert.AreEqual(0, res);
        }

        [TestMethod]
        public void AddMaskToTemplate()
        {
            InsertUpdateDSTMaskTest();

            int res = DbManager.AddMaskToTemplate(TemplateName, AddEquipmentName);

            Assert.AreEqual(0, res);

            AddEquipmentTemplateItem[] templates = DbManager.ReadAllTemplates();
            AddEquipmentTemplateItem template = templates.SingleOrDefault(row => row.TemplateName == TemplateName);

            Assert.IsNotNull(template);
            Assert.AreEqual(TemplateName, template.TemplateName);
            Assert.AreEqual(AddEquipmentName, template.AddEquimpmentName);
        }

        [TestMethod]
        public void GetTransformationFromConfig()
        {
            //В поле конфиг (MONUSER.MONOBJ.CONFIG) у данной машины есть таблица преобразования частоты датчика топлива в объем
            const string correctAvtoNo = "2280рс78";
            //В поле конфиг (MONUSER.MONOBJ.CONFIG) у данной машины нет таблицы преобразования частоты датчика топлива в объем
            const string wrongAvtoNo = "н046уу178";
            //В поле конфиг (MONUSER.MONOBJ.CONFIG) у данной машины есть таблица преобразования частоты датчика топлива в объем
            const string fakeAvtoNo = "ф456чс178";

            MonObj obj = DbManager.GetObjByAvtoNo(correctAvtoNo);
            Assert.IsNotNull(obj);
            string config = DbManager.GetConfig(obj.Objid);

            Assert.IsNotNull(config);

            string parsedConfigData;
            float[][] data = ObjRecord.ParseMcfgConfig(config, out parsedConfigData);
            Assert.IsNotNull(data);

            obj = DbManager.GetObjByAvtoNo(wrongAvtoNo);
            Assert.IsNotNull(obj);
            config = DbManager.GetConfig(obj.Objid);

            Assert.IsNotNull(config);
            data = ObjRecord.ParseMcfgConfig(config, out parsedConfigData);
            Assert.IsNull(data);

            obj = DbManager.GetObjByAvtoNo(fakeAvtoNo);
            Assert.IsNotNull(obj);
            config = DbManager.GetConfig(obj.Objid);

            Assert.IsNotNull(config);
            data = ObjRecord.ParseMcfgConfig(config, out parsedConfigData);
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void GetVersionTest()
        {
            string version = DbManager.GetOracleVersion();

            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void GetStkDataGpsTest()
        {
            string p_carnumber = "2280рс78";
            DateTime p_regdate = DateTime.Now.AddMonths(-1);
            DateTime p_returndate = DateTime.Now;
            double p_imp_run;
            double p_imp_fueldepletion;
            double p_imp_workeadd;
            double p_imp_fueldepletionadd;
            double p_coord_x;
            double p_coord_y;
            string p_imp_address;
            DateTime p_imp_stardate;
            DateTime p_imp_enddate;
            int p_imp_gpsfault;

            DbManager.GetStkDataGps(p_carnumber, p_regdate, p_returndate, out p_imp_run, out p_imp_fueldepletion, out p_imp_workeadd, out p_imp_fueldepletionadd, out p_coord_x, out p_coord_y, out p_imp_address, out p_imp_stardate, out p_imp_enddate, out p_imp_gpsfault);

            Assert.IsTrue(p_imp_gpsfault >= 0);
        }

        [TestMethod]
        public void GetStkDataDetailGpsTest()
        {
            string p_carnumber = "2280рс78";
            DateTime p_regdate = DateTime.Now.AddMonths(-1);
            DateTime p_returndate = DateTime.Now;
            double p_imp_run;
            double p_imp_fueldepletion;
            double p_imp_workeadd;
            double p_imp_fueldepletionadd;
            double p_coord_x;
            double p_coord_y;
            string p_imp_address;
            DateTime p_imp_stardate;
            DateTime p_imp_enddate;
            string detail;
            int p_imp_gpsfault;

            DbManager.GetStkDataDetailGps(p_carnumber, p_regdate, p_returndate, out p_imp_run, out p_imp_fueldepletion, out p_imp_workeadd, out p_imp_fueldepletionadd, out p_coord_x, out p_coord_y, out p_imp_address, out p_imp_stardate, out p_imp_enddate, out detail, out p_imp_gpsfault);

            Assert.IsTrue(p_imp_gpsfault >= 0);
        }

        [TestMethod]
        public void GetStkDataFuelGpsTest()
        {
            string p_carnumber = "2280рс78";
            DateTime p_regdate = DateTime.Now.AddMonths(-1);
            DateTime p_returndate = DateTime.Now;
            double p_imp_run;
            double p_imp_fueldepletion;
            double p_imp_workeadd;
            double p_imp_fueldepletionadd;
            double p_coord_x;
            double p_coord_y;
            string p_imp_address;
            DateTime p_imp_stardate;
            DateTime p_imp_enddate;
            double p_fuel_onregdate;
            double p_fuel_onreturndate;
            int p_imp_gpsfault;

            DbManager.GetStkDataFuelGps(p_carnumber, p_regdate, p_returndate, out p_imp_run, out p_imp_fueldepletion, out p_imp_workeadd, out p_imp_fueldepletionadd, out p_coord_x, out p_coord_y, out p_imp_address, out p_imp_stardate, out p_imp_enddate, out p_fuel_onregdate, out p_fuel_onreturndate, out p_imp_gpsfault);

            Assert.IsTrue(p_imp_gpsfault >= 0);
        }

        [TestMethod]
        public void GetStkDataFuelXmlTest()
        {
            const string avtoNo = "2280рс78";
            DateTime regDate = DateTime.Now.AddMonths(-1);
            DateTime returnDate = DateTime.Now;

            BaseConsumptionInfo[] consumptions = DbManager.GetStkDataFuelXml(avtoNo, regDate, returnDate);

            Assert.IsNotNull(consumptions);
            Assert.IsNull(consumptions.All(item => item.Equipment));
        }

        [TestMethod]
        public void GetGpsFaultNoFaultTest()
        {
            const string avtoNo = "в114ав178";
            var beginDate = new DateTime(2016, 03, 14, 0, 1, 0);
            var endDate = new DateTime(2016, 03, 14, 23, 59, 0);

            int fault = DbManager.GetGpsFault(avtoNo, beginDate, endDate);

            Assert.AreEqual(0, fault);
        }

        [TestMethod]
        public void GetGpsFaultWithFaultTest()
        {
            const string avtoNo = "в113вн98";
            var beginDate = new DateTime(2016, 03, 14, 0, 1, 0);
            var endDate = new DateTime(2016, 03, 14, 23, 59, 0);

            int fault = DbManager.GetGpsFault(avtoNo, beginDate, endDate);

            Assert.AreEqual(1, fault);
        }
    }
}