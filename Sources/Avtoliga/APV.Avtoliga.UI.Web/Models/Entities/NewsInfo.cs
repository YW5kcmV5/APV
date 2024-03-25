namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class NewsInfo : BaseInfo
    {
        public long NewsId { get; set; }

        public string Caption { get; set; }

        public string Text { get; set; }

        public string TextP { get; set; }

        public string LogoUrl { get; set; }

        public string CreatedAt { get; set; }

        public int Likes { get; set; }

        public bool Liked { get; set; }
    }
}