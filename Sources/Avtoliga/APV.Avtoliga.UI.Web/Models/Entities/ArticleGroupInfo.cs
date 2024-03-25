
namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ArticleGroupInfo : BaseInfo
    {
        public long ArticleGroupId { get; set; }

        public string Name { get; set; }

        public ArticleGroupInfo[] Children { get; set; }

        public ArticleInfo[] Articles { get; set; }

        public string Url { get; set; }
    }
}