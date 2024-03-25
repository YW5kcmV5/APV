using APV.Avtoliga.UI.Web.Controllers.API;

namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ProductInfo : BaseInfo
    {
        public long ProductId { get; set; }

        public string Name { get; set; }

        public string Article { get; set; }

        public string ProducerArticle { get; set; }

        public string Period { get; set; }

        public double Cost { get; set; }

        public bool OutOfStock { get; set; }

        public string Url { get; set; }

        public string LogoUrl { get; set; }

        public string DeliveryTime { get; set; }

        public string[] ImageUrls { get; set; }

        public bool SpecialOffer { get; set; }

        public string SpecialOfferDescription { get; set; }

        public ModelInfo Model { get; set; }

        public ProductGroupInfo Group { get; set; }

        public ProducerInfo Producer { get; set; }

        public ProductInfo[] TogetherProducts { get; set; }

        public SearchMenuModel ToSeachMenu()
        {
            TrademarkInfo[] trademarks = SearchController.GetAllTrademarks();
            ProductGroupInfo[] groups = SearchController.GetAllProductGroups();

            return new SearchMenuModel
            {
                TrademarkId = (Model != null) ? Model.TrademarkId : (long?)null,
                ModelId = (Model != null) ? Model.ModelId : (long?)null,
                Producer = Producer,
                ProductGroupId = (Group != null) ? Group.ProductGroupId : ProductGroupInfo.All.ProductGroupId,
                Trademarks = trademarks,
                Groups = groups,
                ProductsInited = true,
            };
        }
    }
}