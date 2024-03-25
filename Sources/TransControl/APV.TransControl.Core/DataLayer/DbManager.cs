using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using APV.DatabaseAccess.Providers;
using APV.EntityFramework.DataLayer;
using APV.TransControl.Common;
using APV.TransControl.Core.Entities;
using APV.TransControl.Core.Entities.Consumption;

namespace APV.TransControl.Core.DataLayer
{
    public static class DbManager
    {
        private static string ToTable(DbSqlDataReader reader)
        {
            var sb = new StringBuilder();
            bool firstRow = true;
            while (reader.Read())
            {
                int count = reader.FieldCount;
                if (firstRow)
                {
                    firstRow = false;
                    for (int i = 0; i < count; i++)
                    {
                        string columnName = reader.GetName(i);
                        sb.Append(columnName);
                        if (i < count - 1)
                        {
                            sb.Append("\t");
                        }
                    }
                    sb.AppendLine();
                }
                for (int i = 0; i < count; i++)
                {
                    string value = reader.GetString(i);
                    sb.Append(value);
                    if (i < count - 1)
                    {
                        sb.Append("\t");
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public const float MaxFuelFreq = 99999.0f;

        private static BaseConsumptionInfo[] ParseStkDataFuelXml(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.Root.Element("car")
                .Element("loading")
                .Elements()
                .Select(node => new BaseConsumptionInfo
            {
                Equipment = (node.Element("equipment") != null) && (node.Element("equipment").Value == "1"),
                Gmt = (node.Element("gmt") != null) && (!string.IsNullOrEmpty(node.Element("gmt").Value))
                                ? DateTime.ParseExact(node.Element("gmt").Value, "yyyy-MM-dd HH:mm:ss", null)
                                : DateTime.Now,
                Value = (node.Element("value") != null) && (!string.IsNullOrEmpty(node.Element("value").Value))
                                ? float.Parse(node.Element("value").Value.Replace(".", ","))
                                : -1.0f,
                GpsFault = (node.Element("gpsfault") != null) && (node.Element("gpsfault").Value == "1"),
            })
            .ToArray();
        }

        public static bool CheckConnection(out string errorMessage)
        {
            return SqlProvider.ThreadInstance.CheckConnection(out errorMessage);
        }

        public static AddEquipmentTemplateItem[] ReadAllTemplates()
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT ADD_EQUIPMENT_NAME, TEMPLATENAME FROM SYNCH_WL.ADD_EQUIPMENT_TEMPLATES ORDER BY TEMPLATENAME",
                CommandType = CommandType.Text
            };

            var list = new List<AddEquipmentTemplateItem>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var template = new AddEquipmentTemplateItem
                        {
                            AddEquimpmentName = (string)reader["ADD_EQUIPMENT_NAME"],
                            TemplateName = (string)reader["TEMPLATENAME"]
                        };
                        list.Add(template);
                    }
                }
            }

            return list.ToArray();
        }

        public static string Execute(string sql)
        {
            sql = sql.Replace("\r\n", "").Replace("\n", "").Replace("\t", " ");

            var cmd = new DbSqlCommand
            {
                CommandText = sql,
                CommandType = CommandType.Text
            };

            var sb = new StringBuilder();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                var columns = new StringBuilder();
                int length = reader.FieldCount;
                for (int i = 0; i < length; i++)
                {
                    string column = reader.GetName(i);
                    columns.Append(column);
                    columns.Append("\t");
                }
                sb.AppendLine();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var row = new StringBuilder();
                        for (int i = 0; i < length; i++)
                        {
                            object value = reader.GetValue(i);
                            string str = TransConverter.ToNullableString(value);
                            row.Append(str);
                            row.Append("\t");
                        }
                        sb.Append(row);
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

        public static AddEquipment[] ReadAllMasks()
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT NAME, FUELCONSUMPTION, DESCRIPTION, INPUTMASK, SPEEDCONDITIONFLAG, SPEEDCONDITION, STATEMASK, ADDRESSENABLED, COUNTMODE, ALGORITHMMASK FROM SYNCH_WL.ADD_EQUIPMENT ORDER BY NAME",
                CommandType = CommandType.Text
            };

