using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Handlers.Roles
{
    public class DeleteRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly DeleteRoleCommandHandler _handler;

        public DeleteRoleCommandHandlerTests()
        {
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _handler = new DeleteRoleCommandHandler(_roleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRoleDeletedSuccessfully()
        {
            // Arrange
            var roleId = 1;
            var role = new Role { Id = roleId, Name = "Admin" };

            _roleRepositoryMock.Setup(r => r.GetById(roleId))
                .ReturnsAsync(role);

            _roleRepositoryMock.Setup(r => r.DeleteAsync(role))
                .Returns(Task.CompletedTask);

            var command = new DeleteRoleCommand { Id = roleId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.Equal("Request processed successfully", result.Message);
            _roleRepositoryMock.Verify(r => r.GetById(roleId), Times.Once);
            _roleRepositoryMock.Verify(r => r.DeleteAsync(role), Times.Once);

        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRoleNotFound()
        {
            // Arrange
            var roleId = 999;

            _roleRepositoryMock.Setup(r => r.GetById(roleId))
                .ReturnsAsync((Role)null);

            var command = new DeleteRoleCommand { Id = roleId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Status == 400 || result.Status == 404);
            Assert.Equal("Role not found", result.Message);
            _roleRepositoryMock.Verify(r => r.GetById(roleId), Times.Once);
            _roleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenExceptionThrown()
        {
            // Arrange
            var roleId = 2;
            var role = new Role { Id = roleId, Name = "Manager" };

            _roleRepositoryMock.Setup(r => r.GetById(roleId))
                .ReturnsAsync(role);

            _roleRepositoryMock.Setup(r => r.DeleteAsync(role))
                .ThrowsAsync(new Exception("Database failure"));

            var command = new DeleteRoleCommand { Id = roleId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Contains("Error deleting role", result.Message);
            _roleRepositoryMock.Verify(r => r.GetById(roleId), Times.Once);
            _roleRepositoryMock.Verify(r => r.DeleteAsync(role), Times.Once);
        }
    }
}
