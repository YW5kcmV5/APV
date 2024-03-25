using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APV.Common.Caching
{
    public static class CacheableUtility
    {
        public static ICollection<T> Clone<T>(ICollection<T> collection)
        {
            if (collection == null)
            {
                return null;
            }
            var newCollection = (ICollection<T>)Activator.CreateInstance(collection.GetType());
            foreach (T item in collection)
            {
                newCollection.Add(item);
            }
            return newCollection;
        }

        public static ICloneable Clone(ICloneable item)
        {
            return (ICloneable) item?.Clone();
        }

        public static object Clone(object item)
        {
            if (item != null)
            {
                if (item is ICloneable)
                {
                    return Clone((ICloneable)item);
                }
                if (IsCollection(item))
                {
                    Type type = item.GetType();
                    object newCollection = Activator.CreateInstance(type);
                    MethodInfo add = type.GetMethod("Add");
                    if (add.GetParameters().Length == 2)
                    {
                        var @int = type.GetInterfaces().First(@interface => @interface.Name == "ICollection`1");
                        add = @int.GetMethod("Add");
                    }
                    var collection = (IEnumerable)item;
                    foreach (object collectionItem in collection)
                    {
                        add.Invoke(newCollection, new[] { collectionItem });
                    }
                    return newCollection;
                }
            }
            return item;
        }

        public static bool IsCollection(Type type)
        {
            if (type == null)
            {
                return false;
            }
            string typeName = type.Name;
            switch (typeName)
            {
                case "List`1":
                case "Collection`1":
                case "SortedList`2":
                case "IList`1":
                case "ICollection`1":
                case "ISortedList`2":
                    return true;
                default:
                    Type baseType = type.BaseType;
                    if ((baseType != null) && (IsCollection(baseType)))
                    {
                        return true;
                    }
                    Type[] interfaces = type.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        if (IsCollection(@interface))
                        {
                            return true;
                        }
                    }
                    return false;
            }
        }

        public static bool IsCollection<T>(ICollection<T> collection)
        {
            return true;
        }

        public static bool IsCollection<T>(IList<T> collection)
        {
            return true;
        }

        public static bool IsCollection(object instance)
        {
            if (instance == null)
            {
                return false;
            }
            return IsCollection(instance.GetType());
        }

        public static void Clear<T>(ICollection<T> collection)
        {
            collection?.Clear();
        }

        public static void Clear<T>(IList<T> list)
        {
            list?.Clear();
        }

        public static void Clear(object item)
        {
            if (!ReferenceEquals(item, null))
            {
                Type type = item.GetType();
                if (IsCollection(item))
                {
                    type.GetMethod("Clear").Invoke(item, null);
                }
            }
        }
    }
}