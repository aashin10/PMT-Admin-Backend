using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Users;
using PmtAdmin.Application.Interfaces;
using PmtAdmin.Application.Services;
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
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPasswordHashingService> _passwordHashingServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _passwordHashingServiceMock = new Mock<IPasswordHashingService>();
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new CreateUserCommandHandler(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _passwordHashingServiceMock.Object,
                _emailServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidSingleUser_CreatesSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "john.doe@experionglobal.com",
                        Name = "John Doe",
                        Type = "Internal",
                        Status = "Active",
                        JiraId = "JIRA-123",
                        CreatedBy = 1
                    }
                }
            };

            var createdUser = new User
            {
                Id = 1,
                Email = "john.doe@experionglobal.com",
                Name = "John Doe",
                Type = "Internal",
                IsActive = true,
                JiraId = "JIRA-123",
                CreatedAt = DateTime.UtcNow
            };

            var userDto = new UserDto
            {
                Id = 1,
                Email = "john.doe@experionglobal.com",
                Name = "John Doe",
                Type = "Internal",
                Status = "Active"
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(x => x.GetByJiraIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);
            _mapperMock.Setup(x => x.Map<List<UserDto>>(It.IsAny<List<User>>())).Returns(new List<UserDto> { userDto });
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().HaveCount(1);
            result.Data.First().Email.Should().Be("john.doe@experionglobal.com");
            result.Message.Should().Be("All users created successfully");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDuplicateEmail_ReturnsError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "existing@test.com",
                        Name = "John Doe"
                    }
                }
            };

            var existingUser = new User { Id = 1, Email = "existing@test.com" };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync("existing@test.com")).ReturnsAsync(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Email already exists");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenDuplicateJiraId_ReturnsError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "john@test.com",
                        Name = "John Doe",
                        JiraId = "JIRA-123"
                    }
                }
            };

            var existingUser = new User { Id = 1, JiraId = "JIRA-123" };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(x => x.GetByJiraIdAsync("JIRA-123")).ReturnsAsync(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Jira ID already exists");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenInvalidEmailFormat_ReturnsError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "invalid-email",
                        Name = "John Doe"
                    }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Invalid email format");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenMissingRequiredFields_ReturnsError()
        {
            // Arrange
            var command = new List<CreateUserDto>
            {
                new CreateUserDto
                {
                    Email = "", // Missing email
                    Name = "John Doe"
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Email is required");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenValidMultipleUsers_CreatesAllSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "user1@test.com", Name = "User One" },
                    new CreateUserDto { Email = "user2@test.com", Name = "User Two" },
                    new CreateUserDto { Email = "user3@test.com", Name = "User Three" }
                }
            };

            var createdUsers = new List<User>
            {
                new User { Id = 1, Email = "user1@test.com", Name = "User One" },
                new User { Id = 2, Email = "user2@test.com", Name = "User Two" },
                new User { Id = 3, Email = "user3@test.com", Name = "User Three" }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto { Id = 1, Email = "user1@test.com", Name = "User One" },
                new UserDto { Id = 2, Email = "user2@test.com", Name = "User Two" },
                new UserDto { Id = 3, Email = "user3@test.com", Name = "User Three" }
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.SetupSequence(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUsers[0])
                .ReturnsAsync(createdUsers[1])
                .ReturnsAsync(createdUsers[2]);
            _mapperMock.Setup(x => x.Map<List<UserDto>>(It.IsAny<List<User>>())).Returns(userDtos);
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().HaveCount(3);
            result.Message.Should().Be("All users created successfully");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Handle_WhenPartialFailure_ReturnsPartialSuccess()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "user1@test.com", Name = "User One" },
                    new CreateUserDto { Email = "invalid-email", Name = "User Two" }, // Invalid
                    new CreateUserDto { Email = "user3@test.com", Name = "User Three" }
                }
            };

            var createdUsers = new List<User>
            {
                new User { Id = 1, Email = "user1@test.com", Name = "User One" },
                new User { Id = 3, Email = "user3@test.com", Name = "User Three" }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto { Id = 1, Email = "user1@test.com", Name = "User One" },
                new UserDto { Id = 3, Email = "user3@test.com", Name = "User Three" }
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.SetupSequence(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUsers[0])
                .ReturnsAsync(createdUsers[1]);
            _mapperMock.Setup(x => x.Map<List<UserDto>>(It.IsAny<List<User>>())).Returns(userDtos);
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().HaveCount(2);
            result.Message.Should().Contain("2 of 3 users created");
            result.Message.Should().Contain("Invalid email format");

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_WhenTypeNotProvided_InfersFromEmail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "user@experionglobal.com",
                        Name = "Internal User"
                        // Type not provided
                    }
                }
            };

            User? capturedUser = null;
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(x => x.Map<List<UserDto>>(It.IsAny<List<User>>()))
                .Returns(new List<UserDto> { new UserDto() });
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser!.Type.Should().Be("Internal"); // Should infer from experionglobal.com domain

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAvatarGeneration_CreatesCorrectUrl()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "john.doe@test.com",
                        Name = "John Doe"
                    }
                }
            };

            User? capturedUser = null;
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(x => x.Map<List<UserDto>>(It.IsAny<List<User>>()))
                .Returns(new List<UserDto> { new UserDto() });
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser!.AvatarUrl.Should().Contain("https://avatar.iran.liara.run/username?username=");
            capturedUser.AvatarUrl.Should().Contain("John");
            capturedUser.AvatarUrl.Should().Contain("Doe");
        }

        [Fact]
        public async Task Handle_WhenPasswordHashing_UsesCorrectFormat()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto
                    {
                        Email = "john.smith@test.com",
                        Name = "John Smith"
                    }
                }
            };

            string? capturedPassword = null;
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Callback<string>(p => capturedPassword = p)
                .Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(new User());
            _mapperMock.Setup(x => x.Map<List<UserDto>>(It.IsAny<List<User>>()))
                .Returns(new List<UserDto> { new UserDto() });
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedPassword.Should().NotBeNull();
            capturedPassword.Should().Be("Smith@experionglobal.123"); // lastname@experionglobal.123
        }
    }
}
