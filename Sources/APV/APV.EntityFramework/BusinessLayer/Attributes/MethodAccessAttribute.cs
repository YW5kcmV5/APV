using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APV.EntityFramework.BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class MethodAccessAttribute : Attribute
    {
        private UserRole[] _roles = new UserRole[0];

        protected UserRole[] GetRoles()
        {
            return _roles;
        }

        protected void SetRoles(UserRole[] roles)
        {
            _roles = roles ?? new UserRole[0];
        }

        public bool Contains(UserRole role)
        {
            return _roles.Contains(role);
        }

        public static MethodAccessAttribute GetAttribute(MethodBase method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            IEnumerable<MethodAccessAttribute> attributes = method.GetCustomAttributes<MethodAccessAttribute>(true);
            MethodAccessAttribute attribute = attributes.FirstOrDefault();
            return attribute;
        }
    }
}