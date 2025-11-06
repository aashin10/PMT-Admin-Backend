using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Application.MappingProfiles;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using Pmt_Admin.Test.Handlers.Users.Mock;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Users
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<UserProfile>();
            });
            var mapper = mapperConfig.CreateMapper();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(mapper, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserExists_ReturnsSuccessWithUser()
        {
            // Arrange
            var users = UserMock.GetUsers();
            var user = users.First();
            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepositoryMock
                .Setup(x => x.GetById(user.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;
            var query = new GetUserByIdQuery { Id = userId };

            _userRepositoryMock
                .Setup(x => x.GetById(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("User not found");
        }
    }
}
