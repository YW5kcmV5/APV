using System;
using System.Collections.Generic;
using System.Linq;
using APV.TransControl.Core.Entities;

namespace APV.TransControl.ExportReport.ReportData
{
    public class CommonReportData
    {
        public double Moto { get; set; }

        public double Dist { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public string Address { get; set; }

        public double FuelDepletion { get; set; }

        public double FuelDepletionAdd { get; set; }

        public string Equipments { get; set; }

        public string MotoDetails { get; set; }

        public string FuelDetails { get; set; }

        public EverestMainData EverestMainData { get; set; }
    }

    public class ReportParams
    {
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public string AvtoNo { get; set; }

        public int ObjId { get; set; }

        public bool ExtendedData { get; set; }
    }

    public class ReportContainer
    {
        private readonly List<object> _data = new List<object>();

        public void Add(CommonReportData data)
        {
            if (data != null)
            {
                _data.Add(data);
            }
        }

        public void Add(MonPos[] monPos)
        {
            if (monPos != null)
            {
                var list = new List<MonPos>(monPos);
                _data.Add(list);
            }
        }

        public void Clear()
        {
            _data.Clear();
        }

        public CommonReportData CommonReportData
        {
            get { return _data.OfType<CommonReportData>().FirstOrDefault(); }
        }

        public MonPos[] MonPos
        {
            get
            {
                List<MonPos> data = (_data.OfType<List<MonPos>>().FirstOrDefault());
                return (data != null) ? data.ToArray() : new MonPos[0];
            }
        }
    }
}
