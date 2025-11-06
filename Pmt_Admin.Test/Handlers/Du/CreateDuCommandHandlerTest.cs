using FluentAssertions;
using Moq;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Handlers.Du;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Du
{
    public class CreateDuCommandHandlerTest
    {
        private readonly Mock<IDuRepository> _duRepositoryMock;
        private readonly Mock<AutoMapper.IMapper> _mapperMock;
        private readonly CreateDuCommandHandler _handler;

        public CreateDuCommandHandlerTest()
        {
            _duRepositoryMock = new Mock<IDuRepository>();
            _mapperMock = new Mock<AutoMapper.IMapper>();
            _handler = new CreateDuCommandHandler(_duRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldCreateDu()
        {
            // Arrange
            var command = new CreateDuCommand 
            { 
                Name = "New DU", 
                Code = "NDU001",
                Description = "A new delivery unit",
                HeadName = "John Manager",
                HeadEmail = "john@example.com"
            };
            
            var du = new DeliveryUnit 
            { 
                Id = 1, 
                Name = command.Name, 
                Code = command.Code,
                Description = command.Description,
                DuHeadName = command.HeadName,
                DuHeadEmail = command.HeadEmail,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            var duDto = new DuDto 
            { 
                Id = du.Id, 
                Name = du.Name, 
                Code = du.Code,
                Description = du.Description,
                DuHeadName = du.DuHeadName,
                DuHeadEmail = du.DuHeadEmail,
                IsActive = true,
                ProjectCount = 0
            };

            _mapperMock.Setup(m => m.Map<DeliveryUnit>(It.IsAny<CreateDuCommand>())).Returns(du);
            _duRepositoryMock.Setup(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>())).ReturnsAsync(du);
            _mapperMock.Setup(m => m.Map<DuDto>(It.IsAny<DeliveryUnit>())).Returns(duDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(du.Id);
            result.Data.Name.Should().Be("New DU");
            result.Data.Code.Should().Be("NDU001");
            result.Message.Should().Be("Delivery Unit created successfully");

            _duRepositoryMock.Verify(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCreatingWithMinimalData_ShouldCreateSuccessfully()
        {
            // Arrange
            var command = new CreateDuCommand 
            { 
                Name = "Minimal DU", 
                Code = "MIN001"
            };
            
            var du = new DeliveryUnit 
            { 
                Id = 1, 
                Name = command.Name, 
                Code = command.Code,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            var duDto = new DuDto 
            { 
                Id = 1, 
                Name = "Minimal DU", 
                Code = "MIN001",
                IsActive = true
            };

            _mapperMock.Setup(m => m.Map<DeliveryUnit>(It.IsAny<CreateDuCommand>())).Returns(du);
            _duRepositoryMock.Setup(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>())).ReturnsAsync(du);
            _mapperMock.Setup(m => m.Map<DuDto>(It.IsAny<DeliveryUnit>())).Returns(duDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(201);
            result.Data.Id.Should().Be(1);
            result.Data.Name.Should().Be("Minimal DU");
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateDuCommand 
            { 
                Name = "Test DU", 
                Code = "TEST001"
            };
            
            var du = new DeliveryUnit 
            { 
                Name = command.Name, 
                Code = command.Code
            };

            _mapperMock.Setup(m => m.Map<DeliveryUnit>(It.IsAny<CreateDuCommand>())).Returns(du);
            _duRepositoryMock.Setup(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenMultipleDuCreationsInSequence_ShouldCreateSuccessfully()
        {
            // Arrange - First DU
            var command1 = new CreateDuCommand 
            { 
                Name = "Engineering", 
                Code = "ENG001"
            };
            
            var du1 = new DeliveryUnit 
            { 
                Id = 1, 
                Name = "Engineering", 
                Code = "ENG001",
                CreatedAt = DateTime.UtcNow
            };
            
            // Arrange - Second DU
            var command2 = new CreateDuCommand 
            { 
                Name = "Marketing", 
                Code = "MKT001"
            };
            
            var du2 = new DeliveryUnit 
            { 
                Id = 2, 
                Name = "Marketing", 
                Code = "MKT001",
                CreatedAt = DateTime.UtcNow
            };

            var duDto1 = new DuDto { Id = 1, Name = "Engineering", Code = "ENG001" };
            var duDto2 = new DuDto { Id = 2, Name = "Marketing", Code = "MKT001" };

            _mapperMock.Setup(m => m.Map<DeliveryUnit>(command1)).Returns(du1);
            _mapperMock.Setup(m => m.Map<DeliveryUnit>(command2)).Returns(du2);
            
            _duRepositoryMock.SetupSequence(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(du1)
                .ReturnsAsync(du2);

            _mapperMock.SetupSequence(m => m.Map<DuDto>(It.IsAny<DeliveryUnit>()))
                .Returns(duDto1)
                .Returns(duDto2);

            // Act
            var result1 = await _handler.Handle(command1, CancellationToken.None);
            var result2 = await _handler.Handle(command2, CancellationToken.None);

            // Assert
            result1.Status.Should().Be(201);
            result1.Data.Id.Should().Be(1);
            result2.Status.Should().Be(201);
            result2.Data.Id.Should().Be(2);
            
            _duRepositoryMock.Verify(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_WhenCreatingWithDescription_ShouldIncludeInResponse()
        {
            // Arrange
            var command = new CreateDuCommand 
            { 
                Name = "Sales", 
                Code = "SAL001",
                Description = "Sales and Account Management"
            };
            
            var du = new DeliveryUnit 
            { 
                Id = 1, 
                Name = "Sales", 
                Code = "SAL001",
                Description = "Sales and Account Management",
                CreatedAt = DateTime.UtcNow
            };
            
            var duDto = new DuDto 
            { 
                Id = 1, 
                Name = "Sales", 
                Code = "SAL001",
                Description = "Sales and Account Management"
            };

            _mapperMock.Setup(m => m.Map<DeliveryUnit>(It.IsAny<CreateDuCommand>())).Returns(du);
            _duRepositoryMock.Setup(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>())).ReturnsAsync(du);
            _mapperMock.Setup(m => m.Map<DuDto>(It.IsAny<DeliveryUnit>())).Returns(duDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Data.Description.Should().Be("Sales and Account Management");
        }

        [Fact]
        public async Task Handle_WhenCreatingWithHeadInformation_ShouldIncludeInResponse()
        {
            // Arrange
            var command = new CreateDuCommand 
            { 
                Name = "Engineering", 
                Code = "ENG001",
                HeadName = "Alice Manager",
                HeadEmail = "alice@example.com"
            };
            
            var du = new DeliveryUnit 
            { 
                Id = 1, 
                Name = "Engineering", 
                Code = "ENG001",
                DuHeadName = "Alice Manager",
                DuHeadEmail = "alice@example.com",
                CreatedAt = DateTime.UtcNow
            };
            
            var duDto = new DuDto 
            { 
                Id = 1, 
                Name = "Engineering", 
                Code = "ENG001",
                DuHeadName = "Alice Manager",
                DuHeadEmail = "alice@example.com"
            };

            _mapperMock.Setup(m => m.Map<DeliveryUnit>(It.IsAny<CreateDuCommand>())).Returns(du);
            _duRepositoryMock.Setup(x => x.CreateNewDu(It.IsAny<DeliveryUnit>(), It.IsAny<CancellationToken>())).ReturnsAsync(du);
            _mapperMock.Setup(m => m.Map<DuDto>(It.IsAny<DeliveryUnit>())).Returns(duDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Data.DuHeadName.Should().Be("Alice Manager");
            result.Data.DuHeadEmail.Should().Be("alice@example.com");
        }
    }
}
