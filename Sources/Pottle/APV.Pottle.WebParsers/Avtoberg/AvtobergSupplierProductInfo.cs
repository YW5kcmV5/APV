using System.Runtime.Serialization;
using APV.Common.Periods;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Pottle.WebParsers.Avtoberg
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class AvtobergSupplierProductInfo : SupplierProductInfo
    {
        private AvtobergSupplierProductInfo()
        {
        }

        public AvtobergSupplierProductInfo(AbsoluteUri url, byte[] htmlHashCode)
            : base(url, htmlHashCode)
        {
        }

        public AvtobergSupplierProductInfo(AvtobergSupplierProductInfo from)
            : base(from)
        {
        }

        /// <summary>
        /// Марка
        /// </summary>
        [DataMember]
        public string TrademarkName { get; set; }

        /// <summary>
        /// Модель
        /// </summary>
        [DataMember]
        public string ModelName { get; set; }

        /// <summary>
        /// Оригинальный номер
        /// </summary>
        [DataMember]
        public string OriginalArticle { get; set; }

        /// <summary>
        /// Модель выпуска
        /// </summary>
        [DataMember]
        public AnnualPeriodCollection ModelPeriod { get; set; }

        /// <summary>
        /// Год выпуска
        /// </summary>
        [DataMember]
        public AnnualPeriodInfo ProducedPeriod { get; set; }

        /// <summary>
        /// Срок доставки
        /// </summary>
        [DataMember]
        public string DeliveryPeriod { get; set; }

        [DataMember]
        public AbsoluteUri[] BuyTogetherProductReferences { get; set; }
    }
}