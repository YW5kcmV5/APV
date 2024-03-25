using System;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Core.BusinessLogic.Extensions
{
    public static class UserManagementExtensions
    {
        public static void SetLocation(this UserEntity user, string address)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            UserManagement.Instance.SetLocation(user, address);
        }
    }
}