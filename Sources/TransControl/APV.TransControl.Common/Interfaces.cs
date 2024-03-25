using System;

namespace APV.TransControl.Common
{
    public interface IConsumptionInfo
    {
        DateTime Gmt { get; }

        float Value { get; }
    }
}