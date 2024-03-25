using System;

namespace APV.Common.Attributes.EnumAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PrefixAttribute : EnumValueAttribute
    {
        private readonly string _prefix;

        public PrefixAttribute(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("prefix");

            _prefix = prefix;
        }

        public string Prefix
        {
            get { return _prefix; }
        }

        public static string GetPrefix<TEnumValue>(TEnumValue value)
        {
            PrefixAttribute attribute = GetAttibute<TEnumValue, PrefixAttribute>(value);
            if (attribute != null)
            {
                return attribute.Prefix;
            }
            return value.ToString();
        }

        public static PrefixAttribute GetAttibute<TEnumValue>(TEnumValue value)
        {
            return GetAttibute<TEnumValue, PrefixAttribute>(value);
        }

        public static PrefixAttribute[] GetAttributes<TEnumValue>()
        {
            return GetAttributes<TEnumValue, PrefixAttribute>();
        }
    }
}