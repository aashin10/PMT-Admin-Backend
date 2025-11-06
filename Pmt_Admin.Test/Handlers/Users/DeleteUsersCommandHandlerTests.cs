using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Domain.Persistance;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Users
{
    public class DeleteUsersCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly DeleteUsersCommandHandler _handler;

        public DeleteUsersCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new DeleteUsersCommandHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUsersExist_ShouldDeleteUsers()
        {
            // Arrange
            var command = new DeleteUsersCommand { Ids = new List<int> { 1, 2 } };

            _userRepositoryMock.Setup(x => x.DeleteUsersByIdsAsync(command.Ids)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().Be("Users deleted successfully");
        }
    }
}
