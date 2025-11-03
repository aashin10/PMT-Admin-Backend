using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Application.Dto;

namespace Pmt_Admin.Test.Roles_and_Permissions.Handler
{
    public class GetRolesQueryHandlerTest
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly GetRolesQueryHandler _handler;

        public GetRolesQueryHandlerTest()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new GetRolesQueryHandler(_roleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Roles_When_Data_Exists()
        {
            // Arrange
            var roles = RoleMock.GetRoleList();
            var query = new GetRolesQuery { Page = 1, PageSize = 10 };

            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var firstRole = result.First();
            Assert.Equal("Admin", firstRole.Name);
            Assert.Equal(2, firstRole.UserCount);
            Assert.Equal(2, firstRole.Permissions.Count);
            Assert.Equal("CreateUser", firstRole.Permissions.First().Name);

            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Roles()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(RoleMock.EmptyRoleList());

            var query = new GetRolesQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Roles_Are_Null()
        {
            // Arrange
            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<Role>)null!);

            var query = new GetRolesQuery { Page = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Paginate_Correctly()
        {
            // Arrange
            var roles = RoleMock.GetRoleList();
            var query = new GetRolesQuery { Page = 2, PageSize = 1 };

            _roleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(roles);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Manager", result.First().Name);

            _roleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }
    }

    // ================================
    // MOCK DATA
    // ================================
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
