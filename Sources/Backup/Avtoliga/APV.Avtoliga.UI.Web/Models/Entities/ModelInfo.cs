namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ModelInfo
    {
        public long ModelId { get; set; }

        public long TrademarkId { get; set; }

        public string Name { get; set; }

        public string Period { get; set; }

        public string Url { get; set; }

        public TrademarkInfo Trademark { get; set; }

        public ProducerInfo Producer { get; set; }

        public string DisplayName
        {
            get { return (!string.IsNullOrWhiteSpace(Period)) ? string.Format("{0} ({1})", Name, Period) : Name; }
        }
    }
}