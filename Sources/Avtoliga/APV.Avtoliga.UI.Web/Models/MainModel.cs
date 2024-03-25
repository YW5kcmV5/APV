using APV.Avtoliga.Common;
using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public class MainModel : BaseModel
    {
        public HelpType Type { get; set; }

        public NewsInfo LastNews { get; set; }

        public FeedbackInfo LastFeedback { get; set; }

        public ArticleInfo[] LastArticles { get; set; }

        public SearchMenuModel ToSeachMenu()
        {
            ProductInfo specialOffertOne;
            ProductInfo specialOffertTwo;
            SearchController.GetSpecialOfferts(out specialOffertOne, out specialOffertTwo);
            TrademarkInfo[] trademarks = SearchController.GetAllTrademarks();
            ProductGroupInfo[] groups = SearchController.GetAllProductGroups();

            return new SearchMenuModel
            {
                ProductsInited = false,
                Trademarks = trademarks,
                Groups = groups,
                SpecialOffertOne = specialOffertOne,
                SpecialOffertTwo = specialOffertTwo,
            };
        }
    }
}