using PmtAdmin.Domain.Entities;
using System.Collections.Generic;

namespace Pmt_Admin.Test.Handlers.Permissions.Mock
{
    public static class PermissionMock
    {
        public static List<Permission> GetPermissions()
        {
            return new List<Permission>
            {
                new Permission { Id = 1, Name = "Create" },
                new Permission { Id = 2, Name = "Read" },
                new Permission { Id = 3, Name = "Update" },
                new Permission { Id = 4, Name = "Delete" }
            };
        }
    }
}
