using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.Common.Caching;

namespace APV.EntityFramework.DataLayer
{
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    [Serializable]
    public abstract class BaseEntity : IEntity, IDisposable
    {
        [NonSerialized]
        private readonly long _typeId;
        [NonSerialized]
        private bool _disposed;
        [NonSerialized]
        private IDataLayerManager _dataLayerManager;
        [NonSerialized]
        private IEntityCollection _container;
        [NonSerialized]
        private DbTableAttribute _attribute;
        [NonSerialized]
        private bool? _supportsKeyword;
        [NonSerialized]
        private IManagement _management;
        [NonSerialized]
        private PropertyCacheManager _cache;

        protected BaseEntity()
        {
            _typeId = this.GetTypeId();
        }

        protected BaseEntity(IEntityCollection container)
            : this()
        {
            if (container == null)
                throw new ArgumentNullException("container");

            _container = container;
        }

        protected BaseEntity(long id)
            : this()
        {
            DataLayerManager.Fill(this, id);
        }

        protected BaseEntity(byte[] dump)
            : this()
        {
            DataLayerManager.Deserialize(this, dump);
        }

        ~BaseEntity()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_cache != null)
                    {
                        _cache.Dispose();
                        _cache = null;
                    }
                    _dataLayerManager = null;
                    _container = null;
                    _attribute = null;
                    _management = null;
                    _supportsKeyword = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected IDataLayerManager DataLayerManager
        {
            get { return _dataLayerManager ?? (_dataLayerManager = EntityFrameworkManager.GetManager(GetType())); }
        }

        protected IManagement Management
        {
            get { return _management ?? (_management = EntityFrameworkManager.GetManagement(GetType())); }
        }

        protected DbTableAttribute Attribute
        {
            get { return _attribute ?? (_attribute = DbTableAttribute.GetAttribute(GetType())); }
        }

        protected PropertyCacheManager Cache
        {
            get { return _cache ?? (_cache = new PropertyCacheManager()); }
        }

        private static readonly SortedList<string, Func<long>> ExpressionsProperty1Cache = new SortedList<string, Func<long>>();
        private static readonly SortedList<string, Func<long?>> ExpressionsProperty2Cache = new SortedList<string, Func<long?>>();

        protected TEntity GetKeyValue<TEntity>(Expression<Func<long>> expression) where TEntity : BaseEntity
        {
            string propertyName = expression.ExtractName();
            return (TEntity) Cache.GetValue(propertyName, () =>
                {
                    var id = (long)Attribute.GetDbFieldValue(this, propertyName);
                    return Activator.CreateInstance(typeof (TEntity), id);
                });
        }

        protected TEntity GetKeyValue<TEntity>(Expression<Func<long?>> expression) where TEntity : BaseEntity
        {
            string propertyName = expression.ExtractName();
            return (TEntity) Cache.GetValue(propertyName, () =>
                {
                    var id = (long?)Attribute.GetDbFieldValue(this, propertyName);
                    return (id != null)
                               ? Activator.CreateInstance(typeof(TEntity), id)
                               : null;
                });
        }

        protected TForeignEntityCollection GetCollection<TForeignEntityCollection, TForeignEntity>(Expression<Func<TForeignEntityCollection>> expression)
            where TForeignEntity : BaseEntity
            where TForeignEntityCollection : IEntityCollection
        {
            string propertyName = expression.ExtractName();
            return (TForeignEntityCollection)Cache.GetValue(propertyName, () =>
                {
                    Type type = typeof (TForeignEntity);
                    return DataLayerManager.GetCollection(this, type);
                });
        }

        protected void SetKeyValue<TEntity>(Expression<Func<long>> expression, TEntity entity) where TEntity : BaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.IsNew)
                throw new ArgumentOutOfRangeException("entity", "Specified entity is new (is not stored in database).");

            string propertyName = expression.ExtractName();
            Cache.Clear(propertyName);

            long id = entity.Id;
            DbFieldAttribute fieldAttribute = Attribute.Fields.Single(field => field.PropertyName == propertyName);
            fieldAttribute.SetValue(this, id);
        }

        protected void SetKeyValue<TEntity>(Expression<Func<long?>> expression, TEntity entity) where TEntity : BaseEntity
        {
            if ((entity != null) && (entity.IsNew))
                throw new ArgumentOutOfRangeException("entity", "Specified entity is new (is not stored in database).");

            long? id = (entity != null) ? entity.Id : (long?)null;
            string propertyName = expression.ExtractName();
            DbFieldAttribute fieldAttribute = Attribute.Fields.Single(field => field.PropertyName == propertyName);

            var currentId = (long?)fieldAttribute.GetValue(this);
            if (currentId != id)
            {
                Cache.Clear(propertyName);
                fieldAttribute.SetValue(this, id);
            }
        }

        protected void ClearCache(Expression<Func<long>> expression)
        {
            string propertyName = expression.ExtractName();
            Cache.Clear(propertyName);
        }

        protected void ClearCache(Expression<Func<long?>> expression)
        {
            string propertyName = expression.ExtractName();
            Cache.Clear(propertyName);
        }

        public IEntityCollection Container
        {
            get { return _container; }
        }

        public BaseEntity Owner
        {
            get { return (_container != null) ? (BaseEntity)_container.Owner : null; }
        }

        public long TypeId
        {
            get { return _typeId; }
        }

        public virtual long Id
        {
            get { return (long)Attribute.PrimaryKey.GetValue(this); }
        }

        public bool IsNew
        {
            get { return (Id == SystemConstants.UnknownId); }
        }

        public bool SupportsKeyword
        {
            get { return (_supportsKeyword ?? (_supportsKeyword = (KeywordAttribute.GetAttribute(GetType()) != null)).Value); }
        }

        public bool Equals(IEntity entity)
        {
            return DataLayerManager.Equals(this, entity);
        }

        public object Clone()
        {
            return DataLayerManager.Clone(this);
        }

        public byte[] Serialize()
        {
            return DataLayerManager.Serialize(this);
        }

        public void Deserialize(byte[] dump)
        {
            DataLayerManager.Deserialize(this, dump);
        }

        public string GetName()
        {
            return (Attribute.NameField != null) ? (string) Attribute.NameField.GetValue(this) : null;
        }

        public string GetDescription()
        {
            return (Attribute.DescriptionField != null) ? (string)Attribute.DescriptionField.GetValue(this) : null;
        }

        public string[] GetAlternativeNames()
        {
            return Attribute.AlternativeNameFields
                            .Select(attribute => (string) attribute.GetValue(this))
                            .Where(value => !string.IsNullOrWhiteSpace(value))
                            .ToArray();
        }

        #region IEntity

        IEntity IEntity.Owner
        {
            get { return Owner; }
        }

        #endregion

        #region Management

        public void Save()
        {
            Management.Save(this);
        }

        public void Reload()
        {
            Management.Reload(this);
        }

        public void Delete()
        {
            Management.Delete(this);
        }

        public void MarkAsDeleted()
        {
            Management.MarkAsDeleted(this);
        }

        public void Restore()
        {
            Management.Restore(this);
        }

        #endregion
    }
}