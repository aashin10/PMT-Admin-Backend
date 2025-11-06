using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Handlers.Du;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Du
{
    public class DeleteDuCommandHandlerTest
    {
        private readonly Mock<IDuRepository> _duRepositoryMock;
        private readonly DeleteDuCommandHandler _handler;

        public DeleteDuCommandHandlerTest()
        {
            _duRepositoryMock = new Mock<IDuRepository>();
            _handler = new DeleteDuCommandHandler(_duRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenDeliveryUnitExists_ShouldDeleteSuccessfully()
        {
            // Arrange
            var command = new DeleteDuCommand(1);
            var du = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(du);

            _duRepositoryMock
                .Setup(x => x.DeleteDuAsync(du, CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().Be("Delivery Unit deleted successfully");
            result.Message.Should().Be("Request processed successfully");

            _duRepositoryMock.Verify(x => x.GetDuById(1, CancellationToken.None), Times.Once);
            _duRepositoryMock.Verify(x => x.DeleteDuAsync(du, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDeliveryUnitDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var command = new DeleteDuCommand(999);

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
            _duRepositoryMock.Verify(x => x.DeleteDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenDeletingWithValidId_ShouldCallRepositoryOnce()
        {
            // Arrange
            var command = new DeleteDuCommand(2);
            var du = new DeliveryUnit
            {
                Id = 2,
                Name = "Marketing",
                Code = "MKT-001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(2, CancellationToken.None))
                .ReturnsAsync(du);

            _duRepositoryMock
                .Setup(x => x.DeleteDuAsync(du, CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(200);
            _duRepositoryMock.Verify(x => x.DeleteDuAsync(It.Is<DeliveryUnit>(d => d.Id == 2), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new DeleteDuCommand(1);
            var du = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                CreatedAt = DateTime.UtcNow
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(du);

            _duRepositoryMock
                .Setup(x => x.DeleteDuAsync(du, CancellationToken.None))
                .ThrowsAsync(new InvalidOperationException("Cannot delete delivery unit with active projects"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenMultipleDeletionsInSequence_ShouldHandleCorrectly()
        {
            // Arrange - First deletion
            var command1 = new DeleteDuCommand(1);
            var du1 = new DeliveryUnit
            {
                Id = 1,
                Name = "Engineering",
                Code = "ENG-001",
                CreatedAt = DateTime.UtcNow
            };

            // Arrange - Second deletion
            var command2 = new DeleteDuCommand(2);
            var du2 = new DeliveryUnit
            {
                Id = 2,
                Name = "Marketing",
                Code = "MKT-001",
                CreatedAt = DateTime.UtcNow
            };

            _duRepositoryMock
                .Setup(x => x.GetDuById(1, CancellationToken.None))
                .ReturnsAsync(du1);

            _duRepositoryMock
                .Setup(x => x.GetDuById(2, CancellationToken.None))
                .ReturnsAsync(du2);

            _duRepositoryMock
                .Setup(x => x.DeleteDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result1 = await _handler.Handle(command1, CancellationToken.None);
            var result2 = await _handler.Handle(command2, CancellationToken.None);

            // Assert
            result1.Status.Should().Be(200);
            result2.Status.Should().Be(200);
            _duRepositoryMock.Verify(x => x.DeleteDuAsync(It.IsAny<DeliveryUnit>(), CancellationToken.None), Times.Exactly(2));
        }
    }
}
