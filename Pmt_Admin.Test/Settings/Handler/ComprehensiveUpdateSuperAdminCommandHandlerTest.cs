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
using Xunit;

namespace Pmt_Admin.Test.Settings.Handler
{
    /// <summary>
    /// Comprehensive unit tests for UpdateSuperAdminCommandHandler
    /// Tests include: success scenarios, validation, not found errors, authorization, partial updates
    /// </summary>
    public class ComprehensiveUpdateSuperAdminCommandHandlerTest
    {
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateSuperAdminCommandHandler _handler;

        public ComprehensiveUpdateSuperAdminCommandHandlerTest()
        {
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateSuperAdminCommandHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        #region Success Scenarios

        [Fact]
        public async Task Handle_Should_Return_200_Success_When_Update_Is_Successful()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new UpdateSuperAdminCommand
            {
                Id = existingUser.Id,
                Name = "Updated Name",
                Email = "updated@example.com",
                IsActive = true,
                UpdatedBy = 1
            };

            var updatedUser = new User { Id = existingUser.Id, Name = command.Name, Email = command.Email };
            var dto = new SuperAdminDto { Id = existingUser.Id, Name = command.Name, Email = command.Email };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(updatedUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(updatedUser))
                .Returns(dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200, "Status should be 200 OK for successful update");
            result.Message.ShouldBe("Super admin updated successfully");
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Update_Name_When_Provided()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Old Name", Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "New Name",
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe("New Name");
        }

        [Fact]
        public async Task Handle_Should_Update_Email_When_Provided()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "old@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Email = "new@example.com",
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Email.ShouldBe("new@example.com");
        }

        [Fact]
        public async Task Handle_Should_Update_IsActive_When_Provided()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsActive = true, IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                IsActive = false,
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.IsActive.ShouldBeFalse();
        }

        [Fact]
        public async Task Handle_Should_Set_UpdatedAt_To_Current_UtcTime()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand { Id = 1, Name = "Updated", UpdatedBy = 1 };

            User capturedUser = null;
            var beforeTest = DateTime.UtcNow;

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterTest = DateTime.UtcNow;

            // Assert
            capturedUser.UpdatedAt.ShouldNotBeNull();
            capturedUser.UpdatedAt.Value.ShouldBeGreaterThanOrEqualTo(beforeTest);
            capturedUser.UpdatedAt.Value.ShouldBeLessThanOrEqualTo(afterTest);
        }

        [Fact]
        public async Task Handle_Should_Set_UpdatedBy_From_Command()
        {
            // Arrange
            var expectedUpdatorId = 999;
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "Updated",
                UpdatedBy = expectedUpdatorId
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.UpdatedBy.ShouldBe(expectedUpdatorId);
        }

        #endregion

        #region Partial Update Tests

        [Fact]
        public async Task Handle_Should_Not_Update_Name_When_Not_Provided()
        {
            // Arrange
            var originalName = "Original Name";
            var existingUser = new User { Id = 1, Name = originalName, Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = null, // Not provided
                Email = "new@example.com",
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe(originalName, "Name should not change when not provided");
        }

        [Fact]
        public async Task Handle_Should_Not_Update_Email_When_Not_Provided()
        {
            // Arrange
            var originalEmail = "original@example.com";
            var existingUser = new User { Id = 1, Name = "Test", Email = originalEmail, IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "New Name",
                Email = null, // Not provided
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Email.ShouldBe(originalEmail, "Email should not change when not provided");
        }

        [Fact]
        public async Task Handle_Should_Not_Update_IsActive_When_Not_Provided()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsActive = true, IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "New Name",
                IsActive = null, // Not provided
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.IsActive.ShouldBeTrue("IsActive should not change when not provided");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_Should_Not_Update_Name_When_Empty_Or_Whitespace(string name)
        {
            // Arrange
            var originalName = "Original Name";
            var existingUser = new User { Id = 1, Name = originalName, Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = name,
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe(originalName);
        }

        #endregion

        #region Not Found Tests

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new UpdateSuperAdminCommand
            {
                Id = 999,
                Name = "Test",
                UpdatedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldContain("Super admin with ID 999 not found");
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_404_Status_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new UpdateSuperAdminCommand
            {
                Id = 999,
                Name = "Test",
                UpdatedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            // NotFoundException should result in 404
            exception.ShouldBeOfType<NotFoundException>();
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task Handle_Should_Throw_When_Repository_GetByIdAsync_Throws()
        {
            // Arrange
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "Test",
                UpdatedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Repository_UpdateAsync_Throws()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new UpdateSuperAdminCommand
            {
                Id = existingUser.Id,
                Name = "Test",
                UpdatedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Update failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_DuplicateEntryException_When_Email_Already_Exists()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Email = "existing@example.com",
                UpdatedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ThrowsAsync(new DuplicateEntryException("Email already exists"));

            // Act & Assert
            await Should.ThrowAsync<DuplicateEntryException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        #endregion

        #region Authorization Tests

        [Fact]
        public async Task Handle_Should_Allow_Update_By_Any_Authorized_User()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new UpdateSuperAdminCommand
            {
                Id = existingUser.Id,
                Name = "Updated",
                UpdatedBy = 123 // Different user
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.UpdatedBy.ShouldBe(123);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Handle_Should_Handle_Very_Long_Names()
        {
            // Arrange
            var veryLongName = new string('A', 500);
            var existingUser = new User { Id = 1, Name = "Old", Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = veryLongName,
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe(veryLongName);
        }

        [Fact]
        public async Task Handle_Should_Handle_Special_Characters_In_Name()
        {
            // Arrange
            var nameWithSpecialChars = "John O'Brien <test@example.com> & Co.";
            var existingUser = new User { Id = 1, Name = "Old", Email = "test@example.com", IsSuperAdmin = true };
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = nameWithSpecialChars,
                UpdatedBy = 1
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe(nameWithSpecialChars);
        }

        [Fact]
        public async Task Handle_Should_Handle_Multiple_Updates_To_Same_User()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Original", Email = "test@example.com", IsSuperAdmin = true };
            var command1 = new UpdateSuperAdminCommand { Id = 1, Name = "Update 1", UpdatedBy = 1 };
            var command2 = new UpdateSuperAdminCommand { Id = 1, Name = "Update 2", UpdatedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command1, CancellationToken.None);
            await _handler.Handle(command2, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Exactly(2));
        }

        #endregion

        #region Verification Tests

        [Fact]
        public async Task Handle_Should_Call_Repository_GetByIdAsync_Once()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new UpdateSuperAdminCommand { Id = existingUser.Id, Name = "Test", UpdatedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_Repository_UpdateAsync_Once()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new UpdateSuperAdminCommand { Id = existingUser.Id, Name = "Test", UpdatedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_Mapper_Once()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new UpdateSuperAdminCommand { Id = existingUser.Id, Name = "Test", UpdatedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<SuperAdminDto>(It.IsAny<User>()), Times.Once);
        }

        #endregion
    }
}
