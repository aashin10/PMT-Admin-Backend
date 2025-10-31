using AutoMapper;
using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Handlers.Roles
{
    public class UpdateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IPermissionRepository> _mockPermissionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateRoleCommandHandler _handler;

        public UpdateRoleCommandHandlerTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockPermissionRepository = new Mock<IPermissionRepository>();
            _mockMapper = new Mock<IMapper>();

            _handler = new UpdateRoleCommandHandler(
                _mockRoleRepository.Object,
                _mockPermissionRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRoleUpdatedSuccessfully()
        {
            // Arrange
            var roleId = 1;
            var command = new UpdateRoleCommand
            {
                Id = roleId,
                Name = "Updated Role",
                Description = "Updated description",
                Metadata = "{\"key\":\"value\"}",
                PermissionIds = new List<int> { 1, 2 }
            };

            var existingRole = new Role { Id = roleId, Name = "Old Role" };
            var updatedRole = new Role { Id = roleId, Name = "Updated Role" };
            var permissions = new List<Permission> { new Permission { Id = 1 }, new Permission { Id = 2 } };

            // Mock repository methods
            _mockRoleRepository
                .SetupSequence(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(existingRole)  // first call
                .ReturnsAsync(updatedRole);  // after update

            _mockPermissionRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(permissions);

            _mockRoleRepository
                .Setup(r => r.UpdateRolePermissionsAsync(It.IsAny<Role>(), It.IsAny<IEnumerable<int>>()))
                .Returns(Task.CompletedTask);

            
                _mockRoleRepository
    .Setup(r => r.UpdateAsync(It.IsAny<Role>()))
    .ReturnsAsync((Role role) => role);

            _mockMapper
                .Setup(m => m.Map<RoleDto>(It.IsAny<Role>()))
                .Returns(new RoleDto { Id = roleId, Name = command.Name });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Status == 200);
            Assert.Equal("Role updated successfully", result.Message);
            Assert.NotNull(result.Data);
            Assert.Equal(command.Name, result.Data.Name);

            _mockRoleRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockRoleRepository.Verify(r => r.UpdateAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRoleNotFound()
        {
            // Arrange
            _mockRoleRepository
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync((Role)null!);

            var command = new UpdateRoleCommand { Id = 99, Name = "Ghost Role" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Status == 200);
            Assert.Equal("Role not found.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMetadataIsInvalidJson()
        {
            // Arrange
            var roleId = 1;
            var existingRole = new Role { Id = roleId, Name = "Old Role" };

            _mockRoleRepository
                .Setup(r => r.GetById(roleId))
                .ReturnsAsync(existingRole);

            var command = new UpdateRoleCommand
            {
                Id = roleId,
                Name = "Updated Role",
                Metadata = "invalid_json"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Status == 200);
            Assert.Equal("Metadata must be a valid JSON string.", result.Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenInvalidPermissionIdsProvided()
        {
            // Arrange
            var roleId = 1;
            var existingRole = new Role { Id = roleId, Name = "Old Role" };
            var permissions = new List<Permission> { new Permission { Id = 1 } }; // only 1 valid permission

            _mockRoleRepository
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(existingRole);

            _mockPermissionRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(permissions);

            var command = new UpdateRoleCommand
            {
                Id = roleId,
                Name = "Updated Role",
                PermissionIds = new List<int> { 1, 2 } // invalid, only one exists
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Status == 200);
            Assert.Equal("One or more permission IDs are invalid.", result.Message);
        }
    }
}
