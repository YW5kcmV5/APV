using System;
using APV.Avtoliga.Common;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Common.Periods;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable]
    public sealed class ProductEntity : BaseEntity, IName
    {
        #region Constructors

        public ProductEntity()
        {
        }

        public ProductEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ProductEntity(long id)
            : base(id)
        {
        }

        public ProductEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ProductId { get; internal set; }

        [DbField]
        public long GroupId { get; internal set; }

        [DbField]
        public long ModelId { get; internal set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxNameLength)]
        public string Name { get; set; }

        [DbField]
        public long? ProducerId { get; internal set; }

        [DbField(Nullable = true, MaxLength = SystemConstants.MaxNameLength)]
        public string ProducerArticle { get; set; }

        [DbField(Nullable = true, MaxLength = SystemConstants.MaxNameLength)]
        public string Article { get; set; }

        [DbField(Nullable = true, MaxLength = Constants.MaxNamePeriod)]
        public string Period { get; internal set; }

        [DbField]
        public bool OutOfStock { get; set; }

        [DbField]
        public int DeliveryTime { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        [DbField]
        public double Cost { get; set; }

        /// <summary>
        /// Специальное предложение
        /// </summary>
        [DbField]
        public bool SpecialOffer { get; set; }

        /// <summary>
        /// Комментарий к специальному предложению
        /// </summary>
        [DbField(Nullable = true, MaxLength = SystemConstants.MaxDescriptionLength)]
        public string SpecialOfferDescription { get; set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnUpdate)]
        public DateTime ModifiedAt { get; set; }

        public AnnualPeriodInfo PeriodInfo
        {
            get { return (Period != null) ? new AnnualPeriodInfo(Period) : null; }
            set { Period = (value != null) ? value.ToString() : null; }
        }

        #endregion

        #region Foreign Keys

        public ProductGroupEntity ProductGroup
        {
            get { return GetKeyValue<ProductGroupEntity>(() => GroupId); }
            set { SetKeyValue(() => GroupId, value); }
        }

        public ModelEntity Model
        {
            get { return GetKeyValue<ModelEntity>(() => ModelId); }
            set { SetKeyValue(() => ModelId, value); }
        }

        public ProducerEntity Producer
        {
            get { return GetKeyValue<ProducerEntity>(() => ProducerId); }
            set { SetKeyValue(() => ProducerId, value); }
        }

        #endregion
    }
}