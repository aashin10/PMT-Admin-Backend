using AutoMapper;
using Moq;
using Pmt_Admin.Test.Settings.Mock;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Handlers.Settings;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Settings;
using Shouldly;

namespace Pmt_Admin.Test.Settings.Handler
{
    public class CreateSuperAdminCommandHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly CreateSuperAdminCommandHandler _handler;

        public CreateSuperAdminCommandHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _handler = new CreateSuperAdminCommandHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Created_When_SuperAdmin_Is_Created_Successfully()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "New Admin",
                Email = "newadmin@example.com"
            };

            var mappedUser = SuperAdminMock.CreateNewSuperAdmin();
            var savedUser = SuperAdminMock.CreateSavedSuperAdmin();
            var expectedDto = SuperAdminMock.CreateNewSuperAdminDto();

            _mapperMock
                .Setup(m => m.Map<User>(command))
                .Returns(mappedUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(savedUser);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(savedUser))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(201);
            result.Message.ShouldBe("SuperAdmin created successfully");
            result.Data.ShouldNotBeNull();
            result.Data.Name.ShouldBe("New Admin");
            result.Data.Email.ShouldBe("newadmin@example.com");
            result.Data.IsActive.ShouldBeTrue();

            // Verify repository was called with correct properties
            _superAdminRepositoryMock.Verify(repo => repo.CreateAsync(
                It.Is<User>(u =>
                    u.IsSuperAdmin == true &&
                    u.IsActive == true &&
                    u.IsDeleted == false &&
                    u.CreatedAt != default
                )), Times.Once);

            _mapperMock.Verify(m => m.Map<User>(command), Times.Once);
            _mapperMock.Verify(m => m.Map<SuperAdminDto>(savedUser), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Set_Correct_Properties_On_User_Entity()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com"
            };

            var mappedUser = new User
            {
                Name = "Test Admin",
                Email = "test@example.com"
            };

            User capturedUser = null;

            _mapperMock
                .Setup(m => m.Map<User>(command))
                .Returns(mappedUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.ShouldNotBeNull();
            capturedUser.IsSuperAdmin.ShouldBeTrue();
            capturedUser.IsActive.ShouldBeTrue();
            capturedUser.IsDeleted.ShouldBeFalse();
            capturedUser.CreatedAt.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
            capturedUser.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Email_Already_Exists()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "existing@example.com"
            };

            _mapperMock
                .Setup(m => m.Map<User>(command))
                .Returns(new User { Name = command.Name, Email = command.Email });

            _superAdminRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new DuplicateEntryException("Email already exists"));

            // Act & Assert
            var exception = await Should.ThrowAsync<DuplicateEntryException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldBe("Email already exists");
            _superAdminRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Handle_Should_Throw_When_Email_Is_Invalid(string email)
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = email
            };

            // Act & Assert
            var exception = await Should.ThrowAsync<ValidationException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldContain("Email is required");
            _superAdminRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Set_Default_Values_When_Optional_Fields_Not_Provided()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com"
                // IsActive and IsSuperAdmin not set
            };

            User capturedUser = null;

            _mapperMock
                .Setup(m => m.Map<User>(command))
                .Returns(new User { Name = command.Name, Email = command.Email });

            _superAdminRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.ShouldNotBeNull();
            capturedUser.IsSuperAdmin.ShouldBeTrue(); // Should default to true
            capturedUser.IsActive.ShouldBeTrue(); // Should default to true
            capturedUser.IsDeleted.ShouldBeFalse(); // Should default to false
        }

        [Fact]
        public async Task Handle_Should_Set_CreatedAt_To_UtcNow()
        {
            // Arrange
            var command = new CreateSuperAdminCommand
            {
                Name = "Test Admin",
                Email = "test@example.com"
            };

            User capturedUser = null;

            _mapperMock
                .Setup(m => m.Map<User>(command))
                .Returns(new User { Name = command.Name, Email = command.Email });

            _superAdminRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            var beforeTest = DateTime.UtcNow;

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterTest = DateTime.UtcNow;

            // Assert
            capturedUser.ShouldNotBeNull();
            capturedUser.CreatedAt.ShouldBeGreaterThanOrEqualTo(beforeTest);
            capturedUser.CreatedAt.ShouldBeLessThanOrEqualTo(afterTest);
        }
    }
}
