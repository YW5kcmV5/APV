using APV.Avtoliga.Common;
using APV.Common;

namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ProductGroupInfo : BaseInfo
    {
        public long ProductGroupId { get; set; }

        public string Name { get; set; }

        public bool IsAll
        {
            get { return (ProductGroupId == SystemConstants.UnknownId); }
        }

        public static readonly ProductGroupInfo All = new ProductGroupInfo
            {
                ProductGroupId = SystemConstants.UnknownId,
                Name = Constants.ProductGroupAllName
            };
    }
}