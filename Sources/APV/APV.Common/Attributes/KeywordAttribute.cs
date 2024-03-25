using System;
using APV.Common.Attributes.Reflection;

namespace APV.Common.Attributes
{
    /// <summary>
    /// Shows that specified entity supports keywords
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class KeywordAttribute : ReflectionClassAttribute
    {
        public static KeywordAttribute GetAttribute(Type instanceType)
        {
            var attribute = GetAttribute<KeywordAttribute>(instanceType);
            return attribute;
        }
    }
}