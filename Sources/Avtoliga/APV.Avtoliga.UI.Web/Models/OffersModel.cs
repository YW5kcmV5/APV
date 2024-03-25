using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class OffersModel : BaseModel
    {
        public OffersModel()
        {
            ProductCollection products = ProductManagement.Instance.GetLastSpecialOffers();
            Products = products.Transform(false);
        }

        public ProductInfo[] Products { get; set; }
    }
}