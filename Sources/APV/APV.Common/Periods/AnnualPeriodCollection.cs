using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace APV.Common.Periods
{
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    public class AnnualPeriodCollection
    {
        private List<AnnualPeriodInfo> _items = new List<AnnualPeriodInfo>();

        public AnnualPeriodCollection()
        {
            _items = new List<AnnualPeriodInfo>();
        }

        /// <summary>
        /// "1994-2002, 2003-", "2010-"
        /// </summary>
        public AnnualPeriodCollection(string period)
        {
            if (string.IsNullOrEmpty(period))
                throw new ArgumentNullException("period");

            string[] items = period.Split(new[] { ", ", "; ", " / ", ",", ";", "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0)
                throw new ArgumentOutOfRangeException("period", string.Format("Annual period collection \"{0}\" cannot be parsed. No periods are found.", period));

            foreach (string item in items)
            {
                var periofInfo = new AnnualPeriodInfo(item);
                Add(periofInfo);
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void Add(AnnualPeriodInfo info)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            //if (Now)
            //    throw new ArgumentOutOfRangeException("info", "The new period can not be added because the curremt interval is active till now.");
            //if (info.From <= To)
            //    throw new ArgumentOutOfRangeException("info", string.Format("The new \"From\" (\"{0:0000}\") value less or equal to existing \"To\" value \"{1:0000}\".", info.From, To));

            _items.Add(info);
        }

        public override string ToString()
        {
            return string.Join(", ", _items);
        }

        [DataMember(IsRequired = true)]
        public AnnualPeriodInfo[] Items
        {
            get { return _items.ToArray(); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _items = _items ?? new List<AnnualPeriodInfo>();
                Clear();
                foreach (AnnualPeriodInfo option in value)
                {
                    Add(option);
                }
            }
        }

        [IgnoreDataMember]
        public int? From
        {
            get { return (_items.Count > 0) ? _items[0].From : (int?)null; }
        }

        [IgnoreDataMember]
        public int? To
        {
            get { return (_items.Count > 0) ? _items[_items.Count - 1].To : null; }
        }

        [IgnoreDataMember]
        public bool Now
        {
            get { return ((_items.Count > 0) && (_items[_items.Count - 1].Now)); }
        }

        public static AnnualPeriodCollection Parse(string period)
        {
            return new AnnualPeriodCollection(period);
        }
    }
}