using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APV.Common.Extensions;
using APV.DatabaseAccess.Providers;
using APV.EntityFramework.DataLayer;
using APV.TransControl.Common;
using APV.TransControl.Core.DataLayer;
using APV.TransControl.Core.Entities;

namespace APV.Vodokanal.Test.ConsoleApplication
{
    public static class SimpleOracleConnector
    {
        //public const string ConnectionString = @"Direct=true;Host=192.168.58.24;Port=1521;SID=tran;uid=synch_wl;Password=123;Unicode=True;Connection Timeout=10;Provider=Oracle;";
        //public const string ConnectionString = @"Direct=true;Host=192.168.58.24;Port=1521;SID=tran;uid=monuser;Password=monuser9;Unicode=True;Connection Timeout=10;Provider=Oracle;";
        //public const string ConnectionString = @"Direct=true;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.58.24)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=TRAN)));uid=monuser;password=monuser9;Unicode=True;Connection Timeout=3;

        public const string Host        = "192.168.58.24";
        public const int    Port        = 1521;
        public const string DbName      = "TRAN";
        public const string Username = "MONUSER";  //"synch_wl"    "SYNCH_WL"      "MONUSER"     "monuser"
        public const string Password = "monuser9";       //"123"         "synch_wl"      "monuser9"    "mon"

        public static void Execute()
        {
            try
            {
                //using (var connection = new DbSqlConnection(ConnectionString))
                //{
                //    connection.Open();
                //    int objId = GetObjIdByAvtoNo(connection, "в640ое98");
                //
                //var adresses = DbManager.ReadAllNumbers();

                //string sql0 = "SELECT XMLTYPE('<?xml version=\"1.0\" encoding=\"windows-1251\"?><response/>').getStringVal() FROM DUAL";
                //var result0 = DbManager.Execute(sql0);

                //var x = DbManager.GetStkDataFuelXml("в640ое98", DateTime.Now.AddMonths(-1).Date, DateTime.Now);
                //int objId = DbManager.GetObjIdByAvtoNo("в640ое98");
                //var y = DbManager.GetFuel(245, new DateTime(2015, 06, 01), DateTime.Now);

                //SYNCH_WL.WL_PROC.GetMainDataFromEverestByAvtoNo
                //SELECT text FROM all_source WHERE owner = 'SYNCH_WL' AND name = 'GetMainDataFromEverestByAvtoNo' AND type  = 'PACKAGE BODY' ORDER BY line

                //string connectionString = ContextManager.ConnectionSettings.ConnectionString;

                string sycnhWlProc = DbManager.GetWlProcPackage();
                string vehicles = DbManager.GetFuelSensorVehicles();
                //string[] sycnWlTablesNames = DbManager.GetSycnWlTablesNames();

                //const string testAvtoNo = "2287рс78";
                const string testAvtoNo = "2145ре78";

                double moto;
                double fuel;
                string equipments;
                string motoDetails;
                string fuelDetails;

                DbManager.CalcDetailDSTMotoByAvtoNo(testAvtoNo, DateTime.Now.AddMonths(-1), DateTime.Now, out moto, out fuel,
                    out equipments, out motoDetails, out fuelDetails);

                DbManager.UpdateEverstLink(testAvtoNo);
                var testAvtoNoEverestDbLink = DbManager.GetDBLinkNameToEverest(testAvtoNo);

                EverestMainData everestMainData0 = DbManager.GetMainDataFromEverestByAvtoNo(testAvtoNo, DateTime.Now);

                string query = string.Format("SELECT MODEL, VEHICLETYPE, ADMINNAME, FUELCONSUMPTION, WORKGRAPHTYPENAME, DINNERTIME FROM SYNCH_WL.V_CARDTRANSP@{0} WHERE (LOWER(TRIM(REPLACE(REPLACE(CARNUMBER, ' ', ''), '-', ''))) = '{1}')", testAvtoNoEverestDbLink, testAvtoNo);
                var cmd = new DbSqlCommand
                {
                    CommandText = query
                };
                var data = new List<string>();
                using (var x = SqlProvider.ThreadInstance.ExecuteReader(cmd))
                {
                    if (x.HasRows)
                    {
                        while (x.Read())
                        {
                            string rowData = "";
                            for (int j = 0; j < x.FieldCount; j++)
                            {
                                rowData += "\t" + x.GetValue(j);
                            }
                            data.Add(rowData);
                        }
                    }
                }
                string text = string.Join("\r\n", data);

                MonObj[] objs = DbManager.ReadAllObjs();
                foreach (MonObj monObj in objs)
                {
                    EverestMainData everestMainData = DbManager.GetMainDataFromEverestByAvtoNo(monObj.Avto_no, DateTime.Now);
                    if (everestMainData.OperationResult == OperationResult.Success)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception. {0}", ex.Message);
                throw;
            }
        }
    }
}
