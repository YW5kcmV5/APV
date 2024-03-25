using System;

namespace APV.EntityFramework.BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ClientAccessAttribute : MethodAccessAttribute
    {
        public ClientAccessAttribute()
        {
            SetRoles(new[] {UserRole.Administrator, UserRole.Client});
        }

        public UserRole[] Roles
        {
            get { return GetRoles(); }
        }
    }
}