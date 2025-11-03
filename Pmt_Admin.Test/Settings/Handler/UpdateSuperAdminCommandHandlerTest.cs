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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmt_Admin.Test.Settings.Handler
{
    public class UpdateSuperAdminCommandHandlerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ISuperAdminRepository> _superAdminRepositoryMock;
        private readonly UpdateSuperAdminCommandHandler _handler;

        public UpdateSuperAdminCommandHandlerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _superAdminRepositoryMock = new Mock<ISuperAdminRepository>();
            _handler = new UpdateSuperAdminCommandHandler(
                _superAdminRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_When_SuperAdmin_Is_Updated_Successfully()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "Updated Admin Name",
                Email = "updatedemail@example.com",
                IsActive = false,
                UpdatedBy = 1
            };

            var updatedDto = new SuperAdminDto
            {
                Id = 1,
                Name = "Updated Admin Name",
                Email = "updatedemail@example.com",
                IsActive = false
            };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            _mapperMock
                .Setup(m => m.Map<SuperAdminDto>(It.IsAny<User>()))
                .Returns(updatedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(200);
            result.Data.ShouldNotBeNull();
            result.Data.Name.ShouldBe("Updated Admin Name");
            result.Data.Email.ShouldBe("updatedemail@example.com");
            result.Data.IsActive.ShouldBeFalse();

            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mapperMock.Verify(m => m.Map<SuperAdminDto>(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_SuperAdmin_Not_Found()
        {
            // Arrange
            var command = new UpdateSuperAdminCommand
            {
                Id = 999,
                Name = "Updated Name"
            };

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
            _mapperMock.Verify(m => m.Map<SuperAdminDto>(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_User_Is_Not_SuperAdmin()
        {
            // Arrange
            var nonSuperAdmin = SuperAdminMock.GetNonSuperAdminUser();
            var command = new UpdateSuperAdminCommand
            {
                Id = 10,
                Name = "Updated Name"
            };

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
        public async Task Handle_Should_Return_Fail_When_SuperAdmin_Is_Deleted()
        {
            // Arrange
            var deletedUser = SuperAdminMock.GetDeletedSuperAdmin();
            var command = new UpdateSuperAdminCommand
            {
                Id = 5,
                Name = "Updated Name"
            };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(5))
                .ReturnsAsync(deletedUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Cannot update deleted SuperAdmin");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Only_Update_Provided_Fields()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "New Name Only",
                Email = null, // Not updating email
                IsActive = null, // Not updating IsActive
                UpdatedBy = 1
            };

            User capturedUser = null;

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

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
            capturedUser.Name.ShouldBe("New Name Only");
            capturedUser.Email.ShouldBe("admin1@example.com"); // Original email unchanged
            capturedUser.IsActive.ShouldBeTrue(); // Original IsActive unchanged
        }

        [Fact]
        public async Task Handle_Should_Set_UpdatedAt_And_UpdatedBy()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Name = "Updated Name",
                UpdatedBy = 1
            };

            User capturedUser = null;

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

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
            capturedUser.UpdatedAt.HasValue.ShouldBeTrue();
            capturedUser.UpdatedAt.Value.ShouldBeGreaterThan(DateTime.UtcNow.AddMinutes(-1));
            capturedUser.UpdatedBy.ShouldBe(1);
        }

        [Fact]
        public async Task Handle_Should_Update_Email_When_Provided()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Email = "newemail@example.com"
            };

            User capturedUser = null;

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

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
            capturedUser.Email.ShouldBe("newemail@example.com");
        }

        [Fact]
        public async Task Handle_Should_Update_IsActive_When_Provided()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                IsActive = false
            };

            User capturedUser = null;

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

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
            capturedUser.IsActive.ShouldBeFalse();
        }

        [Fact]
        public async Task Handle_Should_Return_Fail_When_Email_Already_Exists()
        {
            // Arrange
            var existingUser = SuperAdminMock.GetSuperAdminWithIdOne();
            var command = new UpdateSuperAdminCommand
            {
                Id = 1,
                Email = "existing@example.com"
            };

            _superAdminRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingUser);

            _superAdminRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<User>()))
                .ThrowsAsync(new DuplicateEntryException("Email already exists"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldBe(400);
            result.Message.ShouldBe("Email already exists");
            result.Data.ShouldBeNull();

            _superAdminRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _superAdminRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
        }
    }
}
