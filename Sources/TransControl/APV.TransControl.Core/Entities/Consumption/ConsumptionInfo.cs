using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using APV.Math.Diagrams;
using APV.Math.Diagrams.Devices;
using APV.TransControl.Common;

namespace APV.TransControl.Core.Entities.Consumption
{
    public class ConsumptionInfo : IConsumptionInfo
    {
        private readonly ObjRecord _obj;
        private Diagram _diagram;
        private Diagram _engineDiagram;

        public ConsumptionInfo(ObjRecord obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            _obj = obj;
        }

        public ObjRecord Object
        {
            get { return _obj; }
        }

        public FreqRecord[] Original { get; set; }
        
        /// <summary>
        /// Median Filter 11
        /// </summary>
        public FreqRecord[] MF11 { get; set; }

        /// <summary>
        /// Median Filter 11 + Aperiodic Filter 0.10
        /// </summary>
        public FreqRecord[] MF11_AF10 { get; set; }

        /// <summary>
        /// Median Filter 11 + Digital Filter
        /// </summary>
        public FreqRecord[] MF11_DF { get; set; }

        /// <summary>
        /// Median Filter 11 + Median Filter 5
        /// </summary>
        public FreqRecord[] Begin_MF11_MF5 { get; set; }

        /// <summary>
        /// Median Filter 11 + Median Filter 5
        /// </summary>
        public FreqRecord[] End_MF11_MF5 { get; set; }

        /// <summary>
        /// Median Filter 11 + Median Filter 5 + Aperiodic Filter 0.10
        /// </summary>
        public FreqRecord[] Begin_MF11_MF5_AF10 { get; set; }

        /// <summary>
        /// Median Filter 11 + Median Filter 5 + Aperiodic Filter 0.10
        /// </summary>
        public FreqRecord[] End_MF11_MF5_AF10 { get; set; }

        /// <summary>
        /// Median Filter 11 + Median Filter 5
        /// </summary>
        public FreqRecord[] MF11_MF5
        {
            get
            {
                FreqRecord[] middle = Original.Where(item => (item.GMT >= Start.GMT) && (item.GMT <= End.GMT)).Select(item => new FreqRecord { GMT = item.GMT }).ToArray();

                int count = middle.Length;
                float y0 = Begin_MF11_MF5.Last().Freq;
                float y1 = End_MF11_MF5.First().Freq;
                float k = (y1 - y0) / (count + 1);

                for (int i = 1; i <= count; i++)
                {
                    middle[i - 1].Freq = y0 + k * i;
                }

                return
                    Begin_MF11_MF5
                    .Concat(middle)
                    .Concat(End_MF11_MF5)
                    .ToArray();
            }
        }

        /// <summary>
        /// Median Filter 11 + Median Filter 5 + Aperiodic Filter 0.10
        /// </summary>
        public FreqRecord[] MF11_MF5_AF10
        {
            get
            {
                FreqRecord[] middle = Original.Where(item => (item.GMT >= Start.GMT) && (item.GMT <= End.GMT)).Select(item => new FreqRecord { GMT = item.GMT }).ToArray();

                int count = middle.Length;
                float y0 = Begin_MF11_MF5_AF10.Last().Freq;
                float y1 = End_MF11_MF5_AF10.First().Freq;
                float k = (y1 - y0) / (count + 1);

                for (int i = 1; i <= count; i++)
                {
                    middle[i - 1].Freq = y0 + k * i;
                }

                return
                    Begin_MF11_MF5_AF10
                    .Concat(middle)
                    .Concat(End_MF11_MF5_AF10)
                    .ToArray();
            }
        }

        public FreqRecord Start { get; set; }

        public FreqRecord End { get; set; }

        public float MinFreq { get; set; }

        public float MaxFreq { get; set; }

        public FreqRecord[] Freq
        {
            get
            {
                FreqRecord[] start = Original.Where(item => (item.GMT <= Start.GMT)).Select(item => new FreqRecord { GMT = item.GMT, Freq = MinFreq }).ToArray();
                FreqRecord[] middle = Original.Where(item => (item.GMT > Start.GMT) && (item.GMT < End.GMT)).Select(item => new FreqRecord { GMT = item.GMT, Freq = MinFreq }).ToArray();
                FreqRecord[] end = Original.Where(item => (item.GMT >= End.GMT)).Select(item => new FreqRecord { GMT = item.GMT, Freq = MaxFreq }).ToArray();

                int count = middle.Length;
                float y0 = MinFreq;
                float y1 = MaxFreq;
                float k = (y1 - y0)/(count + 1);

                for (int i = 1; i <= count; i++)
                {
                    middle[i - 1].Freq = y0 + k*i;
                }

                return start.Concat(middle).Concat(end).ToArray();
            }
        }

        public DateTime Gmt
        {
            get { return Start.GMT; }
        }

        public float Value
        {
            get { return _obj.FreqToV(MaxFreq) - _obj.FreqToV(MinFreq); }
        }

