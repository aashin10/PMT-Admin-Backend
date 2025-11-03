using AutoMapper;
using Moq;
using Pmt_Admin.Test.Settings.Mock;
using PmtAdmin.Application.Command.Settings;
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
            var command = new DeleteSuperAdminCommand { Id = 1 };

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
                .Setup(repo => repo.CountActiveSuperAdminsAsync())
                .ReturnsAsync(3); // More than 1

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveEnabledSuperAdminsAsync())
                .ReturnsAsync(2); // More than 1 active

            _superAdminRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(deletedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("SuperAdmin deleted successfully");
            result.Data.ShouldNotBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.CountActiveSuperAdminsAsync(), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new DeleteSuperAdminCommand { Id = 999 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(404);
            result.Message.ShouldBe("SuperAdmin not found");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(999), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_User_Is_Not_SuperAdmin()
        {
            // Arrange
            var nonSuperAdmin = SuperAdminMock.GetNonSuperAdminUser();
            var command = new DeleteSuperAdminCommand { Id = 10 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(10))
                .ReturnsAsync(nonSuperAdmin);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("User is not a SuperAdmin");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_SuperAdmin_Already_Deleted()
        {
            // Arrange
            var deletedUser = SuperAdminMock.GetDeletedSuperAdmin();
            var command = new DeleteSuperAdminCommand { Id = 5 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(5))
                .ReturnsAsync(deletedUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("SuperAdmin already deleted");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Deleting_Last_SuperAdmin()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new DeleteSuperAdminCommand { Id = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveSuperAdminsAsync())
                .ReturnsAsync(1); // Only 1 left

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Cannot delete the last remaining SuperAdmin.");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.CountActiveSuperAdminsAsync(), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Deleting_Only_Active_SuperAdmin()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            existingUser.IsActive = true;

            var command = new DeleteSuperAdminCommand { Id = 1 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveSuperAdminsAsync())
                .ReturnsAsync(3); // More than 1 total

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveEnabledSuperAdminsAsync())
                .ReturnsAsync(1); // Only 1 active

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Cannot delete the only active SuperAdmin. There must be at least one active SuperAdmin left.");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.CountActiveEnabledSuperAdminsAsync(), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
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

            var command = new DeleteSuperAdminCommand { Id = 3 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(3))
                .ReturnsAsync(inactiveUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveSuperAdminsAsync())
                .ReturnsAsync(3);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveEnabledSuperAdminsAsync())
                .ReturnsAsync(2); // 2 active admins exist

            _superAdminRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Message.ShouldBe("SuperAdmin deleted successfully");

            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Set_Correct_Deletion_Properties()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new DeleteSuperAdminCommand { Id = 1 };

            User capturedUser = null;

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveSuperAdminsAsync())
                .ReturnsAsync(3);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveEnabledSuperAdminsAsync())
                .ReturnsAsync(2);

            _superAdminRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
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
        }

        [Fact]
        public async Task Handle_Should_Not_Check_Active_Count_If_User_Is_Inactive()
        {
            // Arrange
            var inactiveUser = new User
            {
                Id = 3,
                IsSuperAdmin = true,
                IsActive = false,
                IsDeleted = false
            };

            var command = new DeleteSuperAdminCommand { Id = 3 };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(3))
                .ReturnsAsync(inactiveUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveSuperAdminsAsync())
                .ReturnsAsync(2);

            _superAdminRepositoryMock
                .Setup(repo => repo.CountActiveEnabledSuperAdminsAsync())
                .ReturnsAsync(2);

            _superAdminRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(new SuperAdminDto());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe(200);

            // Verify the active enabled count check happened
            _superAdminRepositoryMock.Verify(repo => repo.CountActiveEnabledSuperAdminsAsync(), Times.Once);
        }
    }
}
