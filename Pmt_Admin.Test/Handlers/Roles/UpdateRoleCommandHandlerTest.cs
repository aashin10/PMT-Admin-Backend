using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Roles
{
    public class UpdateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateRoleCommandHandler _handler;

        public UpdateRoleCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateRoleCommandHandler(_roleRepositoryMock.Object, _permissionRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WhenRoleExists_ShouldUpdateRole()
        {
            // Arrange
            var command = new UpdateRoleCommand
            {
                Id = 1,
                Name = "Updated Role",
                PermissionIds = new List<int> { 1 }
            };
            var role = new Role { Id = 1, Name = "Original Role" };

            _roleRepositoryMock.Setup(x => x.GetById(command.Id)).ReturnsAsync(role);
            _permissionRepositoryMock.Setup(x => x.GetByIdAsync(command.PermissionIds!)).ReturnsAsync(new List<Permission> { new Permission() });
            _roleRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Role>())).ReturnsAsync(role);
            _roleRepositoryMock.Setup(x => x.GetById(command.Id)).ReturnsAsync(role);
            _mapperMock.Setup(m => m.Map<RoleDto>(It.IsAny<Role>())).Returns(new RoleDto { Id = role.Id, Name = command.Name });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            role.Name.Should().Be("Updated Role");
        }

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var command = new UpdateRoleCommand { Id = 1 };

            _roleRepositoryMock.Setup(x => x.GetById(command.Id)).ReturnsAsync((Role)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("Role not found.");
        }
    }
}
