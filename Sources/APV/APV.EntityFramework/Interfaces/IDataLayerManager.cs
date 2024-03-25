using System;
using System.Data;

namespace APV.EntityFramework.Interfaces
{
    public interface IDataLayerManager
    {
        Type GetEntityType();

        void Fill(IEntity entity, long id);

        bool Equals(IEntity x, IEntity y);

        byte[] Serialize(IEntity entity);

        void Deserialize(IEntity entity, byte[] dump);

        void Copy(IEntity entity, IEntity from);

        IEntity Clone(IEntity entity);

        IEntityCollection GetCollection(IEntity entity, string keyFieldName, long keyId);

        IEntityCollection GetCollection(IEntity container, Type keyEntityType);
    }

    public interface IDataLayerManager<TEntity> : IDataLayerManager where TEntity : IEntity
    {
        void Fill(TEntity entity, IDataRecord row);

        void Fill(TEntity entity, long id);

        TEntity Fill(IDataRecord row, IEntityCollection container = null);

        TEntity Find(long id);

        TEntity Get(long id);

        TEntity FindByName(string name);

        TEntity GetByName(string name);

        bool Equals(TEntity x, TEntity y);

        bool Exists(TEntity entity);

        bool Exists(long id);

        bool Exists(string name);

        void Delete(TEntity entity);

        void Delete(long id);

        void MarkAsDeleted(TEntity entity);

        void MarkAsDeleted(long id);

        long CreateOrUpdate(TEntity entity);

        byte[] Serialize(TEntity entity);

        TEntity Deserialize(byte[] dump);

        void Deserialize(TEntity entity, byte[] dump);

        void Copy(TEntity entity, TEntity from);

        TEntity Clone(TEntity entity);
    }
}