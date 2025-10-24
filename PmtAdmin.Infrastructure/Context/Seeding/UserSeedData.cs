using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Context.Seeding
{
    public static class UserSeedData
    {
        public static List<User> GetUsers()
        {
            var users = new List<User>();
            var baseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            for (int i = 1; i <= 10; i++)
            {
                users.Add(new User
                {
                    Id = i,
                    Email = $"user{i}@pmtadmin.com",
                    Name = $"User {i}",
                    IsActive = true,
                    IsSuperAdmin = i == 1,
                    IsDeleted = false,
                    CreatedAt = baseDate
                });
            }

            return users;
        }

        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(GetUsers());
        }
    }
}
