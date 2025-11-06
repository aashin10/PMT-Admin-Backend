using PmtAdmin.Domain.Entities;
using System.Collections.Generic;

namespace Pmt_Admin.Test.Handlers.Roles.Mock
{
    public static class RoleMock
    {
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };
        }
    }
}
