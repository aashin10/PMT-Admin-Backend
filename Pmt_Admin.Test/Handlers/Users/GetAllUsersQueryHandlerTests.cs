using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Application.Query;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Users
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllUsersQueryHandler(_mapperMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUsersExist_ReturnsSuccessWithUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    Name = "Bob Smith",
                    Email = "bob@company.com",
                    Type = "External",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    Status = "Active"
                },
                new UserDto
                {
                    Id = 2,
                    Name = "Bob Smith",
                    Email = "bob@company.com",
                    Type = "External",
                    Status = "Active"
                }
            };

            var query = new GetAllUsersQuery();

            _userRepositoryMock
                .Setup(x => x.GetAllNonDeletedUsersAsync())
                .ReturnsAsync(users);

            _mapperMock
                .Setup(x => x.Map<List<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data.Should().Contain(u => u.Name == "Alice Johnson");
            result.Data.Should().Contain(u => u.Name == "Bob Smith");

            _userRepositoryMock.Verify(x => x.GetAllNonDeletedUsersAsync(), Times.Once);
            _mapperMock.Verify(x => x.Map<List<UserDto>>(users), Times.Once);
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

            _userRepositoryMock.Verify(x => x.GetAllNonDeletedUsersAsync(), Times.Once);
            _mapperMock.Verify(x => x.Map<List<UserDto>>(It.IsAny<List<User>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenFilterByType_ReturnsFilteredUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    Status = "Active"
                }
            };

            var query = new GetAllUsersQuery { Type = "Internal" };

            _userRepositoryMock
                .Setup(x => x.GetFilteredUsersAsync("Internal", null))
                .ReturnsAsync(users);

            _mapperMock
                .Setup(x => x.Map<List<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(1);
            result.Data.First().Type.Should().Be("Internal");

            _userRepositoryMock.Verify(x => x.GetFilteredUsersAsync("Internal", null), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenFilterByStatus_ReturnsFilteredUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    Status = "Active"
                }
            };

            var query = new GetAllUsersQuery { Status = "Active" };

            _userRepositoryMock
                .Setup(x => x.GetFilteredUsersAsync(null, "Active"))
                .ReturnsAsync(users);

            _mapperMock
                .Setup(x => x.Map<List<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(1);
            result.Data.First().Status.Should().Be("Active");

            _userRepositoryMock.Verify(x => x.GetFilteredUsersAsync(null, "Active"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenFilterByTypeAndStatus_ReturnsFilteredUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto
                {
                    Id = 1,
                    Name = "Alice Johnson",
                    Email = "alice@company.com",
                    Type = "Internal",
                    Status = "Active"
                }
            };

            var query = new GetAllUsersQuery { Type = "Internal", Status = "Active" };

            _userRepositoryMock
                .Setup(x => x.GetFilteredUsersAsync("Internal", "Active"))
                .ReturnsAsync(users);

            _mapperMock
                .Setup(x => x.Map<List<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(1);
            result.Data.First().Type.Should().Be("Internal");
            result.Data.First().Status.Should().Be("Active");

            _userRepositoryMock.Verify(x => x.GetFilteredUsersAsync("Internal", "Active"), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var query = new GetAllUsersQuery();

            _userRepositoryMock
                .Setup(x => x.GetAllNonDeletedUsersAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(query, CancellationToken.None));

            _userRepositoryMock.Verify(x => x.GetAllNonDeletedUsersAsync(), Times.Once);
        }
    }
}
