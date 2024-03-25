using System;
using System.Reflection;

namespace APV.Common.Attributes.Proxy.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class BaseWraperAttribute : Attribute
    {
        public virtual string GetMethodCode(PropertyInfo property)
        {
            return string.Empty;
        }

        public virtual string GetPropertyGetCode(PropertyInfo property)
        {
            return string.Empty;
        }

        public virtual string GetPropertySetCode(PropertyInfo property)
        {
            return string.Empty;
        }
    }
}