using System;
using System.Collections.Generic;
using System.Reflection;

namespace APV.Common.Attributes.EnumAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class EnumValueAttribute : Attribute
    {
        private object _value;

        private static readonly SortedList<int, EnumValueAttribute> Cache = new SortedList<int, EnumValueAttribute>();

        protected static TAttribute ExtractAttributeFromEnumValue<TAttribute>(object value, Type type) where TAttribute : Attribute
        {
            MemberInfo[] members = type.GetMember(value.ToString());
            MemberInfo member = (members.Length > 0) ? members[0] : null;
            object[] attributes = (member != null) ? member.GetCustomAttributes(typeof(TAttribute), false) : null;
            TAttribute attribute = ((attributes != null) && (attributes.Length > 0) && (attributes[0] is TAttribute))
                ? (TAttribute)attributes[0]
                : null;
            return attribute;
        }

        protected static TAttribute GetAttibute<TEnumValue, TAttribute>(TEnumValue value) where TAttribute : EnumValueAttribute
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            Type enumType = typeof(TEnumValue);
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException("value", "Only enumeration is supported.");

            Type attributeType = typeof(TAttribute);
            string stringValue = value.ToString();
            int hashCode = string.Format("{0}-{1}-{2}", attributeType.FullName, enumType.FullName, stringValue).GetHashCode();

            lock (Cache)
            {
                int index = Cache.IndexOfKey(hashCode);
                if (index != -1)
                {
                    return (TAttribute)Cache.Values[index];
                }

                var attribute = ExtractAttributeFromEnumValue<TAttribute>(value, enumType);
                if (attribute != null)
                {
                    attribute._value = value;
                }
                Cache.Add(hashCode, attribute);
                return attribute;
            }
        }

        protected static TAttribute[] GetAttributes<TEnumValue, TAttribute>() where TAttribute : EnumValueAttribute
        {
            Type enumType = typeof(TEnumValue);
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Only enumeration is supported.");

            Array values = Enum.GetValues(enumType);
            var attributes = new List<TAttribute>();
            foreach (TEnumValue value in values)
            {
                TAttribute attribute = GetAttibute<TEnumValue, TAttribute>(value);
                if (attribute != null)
                {
                    attributes.Add(attribute);
                }
            }
            return attributes.ToArray();
        }

        public object Value
        {
            get { return _value; }
        }
    }
}