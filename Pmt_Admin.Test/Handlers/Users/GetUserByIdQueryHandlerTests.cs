using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Users
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_mapperMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserExists_ReturnsSuccessWithUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Alice Johnson",
                Email = "alice@company.com",
                Type = "Internal",
                IsActive = true,
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow.AddDays(-1),
                IsDeleted = false
            };

            var userDto = new UserDto
            {
                Id = 1,
                Name = "Alice Johnson",
                Email = "alice@company.com",
                Type = "Internal",
                Status = "Active",
                Created_At = "10/30/2024",
                Last_Login = "10/29/2024"
            };

            var query = new GetUserByIdQuery { Id = 1 };

            _userRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync(user);

            _mapperMock
                .Setup(x => x.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(1);
            result.Data.Name.Should().Be("Alice Johnson");
            result.Data.Email.Should().Be("alice@company.com");
            result.Data.Type.Should().Be("Internal");
            result.Data.Status.Should().Be("Active");

            _userRepositoryMock.Verify(x => x.GetById(1), Times.Once);
            _mapperMock.Verify(x => x.Map<UserDto>(user), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 999 };

            _userRepositoryMock
                .Setup(x => x.GetById(999))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("User not found");
            result.Data.Should().BeNull();

            _userRepositoryMock.Verify(x => x.GetById(999), Times.Once);
            _mapperMock.Verify(x => x.Map<UserDto>(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserIsDeleted_ReturnsNotFound()
        {
            // Arrange
            var deletedUser = new User
            {
                Id = 1,
                Name = "Deleted User",
                Email = "deleted@company.com",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow,
                IsActive = false
            };

            var query = new GetUserByIdQuery { Id = 1 };

            // Repository should return null for deleted users
            _userRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("User not found");
            result.Data.Should().BeNull();

            _userRepositoryMock.Verify(x => x.GetById(1), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };

            _userRepositoryMock
                .Setup(x => x.GetById(1))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(query, CancellationToken.None));

            _userRepositoryMock.Verify(x => x.GetById(1), Times.Once);
        }
    }
}
