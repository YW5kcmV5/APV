using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace APV.CloudSync.Core.Entities.Collection
{
    [DebuggerDisplay("{Count}")]
    public abstract class BaseEntityCollection<TEntity> : IEnumerable<TEntity>, IDisposable where TEntity : FileSystemEntity
    {
        private bool _disposed;
        private List<TEntity> _items = new List<TEntity>();
        private FolderEntity _parent;

        #region IEnumerable

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            return _items.GetEnumerator();
        }

        #endregion

        #region Constructors

        protected BaseEntityCollection()
        {
        }

        protected BaseEntityCollection(FolderEntity parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _parent = parent;
        }

        protected BaseEntityCollection(FolderEntity parent, IEnumerable<TEntity> entities)
            : this(parent)
        {
            if (entities != null)
            {
                Add(entities);
            }
        }

        #endregion

        #region Protected

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    //Free managed resources
                    if (_items != null)
                    {
                        _items.Clear();
                        _items = null;
                    }
                    _parent = null;
                }

                //Free unmanaged resources here
            }
        }

        #endregion

        /// <summary>
        /// Destructor (do not declare in inherited classes)
        /// </summary>
        ~BaseEntityCollection()
        {
            //CG called destructor
            Dispose(false);
        }

        /// <summary>
        /// Disposer (do not declare in inherited classes)
        /// </summary>
        public void Dispose()
        {
            //Dispose manually
            Dispose(true);
            //GC should no call destructor
            GC.SuppressFinalize(this);
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (entities != null)
            {
                foreach (TEntity entity in entities)
                {
                    if (entity != null)
                    {
                        Add(entity);
                    }
                }
            }
        }

        public void Add(TEntity entity)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Parent = Parent;

            if (Root != null)
            {
                bool canAdd = Root.OnAddEntity(entity);
                if (canAdd)
                {
                    _items.Add(entity);
                }
            }
            else
            {
                _items.Add(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            int index = _items.IndexOf(entity);
            if (index != -1)
            {
                Root?.OnDeleteEntity(entity);
                entity.Parent = null;
                _items.RemoveAt(index);
            }
        }

        public void Clear()
        {
            int length = _items.Count;
            for (int i = 0; i < length; i++)
            {
                TEntity entity = _items[i];
                Root?.OnDeleteEntity(entity);
                entity.Parent = null;
            }
            _items.Clear();
        }

        [IgnoreDataMember]
        public FolderEntity Parent
        {
            get { return _parent; }
        }

        [IgnoreDataMember]
        public RootFolderEntity Root
        {
            get
            {
                return (_parent != null)
                    ? _parent is RootFolderEntity
                        ? (RootFolderEntity) _parent
                        : _parent.Root
                    : null;
            }
        }

        [IgnoreDataMember]
        public int Count
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _items.Count;
            }
        }

        [IgnoreDataMember]
        public TEntity this[int index]
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if ((index < 0) || (index >= _items.Count))
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index \"{index}\" is out of range [0-\"{_items.Count}\"].");

                return _items[index];
            }
        }
    }
}