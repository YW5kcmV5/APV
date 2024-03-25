using System;
using System.Reflection;

namespace APV.Common.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (property.CanRead)
            {
                MethodInfo methodGet = property.GetGetMethod() ?? property.GetGetMethod(true);
                if (methodGet != null)
                {
                    return methodGet.IsStatic;
                }
            }
            if (property.CanWrite)
            {
                MethodInfo methodSet = property.GetSetMethod() ?? property.GetSetMethod(true);
                if (methodSet != null)
                {
                    return methodSet.IsStatic;
                }
            }
            return false;
        }
    }
}
