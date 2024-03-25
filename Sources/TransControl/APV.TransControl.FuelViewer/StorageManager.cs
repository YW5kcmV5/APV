using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using APV.TransControl.Core.DataLayer;
using APV.TransControl.Core.Entities;

namespace APV.TransControl.FuelViewer
{
    public class StorageManager
    {
        //private const string FolderName = "Storage";
        //private const string FileName = "Data";
        private static readonly SortedList<int, ObjRecord> Cache = new SortedList<int, ObjRecord>();
        private static bool _inited;

        /*
        private static string GetFileName(int objId)
        {
            return Path.Combine(Environment.CurrentDirectory, string.Format("{0}\\{1}_{2}.bin", FolderName, FileName, objId));
        }

        private static ObjDataCollection LoadFromFile(int objId)
        {
            string fileName = GetFileName(objId);
            if (!File.Exists(fileName))
            {
                string folder = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                using(File.Create(fileName)) {}
            }
            byte[] data = File.ReadAllBytes(fileName);

            return (data.Length > 0)
                       ? Serializer.BinaryDeserialize<ObjDataCollection>(data, true)
                       : new ObjDataCollection { Data = new ObjData[0] };
        }

        private static void SaveToFile(int objId, ObjDataCollection dataCollection)
        {
            string fileName = GetFileName(objId);
            byte[] data = Serializer.BinarySerialize(dataCollection, true);
            File.WriteAllBytes(fileName, data);
        }

        private static FreqRecord[] LoadDataFromDb(ObjDataCollection dataCollection, int objId, DateTime from, DateTime to, out bool needToSave)
        {
            from = from.ToLocalTime();
            to = to.ToLocalTime();

            needToSave = false;

            var fromIntervals = new List<DateTime>();
            var toIntervals = new List<DateTime>();

            //1) Не надо загружать
            int length = dataCollection.Data.Length;
            for (int i = 0; i < length; i++)
            {
                ObjData data = dataCollection.Data[i];
                if ((from >= data.From) && (to <= data.To))
                {
                    return data.Data.Where(item => item.GMT >= from && item.GMT <= to).ToArray();
                }

                //Поиск подинтервалов при частичном пересечении
                if (
                    ((from < data.From) && (to >= data.From) && (to <= data.To)) ||
                    ((to > data.To) && (from >= data.From) && (from <= data.To)) ||
                    ((from < data.From) && (to > data.To))
                   )
                {
                    ObjData p = (i > 1) ? dataCollection.Data[i - 1] : null;
                    ObjData n = (i < length - 1) ? dataCollection.Data[i + 1] : null;

                    if ((p == null) && (from < data.From))
                    {
                        //Раньше первого
                        fromIntervals.Add(from);
                        toIntervals.Add(data.From);
                    }
                    if ((n == null) && (to > data.To))
                    {
                        //Позднее последнего
                        fromIntervals.Add(data.To);
                        toIntervals.Add(to);
                    }
                    if ((p != null) && (n != null))
                    {
                        fromIntervals.Add(p.To);
                        toIntervals.Add(n.From);
                    }
                }
            }

            needToSave = true;

            //2) Нет пересекающихся интервалов, надо загрузить полностью
            if (fromIntervals.Count == 0)
            {
                FreqRecord[] items = DbManager.GetFuel(objId, from, to);
                dataCollection.Add(from, to, items);
            }

            //3) Есть переcекающиеся интервалы, надо сформировать несколько условий загрузки для каждого интервала
            for (int i = 0; i < fromIntervals.Count; i++)
            {
                //Загружаем в начале (от from до first)
                FreqRecord[] items = DbManager.GetFuel(objId, fromIntervals[i], toIntervals[i]);
                dataCollection.Add(fromIntervals[i], toIntervals[i], items);
            }

            return dataCollection.Get(from, to);
        }
        */

        private static ObjRecord LoadObjFromDb(string avtoNo)
        {
            int objId = DbManager.GetObjIdByAvtoNo(avtoNo);
            if (objId > 0)
            {
                string config = DbManager.GetConfig(objId);
                if (!string.IsNullOrEmpty(config))
                {
                    return new ObjRecord
                               {
                                   AvtoNo = avtoNo,
                                   ObjId = objId,
                                   ConfigData = config,
                                   Data = null
                               };
                }
            }

            return null;
        }
        
        public static void Init()
        {
            if (_inited)
            {
                return;
            }

            string xml = DbManager.GetFuelSensorVehicles();

            if (string.IsNullOrWhiteSpace(xml))
                throw new InvalidOperationException("List of vehicles with fuel sensors cannot be loaded from database. Procedure \"get_fuel_sensor_vehicles\" return empty xml string (probably exception happened on a Oracle server).");

            XDocument doc = XDocument.Parse(xml);
            string[] addresses = doc.Root.Elements().Select(node => node.Value).ToArray();

            foreach (string avtoNo in addresses)
            {
                ObjRecord obj = LoadObjFromDb(avtoNo);
                if (obj != null)
                {
                    lock (Cache)
                    {
                        Cache.Add(obj.ObjId, obj);
                    }
                }
            }

            _inited = true;
        }

        public static FreqRecord[] Load(int objId, DateTime from, DateTime to)
        {
            from = from.ToLocalTime();
            to = to.ToLocalTime();

            return DbManager.GetFuel(objId, from, to);

            //ObjRecord obj;
            //lock (Cache)
            //{
            //    obj = Cache[objId];
            //}

            //if (obj.Data == null)
            //{
            //    obj.Data = LoadFromFile(objId);
            //}

            //bool needToSave;
            //FreqRecord[] data = LoadDataFromDb(conn, obj.Data, objId, from, to, out needToSave);

            //if (needToSave)
            //{
            //    SaveToFile(obj.ObjId, obj.Data);
            //}

            //return data;
        }

        public static ObjRecord[] Objects
        {
            get
            {
                lock (Cache)
                {
                    return Cache.Values.OrderBy(item => item.AvtoNo).ToArray();
                }
            }
        }
    }
}