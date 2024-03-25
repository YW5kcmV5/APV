using APV.TransControl.Common;

namespace APV.TransControl.Core.Entities
{
    public struct EverestMainData
    {
        public string AvtoNo { get; set; }

        public string Model { get; set; }

        public VehicleType VehicleType { get; set; }

        public string AdminName { get; set; }

        public double FuelConsumption { get; set; }

        public double FuelConsumptionNorm { get; set; }

        public string WorkGraphTypeName { get; set; }

        public string DinnerTime { get; set; }

        public OperationResult OperationResult { get; set; }
    }
}