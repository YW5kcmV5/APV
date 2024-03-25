using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace APV.Common.Periods
{
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    public sealed class AnnualPeriodInfo
    {
        private int _from = 2000;
        private int? _to;

        public AnnualPeriodInfo()
        {
        }

        public AnnualPeriodInfo(int from, int? to = null)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// "1994-2002", "2010-"
        /// </summary>
        public AnnualPeriodInfo(string period)
        {
            if (string.IsNullOrEmpty(period))
                throw new ArgumentNullException("period");

            string[] items = period.Split(new[] { " - ", " – ", "-", "–" }, StringSplitOptions.RemoveEmptyEntries);
            if ((items.Length != 1) && (items.Length != 2))
                throw new ArgumentOutOfRangeException("period", string.Format("Annual period \"{0}\" cannot be parsed.", period));

            int from = int.Parse(items[0].Trim(), NumberStyles.Integer);
            int? to = (items.Length == 2) ? int.Parse(items[1], NumberStyles.Integer) : (int?) null;

            if ((to != null) && (from > to))
            {
                From = to.Value;
                To = from;
            }
            else
            {
                From = from;
                To = to;
            }
        }

        public override string ToString()
        {
            return (_to != null) ? string.Format("{0:0000}-{1:0000}", _from, _to) : string.Format("{0:0000}-", _from);
        }

        [DataMember(Order = 0)]
        public int From
        {
            get { return _from; }
            set
            {
                if (value != _from)
                {
                    if (value <= 0)
                        throw new ArgumentOutOfRangeException("value", "Value can not be less or equal zero.");

                    _from = value;
                }
            }
        }

        [DataMember(Order = 1, IsRequired = false)]
        public int? To
        {
            get { return _to; }
            set
            {
                if (value != _to)
                {
                    if (value <= 0)
                        throw new ArgumentOutOfRangeException("value", "Value can not be less or equal zero.");
                    if (value < From)
                        throw new ArgumentOutOfRangeException("value", "Value \"To\" can not be less then \"From\".");

                    _to = value;
                }
            }
        }

        [IgnoreDataMember]
        public bool Now
        {
            get { return (To == null) || (To == DateTime.UtcNow.Year); }
        }

        public static AnnualPeriodInfo Parse(string period)
        {
            return new AnnualPeriodInfo(period);
        }
    }
}