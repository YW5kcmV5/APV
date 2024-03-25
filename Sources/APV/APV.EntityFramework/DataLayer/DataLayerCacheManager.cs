using System;
using System.Collections.Generic;
using APV.Common.Attributes.Db;

namespace APV.EntityFramework.DataLayer
{
    internal static class DataLayerCacheManager
    {
        private const int DefaultCapacity = 15000;

        private const int LimitStep = 50;

        private class Container
        {
            public readonly SortedList<long, BaseEntity> SortedValue = new SortedList<long, BaseEntity>(DefaultCapacity);

            public readonly List<BaseEntity> Values = new List<BaseEntity>(DefaultCapacity);

            public int Limit;
        }

        private static readonly SortedList<int, Container> Cache = new SortedList<int, Container>();

        private static Container GetContainer(Type type)
        {
            int key = type.FullName.GetHashCode();
            int index = Cache.IndexOfKey(key);
            if (index != -1)
            {
                return Cache.Values[index];
            }
            
            DbTableAttribute attribute = DbTableAttribute.GetAttribute(type);
            var container = new Container
                {
                    Limit = attribute.CacheLimit
                };
            Cache.Add(key, container);
            return container;
        }

        public static TEntity Get<TEntity>(long id) where TEntity : BaseEntity
        {
            lock (Cache)
            {
                Container container = GetContainer(typeof (TEntity));
                SortedList<long, BaseEntity> sortedValues = container.SortedValue;
                int index = sortedValues.IndexOfKey(id);
                if (index != -1)
                {
                    var entity = (TEntity) sortedValues.Values[index];
                    //var clone = (TEntity) entity.Clone();
                    //return clone;
                    return entity;
                }
                return null;
            }
        }

        public static void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long id = entity.Id;
            lock (Cache)
            {
                Container container = GetContainer(typeof(TEntity));
                SortedList<long, BaseEntity> sortedValues = container.SortedValue;
                int index = sortedValues.IndexOfKey(id);
                if (index == -1)
                {
                    int limit = container.Limit;
                    if (limit != 0)
                    {
                        List<BaseEntity> values = container.Values;
                        values.Add(entity);
                        if ((limit != 0) && (values.Count >= limit + LimitStep))
                        {
                            int countToDelete = Math.Min(2*LimitStep, values.Count);
                            for (int i = 0; i <= countToDelete; i++)
                            {
                                var entityToDelete = (TEntity)values[0];
                                long idToDelete = entityToDelete.Id;
                                values.RemoveAt(0);
                                sortedValues.Remove(idToDelete);
                            }
                        }
                    }
                    var clone = (TEntity)entity.Clone();
                    sortedValues.Add(id, clone);
                }
                else
                {
                    if (container.Limit != 0)
                    {
                        BaseEntity existingEntity = sortedValues.Values[index];
                        List<BaseEntity> values = container.Values;
                        index = values.IndexOf(existingEntity);
                        values.RemoveAt(index);
                        values.Add(entity);
                    }
                    var clone = (TEntity)entity.Clone();
                    sortedValues[id] = clone;
                }
            }
        }

        public static void Delete<TEntity>(long id) where TEntity : BaseEntity
        {
            lock (Cache)
            {
                Container container = GetContainer(typeof(TEntity));
                SortedList<long, BaseEntity> sortedValues = container.SortedValue;
                int index = sortedValues.IndexOfKey(id);
                if (index != -1)
                {
                    if (container.Limit != 0)
                    {
                        var entity = (TEntity)sortedValues.Values[index];
                        List<BaseEntity> values = container.Values;
                        index = values.IndexOf(entity);
                        values.RemoveAt(index);
                    }
                    sortedValues.RemoveAt(index);
                }
            }
        }
    }
}