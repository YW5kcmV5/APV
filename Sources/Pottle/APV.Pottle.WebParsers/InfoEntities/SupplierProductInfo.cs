using System;
using System.Runtime.Serialization;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities.Collection;

namespace APV.Pottle.WebParsers.InfoEntities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class SupplierProductInfo : BaseParserInfo
    {
        protected SupplierProductInfo()
        {
        }

        public SupplierProductInfo(AbsoluteUri url, byte[] htmlHashCode)
            : base(url)
        {
            if (htmlHashCode == null)
                throw new ArgumentNullException("htmlHashCode");

            HtmlHashCode = htmlHashCode;
        }

        public SupplierProductInfo(SupplierProductInfo from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            Url = from.Url;
            ProductName = from.ProductName;
            ProductCost = from.ProductCost;
            Article = from.Article;
            OutOfStock = from.OutOfStock;
            ProducerCountryName = from.ProducerCountryName;
            ProducerCompanyName = from.ProducerCompanyName;
            Navigator = from.Navigator;
            Images = from.Images;
            Options = new ProductOptionCollection(from.Options);
            HtmlHashCode = from.HtmlHashCode;
        }

        [DataMember(Order = 0)]
        public string ProductName { get; set; }

        [DataMember(Order = 1)]
        public string Article { get; set; }

        [DataMember(Order = 2)]
        public double ProductCost { get; set; }

        [DataMember(Order = 3)]
        public string ProducerCountryName { get; set; }

        [DataMember(Order = 4)]
        public string ProducerCompanyName { get; set; }

        [DataMember(Order = 5)]
        public bool OutOfStock { get; set; }

        [DataMember(Order = 6)]
        public string[] Navigator { get; set; }

        [DataMember(Order = 7)]
        public ProductOptionCollection Options { get; set; }

        [DataMember(Order = 8)]
        public ImageInfo[] Images { get; set; }

        [DataMember(IsRequired = true, Order = 9)]
        public byte[] HtmlHashCode { get; private set; }
    }
}