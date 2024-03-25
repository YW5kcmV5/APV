using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class ArticlesModel : BaseModel
    {
        public ArticleGroupInfo ArticleGroup { get; set; }

        public ArticleGroupInfo[] Groups { get; set; }

        public ArticleInfo[] Articles { get; set; }
    }
}