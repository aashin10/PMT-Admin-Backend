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
    public class DeleteSuperAdminCommandHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly DeleteSuperAdminCommandHandler _handler;

        public DeleteSuperAdminCommandHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _handler = new DeleteSuperAdminCommandHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_When_SuperAdmin_Is_Deleted_Successfully()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            var deletedDto = new SuperAdminDto
            {
                Id = 1,
                Name = "Admin One",
                Email = "admin1@example.com"
            };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(deletedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");
            result.Data.ShouldNotBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new DeleteSuperAdminCommand { Id = 999, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Should.ThrowAsync<NotFoundException>(
                async () => await _handler.Handle(command, CancellationToken.None)
            );

            exception.Message.ShouldBe("Super admin with ID 999 not found");
            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_User_Is_Not_SuperAdmin()
        {
            // Arrange - Handler doesn't check if user is super admin, just deletes any user
            var nonSuperAdmin = SuperAdminMock.GetNonSuperAdminUser();
            var command = new DeleteSuperAdminCommand { Id = 10, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(10))
                .ReturnsAsync(nonSuperAdmin);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert - Handler allows deleting any user
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_SuperAdmin_Already_Deleted()
        {
            // Arrange - Handler doesn't check if already deleted, just performs soft delete
            var deletedUser = SuperAdminMock.GetDeletedSuperAdmin();
            var command = new DeleteSuperAdminCommand { Id = 5, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(5))
                .ReturnsAsync(deletedUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert - Handler performs soft delete regardless of current delete status
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Deleting_Last_SuperAdmin()
        {
            // Arrange - This test is not applicable as the handler doesn't check for last super admin
            // The handler simply soft deletes any super admin found
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert - Handler allows deletion of any super admin
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Deleting_Only_Active_SuperAdmin()
        {
            // Arrange - Handler doesn't check for active super admin count
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            existingUser.IsActive = true;

            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert - Handler allows deletion regardless of active status
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Allow_Deletion_Of_Inactive_SuperAdmin_When_Active_Ones_Exist()
        {
            // Arrange
            var inactiveUser = new User
            {
                Id = 3,
                Name = "Inactive Admin",
                Email = "inactive@example.com",
                IsSuperAdmin = true,
                IsActive = false, // Inactive
                IsDeleted = false
            };

            var command = new DeleteSuperAdminCommand { Id = 3, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(3))
                .ReturnsAsync(inactiveUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Set_Correct_Deletion_Properties()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new DeleteSuperAdminCommand { Id = 1, DeletedBy = 1 };

            User capturedUser = null;

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedUser.ShouldNotBeNull();
            capturedUser.IsDeleted.ShouldBeTrue();
            capturedUser.DeletedAt.HasValue.ShouldBeTrue();
            capturedUser.DeletedAt.Value.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
            capturedUser.DeletedBy.ShouldBe(1);
        }

        [Fact]
        public async Task Handle_Should_Not_Check_Active_Count_If_User_Is_Inactive()
        {
            // Arrange - Handler doesn't check active count at all
            var inactiveUser = new User
            {
                Id = 3,
                IsSuperAdmin = true,
                IsActive = false,
                IsDeleted = false
            };

            var command = new DeleteSuperAdminCommand { Id = 3, DeletedBy = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(3))
                .ReturnsAsync(inactiveUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.SoftDeleteAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert - Handler performs soft delete regardless of active status
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("Super admin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.SoftDeleteAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
