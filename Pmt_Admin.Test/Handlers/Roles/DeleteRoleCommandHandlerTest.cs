using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Handlers.Role;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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
        public async Task Handle_WhenRoleExists_ShouldDeleteRole()
        {
            // Arrange
            var command = new DeleteRoleCommand { Id = 1 };
            var role = new Role { Id = 1 };

            _roleRepositoryMock.Setup(x => x.GetById(command.Id)).ReturnsAsync(role);
            _roleRepositoryMock.Setup(x => x.DeleteAsync(role)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().Be("Role deleted successfully");
        }

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var command = new DeleteRoleCommand { Id = 1 };

            _roleRepositoryMock.Setup(x => x.GetById(command.Id)).ReturnsAsync((Role)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("Role not found");
        }
    }
}
