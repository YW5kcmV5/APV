using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APV.Common.Extensions
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetAllProperties(this Type type, bool includeStatic = false)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            List<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
            if (includeStatic)
            {
                properties.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Static));
                properties.AddRange(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Static));
            }
            return properties.ToArray();
        }

        public static FieldInfo[] GetAllFields(this Type type, bool includeStatic = false)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            List<FieldInfo> fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
            fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
            if (includeStatic)
            {
                fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Static));
                fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static));
            }
            return fields.ToArray();
        }
    }
}