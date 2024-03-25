using System;
using System.Collections.Generic;
using System.Linq;

namespace APV.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DataContractNamespaceAttribute : Attribute
    {
        private static readonly SortedList<int, DataContractNamespaceAttribute[]> Cache = new SortedList<int, DataContractNamespaceAttribute[]>();

        public string Prefix { get; private set; }

        public string Uri { get; private set; }

        public DataContractNamespaceAttribute(string prefix, string uri)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri));

            Prefix = prefix;
            Uri = uri;
        }

        public static DataContractNamespaceAttribute[] GetAttirbutes<T>(T instance) where T : class
        {
            Type type = typeof(T);
            return GetAttirbutes(type);
        }

        public static DataContractNamespaceAttribute[] GetAttirbutes(Type type)
        {
            lock (Cache)
            {
                int hashCode = type.FullName.GetHashCode();
                int index = Cache.IndexOfKey(hashCode);
                if (index != -1)
                {
                    return Cache.Values[index];
                }

                var items = type
                    .GetCustomAttributes(typeof(DataContractNamespaceAttribute), true)
                    .Cast<DataContractNamespaceAttribute>()
                    .ToArray();
                Cache.Add(hashCode, items);
                return items;
            }
        }
    }
}