using System;
using System.Linq;
using System.Collections.Generic;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes;

namespace APV.EntityFramework.DataLayer
{
    public abstract class BaseEntityCollection<TEntity> : List<TEntity>, IEntityCollection, IDisposable where TEntity : BaseEntity
    {
        private readonly HashSet<long> _identifiers = new HashSet<long>();
        private bool _disposed;
        private BaseEntity _owner;
        private bool _readonly;
        private bool _isNew;
        private bool? _supportsKeyword;
        private IManagement _management;

        protected BaseEntityCollection()
        {
        }

        protected BaseEntityCollection(BaseEntity owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected BaseEntityCollection(IEnumerable<TEntity> owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            AddRange(owner);
        }

        ~BaseEntityCollection()
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
                    _owner = null;
                    _identifiers.Clear();
                    base.Clear();
                }
            }
        }

        protected IManagement Management
        {
            get { return _management ?? (_management = EntityFrameworkManager.GetManagement(typeof(TEntity))); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetReadOnly()
        {
            _readonly = true;
        }

        public int IndexOf(long id)
        {
            if (_identifiers.Contains(id))
            {
                int length = Count;
                for (int i = 0; i < length; i++)
                {
                    TEntity entity = this[i];
                    if (entity.Id == id)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public new int IndexOf(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return IndexOf(entity.Id);
        }

        public bool Contains(long id)
        {
            return (_identifiers.Contains(id));
        }

        public new bool Contains(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return Contains(entity.Id);
        }

        public new void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");

            long id = entity.Id;

            bool isNew = (id == SystemConstants.UnknownId);
            if (Count == 0)
            {
                _isNew = isNew;
            }

            if (_isNew != isNew)
                throw new ArgumentOutOfRangeException("entity", string.Format("Current collection is marked as \"{0}\", but entity is not.", _isNew ? "new" : "not new"));

            if (!_isNew)
            {
                if (!_identifiers.Contains(id))
                {
                    _identifiers.Add(id);
                    base.Add(entity);
                }
            }
            else
            {
                base.Add(entity);
            }
        }

        public new void AddRange(IEnumerable<TEntity> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");

            foreach (TEntity entity in collection)
            {
                Add(entity);
            }
        }

        public new void Insert(int index, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");

            long id = entity.Id;
            if (!_identifiers.Contains(id))
            {
                _identifiers.Add(id);
                base.Insert(index, entity);
            }
        }

        public new void InsertRange(int index, IEnumerable<TEntity> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");

            foreach (TEntity entity in collection)
            {
                long id = entity.Id;
                if (!_identifiers.Contains(id))
                {
                    _identifiers.Add(id);
                    base.Insert(index, entity);
                    index++;
                }
            }
        }

        public new void RemoveAt(int index)
        {
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");
            if ((index < 0) || (index >= Count))
                throw new ArgumentOutOfRangeException("index", string.Format("Index \"{0}\" is out of range [0..{1}].", index, Count - 1));

            long id = this[index].Id;
            _identifiers.Remove(id);
            base.RemoveAt(index);
        }

        public new bool Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");

            if (_identifiers.Contains(entity.Id))
            {
                int index = IndexOf(entity);
                if (index != -1)
                {
                    _identifiers.Remove(entity.Id);
                    base.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        public new void RemoveRange(int index, int count)
        {
            if ((index < 0) || (index + count >= Count))
                throw new ArgumentOutOfRangeException("index", string.Format("Index \"{0}\" and count \"{1}\" is out of range [0..{2}].", index, count, Count - 1));

            for (int i = index; i < index + count; i++)
            {
                long id = this[index].Id;
                _identifiers.Remove(id);
                base.RemoveAt(index);
            }
        }

        public new void Clear()
        {
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");

            _identifiers.Clear();
            base.Clear();
        }

        #region Linq

        public TEntity FirstOrDefault()
        {
            return (Count > 0) ? this[0] : null;
        }

        #endregion

        #region Database operation

        public void Delete()
        {
            Management.Delete(this);
        }

        public void Save()
        {
            Management.Save(this);
        }

        #endregion

        #region IEntityCollection

        IEntity IEntityCollection.Owner
        {
            get { return Owner; }
        }

        IEnumerator<IEntity> IEnumerable<IEntity>.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<IEntity>.Add(IEntity item)
        {
            Add((TEntity)item);
        }

        bool ICollection<IEntity>.Contains(IEntity item)
        {
            return Contains((TEntity)item);
        }

        void ICollection<IEntity>.CopyTo(IEntity[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var items = (TEntity[])Convert.ChangeType(array, typeof(TEntity[]));
            CopyTo(items, arrayIndex);
        }

        bool ICollection<IEntity>.Remove(IEntity item)
        {
            return Remove((TEntity)item);
        }

        bool ICollection<IEntity>.IsReadOnly
        {
            get { return Readonly; }
        }

        int IList<IEntity>.IndexOf(IEntity item)
        {
            return IndexOf((TEntity)item);
        }

        void IList<IEntity>.Insert(int index, IEntity item)
        {
            Insert(index, (TEntity) item);
        }

        IEntity IList<IEntity>.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (TEntity) value; }
        }

        #endregion

        #region Properties

        public bool IsNew
        {
            get { return _isNew; }
        }

        public bool Readonly
        {
            get { return _readonly; }
        }

        public BaseEntity Owner
        {
            get { return _owner; }
        }

        public new TEntity this[int index]
        {
            get { return base[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (_readonly)
                    throw new InvalidOperationException("Collection is readonly.");
                if ((index < 0) || (index >= Count))
                    throw new ArgumentOutOfRangeException("index", string.Format("Index \"{0}\" is out of range [0..{1}].", index, Count - 1));
                if (value.Id != this[index].Id)
                    throw new ArgumentOutOfRangeException("value", string.Format("Icorrect id \"{0}\", can be \"{1}\" only.", value.Id, this[index].Id));

                base[index] = value;
            }
        }

        public bool SupportsKeyword
        {
            get { return (_supportsKeyword ?? (_supportsKeyword = (KeywordAttribute.GetAttribute(GetType()) != null)).Value); }
        }

        #endregion
    }
}