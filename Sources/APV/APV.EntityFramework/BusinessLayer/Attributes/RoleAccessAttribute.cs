using System;

namespace APV.EntityFramework.BusinessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RoleAccessAttribute : MethodAccessAttribute
    {
        public UserRole[] Roles
        {
            get { return GetRoles(); }
            set { SetRoles(value); }
        }
    }
}