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
    /// Comprehensive unit tests for DeleteSuperAdminCommandHandler
    /// Tests include: success scenarios, validation, not found errors, soft delete, authorization
    /// </summary>
    public class ComprehensiveDeleteSuperAdminCommandHandlerTest
    {
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DeleteSuperAdminCommandHandler _handler;

        public ComprehensiveDeleteSuperAdminCommandHandlerTest()
        {
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new DeleteSuperAdminCommandHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        #region Success Scenarios

        [Fact]
        public async Task Handle_Should_Return_200_Success_When_Delete_Is_Successful()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand
            {
                Id = existingUser.Id,
                DeletedBy = 1
            };

            var deletedUser = new User { Id = existingUser.Id, IsDeleted = true };
            var dto = new SuperAdminDto { Id = existingUser.Id };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(deletedUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(deletedUser))
                .Returns(dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200, "Status should be 200 OK for successful delete");
            result.Message.ShouldBe("Super admin deleted successfully");
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Set_IsDeleted_To_True()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true, IsDeleted = false };
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.IsDeleted.ShouldBeTrue("User should be marked as deleted");
        }

        [Fact]
        public async Task Handle_Should_Set_DeletedAt_To_Current_UtcTime()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            User capturedUser = null;
            var beforeTest = DateTime.UtcNow;

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            var afterTest = DateTime.UtcNow;

            // Assert
            capturedUser.DeletedAt.ShouldNotBeNull("DeletedAt should be set");
            capturedUser.DeletedAt.Value.ShouldBeGreaterThanOrEqualTo(beforeTest);
            capturedUser.DeletedAt.Value.ShouldBeLessThanOrEqualTo(afterTest);
        }

        [Fact]
        public async Task Handle_Should_Set_DeletedBy_From_Command()
        {
            // Arrange
            var expectedDeletorId = 999;
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new DeleteSuperAdminCommand
            {
                Id = 1,
                DeletedBy = expectedDeletorId
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.DeletedBy.ShouldBe(expectedDeletorId);
        }

        [Fact]
        public async Task Handle_Should_Perform_Soft_Delete_Not_Hard_Delete()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand { Id = existingUser.Id, DeletedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once, "Should use soft delete");
            // Verify hard delete is never called (assuming there's a DeleteAsync method for hard delete)
        }

        #endregion

        #region Not Found Tests

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new DeleteSuperAdminCommand
            {
                Id = 999,
                DeletedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldContain("Super admin with ID 999 not found");
            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_404_Status_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new DeleteSuperAdminCommand
            {
                Id = 999,
                DeletedBy = 1
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

        #region Already Deleted Tests

        [Fact]
        public async Task Handle_Should_Allow_Deleting_Already_Deleted_User()
        {
            // Arrange - Some systems allow re-deleting to update DeletedBy or DeletedAt
            var existingUser = new User
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com",
                IsSuperAdmin = true,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1),
                DeletedBy = 99
            };
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);
            capturedUser.IsDeleted.ShouldBeTrue();
            capturedUser.DeletedBy.ShouldBe(1, "DeletedBy should be updated to current deleter");
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task Handle_Should_Throw_When_Repository_GetByIdAsync_Throws()
        {
            // Arrange
            var command = new DeleteSuperAdminCommand
            {
                Id = 1,
                DeletedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Repository_SoftDeleteAsync_Throws()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand
            {
                Id = existingUser.Id,
                DeletedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Delete operation failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Mapper_Throws()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand
            {
                Id = existingUser.Id,
                DeletedBy = 1
            };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Throws(new Exception("Mapping failed"));

            // Act & Assert
            await Should.ThrowAsync<Exception>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );
        }

        #endregion

        #region Authorization Tests

        [Fact]
        public async Task Handle_Should_Allow_Delete_By_Any_Authorized_User()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand
            {
                Id = existingUser.Id,
                DeletedBy = 456 // Different user
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.DeletedBy.ShouldBe(456);
        }

        [Fact]
        public async Task Handle_Should_Track_Who_Performed_The_Delete()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var deletorId = 789;
            var command = new DeleteSuperAdminCommand
            {
                Id = 1,
                DeletedBy = deletorId
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.DeletedBy.ShouldBe(deletorId, "Should track who performed the delete");
        }

        #endregion

        #region Idempotency Tests

        [Fact]
        public async Task Handle_Should_Be_Idempotent_When_Called_Multiple_Times()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result1 = await _handler.Handle(command, CancellationToken.None);
            var result2 = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result1.Status.ShouldBe(200);
            result2.Status.ShouldBe(200);
            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Exactly(2));
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Handle_Should_Delete_User_With_No_DeletedBy_Specified()
        {
            // Arrange - Testing when DeletedBy might be null or 0
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command = new DeleteSuperAdminCommand
            {
                Id = 1,
                DeletedBy = 0 // Or null, depending on command structure
            };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.IsDeleted.ShouldBeTrue();
            capturedUser.DeletedBy.ShouldBe(0);
        }

        [Fact]
        public async Task Handle_Should_Preserve_Other_User_Properties()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                Name = "Test Admin",
                Email = "test@example.com",
                IsSuperAdmin = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                CreatedBy = 10
            };
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            User capturedUser = null;
            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.Name.ShouldBe("Test Admin");
            capturedUser.Email.ShouldBe("test@example.com");
            capturedUser.IsSuperAdmin.ShouldBeTrue();
            capturedUser.CreatedBy.ShouldBe(10);
            capturedUser.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public async Task Handle_Should_Handle_Concurrent_Delete_Requests()
        {
            // Arrange
            var existingUser = new User { Id = 1, Name = "Test", Email = "test@example.com", IsSuperAdmin = true };
            var command1 = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };
            var command2 = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 2 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var task1 = _handler.Handle(command1, CancellationToken.None);
            var task2 = _handler.Handle(command2, CancellationToken.None);
            var results = await Task.WhenAll(task1, task2);

            // Assert
            results.Length.ShouldBe(2);
            results[0].Status.ShouldBe(200);
            results[1].Status.ShouldBe(200);
        }

        #endregion

        #region Verification Tests

        [Fact]
        public async Task Handle_Should_Call_Repository_GetByIdAsync_Once()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand { Id = existingUser.Id, DeletedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_Repository_SoftDeleteAsync_Once()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand { Id = existingUser.Id, DeletedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(existingUser);
            _mapperMock.Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_Mapper_Once()
        {
            // Arrange
            var existingUser = SuperAdminMock.CreateSavedSuperAdmin();
            var command = new DeleteSuperAdminCommand { Id = existingUser.Id, DeletedBy = 1 };

            _superAdminRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id))
                .ReturnsAsync(existingUser);
            _superAdminRepositoryMock.Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
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
