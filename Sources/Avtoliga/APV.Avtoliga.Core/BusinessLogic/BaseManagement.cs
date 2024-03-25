using System;
using System.Reflection;
using APV.Avtoliga.Core.Application;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Common.Attributes.Proxy;
using APV.Common.Attributes.Proxy.Interfaces;
using APV.EntityFramework.Interfaces;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public abstract class BaseManagement<TEntity, TEntityCollection, TDataLayerManager> : BaseMarshalProxy, IManagement<TEntity>
        where TEntity : BaseEntity
        where TEntityCollection : BaseEntityCollection<TEntity>
        where TDataLayerManager : BaseDataLayerManager<TEntity, TEntityCollection>
    {
        protected static readonly ContextManager ContextManager = (ContextManager)EntityFrameworkManager.GetContextManager();

        private readonly TDataLayerManager _databaseManager = (TDataLayerManager)EntityFrameworkManager.GetManager<TEntity>();

        protected TDataLayerManager DatabaseManager
        {
            get { return _databaseManager; }
        }

        protected UserContext Context
        {
            get { return (UserContext)ContextManager.GetContext(); }
        }

        protected IUser User
        {
            get { return Context.User; }
        }

        protected bool Authorized
        {
            get { return (Context.Authorized); }
        }

        protected bool IsAdmin
        {
            get { return (Authorized) && (User.UserRole == UserRole.Administrator); }
        }

        protected long TypeId
        {
            get { return EntityHelper.GetTypeId<TEntity>(); }
        }

        private void CheckAccess(MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            MethodAccessAttribute attribute = MethodAccessAttribute.GetAttribute(method);
            if (attribute is AnonymousAccessAttribute)
            {
                return;
            }

            if (!Authorized)
                throw new UnauthorizedAccessException("Anonymous user. The security context does not contain an authorized user.");

            if ((attribute != null) && (!attribute.Contains(User.UserRole)))
            {
                string manager = GetType().Name;
                string operation = method.Name;
                throw new UnauthorizedAccessException(string.Format("User \"{0}\" has no access to operation \"{1}.{2}\".", User.Username, manager, operation));
            }
        }

        protected override void OnMethodInvoke(IProxyManager manager, MethodBase methodBase)
        {
            CheckAccess(methodBase);
        }

        #region IManagement

        Type IManagement.GetEntityType()
        {
            return typeof (TEntity);
        }

        IEntity IManagement.Find(long id)
        {
            return Find(id);
        }

        IEntity IManagement.Get(long id)
        {
            return Get(id);
        }

        IEntity IManagement.FindByName(string name)
        {
            return FindByName(name);
        }

        IEntity IManagement.GetByName(string name)
        {
            return GetByName(name);
        }

        void IManagement.Save(IEntity entity)
        {
            Save((TEntity)entity);
        }

        void IManagement.Save(IEntityCollection collection)
        {
            Save((TEntityCollection)collection);
        }

        void IManagement.Reload(IEntity entity)
        {
            Reload((TEntity)entity);
        }

        void IManagement.Delete(IEntity entity)
        {
            Delete((TEntity)entity);
        }

        public void Delete(IEntityCollection collection)
        {
            Delete((TEntityCollection)collection);
        }

        void IManagement.MarkAsDeleted(IEntity entity)
        {
            MarkAsDeleted((TEntity)entity);
        }

        void IManagement.Restore(IEntity entity)
        {
            Restore((TEntity)entity);
        }

        #endregion

        [ClientAccess]
        public virtual TEntity Find(long id)
        {
            return DatabaseManager.Find(id);
        }

        [ClientAccess]
        public virtual TEntity Get(long id)
        {
            return DatabaseManager.Get(id);
        }

        [ClientAccess]
        public virtual TEntity FindByName(string name)
        {
            return DatabaseManager.FindByName(name);
        }

        [ClientAccess]
        public virtual TEntity GetByName(string name)
        {
            return DatabaseManager.GetByName(name);
        }

        [ClientAccess]
        public virtual bool Exists(long id)
        {
            return DatabaseManager.Exists(id);
        }

        [ClientAccess]
        public virtual bool Exists(string name)
        {
            return DatabaseManager.Exists(name);
        }

        [ClientAccess]
        public virtual void Save(TEntity entity)
        {
            DatabaseManager.CreateOrUpdate(entity);
        }

        [ClientAccess]
        public virtual void Save(TEntityCollection collection)
        {
            DatabaseManager.CreateOrUpdate(collection);
        }

        [ClientAccess]
        public virtual void Reload(TEntity entity)
        {
            DatabaseManager.CreateOrUpdate(entity);
        }

        [ClientAccess]
        public virtual void Delete(TEntity entity)
        {
            DatabaseManager.Delete(entity);
        }

        [ClientAccess]
        public virtual void Delete(TEntityCollection collection)
        {
            DatabaseManager.Delete(collection);
        }

        [ClientAccess]
        public virtual void MarkAsDeleted(TEntity entity)
        {
            DatabaseManager.MarkAsDeleted(entity);
        }

        [ClientAccess]
        public virtual void Restore(TEntity entity)
        {
            DatabaseManager.Restore(entity);
        }

        [ClientAccess]
        public virtual TEntityCollection GetAll()
        {
            return (TEntityCollection) DatabaseManager.GetAll();
        }
    }
}