            var list = new List<AddEquipment>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var mask = new AddEquipment
                            {
                                Name = (string) reader["NAME"],
                                FuelConsumption = TransConverter.ToDouble(reader["FUELCONSUMPTION"]),
                                Description = TransConverter.ToNullableString(reader["DESCRIPTION"]),
                                InputMask = TransConverter.ToByteArray(reader["INPUTMASK"]),
                                SpeedConditionFlag = TransConverter.ToNullableString(reader["SPEEDCONDITIONFLAG"]),
                                SpeedCondition = TransConverter.ToDouble(reader["SPEEDCONDITION"]),
                                StateMask = TransConverter.ToByteArray(reader["STATEMASK"]),
                                AddressEnabled = TransConverter.ToDouble(reader["ADDRESSENABLED"]) > 0,
                                CountMode = TransConverter.ToDouble(reader["COUNTMODE"]) > 0,
                                AlgorithmMask = TransConverter.ToByteArray(reader["ALGORITHMMASK"])
                            };
                        list.Add(mask);
                    }
                }
            }

            return list.ToArray();
        }

        public static string[] ReadAllNumbers()
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT SYNCH_WL.WL_PROC.ConvertAvtoNo(AVTO_NO) AS AVTO_NO FROM MONUSER.MONOBJ WHERE (OBJID is not null) AND (AVTO_NO is not null) ORDER BY AVTO_NO",
                CommandType = CommandType.Text
            };

            var list = new List<string>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string avtoNo = (string) reader["AVTO_NO"];
                        list.Add(avtoNo);
                    }
                }
            }

            return list.Distinct().ToArray();
        }

        public static MonObj[] ReadAllObjs()
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT OBJID, AVTO_NO, AVTO_MODEL FROM MONUSER.MONOBJ WHERE (OBJID is not null) AND (AVTO_NO is not null) ORDER BY AVTO_NO",
                CommandType = CommandType.Text
            };

            var list = new List<MonObj>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var mask = new MonObj
                            {
                                Objid = decimal.ToInt32((decimal) reader["OBJID"]),
                                Avto_no = (string) reader["AVTO_NO"],
                                Avto_model =
                                    !(reader["AVTO_MODEL"] is DBNull)
                                        ? ((string) reader["AVTO_MODEL"]).Trim()
                                        : string.Empty
                            };
                        list.Add(mask);
                    }
                }
            }

            return list.ToArray();
        }

        public static MonPos[] GetAllMonPos(int objId, DateTime from, DateTime to)
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT GMT, FORMAT, STATE, CAST(LAT as FLOAT) as LAT, CAST(LON as FLOAT) as LON, CAST(SPEED as FLOAT) as SPEED, CAST(DIST as FLOAT) as DIST, INPUT FROM MONUSER.MONPOS WHERE OBJID = :OBJID AND GMT >= SYNCH_WL.WL_PROC.TO_GMT(:BEGIN) AND GMT <= SYNCH_WL.WL_PROC.TO_GMT(:END) AND ((GMT is not null) AND (FORMAT is not null)) ORDER BY GMT",
                CommandType = CommandType.Text
            };

            var p1 = new SqlParameter("OBJID", objId);
            var p2 = new SqlParameter("BEGIN", from);
            var p3 = new SqlParameter("END", to);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);

            var list = new List<MonPos>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var pos = new MonPos
                            {
                                Objid = objId,
                                Gmt = (DateTime) reader["GMT"],
                                Format = !(reader["FORMAT"] is DBNull) ? (byte[]) reader["FORMAT"] : null,
                                State = !(reader["STATE"] is DBNull) ? (byte[]) reader["STATE"] : null,
                                Input = !(reader["INPUT"] is DBNull) ? (byte[]) reader["INPUT"] : null,
                                Lat = !(reader["LAT"] is DBNull) ? decimal.ToDouble((decimal) reader["LAT"]) : 0.0,
                                Lon = !(reader["LON"] is DBNull) ? decimal.ToDouble((decimal) reader["LON"]) : 0.0,
                                Speed = !(reader["SPEED"] is DBNull) ? decimal.ToDouble((decimal) reader["SPEED"]) : 0.0,
                                Dist = !(reader["DIST"] is DBNull) ? decimal.ToDouble((decimal) reader["DIST"]) : 0.0
                            };
                        list.Add(pos);
                    }
                }
            }
            return list.ToArray();
        }

        public static void FillObjMasks(MonObj[] objects, AddEquipment[] masks)
        {
            if (objects == null)
                throw new ArgumentNullException("objects");
            if (masks == null)
                throw new ArgumentNullException("masks");

            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT OBJID, ADD_EQUIPMENT_NAME FROM SYNCH_WL.OBJ_ADD_EQUIPMENT ORDER BY OBJID",
                CommandType = CommandType.Text
            };

            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int objId = decimal.ToInt32((decimal) reader["OBJID"]);
                        var name = (string) reader["ADD_EQUIPMENT_NAME"];
                        MonObj obj = objects.SingleOrDefault(item => (item != null) && (item.Objid == objId));
                        if (obj != null)
                        {
                            obj.AddEquipmentName.Add(name);
                            AddEquipment mask = masks.SingleOrDefault(item => (item != null) && (item.Name == name));
                            if (mask != null)
                            {
                                obj.AddEquipment.Add(mask);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.ReplaceTemplateToObjByObjId
        /// </summary>
        public static int ReplaceTemplateToObjByObjId(int objId, string templateName)
        {
            var cmd = new DbSqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "SYNCH_WL.WL_PROC.ReplaceTemplateToObjByObjId"
                };

            var p1 = new SqlParameter("ReplaceTemplateToObjByObjId", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("TEMPLATE_NAME", templateName);
            var p3 = new SqlParameter("OBJ_ID", objId);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = (int)p1.Value;
            return res;
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.DelAllMasksFromObjByObjId
        /// </summary>
        public static int DelAllMasksFromObjByObjId(int objId)
        {
            var cmd = new DbSqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SYNCH_WL.WL_PROC.DelAllMasksFromObjByObjId"
            };

            var p1 = new SqlParameter("DelAllMasksFromObjByObjId", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("OBJ_ID", objId);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = (int)p1.Value;
            return res;
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.AddMaskToTemplate
        /// </summary>
        public static int AddMaskToTemplate(string templateName, string maskName)
        {
            var cmd = new DbSqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SYNCH_WL.WL_PROC.AddMaskToTemplate"
            };

            var p1 = new SqlParameter("AddMaskToTemplate", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("TEMPLATE_NAME", templateName);
            var p3 = new SqlParameter("MASK_NAME", maskName);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = (int)p1.Value;
            return res;
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.DelMaskFromTemplate
        /// </summary>
        public static int DelMaskFromTemplate(string templateName, string maskName)
        {
            var cmd = new DbSqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SYNCH_WL.WL_PROC.DelMaskFromTemplate"
            };

            var p1 = new SqlParameter("DelMaskFromTemplate", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("TEMPLATE_NAME", templateName);
            var p3 = new SqlParameter("MASK_NAME", maskName);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = (int)p1.Value;
            return res;
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.InsertUpdateDSTMask
        /// </summary>
        public static int InsertUpdateDSTMask(AddEquipment equipment)
        {
            //FUNCTION InsertUpdateDSTMask
            //(MASK_NAME IN NVARCHAR2,
            // FUEL_CONSUMPTION IN NUMBER,
            // MASK IN RAW,
            // MASK_DESCRIPTION IN NVARCHAR2 := '',
            // SPEED_CONDITION_FLAG IN CHAR := NULL,
            // SPEED_CONDITION IN NUMBER := 0.0,
            // STATE_MASK IN RAW := HEXTORAW('00000000'),
            // ADDRESS_ENABLED IN NUMBER := 1,
            // COUNT_MODE IN NUMBER := 0,
            // ALGORITHM_MASK IN RAW := HEXTORAW('00000000000000000000000000000000'))

            var cmd = new DbSqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SYNCH_WL.WL_PROC.InsertUpdateDSTMask"
            };

            string speedConditionFlag = (!string.IsNullOrWhiteSpace(equipment.SpeedConditionFlag))
                                            ? equipment.SpeedConditionFlag.Trim()
                                            : null;
            string maskDescription = (!string.IsNullOrWhiteSpace(equipment.Description))
                                         ? equipment.Description.Trim()
                                         : string.Empty;

            var p1 = new SqlParameter("InsertUpdateDSTMask", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("MASK_NAME", equipment.Name);
            var p3 = new SqlParameter("FUEL_CONSUMPTION", SqlDbType.Decimal) {Value = equipment.FuelConsumption};
            var p4 = new SqlParameter("MASK", equipment.InputMask);
            var p5 = new SqlParameter("MASK_DESCRIPTION", maskDescription);
            var p6 = new SqlParameter("SPEED_CONDITION_FLAG", SqlDbType.Char) {Value = speedConditionFlag};
            var p7 = new SqlParameter("SPEED_CONDITION", SqlDbType.Decimal) {Value = equipment.SpeedCondition};
            var p8 = new SqlParameter("STATE_MASK", equipment.StateMask);
            var p9 = new SqlParameter("ADDRESS_ENABLED", SqlDbType.Decimal) {Value = equipment.AddressEnabled ? 1 : 0};
            var p10 = new SqlParameter("COUNT_MODE", SqlDbType.Decimal) {Value = equipment.CountMode ? 1 : 0};
            var p11 = new SqlParameter("ALGORITHM_MASK", equipment.AlgorithmMask);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);
            cmd.Parameters.Add(p9);
            cmd.Parameters.Add(p10);
            cmd.Parameters.Add(p11);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = TransConverter.ToInt(p1.Value);
            return res;
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.DeleteDSTMask
        /// </summary>
        public static int DeleteDSTMask(string maskName)
        {
            if (maskName == null)
                throw new ArgumentNullException("maskName");
            if (string.IsNullOrWhiteSpace(maskName))
                throw new ArgumentOutOfRangeException("maskName", "Mask name is white space.");

            var cmd = new DbSqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SYNCH_WL.WL_PROC.DeleteDSTMask"
            };

            var p1 = new SqlParameter("DeleteDSTMask", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("MASK_NAME", maskName);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = TransConverter.ToInt(p1.Value);
            return res;
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.DeleteTemplate
        /// </summary>
        public static int DeleteTemplate(string templateName)
        {
            var cmd = new DbSqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "SYNCH_WL.WL_PROC.DeleteTemplate"
            };

            var p1 = new SqlParameter("DeleteTemplate", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("TEMPLATE_NAME", templateName);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = (int)p1.Value;
            return res;
        }

        /// <summary>
        /// get_fuel_sensor_vehicles
        /// </summary>
        public static string GetFuelSensorVehicles()
        {
            var cmd = new DbSqlCommand
                {
                    CommandText = @"SYNCH_WL.WL_PROC.get_fuel_sensor_vehicles",
                    CommandType = CommandType.StoredProcedure
                };

            var p1 = new SqlParameter("p_Response", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Output,
                    Size = 32000,
                };
            cmd.Parameters.Add(p1);

            SqlProvider.ThreadInstance.ExecuteNonQuery(cmd);

            return TransConverter.ToNullableString(p1.Value);
        }

        /// <summary>
        /// GetObjIdByAvtoNo
        /// </summary>
        public static int GetObjIdByAvtoNo(string avtoNo)
        {
            var cmd = new DbSqlCommand
                {
                    CommandText = "SYNCH_WL.WL_PROC.GetObjIdByAvtoNo",
                    CommandType = CommandType.StoredProcedure
                };

            var p1 = new SqlParameter("OBJ_ID", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("AVTONO", avtoNo);
            
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p1);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            if ((p1.Value != null) && !(p1.Value is DBNull))
            {
                if (p1.Value is decimal)
                    return decimal.ToInt32((decimal) p1.Value);
                return (int) p1.Value;
            }
            return -1;
        }

        /// <summary>
        /// GetObjIdByAvtoNo
        /// </summary>
        public static int GetGpsFault(string avtoNo, DateTime beginDate, DateTime endDate)
        {
            var cmd = new DbSqlCommand
            {
                CommandText = "SYNCH_WL.WL_PROC.GetGpsFault",
                CommandType = CommandType.StoredProcedure
            };

            var p1 = new SqlParameter("GPSFAULT", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            var p2 = new SqlParameter("AVTONO", avtoNo);
            var p3 = new SqlParameter("BEGINDATE", beginDate);
            var p4 = new SqlParameter("ENDDATE", endDate);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            var res = (int)p1.Value;
            return res;
        }

        public static MonObj GetObj(int objId)
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT OBJID, AVTO_NO, AVTO_MODEL FROM MONUSER.MONOBJ WHERE (OBJID is not null) AND (AVTO_NO is not null) AND (OBJID = :OBJID)",
                CommandType = CommandType.Text
            };

            var p1 = new SqlParameter("OBJID", objId);
            cmd.Parameters.Add(p1);

            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd, CommandBehavior.CloseConnection | CommandBehavior.SingleRow))
            {
                if ((reader.HasRows) && (reader.Read()))
                {
                    var obj = new MonObj
                        {
                            Objid = decimal.ToInt32((decimal) reader["OBJID"]),
                            Avto_no = (string) reader["AVTO_NO"],
                            Avto_model =
                                !(reader["AVTO_MODEL"] is DBNull)
                                    ? ((string) reader["AVTO_MODEL"]).Trim()
                                    : string.Empty
                        };
                    return obj;
                }
            }
            return null;
        }

        public static MonObj GetObjByAvtoNo(string avtoNo)
        {
            int objId = GetObjIdByAvtoNo(avtoNo);
            return (objId != -1) ? GetObj(objId) : null;
        }

        public static string GetOracleVersion()
        {
            var cmd = new DbSqlCommand
            {
                CommandText = @"SELECT * FROM V$VERSION",
                CommandType = CommandType.Text
            };

            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd, CommandBehavior.CloseConnection | CommandBehavior.SingleRow))
            {
                return ToTable(reader);
            }
        }

        /// <summary>
        /// get_stk_data_fuel_xml
        /// </summary>
        public static BaseConsumptionInfo[] GetStkDataFuelXml(string avtoNo, DateTime regDate, DateTime returnDate)
        {
            var cmd = new DbSqlCommand
                {
                    CommandText = @"SYNCH_WL.WL_PROC.get_stk_data_fuel_xml",
                    CommandType = CommandType.StoredProcedure
                };

            var p1 = new SqlParameter("p_CarNumber", SqlDbType.VarChar)
                         {
                             Value = avtoNo,
                             Direction = ParameterDirection.Input,
                         };
            cmd.Parameters.Add(p1);

            var p2 = new SqlParameter("p_RegDate", SqlDbType.DateTime)
                         {
                             Value = regDate,
                             Direction = ParameterDirection.Input
                         };
            cmd.Parameters.Add(p2);

            var p3 = new SqlParameter("p_ReturnDate", SqlDbType.DateTime)
                         {
                             Value = returnDate,
                             Direction = ParameterDirection.Input
                         };
            cmd.Parameters.Add(p3);

            var p4 = new SqlParameter("p_Response", SqlDbType.VarChar)
                         {
                             Direction = ParameterDirection.Output,
                             Size = 32000,
                         };
            cmd.Parameters.Add(p4);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            if ((p4.Value != null) && !(p4.Value is DBNull))
            {
                return ParseStkDataFuelXml((string) p4.Value);
            }
            return new BaseConsumptionInfo[0];
        }

        public static void GetStkData(string p_carnumber, DateTime p_regdate, DateTime p_returndate, out double p_imp_run,
            out double p_imp_fueldepletion, out double p_imp_workeadd, out double p_imp_fueldepletionadd, out double p_coord_x,
            out double p_coord_y, out string p_imp_address, out DateTime p_imp_stardate, out DateTime p_imp_enddate)
        {
            p_imp_run = 0.0;
            p_imp_fueldepletion = 0.0;
            p_imp_workeadd = 0.0;
            p_imp_fueldepletionadd = 0.0;
            p_coord_x = 0.0;
            p_coord_y = 0.0;
            p_imp_address = string.Empty;
            p_imp_stardate = DateTime.Now;
            p_imp_enddate = DateTime.Now;

            var cmd = new DbSqlCommand
            {
                CommandText = @"SYNCH_WL.WL_PROC.get_stk_data",
                CommandType = CommandType.StoredProcedure
            };

            var p2 = new SqlParameter("p_carnumber", p_carnumber);
            var p3 = new SqlParameter("p_regdate", p_regdate);
            var p4 = new SqlParameter("p_returndate", p_returndate);
            var p5 = new SqlParameter("p_imp_run", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p6 = new SqlParameter("p_imp_fueldepletion", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p7 = new SqlParameter("p_imp_workeadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p8 = new SqlParameter("p_imp_fueldepletionadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p9 = new SqlParameter("p_coord_x", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p10 = new SqlParameter("p_coord_y", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p11 = new SqlParameter("p_imp_address", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
            var p12 = new SqlParameter("p_imp_stardate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p13 = new SqlParameter("p_imp_enddate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);
            cmd.Parameters.Add(p9);
            cmd.Parameters.Add(p10);
            cmd.Parameters.Add(p11);
            cmd.Parameters.Add(p12);
            cmd.Parameters.Add(p13);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);

            if (!TransConverter.IsNull(p5.Value))
            {
                p_imp_run = TransConverter.ToDouble(p5.Value);
            }
            if (!TransConverter.IsNull(p6.Value))
            {
                p_imp_fueldepletion = TransConverter.ToDouble(p6.Value);
            }
            if (!TransConverter.IsNull(p7.Value))
            {
                p_imp_workeadd = TransConverter.ToDouble(p7.Value);
            }
            if (!TransConverter.IsNull(p8.Value))
            {
                p_imp_fueldepletionadd = TransConverter.ToDouble(p8.Value);
            }
            if (!TransConverter.IsNull(p9.Value))
            {
                p_coord_x = TransConverter.ToDouble(p9.Value);
            }
            if (!TransConverter.IsNull(p10.Value))
            {
                p_coord_y = TransConverter.ToDouble(p10.Value);
            }
            if (!TransConverter.IsNull(p11.Value))
            {
                p_imp_address = TransConverter.ToString(p11.Value);
            }
            if (!TransConverter.IsNull(p12.Value))
            {
                p_imp_stardate = TransConverter.ToDateTime(p12.Value);
            }
            if (!TransConverter.IsNull(p13.Value))
            {
                p_imp_enddate = TransConverter.ToDateTime(p13.Value);
            }
        }

        public static void GetStkDataGps(string p_carnumber, DateTime p_regdate, DateTime p_returndate, out double p_imp_run,
            out double p_imp_fueldepletion, out double p_imp_workeadd, out double p_imp_fueldepletionadd, out double p_coord_x,
            out double p_coord_y, out string p_imp_address, out DateTime p_imp_stardate, out DateTime p_imp_enddate, out int p_imp_gpsfault)
        {
            p_imp_run = 0.0;
            p_imp_fueldepletion = 0.0;
            p_imp_workeadd = 0.0;
            p_imp_fueldepletionadd = 0.0;
            p_coord_x = 0.0;
            p_coord_y = 0.0;
            p_imp_address = string.Empty;
            p_imp_stardate = DateTime.Now;
            p_imp_enddate = DateTime.Now;
            p_imp_gpsfault = -99;

            var cmd = new DbSqlCommand
            {
                CommandText = @"SYNCH_WL.WL_PROC.get_stk_data_gps",
                CommandType = CommandType.StoredProcedure
            };

            var p2 = new SqlParameter("p_carnumber", p_carnumber);
            var p3 = new SqlParameter("p_regdate", p_regdate);
            var p4 = new SqlParameter("p_returndate", p_returndate);
            var p5 = new SqlParameter("p_imp_run", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p6 = new SqlParameter("p_imp_fueldepletion", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p7 = new SqlParameter("p_imp_workeadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p8 = new SqlParameter("p_imp_fueldepletionadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p9 = new SqlParameter("p_coord_x", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p10 = new SqlParameter("p_coord_y", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p11 = new SqlParameter("p_imp_address", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
            var p12 = new SqlParameter("p_imp_stardate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p13 = new SqlParameter("p_imp_enddate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p14 = new SqlParameter("p_imp_gpsfault", SqlDbType.Int) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);
            cmd.Parameters.Add(p9);
            cmd.Parameters.Add(p10);
            cmd.Parameters.Add(p11);
            cmd.Parameters.Add(p12);
            cmd.Parameters.Add(p13);
            cmd.Parameters.Add(p14);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);

            if (!TransConverter.IsNull(p5.Value))
            {
                p_imp_run = TransConverter.ToDouble(p5.Value);
            }
            if (!TransConverter.IsNull(p6.Value))
            {
                p_imp_fueldepletion = TransConverter.ToDouble(p6.Value);
            }
            if (!TransConverter.IsNull(p7.Value))
            {
                p_imp_workeadd = TransConverter.ToDouble(p7.Value);
            }
            if (!TransConverter.IsNull(p8.Value))
            {
                p_imp_fueldepletionadd = TransConverter.ToDouble(p8.Value);
            }
            if (!TransConverter.IsNull(p9.Value))
            {
                p_coord_x = TransConverter.ToDouble(p9.Value);
            }
            if (!TransConverter.IsNull(p10.Value))
            {
                p_coord_y = TransConverter.ToDouble(p10.Value);
            }
            if (!TransConverter.IsNull(p11.Value))
            {
                p_imp_address = TransConverter.ToString(p11.Value);
            }
            if (!TransConverter.IsNull(p12.Value))
            {
                p_imp_stardate = TransConverter.ToDateTime(p12.Value);
            }
            if (!TransConverter.IsNull(p13.Value))
            {
                p_imp_enddate = TransConverter.ToDateTime(p13.Value);
            }
            if (!TransConverter.IsNull(p14.Value))
            {
                p_imp_gpsfault = TransConverter.ToInt(p14.Value);
            }
        }

        public static void GetStkDataDetailGps(string p_carnumber, DateTime p_regdate, DateTime p_returndate, out double p_imp_run,
            out double p_imp_fueldepletion, out double p_imp_workeadd, out double p_imp_fueldepletionadd, out double p_coord_x,
            out double p_coord_y, out string p_imp_address, out DateTime p_imp_stardate, out DateTime p_imp_enddate, out string detail, out int p_imp_gpsfault)
        {
            p_imp_run = 0.0;
            p_imp_fueldepletion = 0.0;
            p_imp_workeadd = 0.0;
            p_imp_fueldepletionadd = 0.0;
            p_coord_x = 0.0;
            p_coord_y = 0.0;
            p_imp_address = string.Empty;
            p_imp_stardate = DateTime.Now;
            p_imp_enddate = DateTime.Now;
            detail = null;
            p_imp_gpsfault = -99;

            var cmd = new DbSqlCommand
            {
                CommandText = @"SYNCH_WL.WL_PROC.get_stk_data_detail_gps",
                CommandType = CommandType.StoredProcedure
            };

            var p2 = new SqlParameter("p_carnumber", p_carnumber);
            var p3 = new SqlParameter("p_regdate", p_regdate);
            var p4 = new SqlParameter("p_returndate", p_returndate);
            var p5 = new SqlParameter("p_imp_run", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p6 = new SqlParameter("p_imp_fueldepletion", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p7 = new SqlParameter("p_imp_workeadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p8 = new SqlParameter("p_imp_fueldepletionadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p9 = new SqlParameter("p_coord_x", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p10 = new SqlParameter("p_coord_y", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p11 = new SqlParameter("p_imp_address", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
            var p12 = new SqlParameter("p_imp_stardate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p13 = new SqlParameter("p_imp_enddate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p14 = new SqlParameter("detail", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
            var p15 = new SqlParameter("p_imp_gpsfault", SqlDbType.Int) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);
            cmd.Parameters.Add(p9);
            cmd.Parameters.Add(p10);
            cmd.Parameters.Add(p11);
            cmd.Parameters.Add(p12);
            cmd.Parameters.Add(p13);
            cmd.Parameters.Add(p14);
            cmd.Parameters.Add(p15);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);

            if (!TransConverter.IsNull(p5.Value))
            {
                p_imp_run = TransConverter.ToDouble(p5.Value);
            }
            if (!TransConverter.IsNull(p6.Value))
            {
                p_imp_fueldepletion = TransConverter.ToDouble(p6.Value);
            }
            if (!TransConverter.IsNull(p7.Value))
            {
                p_imp_workeadd = TransConverter.ToDouble(p7.Value);
            }
            if (!TransConverter.IsNull(p8.Value))
            {
                p_imp_fueldepletionadd = TransConverter.ToDouble(p8.Value);
            }
            if (!TransConverter.IsNull(p9.Value))
            {
                p_coord_x = TransConverter.ToDouble(p9.Value);
            }
            if (!TransConverter.IsNull(p10.Value))
            {
                p_coord_y = TransConverter.ToDouble(p10.Value);
            }
            if (!TransConverter.IsNull(p11.Value))
            {
                p_imp_address = TransConverter.ToString(p11.Value);
            }
            if (!TransConverter.IsNull(p12.Value))
            {
                p_imp_stardate = TransConverter.ToDateTime(p12.Value);
            }
            if (!TransConverter.IsNull(p13.Value))
            {
                p_imp_enddate = TransConverter.ToDateTime(p13.Value);
            }
            if (!TransConverter.IsNull(p14.Value))
            {
                detail = TransConverter.ToString(p14.Value);
            }
            if (!TransConverter.IsNull(p15.Value))
            {
                p_imp_gpsfault = TransConverter.ToInt(p15.Value);
            }
        }

        public static void GetStkDataFuelGps(string p_carnumber, DateTime p_regdate, DateTime p_returndate, out double p_imp_run,
            out double p_imp_fueldepletion, out double p_imp_workeadd, out double p_imp_fueldepletionadd, out double p_coord_x,
            out double p_coord_y, out string p_imp_address, out DateTime p_imp_stardate, out DateTime p_imp_enddate, 
            out double p_fuel_onregdate, out double p_fuel_onreturndate, out int p_imp_gpsfault)
        {
            p_imp_run = 0.0;
            p_imp_fueldepletion = 0.0;
            p_imp_workeadd = 0.0;
            p_imp_fueldepletionadd = 0.0;
            p_coord_x = 0.0;
            p_coord_y = 0.0;
            p_imp_address = string.Empty;
            p_imp_stardate = DateTime.Now;
            p_imp_enddate = DateTime.Now;
            p_fuel_onregdate = 0.0;
            p_fuel_onreturndate = 0.0;
            p_imp_gpsfault = -99;

            var cmd = new DbSqlCommand
            {
                CommandText = @"SYNCH_WL.WL_PROC.get_stk_data_fuel_gps",
                CommandType = CommandType.StoredProcedure
            };

            var p2 = new SqlParameter("p_carnumber", p_carnumber);
            var p3 = new SqlParameter("p_regdate", p_regdate);
            var p4 = new SqlParameter("p_returndate", p_returndate);
            var p5 = new SqlParameter("p_imp_run", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p6 = new SqlParameter("p_imp_fueldepletion", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p7 = new SqlParameter("p_imp_workeadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p8 = new SqlParameter("p_imp_fueldepletionadd", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p9 = new SqlParameter("p_coord_x", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p10 = new SqlParameter("p_coord_y", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p11 = new SqlParameter("p_imp_address", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
            var p12 = new SqlParameter("p_imp_stardate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p13 = new SqlParameter("p_imp_enddate", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
            var p14 = new SqlParameter("p_fuel_onregdate", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p15 = new SqlParameter("p_fuel_onreturndate", SqlDbType.Float) { Direction = ParameterDirection.Output };
            var p16 = new SqlParameter("p_imp_gpsfault", SqlDbType.Int) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);
            cmd.Parameters.Add(p9);
            cmd.Parameters.Add(p10);
            cmd.Parameters.Add(p11);
            cmd.Parameters.Add(p12);
            cmd.Parameters.Add(p13);
            cmd.Parameters.Add(p14);
            cmd.Parameters.Add(p15);
            cmd.Parameters.Add(p16);

            SqlProvider.ThreadInstance.ExecuteScalar(cmd);

            if (!TransConverter.IsNull(p5.Value))
            {
                p_imp_run = TransConverter.ToDouble(p5.Value);
            }
            if (!TransConverter.IsNull(p6.Value))
            {
                p_imp_fueldepletion = TransConverter.ToDouble(p6.Value);
            }
            if (!TransConverter.IsNull(p7.Value))
            {
                p_imp_workeadd = TransConverter.ToDouble(p7.Value);
            }
            if (!TransConverter.IsNull(p8.Value))
            {
                p_imp_fueldepletionadd = TransConverter.ToDouble(p8.Value);
            }
            if (!TransConverter.IsNull(p9.Value))
            {
                p_coord_x = TransConverter.ToDouble(p9.Value);
            }
            if (!TransConverter.IsNull(p10.Value))
            {
                p_coord_y = TransConverter.ToDouble(p10.Value);
            }
            if (!TransConverter.IsNull(p11.Value))
            {
                p_imp_address = TransConverter.ToString(p11.Value);
            }
            if (!TransConverter.IsNull(p12.Value))
            {
                p_imp_stardate = TransConverter.ToDateTime(p12.Value);
            }
            if (!TransConverter.IsNull(p13.Value))
            {
                p_imp_enddate = TransConverter.ToDateTime(p13.Value);
            }
            if (!TransConverter.IsNull(p14.Value))
            {
                p_fuel_onregdate = TransConverter.ToDouble(p14.Value);
            }
            if (!TransConverter.IsNull(p15.Value))
            {
                p_fuel_onreturndate = TransConverter.ToDouble(p15.Value);
            }
            if (!TransConverter.IsNull(p16.Value))
            {
                p_imp_gpsfault = TransConverter.ToInt(p16.Value);
            }
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.CalcDetailDSTMotoByAvtoNo
        /// </summary>
        public static OperationResult CalcDetailDSTMotoByAvtoNo(string avtoNo, DateTime from, DateTime to, out double moto, out double fuel, out string equipments, out string motoDetails, out string fuelDetails)
        {
            moto = 0.0;
            fuel = 0.0;
            equipments = "";
            motoDetails = "";
            fuelDetails = "";

            try
            {
                var cmd = new DbSqlCommand
                {
                    CommandText = @"SYNCH_WL.WL_PROC.CalcDetailDSTMotoByAvtoNo",
                    CommandType = CommandType.StoredProcedure
                };

                var p1 = new SqlParameter("CalcDetailDSTMotoByAvtoNo", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                var p2 = new SqlParameter("AVTO_NO", avtoNo);
                var p3 = new SqlParameter("BEGINDATE", from);
                var p4 = new SqlParameter("ENDDATE", to);
                var p5 = new SqlParameter("MOTO", SqlDbType.Float) { Direction = ParameterDirection.Output };
                var p6 = new SqlParameter("FUEL_CONSUMPTION", SqlDbType.Float) { Direction = ParameterDirection.Output };
                var p7 = new SqlParameter("AddEquipmentNames", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
                var p8 = new SqlParameter("MotoDetails", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };
                var p9 = new SqlParameter("FuelConsumptionDetails", SqlDbType.NVarChar) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                cmd.Parameters.Add(p5);
                cmd.Parameters.Add(p6);
                cmd.Parameters.Add(p7);
                cmd.Parameters.Add(p8);
                cmd.Parameters.Add(p9);

                SqlProvider.ThreadInstance.ExecuteScalar(cmd);
                OperationResult result = TransConverter.ToOperationResult(p1.Value);
                if (result == OperationResult.Success)
                {
                    if (!TransConverter.IsNull(p5.Value))
                    {
                        moto = TransConverter.ToDouble(p5.Value);
                    }
                    if (!TransConverter.IsNull(p6.Value))
                    {
                        fuel = TransConverter.ToDouble(p6.Value);
                    }
                    if (!TransConverter.IsNull(p7.Value))
                    {
                        equipments = TransConverter.ToString(p7.Value);
                    }
                    if (!TransConverter.IsNull(p8.Value))
                    {
                        motoDetails = TransConverter.ToString(p8.Value);
                    }
                    if (!TransConverter.IsNull(p9.Value))
                    {
                        fuelDetails = TransConverter.ToString(p9.Value);
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return OperationResult.Exception;
            }
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.GetFuelConsFromEverestByAvtoNo
        /// </summary>
        public static OperationResult GetFuelConsNormFromEverestByAvtoNo(string avtoNo, DateTime regDate, out double fuelCons)
        {
            fuelCons = (int)(OperationResult.NotFound);
            try
            {
                var cmd = new DbSqlCommand
                {
                    CommandText = @"SYNCH_WL.WL_PROC.GetFuelConsFromEverestByAvtoNo",
                    CommandType = CommandType.StoredProcedure
                };

                var p1 = new SqlParameter("GetFuelConsFromEverestByAvtoNo", SqlDbType.Float)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                var p2 = new SqlParameter("AVTO_NO", avtoNo);
                var p3 = new SqlParameter("p_REGDATE", regDate);

                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);

                SqlProvider.ThreadInstance.ExecuteNonQuery(cmd);

                double result = TransConverter.ToDouble(p1.Value);
                if (result >= 0)
                {
                    fuelCons = result;
                    return OperationResult.Success;
                }
                return (OperationResult)((int)System.Math.Truncate(result));
            }
            catch (Exception)
            {
                return OperationResult.Exception;
            }
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.GetMainDataFromEverestByAvtoNo
        /// </summary>
        public static EverestMainData GetMainDataFromEverestByAvtoNo(string avtoNo, DateTime regDate, bool tryToUpdateEverestLinkIfFailed = true)
        {
            var everestMainData = new EverestMainData();
            try
            {
                everestMainData.OperationResult = OperationResult.NotFound;

                var cmd = new DbSqlCommand
                    {
                        CommandText = @"SYNCH_WL.WL_PROC.GetMainDataFromEverestByAvtoNo",
                        CommandType = CommandType.StoredProcedure
                    };

                //(
                //AVTO_NO IN NVARCHAR2,
                //MODEL OUT VARCHAR,
                //VEHICLETYPE OUT CHAR,
                //ADMINNAME OUT VARCHAR2,
                //FUELCONSUMPTION OUT NUMBER,
                //WORKGRAPHTYPENAME OUT VARCHAR2,
                //DINNERTIME OUT CHAR)
                //RETURN NUMBER IS

                var p1 = new SqlParameter("GetMainDataFromEverestByAvtoNo", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                var p2 = new SqlParameter("AVTO_NO", avtoNo);
                var p3 = new SqlParameter("MODEL", SqlDbType.VarChar) {Direction = ParameterDirection.Output};
                var p4 = new SqlParameter("VEHICLETYPE", SqlDbType.Char) {Direction = ParameterDirection.Output};
                var p5 = new SqlParameter("ADMINNAME", SqlDbType.VarChar) {Direction = ParameterDirection.Output};
                var p6 = new SqlParameter("FUELCONSUMPTION", SqlDbType.Decimal) {Direction = ParameterDirection.Output};
                var p7 = new SqlParameter("WORKGRAPHTYPENAME", SqlDbType.VarChar, 1000)
                    {
                        Direction = ParameterDirection.Output
                    };
                var p8 = new SqlParameter("DINNERTIME", SqlDbType.Char, 1000) {Direction = ParameterDirection.Output};

                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                cmd.Parameters.Add(p5);
                cmd.Parameters.Add(p6);
                cmd.Parameters.Add(p7);
                cmd.Parameters.Add(p8);

                SqlProvider.ThreadInstance.ExecuteScalar(cmd);
                int resultValue = TransConverter.ToInt(p1.Value);
                var result = (OperationResult) resultValue;
                if (result == OperationResult.Success)
                {
                    everestMainData.AvtoNo = avtoNo;
                    everestMainData.AdminName = TransConverter.ToString(p5.Value).Trim();
                    everestMainData.DinnerTime = TransConverter.ToString(p8.Value).Trim();
                    everestMainData.FuelConsumption = TransConverter.ToDouble(p6.Value);
                    everestMainData.Model = TransConverter.ToString(p3.Value).Trim();
                    everestMainData.VehicleType = TransConverter.ToVehicleType(p4.Value);
                    everestMainData.WorkGraphTypeName = TransConverter.ToString(p7.Value).Trim();

                    double fuelCons;
                    OperationResult internalResult = GetFuelConsNormFromEverestByAvtoNo(avtoNo, regDate, out fuelCons);
                    everestMainData.FuelConsumptionNorm = (internalResult == OperationResult.Success)
                                                              ? fuelCons
                                                              : (int) internalResult;
                }
                else if (tryToUpdateEverestLinkIfFailed)
                {
                    //Try to update everest link;
                    if (UpdateEverstLink(avtoNo))
                    {
                        return GetMainDataFromEverestByAvtoNo(avtoNo, regDate, false);
                    }
                }
                everestMainData.OperationResult = result;
            }
            catch (Exception)
            {
                everestMainData.OperationResult = OperationResult.Exception;
            }
            return everestMainData;
        }

        public static string GetTransformation(string avtoNo)
        {
            var cmd = new DbSqlCommand { CommandText = @"SELECT (SYNCH_WL.WL_PROC.GetTransformationToXml(:AVTO_NO)) FROM DUAL" };

            var p1 = new SqlParameter("AVTO_NO", avtoNo);

            cmd.Parameters.Add(p1);

            string xml = TransConverter.ToNullableString(SqlProvider.ThreadInstance.ExecuteScalar(cmd));
            return xml;
        }

        /// <summary>
        /// SELECT CONFIG FROM MONUSER.MONOBJ WHERE MONUSER.MONOBJ.OBJID = :OBJID
        /// </summary>
        public static string GetConfig(int objId)
        {
            var cmd = new DbSqlCommand
                {
                    CommandText = @"SELECT CONFIG FROM MONUSER.MONOBJ WHERE MONUSER.MONOBJ.OBJID = :OBJID"
                };

            var p1 = new SqlParameter("OBJID", SqlDbType.Decimal)
            {
                Value = objId,
                Direction = ParameterDirection.Input
            };
            cmd.Parameters.Add(p1);

            object config = SqlProvider.ThreadInstance.ExecuteScalar(cmd);
            if (config != null)
            {
                return Convert.ToBase64String((byte[])config);
            }
            return null;
        }

        public static FreqRecord[] GetFuel(int objId, DateTime from, DateTime to)
        {
            //const string engineStateMask = "00010000";

            var cmd = new DbSqlCommand
                {
                    CommandText = string.Format(
//@"SELECT GMT, CAST(FUEL AS FLOAT) AS FUEL, STATE)
                        @"SELECT GMT, CAST(FUEL AS FLOAT) AS FUEL,
(CASE WHEN(UTL_RAW.BIT_AND(STATE, HEXTORAW('00010000')) = HEXTORAW('00010000')) THEN 1 ELSE 0 END)
FROM MONUSER.MONPOS WHERE
OBJID = :OBJID
AND GMT >= :GMT_FROM
AND GMT <= :GMT_TO
ORDER BY GMT"
//, engineStateMask
                        )
                };

            var p1 = new SqlParameter("OBJID", SqlDbType.Decimal)
                         {
                             Value = objId,
                             Direction = ParameterDirection.Input
                         };
            var p2 = new SqlParameter("GMT_FROM", SqlDbType.Timestamp)
                         {
                             Value = from.ToUniversalTime(),
                             Direction = ParameterDirection.Input
                         };
            var p3 = new SqlParameter("GMT_TO", SqlDbType.Timestamp)
                         {
                             Value = to.ToUniversalTime(),
                             Direction = ParameterDirection.Input
                         };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);

            var result = new List<FreqRecord>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    DateTime gmt = DateTime.SpecifyKind(Convert.ToDateTime(reader[0]), DateTimeKind.Utc).ToLocalTime();

                    long freq = 0;
                    //byte[] state = null;

                    object value = reader[1];
                    if ((value != null) && !(value is DBNull))
                    {
                        if (value is decimal)
                        {
                            freq = (long) (decimal) value;
                        }
                        else if (value is long)
                        {
                            freq = (long) value;
                        }
                        else if (value is int)
                        {
                            freq = (int)value;
                        }
                    }
                    freq = (freq >= MaxFuelFreq) ? 0 : freq;

                    var engine = ((decimal)reader[2] > 0);

                    //value = reader[2];
                    //if ((value != null) && !(value is DBNull))
                    //{
                    //    state = (byte[]) value;
                    //}

                    //result.Add(new FreqRecord {GMT = gmt, Freq = (int) freq, State = state });
                    result.Add(new FreqRecord { GMT = gmt, Freq = (int)freq, IsEngine = engine, });
                }
            }
            //var r = result.Where(item => item.IsEngine).ToArray();
            return result.ToArray();
        }

        public static string GetWlProcPackage()
        {
            var cmd = new DbSqlCommand { CommandText = @"SELECT dbms_metadata.get_ddl('PACKAGE', 'WL_PROC', 'SYNCH_WL') FROM dual", };
            return (string)SqlProvider.ThreadInstance.ExecuteScalar(cmd);
        }

        public static string[] GetSycnWlTablesNames()
        {
            var cmd = new DbSqlCommand {CommandText = @"SELECT table_name from all_tables where owner = 'SYNCH_WL'"};
            var tablesNames = new List<string>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tablesNames.Add((string)reader.GetValue(0));
                    }
                }
            }
            return tablesNames.ToArray();
        }

        public static bool CheckEverestDbLink(string everestDbLink)
        {
            if (everestDbLink == null)
                throw new ArgumentNullException("everestDbLink");
            if (string.IsNullOrWhiteSpace(everestDbLink))
                throw new ArgumentOutOfRangeException("everestDbLink", "Everest db link is empty or white space.");

            try
            {
                var dbLInkCmd = new DbSqlCommand { CommandText = string.Format(@"SELECT COUNT(*) FROM SYNCH_WL.V_CARDTRANSP@{0}", everestDbLink) };
                SqlProvider.ThreadInstance.ExecuteScalar(dbLInkCmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// SYNCH_WL.WL_PROC.GetDBLinkNameToEverest
        /// </summary>
        public static string GetDBLinkNameToEverest(string avtoNo, int objId)
        {
            var cmd = new DbSqlCommand { CommandText = @"SELECT (SYNCH_WL.WL_PROC.GetDBLinkNameToEverest(:AVTO_NO, :OBJ_ID)) FROM DUAL" };

            var p1 = new SqlParameter("AVTO_NO", avtoNo);
            var p2 = new SqlParameter("OBJ_ID", objId);

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);

            string dbLink = TransConverter.ToNullableString(SqlProvider.ThreadInstance.ExecuteScalar(cmd));
            return dbLink;
        }

        public static string GetDBLinkNameToEverest(string avtoNo)
        {
            if (avtoNo == null)
                throw new ArgumentNullException("avtoNo");
            if (string.IsNullOrWhiteSpace(avtoNo))
                throw new ArgumentOutOfRangeException("avtoNo", "Avto number is empty or white space.");

            MonObj obj = GetObjByAvtoNo(avtoNo);
            return (obj != null) ? GetDBLinkNameToEverest(obj.Avto_no, obj.Objid) : null;
        }

        public static string[] ListDbLinks()
        {
            //SELECT owner,db_link,host,username FROM dba_db_links
            var cmd = new DbSqlCommand { CommandText = @"SELECT db_link FROM dba_db_links" };
            var dbLinks = new List<string>();
            using (DbSqlDataReader reader = SqlProvider.ThreadInstance.ExecuteReader(cmd))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dbLinks.Add((string)reader.GetValue(0));
                    }
                }
            }
            return dbLinks.ToArray();
        }

        public static string FindActualDbLinkToEverest(string avtoNo)
        {
            if (avtoNo == null)
                throw new ArgumentNullException("avtoNo");
            if (string.IsNullOrWhiteSpace(avtoNo))
                throw new ArgumentOutOfRangeException("avtoNo", "Avto number is empty or white space.");

            MonObj obj = GetObjByAvtoNo(avtoNo);
            if (obj != null)
            {
                avtoNo = obj.Avto_no.Replace(" ", string.Empty).Replace("-", string.Empty).Trim().ToLowerInvariant();
                string[] dbLinks = ListDbLinks();
                foreach (string dbLink in dbLinks)
                {
                    string query = string.Format(@"SELECT COUNT(*) FROM SYNCH_WL.V_CARDTRANSP@{0} WHERE (LOWER(TRIM(REPLACE(REPLACE(CARNUMBER, ' ', ''), '-', ''))) = :AVTO_NO)", dbLink);
                    try
                    {
                        var cmd = new DbSqlCommand { CommandText = query };
                        var p1 = new SqlParameter("AVTO_NO", avtoNo);
                        cmd.Parameters.Add(p1);
                        var count = TransConverter.ToInt(SqlProvider.ThreadInstance.ExecuteScalar(cmd));
                        if (count > 0)
                        {
                            return dbLink;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return null;
        }

        public static bool UpdateEverstLink(string avtoNo)
        {
            if (avtoNo == null)
                throw new ArgumentNullException("avtoNo");
            if (string.IsNullOrWhiteSpace(avtoNo))
                throw new ArgumentOutOfRangeException("avtoNo", "Avto number is empty or white space.");

            string dbLink = GetDBLinkNameToEverest(avtoNo);
            string actualDbLink = FindActualDbLinkToEverest(avtoNo);

            if ((actualDbLink != null) && ((dbLink == null) || (string.Compare(dbLink, actualDbLink, StringComparison.InvariantCultureIgnoreCase) != 0)))
            {
                int objId = GetObjIdByAvtoNo(avtoNo);
                var cmd = new DbSqlCommand { CommandText = @"UPDATE SYNCH_WL.DATASOURCE SET SYNCH_WL.DATASOURCE.DBLINKNAME = :DBLINKNAME WHERE OBJID = :OBJID" };
                var p1 = new SqlParameter("DBLINKNAME", actualDbLink);
                var p2 = new SqlParameter("OBJID", objId);
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                SqlProvider.ThreadInstance.ExecuteNonQuery(cmd);
                return true;
            }
            return false;
        }
    }
}