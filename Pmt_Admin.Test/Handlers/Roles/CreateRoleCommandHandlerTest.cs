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
    public class CreateRoleCommandHandlerTest
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateRoleCommandHandler _handler;

        public CreateRoleCommandHandlerTest()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateRoleCommandHandler(_roleRepositoryMock.Object, _permissionRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldCreateRole()
        {
            // Arrange
            var command = new CreateRoleCommand
            {
                Name = "New Role",
                PermissionIds = new List<int> { 1 }
            };
            var role = new Role { Id = 1, Name = command.Name };

            _roleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Role>());
            _permissionRepositoryMock.Setup(x => x.GetByIdAsync(command.PermissionIds!)).ReturnsAsync(new List<Permission> { new Permission() });
            _roleRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Role>())).ReturnsAsync(role);
            _mapperMock.Setup(m => m.Map<RoleDto>(It.IsAny<Role>())).Returns(new RoleDto { Id = role.Id, Name = role.Name });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(role.Id);
        }

        [Fact]
        public async Task Handle_WhenRoleNameIsNotUnique_ShouldReturnFail()
        {
            // Arrange
            var command = new CreateRoleCommand { Name = "Existing Role" };

            _roleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Role>
            {
                new Role { Id = 2, Name = "Existing Role" }
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Be("Role name already exists.");
        }

        [Fact]
        public async Task Handle_WhenPermissionIdsAreInvalid_ShouldReturnFail()
        {
            // Arrange
            var command = new CreateRoleCommand { Name = "New Role", PermissionIds = new List<int> { 999 } };

            _roleRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Role>());
            _permissionRepositoryMock.Setup(x => x.GetByIdAsync(command.PermissionIds!)).ReturnsAsync(new List<Permission>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Be("One or more permissions are invalid.");
        }
    }
}
