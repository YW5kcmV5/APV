namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ProducerInfo : BaseInfo
    {
        public long ProducerId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ExternalUrl { get; set; }

        public string LogoUrl { get; set; }

        public string Title { get; set; }

        public string About { get; set; }
    }
}