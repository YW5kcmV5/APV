using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace APV.TransControl.Core.Entities.Collection
{
    [XmlRoot]
    [Serializable]
    public class ObjDataCollection : ICloneable
    {
        [XmlArray]
        public ObjData[] Data { get; set; }

        public bool Completed { get { return ((Data != null) && (Data.Length > 0)); } }

        public object Clone()
        {
            return new ObjDataCollection
                       {
                           Data = (Completed) ? Data.Select(item => (ObjData) item.Clone()).ToArray() : null
                       };
        }
        
        public void Add(DateTime from, DateTime to, FreqRecord[] data)
        {
            from = from.ToLocalTime();
            to = to.ToLocalTime();

            var objData = new ObjData { From = from, To = to, Data = data };

            if (Completed)
            {
                List<ObjData> items = Data.ToList();
                items.Add(objData);

                //1. Sort by From
                items = items.OrderBy(item => item.From).ToList();

                //2. Synchronize (concatenate if needed)
                int i = 0;
                while (i + 1 < items.Count)
                {
                    ObjData c = items[i];
                    ObjData n = items[i + 1];
                    bool canConcatenate = (n.From <= c.To);
                    if (canConcatenate)
                    {
                        items.RemoveAt(i + 1);

                        List<FreqRecord> records = c.Data.ToList();
                        records.AddRange(n.Data.Where(item => item.GMT >= c.To));
                        c.Data = records.ToArray();
                        c.To = n.To;
                    }
                    else
                    {
                        i++;
                    }
                }

                Data = items.ToArray();
            }
            else
            {
                Data = new[] { objData };
            }
        }

        public FreqRecord[] Get(DateTime from, DateTime to)
        {
            from = from.ToLocalTime();
            to = to.ToLocalTime();

            ObjData data = Data.SingleOrDefault(item => (item.From >= from && item.To <= to));
            return (data != null)
                ? data.Data.Where(item => item.GMT >= from && item.GMT <= to).ToArray()
                : new FreqRecord[0];
        }
    }
}