using AutoMapper;
using Moq;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using RoleManagement.Application.Commands;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Handlers.Roles
{
    public class CreateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateRoleCommandHandler _handler;

        public CreateRoleCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateRoleCommandHandler(
                _roleRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_CreateRole_WithoutPermissions_WhenPermissionIdsIsNull()
        {
            // Arrange
            var command = new CreateRoleCommand
            {
                Name = "Admin",
                Description = "Administrator Role",
                PermissionIds = null
            };

            var createdRole = new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "Administrator Role",
                CreatedAt = DateTime.UtcNow
            };

            var roleDto = new RoleDto
            {
                Id = createdRole.Id,
                Name = createdRole.Name,
                Description = createdRole.Description
            };

            _roleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Role>()))
                .ReturnsAsync(createdRole);

            _mapperMock.Setup(m => m.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(roleDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Status == 200 || result.Status == 201);
            Assert.Equal("Role created successfully", result.Message);
            Assert.Equal(roleDto.Name, result.Data.Name);
            _roleRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Role>()), Times.Once);
            _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_CreateRole_WithPermissions_WhenPermissionIdsProvided()
        {
            // Arrange
            var command = new CreateRoleCommand
            {
                Name = "Manager",
                Description = "Manager Role",
                PermissionIds = new List<int> { 10, 20 }
            };

            var createdRole = new Role
            {
                Id = 2,
                Name = "Manager",
                Description = "Manager Role",
                CreatedAt = DateTime.UtcNow
            };

            var permissions = new List<Permission>
            {
                new Permission { Id = 10, Name = "Read" },
                new Permission { Id = 20, Name = "Write" }
            };

            var roleDto = new RoleDto
            {
                Id = createdRole.Id,
                Name = createdRole.Name,
                Description = createdRole.Description
            };

            _roleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Role>()))
                .ReturnsAsync(createdRole);

            _permissionRepositoryMock.Setup(p => p.GetByIdAsync(command.PermissionIds))
                .ReturnsAsync(permissions);

            _mapperMock.Setup(m => m.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(roleDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Status == 200 || result.Status == 201);
            Assert.Equal("Role created successfully", result.Message);
            Assert.Equal("Manager", result.Data.Name);
            _roleRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Role>()), Times.Once);
            _roleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Role>()), Times.Once);
            _permissionRepositoryMock.Verify(p => p.GetByIdAsync(command.PermissionIds), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailResponse_OnException()
        {
            // Arrange
            var command = new CreateRoleCommand
            {
                Name = "ErrorRole",
                Description = "This will fail"
            };

            _roleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Role>()))
                .ThrowsAsync(new Exception("DB Connection Error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Status != 200 && result.Status != 201);
            Assert.Contains("Error creating role", result.Message);
            _roleRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Role>()), Times.Once);
        }
    }
}
