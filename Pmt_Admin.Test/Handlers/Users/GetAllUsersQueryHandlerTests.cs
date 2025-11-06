using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Application.MappingProfiles;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using Pmt_Admin.Test.Handlers.Users.Mock;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Users
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<UserProfile>();
            });
            var mapper = mapperConfig.CreateMapper();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllUsersQueryHandler(mapper, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUsersExist_ReturnsSuccessWithUsers()
        {
            // Arrange
            var users = UserMock.GetUsers();
            var query = new GetAllUsersQuery();

            _userRepositoryMock
                .Setup(x => x.GetAllNonDeletedUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(users.Count);
        }

        [Fact]
        public async Task Handle_WhenNoUsersExist_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var users = new List<User>();
            var query = new GetAllUsersQuery();

            _userRepositoryMock
                .Setup(x => x.GetAllNonDeletedUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No users found");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }
    }
}
