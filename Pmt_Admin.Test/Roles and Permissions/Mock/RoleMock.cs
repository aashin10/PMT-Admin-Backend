using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;

namespace PmtAdmin.Test.Mock
{
    public static class RoleMock
    {
        public static List<Role> GetRoleList()
        {
            return new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Administrator role",
                    Metadata = "{\"level\": \"high\"}",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow,
                    RolePermissions = new List<RolePermission>
                    {
                        new RolePermission
                        {
                            Permission = new Permission
                            {
                                Id = 101,
                                Name = "CreateUser",
                                Description = "Allows creating users"
                            }
                        },
                        new RolePermission
                        {
                            Permission = new Permission
                            {
                                Id = 102,
                                Name = "DeleteUser",
                                Description = "Allows deleting users"
                            }
                        }
                    },
                    ProjectMembers = new List<ProjectMember>
                    {
                        new ProjectMember { Id = 11 },
                        new ProjectMember { Id = 12 }
                    }
                },
                new Role
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "Manager role",
                    Metadata = "{\"level\": \"medium\"}",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow,
                    RolePermissions = new List<RolePermission>
                    {
                        new RolePermission
                        {
                            Permission = new Permission
                            {
                                Id = 201,
                                Name = "ViewReports",
                                Description = "Allows viewing reports"
                            }
                        }
                    },
                    ProjectMembers = new List<ProjectMember>
                    {
                        new ProjectMember { Id = 21 }
                    }
                }
            };
        }

        public static List<RoleDto> GetRoleDtoList()
        {
            return new List<RoleDto>
            {
                new RoleDto
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Administrator role",
                    Metadata = "{\"level\": \"high\"}",
                    CreatedAt = DateTime.UtcNow.AddDays(-5).ToString("o"),
                    UserCount = 2,
                    Permissions = new List<PermissionDto>
                    {
                        new PermissionDto { Id = 101, Name = "CreateUser", Description = "Allows creating users" },
                        new PermissionDto { Id = 102, Name = "DeleteUser", Description = "Allows deleting users" }
                    }
                },
                new RoleDto
                {
                    Id = 2,
                    Name = "Manager",
                    Description = "Manager role",
                    Metadata = "{\"level\": \"medium\"}",
                    CreatedAt = DateTime.UtcNow.AddDays(-2).ToString("o"),
                    UserCount = 1,
                    Permissions = new List<PermissionDto>
                    {
                        new PermissionDto { Id = 201, Name = "ViewReports", Description = "Allows viewing reports" }
                    }
                }
            };
        }

        public static List<Role> EmptyRoleList() => new List<Role>();
    }
}
