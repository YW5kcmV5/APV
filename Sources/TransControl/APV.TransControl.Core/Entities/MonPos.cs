using System;
using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities
{
    [XmlType(Namespace = "TransControl")]
    [XmlRootAttribute(Namespace = "TransControl")]
    //[DBObjectAttribute(DBTableName = "MONPOS", DBUserName = "MONUSER")]
    public class MonPos
    {
        //[DBType(OracleType.NUMERIC, Nullable = false)]

        //[DBType(OracleType.TIMESTAMP, 6)]

        //[DBType(OracleType.TIMESTAMP, 6)]

        //[DBType(OracleType.RAW, 64)]

        //[DBType(OracleType.BLOB)]

        //[DBType(OracleType.RAW, 4)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "FUEL_2")]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "MOTO_2")]

        //[DBType(OracleType.RAW, 2)]

        //[DBType(OracleType.RAW, 2)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.BINARY_DOUBLE)]

        //[DBType(OracleType.RAW, 16)]

        //[DBType(OracleType.RAW, 16)]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "TEMPR_IN")]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "TEMPR_OUT")]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "PARAM_1")]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "PARAM_2")]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "PARAM_3")]

        //[DBType(OracleType.BINARY_DOUBLE, DBFieldName = "PARAM_4")]

        public static int Count()
        {
            throw new NotImplementedException();
        }

        public static int Count(int objId, DateTime from, DateTime to)
        {
            ////string sqlCondition = "((OBJID = {0}) AND (EVGMT >= {1}) AND (EVGMT <= {2}))";
            //string sqlCondition = "((OBJID = {0}) AND (GMT >= {1}) AND (GMT <= {2}))";
            //DBParam paramObjId = new DBParam("OBJID", objId);
            //DBParam paramFrom = new DBParam("GMT", from);
            //DBParam paramTo = new DBParam("GMT", to);
            //return Count(conn, typeof(MonPos), sqlCondition, new DBParam[3] { paramObjId, paramFrom, paramTo });
            throw new NotImplementedException();
        }

        public static int Count(DateTime from, DateTime to)
        {
            ////string sqlCondition = "((EVGMT >= {0}) AND (EVGMT <= {1}))";
            //string sqlCondition = "((GMT >= {0}) AND (GMT <= {1}))";
            //DBParam paramFrom = new DBParam("GMT", from);
            //DBParam paramTo = new DBParam("GMT", to);
            //return Count(conn, typeof(MonPos), sqlCondition, new DBParam[2] { paramFrom, paramTo });
            throw new NotImplementedException();
        }

        public static MonPos[] SelectAll()
        {
            //DataObject[] objects = SelectAll(conn, typeof(MonPos));
            //return ToDataObjectArray<MonPos>(objects);
            throw new NotImplementedException();
        }

        public static MonPos[] Select(int objId, DateTime from, DateTime to)
        {
            ////string sqlCondition = "((OBJID = {0}) AND (EVGMT >= {1}) AND (EVGMT <= {2}))";
            //string sqlCondition = "((OBJID = {0}) AND (GMT >= {1}) AND (GMT <= {2}))";
            //DBParam paramObjId = new DBParam("OBJID", objId);
            //DBParam paramFrom = new DBParam("GMT", from);
            //DBParam paramTo = new DBParam("GMT", to);
            //DataObject[] objects = Select(conn, typeof(MonPos), sqlCondition, new DBParam[3] { paramObjId, paramFrom, paramTo });
            //return ToDataObjectArray<MonPos>(objects);
            throw new NotImplementedException();
        }

        public static MonPos[] Select(DateTime from, DateTime to)
        {
            ////string sqlCondition = "((EVGMT >= {0}) AND (EVGMT <= {1}))";
            //string sqlCondition = "((GMT >= {0}) AND (GMT <= {1}))";
            //DBParam paramFrom = new DBParam("GMT", from);
            //DBParam paramTo = new DBParam("GMT", to);
            //DataObject[] objects = Select(conn, typeof(MonPos), sqlCondition, new DBParam[2] { paramFrom, paramTo });
            //return ToDataObjectArray<MonPos>(objects);
            throw new NotImplementedException();
        }

        public static double CalcDist(MonPos[] pos, DateTime from, DateTime to)
        {
            double dist = 0.0;
            if ((pos == null) || (pos.Length <= 1))
            {
                return dist;
            }
            MonPos previous = pos[0];
            for (int i = 1; i < pos.Length; i++)
            {
                MonPos current = pos[i];
                if ((current.Gmt >= from) && (current.Gmt <= to))
                {
                    if (current.Dist > previous.Dist)
                    {
                        dist += current.Dist - previous.Dist;
                    }
                }
                previous = current;
            }
            return dist;
        }

        [XmlElement]
        public int Objid { get; set; }

        [XmlElement]
        public DateTime Gmt { get; set; }

        [XmlElement]
        public DateTime Evgmt { get; set; }

        [XmlElement]
        public byte[] Event { get; set; }

        [XmlElement]
        public byte[] Format { get; set; }

        [XmlElement]
        public byte[] State { get; set; }

        [XmlElement]
        public double Lat { get; set; }

        [XmlElement]
        public double Lon { get; set; }

        [XmlElement]
        public double Alt { get; set; }

        [XmlElement]
        public double Speed { get; set; }

        [XmlElement]
        public double Heading { get; set; }

        [XmlElement]
        public double Dist { get; set; }

        [XmlElement]
        public double Fuel { get; set; }

        [XmlElement]
        public double Fuel_2 { get; set; }

        [XmlElement]
        public double Moto { get; set; }

        [XmlElement]
        public double Moto_2 { get; set; }

        [XmlElement]
        public byte[] Gsm { get; set; }

        [XmlElement]
        public byte[] Gps { get; set; }

        [XmlElement]
        public double Power { get; set; }

        [XmlElement]
        public double Battery { get; set; }

        [XmlElement]
        public byte[] Input { get; set; }

        [XmlElement]
        public byte[] Output { get; set; }

        [XmlElement]
        public double Tempr_in { get; set; }

        [XmlElement]
        public double Tempr_out { get; set; }

        [XmlElement]
        public double Param1 { get; set; }

        [XmlElement]
        public double Param2 { get; set; }

        [XmlElement]
        public double Param3 { get; set; }

        [XmlElement]
        public double Param4 { get; set; }
    }
}