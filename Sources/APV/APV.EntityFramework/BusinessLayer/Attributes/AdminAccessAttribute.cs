using System;

namespace APV.EntityFramework.BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AdminAccessAttribute : MethodAccessAttribute
    {
        public AdminAccessAttribute()
        {
            SetRoles(new[] {UserRole.Administrator});
        }

        public UserRole[] Roles
        {
            get { return GetRoles(); }
        }
    }
}