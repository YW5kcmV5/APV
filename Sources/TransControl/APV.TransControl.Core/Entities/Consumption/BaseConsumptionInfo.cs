using System;
using APV.TransControl.Common;

namespace APV.TransControl.Core.Entities.Consumption
{
    public class BaseConsumptionInfo : IConsumptionInfo
    {
        public DateTime Gmt { get; set; }

        public float Value { get; set; }

        public bool GpsFault { get; set; }

        public bool Equipment { get; set; }
    }
}