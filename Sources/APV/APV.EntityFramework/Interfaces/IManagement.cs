using System;

namespace APV.EntityFramework.Interfaces
{
    public interface IManagement
    {
        Type GetEntityType();

        IEntity Find(long id);

        IEntity Get(long id);

        IEntity FindByName(string name);

        IEntity GetByName(string name);

        bool Exists(long id);

        void Save(IEntity entity);

        void Save(IEntityCollection collection);

        void Reload(IEntity entity);

        void Delete(IEntity entity);

        void Delete(IEntityCollection collection);

        void MarkAsDeleted(IEntity entity);

        void Restore(IEntity entity);
    }

    public interface IManagement<TEntity> : IManagement where TEntity : IEntity
    {
        new TEntity Find(long id);

        new TEntity Get(long id);

        new TEntity FindByName(string name);

        new TEntity GetByName(string name);

        void Save(TEntity entity);

        void Delete(TEntity entity);

        void MarkAsDeleted(TEntity entity);

        void Restore(TEntity entity);
    }
}