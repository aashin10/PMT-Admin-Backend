using AutoMapper;
using Moq;
using Pmt_Admin.Test.Settings.Mock;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Handlers.Settings;
using PmtAdmin.Application.Interfaces;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Settings;
using Shouldly;
using Xunit;

namespace Pmt_Admin.Test.Settings.Handler
{
    /// <summary>
    /// Comprehensive unit tests for CreateSuperAdminCommandHandler
    /// Tests include: success scenarios, validation, error handling, status codes, edge cases
    /// </summary>
    public class ComprehensiveCreateSuperAdminCommandHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly CreateSuperAdminCommandHandler _handler;

        public ComprehensiveCreateSuperAdminCommandHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new CreateSuperAdminCommandHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object,
                _emailServiceMock.Object);
        }

        #region Success Scenarios

        [Fact]
        public async Task Handle_Should_Return_201_Created_When_SuperAdmin_Created_Successfully()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                CreatedBy = 1
            };

            var mappedUser = new User { Name = command.Name, Email = command.Email };
            var savedUser = new User 
            { 
                Id = 100,
                Name = command.Name, 
                Email = command.Email,
                IsSuperAdmin = true,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
            var expectedDto = new SuperAdminDto
            {
                Id = 100,
                Name = command.Name,
                Email = command.Email,
                IsActive = true
            };

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(mappedUser);
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>())).ReturnsAsync(savedUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(savedUser)).Returns(expectedDto);
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(201, "Status should be 201 Created for successful creation");
            result.Message.ShouldBe("SuperAdmin created successfully");
            result.Data.ShouldNotBeNull();
            result.Data.Email.ShouldBe(command.Email);
        }

        [Fact]
        public async Task Handle_Should_Set_IsSuperAdmin_To_True()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Admin User",
                Email = "admin@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.ShouldNotBeNull();
            capturedUser.IsSuperAdmin.ShouldBeTrue("Created user must be marked as super admin");
        }

        [Fact]
        public async Task Handle_Should_Set_IsActive_To_True_By_Default()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.IsActive.ShouldBeTrue("New super admin should be active by default");
        }

        [Fact]
        public async Task Handle_Should_Set_IsDeleted_To_False()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.IsDeleted.ShouldBeFalse("New user should not be marked as deleted");
        }

        [Fact]
        public async Task Handle_Should_Set_CreatedAt_To_Current_UtcTime()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            var beforeTest = DateTime.UtcNow;

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterTest = DateTime.UtcNow;

            // Assert
            capturedUser.CreatedAt.ShouldBeGreaterThanOrEqualTo(beforeTest);
            capturedUser.CreatedAt.ShouldBeLessThanOrEqualTo(afterTest);
        }

        [Fact]
        public async Task Handle_Should_Set_CreatedBy_From_Command()
        {
            // Arrange
            var expectedCreatorId = 999;
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = expectedCreatorId
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.CreatedBy.ShouldBe(expectedCreatorId);
        }

        [Fact]
        public async Task Handle_Should_Generate_And_Set_Password_Hash()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.PasswordHash.ShouldNotBeNull("Password hash must be set");
            capturedUser.PasswordHash.ShouldNotBeEmpty("Password hash must not be empty");
        }

        [Fact]
        public async Task Handle_Should_Send_Welcome_Email_With_Credentials()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User { Email = command.Email, Name = command.Name });
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _emailServiceMock.Verify(e => e.SendEmailAsync(
                command.Email,
                "PMT Admin - Your Super Admin Account Credentials",
                It.Is<string>(body => body.Contains(command.Email) && body.Contains("Welcome to PMT Admin"))
            ), Times.Once, "Welcome email should be sent with credentials");
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Null()
        {
            // Act & Assert
            var exception = await Should.ThrowAsync<ValidationException>(
                async () => await _handler.Handle(null, CancellationToken.None)
            );

            exception.Message.ShouldBe("Request cannot be null");
            _superAdminRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_Should_Throw_ValidationException_When_Email_Is_Invalid(string email)
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = email,
                CreatedBy = 1
            };

            // Act & Assert
            var exception = await Should.ThrowAsync<ValidationException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldBe("Email is required");
            _superAdminRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task Handle_Should_Throw_DuplicateEntryException_When_Email_Already_Exists()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "existing@example.com",
                CreatedBy = 1
            };

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new DuplicateEntryException("Email already exists"));

            // Act & Assert
            var exception = await Should.ThrowAsync<DuplicateEntryException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldBe("Email already exists");
        }

        [Fact]
        public async Task Handle_Should_Propagate_Exception_When_Repository_Throws()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            var exception = await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldContain("Database connection failed");
        }

        [Fact]
        public async Task Handle_Should_Still_Return_Success_Even_If_Email_Send_Fails()
        {
            // Arrange - This tests resilience: user creation succeeds even if email fails
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            var savedUser = SuperAdminMock.CreateSavedSuperAdmin();
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(savedUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Email service unavailable"));

            // Act & Assert - Should throw because email is critical in this implementation
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Handle_Should_Handle_Very_Long_Names()
        {
            // Arrange
            var veryLongName = new string('A', 200);
            var command = new CreateSuperAdminCommand
            {
                Name = veryLongName,
                Email = "test@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User { Name = veryLongName });
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe(veryLongName);
        }

        [Fact]
        public async Task Handle_Should_Handle_Special_Characters_In_Name()
        {
            // Arrange
            var nameWithSpecialChars = "John O'Brien-Smith <admin@test.com>";
            var command = new CreateSuperAdminCommand
            {
                Name = nameWithSpecialChars,
                Email = "test@example.com",
                CreatedBy = 1
            };

            User capturedUser = null;
            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User { Name = nameWithSpecialChars });
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe(nameWithSpecialChars);
        }

        [Fact]
        public async Task Handle_Should_Handle_Concurrent_Requests()
        {
            // Arrange
            var command1 = new CreateSuperAdminCommand { Name = "Admin1", Email = "admin1@example.com", CreatedBy = 1 };
            var command2 = new CreateSuperAdminCommand { Name = "Admin2", Email = "admin2@example.com", CreatedBy = 1 };

            _mapperMock.Setup(m => m.Map<User>(It.IsAny<CreateSuperAdminCommand>())).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var task1 = _handler.Handle(command1, CancellationToken.None);
            var task2 = _handler.Handle(command2, CancellationToken.None);
            var results = await Task.WhenAll(task1, task2);

            // Assert
            results.Length.ShouldBe(2);
            results[0].Status.ShouldBe(201);
            results[1].Status.ShouldBe(201);
        }

        #endregion

        #region Verification Tests

        [Fact]
        public async Task Handle_Should_Call_Repository_CreateAsync_Exactly_Once()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User());
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_Mapper_Twice()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com",
                CreatedBy = 1
            };

            _mapperMock.Setup(m => m.Map<User>(command)).Returns(new User());
            _superAdminRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(new User());
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>())).Returns(new SuperAdminDto());
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<User>(command), Times.Once, "Should map command to User");
            _mapperMock.Verify(m => m.Map<SuperAdminDto>(It.IsAny<User>()), Times.Once, "Should map User to DTO");
        }

        #endregion
    }
}