        public bool HasOverloadedValues
        {
            get
            {
                float maxFreqValue = _obj.TransformationTable.Last()[0];
                return (Original.Count(item => item.Freq > maxFreqValue) > 0);
            }
        }

        public bool Verified { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1:00} л.)", Gmt.ToLocalTime(), Value);
        }

        public string GetInfo()
        {
            string name = (Value < 0) ? "слива топлива" : "заправки";
            string info = string.Format(
@"Объем: {0:0.00} литров
Время: {1}
Длительность {2}: {3:0} минут
Общая длительность: {4:0} минут ({5} значений)
Начало {2}: {6} ({7} значений, зажигание {8})
Конец {2}: {9} ({10} значений, зажигание {11})",
                Value, Gmt.ToLocalTime(), name, (End.GMT - Start.GMT).TotalMinutes, (Original.Last().GMT - Original.First().GMT).TotalMinutes, Freq.Length,
                Start.GMT.ToLocalTime(), Begin_MF11_MF5.Length, Start.IsEngine ? "включено" : "выключено",
                End.GMT.ToLocalTime(), End_MF11_MF5.Length, End.IsEngine ? "включено" : "выключено");

            if (HasOverloadedValues)
            {
                info += "\r\nИмеются значения, большее чем максимальное в таблице трансформации!";
            }

            if (!Verified)
            {
                info += "\r\nРассчет не совпадает со значением, полученным из базы!";
            }

            return info;
        }

        public FreqRecord[] this[string name]
        {
            get
            {
                PropertyInfo property = GetType().GetProperties().Single(item => string.Compare(item.Name.Replace("_", string.Empty), name.Replace("_", string.Empty), true, CultureInfo.InvariantCulture) == 0);
                return (FreqRecord[])property.GetValue(this, null);
            }
        }

        public Diagram GetDiagram(Control container, out bool isCreated)
        {
            isCreated = false;
            if(_diagram == null)
            {
                float i = 0;
                //DateTime startFrom = Original[0].GMT;
                
                float[] x = Original.Select(item => i++).ToArray();
                //float[] x = Original.Select(item => (float)(item.GMT - startFrom).TotalSeconds).ToArray();
                
                float[] original = Original.Select(item => item.Freq).ToArray();
                float[] mf11 = MF11.Select(item => item.Freq).ToArray();
                float[] mf11_af10 = MF11_AF10.Select(item => item.Freq).ToArray();
                float[] mf11_df = MF11_DF.Select(item => item.Freq).ToArray();
                float[] mf11_mf5 = MF11_MF5.Select(item => item.Freq).ToArray();
                float[] mf11_mf5_af10 = MF11_MF5_AF10.Select(item => item.Freq).ToArray();

                float[] freq = Freq.Select(item => item.Freq).ToArray();

                _diagram = new Diagram(x, original, new SimpleDevice(container)) { Name = "original", Description = "Original", ForePen = Pens.White };

                new Diagram(x, mf11, _diagram) { Name = "mf11", Description = "Median Filter 11", ForePen = Pens.DarkRed };
                new Diagram(x, mf11_af10, _diagram) { Name = "mf11_af10", Description = "Median Filter 11 + Aperiodic Filter 0.10", ForePen = Pens.DarkBlue };
                new Diagram(x, mf11_df, _diagram) { Name = "mf11_df", Description = "Median Filter 11 + Digital Filter", ForePen = Pens.DarkCyan };
                new Diagram(x, mf11_mf5, _diagram) { Name = "mf11_mf5", Description = "Median Filter 11 + Median Filter 5", ForePen = Pens.DarkOrange };
                new Diagram(x, mf11_mf5_af10, _diagram) { Name = "mf11_mf5_af10", Description = "Median Filter 11 + Median Filter 5 + Aperiodic Filter 0.10", ForePen = Pens.DarkGreen };

                new Diagram(x, freq, _diagram) { Name = "freq", Description = "Уровень топлива на момент и после заправки", ForePen = Pens.Yellow, ShowPoints = false };

                if (HasOverloadedValues)
                {
                    float maxFreqValue = _obj.TransformationTable.Last()[0];
                    float[] maxFreq = Original.Select(item => maxFreqValue).ToArray();
                    new Diagram(x, maxFreq, _diagram) { Name = "maxFreq", Description = "Максимально допустимое значение датчика уровня, Гц", ForePen = Pens.DarkRed, ShowPoints = false };
                }

                isCreated = true;
            }
            return _diagram;
        }

        public Diagram GetEngineDiagram(Control container, out bool isCreated)
        {
            isCreated = false;
            if (_engineDiagram == null)
            {
                float i = 0;
                float[] x = Original.Select(item => i++).ToArray();
                float[] engine = Original.Select(item => item.IsEngine ? 1.0f : 0.0f).ToArray();
                _engineDiagram = new Diagram(x, engine, new SimpleDevice(container), -0.5f, +1.5f)
                                     {
                                         Name = "Engine",
                                         Description = "Состояние зажигания",
                                         ForePen = Pens.DarkGreen,
                                         ShowAxisY = false
                                     };
                isCreated = true;
            }
            return _engineDiagram;
        }
    }
}
