using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Settings.Mock  
{
    public class SuperAdminMock
    {
        public static List<User> GetSuperAdminList()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Admin One",
                    Email = "admin1@example.com",
                    IsSuperAdmin = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new User
                {
                    Id = 2,
                    Name = "Admin Two",
                    Email = "admin2@example.com",
                    IsSuperAdmin = true,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new User
                {
                    Id = 3,
                    Name = "Admin Three",
                    Email = "admin3@example.com",
                    IsSuperAdmin = true,
                    IsActive = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                }
            };
        }

        // Alias method for compatibility
        public static List<User> GetAllSuperAdmins() => GetSuperAdminList();

        public static List<SuperAdminDto> GetSuperAdminDtoList()
        {
            return new List<SuperAdminDto>
            {
                new SuperAdminDto
                {
                    Id = 1,
                    Name = "Admin One",
                    Email = "admin1@example.com",
                    IsActive = true
                },
                new SuperAdminDto
                {
                    Id = 2,
                    Name = "Admin Two",
                    Email = "admin2@example.com",
                    IsActive = true
                },
                new SuperAdminDto
                {
                    Id = 3,
                    Name = "Admin Three",
                    Email = "admin3@example.com",
                    IsActive = false
                }
            };
        }

        // Alias method for compatibility
        public static List<SuperAdminDto> GetSuperAdminDtos() => GetSuperAdminDtoList();

        public static User GetSuperAdminWithIdOne()
        {
            return new User
            {
                Id = 1,
                Name = "Admin One",
                Email = "admin1@example.com",
                IsSuperAdmin = true,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            };
        }

        public static User GetDeletedSuperAdmin()
        {
            return new User
            {
                Id = 5,
                Name = "Deleted Admin",
                Email = "deleted@example.com",
                IsSuperAdmin = true,
                IsActive = false,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-10),
                CreatedAt = DateTime.UtcNow.AddMonths(-12)
            };
        }

        public static User GetNonSuperAdminUser()
        {
            return new User
            {
                Id = 10,
                Name = "Regular User",
                Email = "user@example.com",
                IsSuperAdmin = false,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            };
        }

        public static SuperAdminDto GetSuperAdminDtoWithIdOne()
        {
            return new SuperAdminDto
            {
                Id = 1,
                Name = "Admin One",
                Email = "admin1@example.com",
                IsActive = true
            };
        }

        public static User CreateNewSuperAdmin()
        {
            return new User
            {
                Name = "New Admin",
                Email = "newadmin@example.com",
                IsSuperAdmin = true,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static User CreateSavedSuperAdmin()
        {
            return new User
            {
                Id = 100,
                Name = "New Admin",
                Email = "newadmin@example.com",
                IsSuperAdmin = true,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static SuperAdminDto CreateNewSuperAdminDto()
        {
            return new SuperAdminDto
            {
                Id = 100,
                Name = "New Admin",
                Email = "newadmin@example.com",
                IsActive = true
            };
        }

        public static List<User> GetAllUsersIncludingNonSuperAdmins()
        {
            var list = GetSuperAdminList();
            list.Add(GetNonSuperAdminUser());
            return list;
        }

        public static List<User> EmptySuperAdminList() => new List<User>();

        public static User EmptySuperAdmin() => null;
    }
}
