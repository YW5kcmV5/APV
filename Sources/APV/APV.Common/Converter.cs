using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace APV.Common
{
    public static class Converter
    {
        public static string ToXmlString(this byte[] value)
        {
            if (value == null)
            {
                return null;
            }
            return Convert.ToBase64String(value);
        }

        public static string ToXmlString(this Enum value)
        {
            if (value == null)
            {
                return null;
            }

            MemberInfo[] members = value.GetType().GetMember(value.ToString());
            MemberInfo member = (members.Length > 0) ? members[0] : null;
            object[] attributes = (member != null) ? member.GetCustomAttributes(typeof(EnumMemberAttribute), false) : null;
            EnumMemberAttribute attribute = ((attributes != null) && (attributes.Length > 0) && (attributes[0] is EnumMemberAttribute))
                ? (EnumMemberAttribute)attributes[0]
                : null;
            if ((attribute != null) && (!string.IsNullOrEmpty(attribute.Value)))
            {
                return attribute.Value;
            }
            return value.ToString();
        }

        public static string ToXmlString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToXmlString(this decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToXmlString(this DateTime value)
        {
            return value.ToString("o", CultureInfo.InvariantCulture);
        }

        public static string ToXmlString(this object value)
        {
            if (ReferenceEquals(value, null))
            {
                return null;
            }
            if (value is DateTime)
            {
                return ((DateTime)value).ToXmlString();
            }
            if (value is Enum)
            {
                return ((Enum) value).ToXmlString();
            }
            if (value is byte[])
            {
                return ((byte[])value).ToXmlString();
            }
            if (value is float)
            {
                return ((float)value).ToXmlString();
            }
            if (value is decimal)
            {
                return ((decimal)value).ToXmlString();
            }
            return value.ToString();
        }

        public static object FromXmlString(string value, Type type)
        {
            if (ReferenceEquals(value, null))
            {
                return null;
            }
            if ((type == typeof(byte)) || (type == typeof(byte?)))
            {
                return byte.Parse(value);
            }
            if ((type == typeof(int)) || (type == typeof(int?)))
            {
                return int.Parse(value);
            }
            if ((type == typeof(long)) || (type == typeof(long?)))
            {
                return long.Parse(value);
            }
            if ((type == typeof(float)) || (type == typeof(float?)))
            {
                return float.Parse(value, CultureInfo.InvariantCulture);
            }
            if ((type == typeof(decimal)) || (type == typeof(decimal?)))
            {
                return decimal.Parse(value, CultureInfo.InvariantCulture);
            }
            if (type == typeof(byte[]))
            {
                return Convert.FromBase64String(value);
            }
            if ((type == typeof(DateTime)) || (type == typeof(DateTime?)))
            {
                return DateTime.ParseExact(value, "o", CultureInfo.InvariantCulture);
            }
            if (type.IsEnum)
            {
                Array names = Enum.GetNames(type);
                foreach (string name in names)
                {
                    MemberInfo[] members = type.GetMember(name);
                    MemberInfo member = (members.Length > 0) ? members[0] : null;
                    object[] attributes = member?.GetCustomAttributes(typeof(EnumMemberAttribute), false);
                    EnumMemberAttribute attribute = ((attributes != null) && (attributes.Length > 0) && (attributes[0] is EnumMemberAttribute))
                        ? (EnumMemberAttribute)attributes[0]
                        : null;
                    if ((attribute != null) && (!string.IsNullOrEmpty(attribute.Value)) && (attribute.Value == value))
                    {
                        return Enum.Parse(type, name);
                    }
                }

                return Enum.Parse(type, value);
            }
            throw new NotSupportedException($"Specified type \"{type.FullName}\" is not supported.");
        }
    }
}