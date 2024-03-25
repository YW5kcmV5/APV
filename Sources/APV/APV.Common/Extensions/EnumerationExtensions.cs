using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using APV.Common.Attributes.EnumAttributes;

namespace APV.Common.Extensions
{
    public static class EnumerationExtensions
    {
        private static readonly SortedList<string, string> EnumStringValues = new SortedList<string, string>();
        private static readonly SortedList<string, string> EnumDescriptions = new SortedList<string, string>(); 

        public static string GetStringValue<TEnum>(this TEnum value) where TEnum : struct
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException("value", "Only enumeration is supported.");

            string hash = string.Format("{0}.{1}",enumType.FullName, value);
            lock (EnumStringValues)
            {
                int index = EnumStringValues.IndexOfKey(hash);
                if (index != -1)
                {
                    return EnumStringValues.Values[index];
                }

                MemberInfo[] members = enumType.GetMember(value.ToString());
                MemberInfo member = (members.Length > 0) ? members[0] : null;
                object[] attributes = (member != null) ? member.GetCustomAttributes(typeof(EnumMemberAttribute), false) : null;
                EnumMemberAttribute attribute = ((attributes != null) && (attributes.Length > 0) && (attributes[0] is EnumMemberAttribute))
                    ? (EnumMemberAttribute)attributes[0]
                    : null;

                string stringValue = ((attribute != null) && (!string.IsNullOrEmpty(attribute.Value)))
                                         ? attribute.Value
                                         : value.ToString();
                EnumStringValues.Add(hash, stringValue);
                return stringValue;
            }
        }

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException("value", "Only enumeration is supported.");
            
            string hash = string.Format("{0}.{1}",enumType.FullName, value);
            lock (EnumDescriptions)
            {
                int index = EnumDescriptions.IndexOfKey(hash);
                if (index != -1)
                {
                    return EnumDescriptions.Values[index];
                }

                MemberInfo[] members = enumType.GetMember(value.ToString());
                MemberInfo member = (members.Length > 0) ? members[0] : null;
                object[] attributes = (member != null) ? member.GetCustomAttributes(typeof(DescriptionAttribute), false) : null;
                DescriptionAttribute attribute = ((attributes != null) && (attributes.Length > 0) && (attributes[0] is DescriptionAttribute))
                    ? (DescriptionAttribute)attributes[0]
                    : null;

                string description = ((attribute != null) && (!string.IsNullOrEmpty(attribute.Description)))
                                         ? attribute.Description
                                         : value.ToString();
                EnumDescriptions.Add(hash, description);
                return description;
            }
        }

        public static string GetPrefix<TEnum>(this TEnum value) where TEnum : struct
        {
            return PrefixAttribute.GetPrefix(value);
        }

        public static bool TryParseEnum<TEnum>(this string stringValue, out TEnum enumValue, bool ignoreCase = true) where TEnum : struct
        {
            if (string.IsNullOrEmpty(stringValue))
                throw new ArgumentNullException("stringValue");

            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentOutOfRangeException("stringValue", "Only enumeration is supported.");

            Array values = Enum.GetValues(enumType);
            foreach (TEnum value in values)
            {
                string enumStringValue = GetStringValue(value);
                if (string.Compare(stringValue, enumStringValue, ignoreCase, CultureInfo.InvariantCulture) == 0)
                {
                    enumValue = value;
                    return true;
                }
            }

            return Enum.TryParse(stringValue, ignoreCase, out enumValue);
        }

        public static TEnum ParseEnum<TEnum>(this string stringValue, bool ignoreCase = true) where TEnum : struct
        {
            if (string.IsNullOrEmpty(stringValue))
                throw new ArgumentNullException("stringValue");

            TEnum enumValue;
            if (TryParseEnum(stringValue, out enumValue, ignoreCase))
            {
                return enumValue;
            }

            throw new ArgumentOutOfRangeException("stringValue", string.Format("String value \"{0}\" can not be parsed to \"{1}\" enum.", stringValue, typeof(TEnum).FullName));
        }
    }
}