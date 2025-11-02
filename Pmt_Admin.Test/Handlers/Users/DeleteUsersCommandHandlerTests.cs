using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task Handle_WhenSingleUserExists_SoftDeletesSuccessfully()
        {
            // Arrange
            var command = new DeleteUsersCommand
            {
                Ids = new List<int> { 1 }
            };

            _userRepositoryMock
                .Setup(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().Be("Users deleted successfully");

            _userRepositoryMock.Verify(x => x.DeleteUsersByIdsAsync(It.Is<IEnumerable<int>>(ids => 
                ids.Count() == 1 && ids.First() == 1)), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMultipleUsersExist_SoftDeletesAllSuccessfully()
        {
            // Arrange
            var command = new DeleteUsersCommand
            {
                Ids = new List<int> { 1, 2, 3, 4, 5 }
            };

            _userRepositoryMock
                .Setup(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().Be("Users deleted successfully");

            _userRepositoryMock.Verify(x => x.DeleteUsersByIdsAsync(It.Is<IEnumerable<int>>(ids => 
                ids.Count() == 5)), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmptyIdList_StillCallsRepository()
        {
            // Arrange
            var command = new DeleteUsersCommand
            {
                Ids = new List<int>()
            };

            _userRepositoryMock
                .Setup(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);

            _userRepositoryMock.Verify(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var command = new DeleteUsersCommand
            {
                Ids = new List<int> { 1, 2 }
            };

            _userRepositoryMock
                .Setup(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(command, CancellationToken.None));

            _userRepositoryMock.Verify(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenLargeNumberOfIds_DeletesAllSuccessfully()
        {
            // Arrange
            var ids = Enumerable.Range(1, 100).ToList();
            var command = new DeleteUsersCommand
            {
                Ids = ids
            };

            _userRepositoryMock
                .Setup(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);

            _userRepositoryMock.Verify(x => x.DeleteUsersByIdsAsync(It.Is<IEnumerable<int>>(idList => 
                idList.Count() == 100)), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDuplicateIds_PassesAllToRepository()
        {
            // Arrange
            var command = new DeleteUsersCommand
            {
                Ids = new List<int> { 1, 1, 2, 2, 3 } // Duplicates
            };

            IEnumerable<int>? capturedIds = null;
            _userRepositoryMock
                .Setup(x => x.DeleteUsersByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .Callback<IEnumerable<int>>(ids => capturedIds = ids)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            capturedIds.Should().HaveCount(5); // Handler doesn't dedupe, repository should handle it
        }
    }
}
