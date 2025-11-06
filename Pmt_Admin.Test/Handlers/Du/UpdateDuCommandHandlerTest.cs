using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Du;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Du
{
    public class UpdateDuCommandHandlerTest
    {
        private readonly Mock<IDuRepository> _duRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateDuCommandHandler _handler;

        public UpdateDuCommandHandlerTest()
        {
            _duRepositoryMock = new Mock<IDuRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateDuCommandHandler(_duRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WhenDeliveryUnitExists_ShouldUpdateSuccessfully()
        {
            // Arrange
            var command = new UpdateDuCommand
            {
                Id = 1,
                Name = "Engineering Updated",
                Code = "ENG-001",
                Description = "Updated Software Development",
                HeadName = "John Updated",
                HeadEmail = "john.updated@example.com"
            };

            var existingDu = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                Description = "Software Development",
                DuHeadName = "John Doe",
                DuHeadEmail = "john@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var updatedDuDto = new DuDto
            {
                Id = 1,
                Name = "Engineering Updated",
                Code = "ENG-001",
                Description = "Updated Software Development",
                DuHeadName = "John Updated",
                DuHeadEmail = "john.updated@example.com",
                IsActive = true,
                ProjectCount = 5
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(existingDu);

            _duRepositoryMock
                .Setup(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None))
                .ReturnsAsync(existingDu);

            _mapperMock
                .Setup(x => x.Map<DuDto>(It.IsAny<DeliveryUnit>()))
                .Returns(updatedDuDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(1);
            result.Data.Name.Should().Be("Engineering Updated");
            result.Data.Description.Should().Be("Updated Software Development");
            result.Message.Should().Be("Delivery Unit updated successfully");

            _duRepositoryMock.Verify(x => x.GetDuById(1, CancellationToken.None), Times.Once);
            _duRepositoryMock.Verify(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDeliveryUnitDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var command = new UpdateDuCommand
            {
                Id = 999,
                Name = "Non-existent DU",
                Code = "NE-001"
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(999, CancellationToken.None))
                .ReturnsAsync((DeliveryUnit)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Message.Should().Be("Delivery Unit not found");
            result.Data.Should().BeNull();

            _duRepositoryMock.Verify(x => x.GetDuById(999, CancellationToken.None), Times.Once);
            _duRepositoryMock.Verify(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUpdatingOnlyName_ShouldUpdateSuccessfully()
        {
            // Arrange
            var command = new UpdateDuCommand
            {
                Id = 1,
                Name = "Engineering New Name",
                Code = null,
                Description = null,
                HeadName = null,
                HeadEmail = null
            };

            var existingDu = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                Description = "Software Development",
                DuHeadName = "John",
                DuHeadEmail = "john@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var updatedDuDto = new DuDto
            {
                Id = 1,
                Name = "Engineering New Name",
                Code = "ENG-001",
                Description = "Software Development",
                DuHeadName = "John",
                DuHeadEmail = "john@example.com"
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(existingDu);

            _duRepositoryMock
                .Setup(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None))
                .ReturnsAsync(existingDu);

            _mapperMock
                .Setup(x => x.Map<DuDto>(It.IsAny<DeliveryUnit>()))
                .Returns(updatedDuDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Name.Should().Be("Engineering New Name");
            result.Data.Code.Should().Be("ENG-001"); // Code remains unchanged

            _duRepositoryMock.Verify(x => x.GetDuById(1, CancellationToken.None), Times.Once);
            _duRepositoryMock.Verify(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUpdatingHeadInformation_ShouldUpdateSuccessfully()
        {
            // Arrange
            var command = new UpdateDuCommand
            {
                Id = 1,
                HeadName = "New Head",
                HeadEmail = "newhead@example.com"
            };

            var existingDu = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                DuHeadName = "Old Head",
                DuHeadEmail = "oldhead@example.com",
                CreatedAt = DateTime.UtcNow
            };

            var updatedDuDto = new DuDto
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                DuHeadName = "New Head",
                DuHeadEmail = "newhead@example.com"
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(existingDu);

            _duRepositoryMock
                .Setup(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None))
                .ReturnsAsync(existingDu);

            _mapperMock
                .Setup(x => x.Map<DuDto>(It.IsAny<DeliveryUnit>()))
                .Returns(updatedDuDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.DuHeadName.Should().Be("New Head");
            result.Data.DuHeadEmail.Should().Be("newhead@example.com");
        }

        [Fact]
        public async Task Handle_WhenUpdatingDescription_ShouldUpdateSuccessfully()
        {
            // Arrange
            var command = new UpdateDuCommand
            {
                Id = 1,
                Description = "Updated description with more details"
            };

            var existingDu = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Description = "Old description",
                CreatedAt = DateTime.UtcNow
            };

            var updatedDuDto = new DuDto
            {
                Id = 1,
                Name = "Engineering",
                Description = "Updated description with more details"
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(existingDu);

            _duRepositoryMock
                .Setup(x => x.UpdateDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None))
                .ReturnsAsync(existingDu);

            _mapperMock
                .Setup(x => x.Map<DuDto>(It.IsAny<DeliveryUnit>()))
                .Returns(updatedDuDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Description.Should().Be("Updated description with more details");
        }
    }
}
