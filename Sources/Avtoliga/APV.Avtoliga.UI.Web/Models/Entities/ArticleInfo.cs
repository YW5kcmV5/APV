using System;
using APV.Avtoliga.UI.Web.Controllers.API;

namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ArticleInfo : BaseInfo
    {
        public long ArticleId { get; set; }
        
        public long ArticleGroupId { get; set; }

        public ArticleGroupInfo ArticleGroup { get; set; }

        public ArticleGroupInfo[] Breadcrumbs { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Html { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Url { get; set; }

        public SearchMenuModel ToSeachMenu()
        {
            TrademarkInfo[] trademarks = SearchController.GetAllTrademarks();
            ProductGroupInfo[] groups = SearchController.GetAllProductGroups();

            return new SearchMenuModel
            {
                ProductsInited = true,
                Trademarks = trademarks,
                Groups = groups,
            };
        }
    }
}