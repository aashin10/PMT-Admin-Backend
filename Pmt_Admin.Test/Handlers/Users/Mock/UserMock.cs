using PmtAdmin.Domain.Entities;
using System.Collections.Generic;

namespace Pmt_Admin.Test.Handlers.Users.Mock
{
    public static class UserMock
    {
        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
                new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" }
            };
        }
    }
}
