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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Users
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHashingService> _passwordHashingServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHashingServiceMock = new Mock<IPasswordHashingService>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateUserCommandHandler(
                _userRepositoryMock.Object,
                _mapperMock.Object,
                _passwordHashingServiceMock.Object,
                _emailServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldCreateUser()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "test@test.com", Name = "Test User" }
                }
            };
            var user = new User { Id = 1, Email = "test@test.com", Name = "Test User" };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_WhenEmailExists_ShouldReturnError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "existing@test.com", Name = "Test User" }
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
        }

        [Fact]
        public async Task Handle_WhenJiraIdExists_ShouldReturnError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "test@test.com", Name = "Test User", JiraId = "JIRA-123" }
                }
            };
            var existingUser = new User { Id = 1, JiraId = "JIRA-123" };

            _userRepositoryMock.Setup(x => x.GetByJiraIdAsync("JIRA-123")).ReturnsAsync(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Jira ID already exists");
        }

        [Fact]
        public async Task Handle_WhenEmailFormatIsInvalid_ShouldReturnError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "invalid-email", Name = "Test User" }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Invalid email format");
        }

        [Fact]
        public async Task Handle_WhenRequiredFieldsAreMissing_ShouldReturnError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "", Name = "Test User" } // Missing email
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Contain("Email is required");
        }

        [Fact]
        public async Task Handle_WhenMultipleUsersAreValid_ShouldCreateAllSuccessfully()
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

            var user1 = new User { Id = 1, Email = "user1@test.com", Name = "User One" };
            var user2 = new User { Id = 2, Email = "user2@test.com", Name = "User Two" };
            var user3 = new User { Id = 3, Email = "user3@test.com", Name = "User Three" };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.SetupSequence(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(user1)
                .ReturnsAsync(user2)
                .ReturnsAsync(user3);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_WhenPartialFailureOccurs_ShouldReturnPartialSuccess()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "user1@test.com", Name = "User One" },
                    new CreateUserDto { Email = "user2@test.com", Name = "User Two" }, // Valid email, but repo will fail
                    new CreateUserDto { Email = "user3@test.com", Name = "User Three" }
                }
            };

            var user1 = new User { Id = 1, Email = "user1@test.com", Name = "User One" };
            var user3 = new User { Id = 3, Email = "user3@test.com", Name = "User Three" };

            // Setup GetByEmailAsync to return null for all users (no duplicates)
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            
            // Setup CreateAsync sequence: success, failure, success
            _userRepositoryMock.SetupSequence(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(user1)
                .ThrowsAsync(new Exception("Database constraint violation")) 
                .ReturnsAsync(user3);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(207); // Partial success
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Message.Should().Contain("2 of 3 users created");
            result.Message.Should().Contain("Database constraint violation");
            
            // Verify CreateAsync was called 3 times
            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Handle_WhenTypeIsNotProvided_ShouldInferFromEmail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "user@experionglobal.com", Name = "Internal User" }
                }
            };

            User? capturedUser = null;
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser!.Type.Should().Be("Internal"); // Should infer from experionglobal.com domain
        }

        [Fact]
        public async Task Handle_WhenAvatarIsGenerated_ShouldContainCorrectUrl()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "john.doe@test.com", Name = "John Doe" }
                }
            };

            User? capturedUser = null;
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser!.AvatarUrl.Should().Contain("https://avatar.iran.liara.run/username?username=");
            capturedUser.AvatarUrl.Should().Contain("John");
            capturedUser.AvatarUrl.Should().Contain("Doe");
        }

        [Fact]
        public async Task Handle_WhenPasswordIsHashed_ShouldUseCorrectFormat()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Users = new List<CreateUserDto>
                {
                    new CreateUserDto { Email = "john.smith@test.com", Name = "John Smith" }
                }
            };

            string? capturedPassword = null;
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _passwordHashingServiceMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Callback<string>(p => capturedPassword = p)
                .Returns("hashed_password");
            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(new User());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedPassword.Should().NotBeNull();
            capturedPassword.Should().Be("Smith@experionglobal.123"); // lastname@experionglobal.123
        }
    }
}
