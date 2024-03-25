namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class TrademarkInfo : BaseInfo
    {
        public long TrademarkId { get; set; }

        public string Name { get; set; }

        public string About { get; set; }

        public string Url { get; set; }

        public string LogoUrl { get; set; }

        public ModelInfo[] Models { get; set; }
    }
}