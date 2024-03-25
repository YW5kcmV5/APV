using System;
using APV.Common;

namespace APV.EntityFramework.DataLayer
{
    public static class EntityHelper
    {
        public static long GetTypeId(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string fullName = type.FullName;
            long hashCode = fullName.GetLongHashCode();
            return hashCode;
        }

        public static long GetTypeId<TBaseEntity>() where TBaseEntity : BaseEntity
        {
            return typeof (TBaseEntity).GetTypeId();
        }

        public static long GetTypeId<TBaseEntity>(this TBaseEntity entity) where TBaseEntity : BaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return entity.GetType().GetTypeId();
        }
    }
}