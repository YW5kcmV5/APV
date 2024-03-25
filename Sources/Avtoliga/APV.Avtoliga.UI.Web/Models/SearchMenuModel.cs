using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class SearchMenuModel : BaseModel
    {
        public long? TrademarkId { get; set; }

        public long? ModelId { get; set; }

        public long ProductGroupId { get; set; }

        public ProducerInfo Producer { get; set; }

        public TrademarkInfo[] Trademarks { get; set; }

        public ProductGroupInfo[] Groups { get; set; }

        public ProductInfo SpecialOffertOne { get; set; }

        public ProductInfo SpecialOffertTwo { get; set; }

        public bool ProductsInited { get; set; }
    }
}