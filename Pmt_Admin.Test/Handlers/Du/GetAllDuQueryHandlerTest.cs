using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Du;
using PmtAdmin.Application.Query.Delivery_Unit;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Du
{
    public class GetAllDuQueryHandlerTest
    {
        private readonly Mock<IDuRepository> _duRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllDuQueryHandler _handler;

        public GetAllDuQueryHandlerTest()
        {
            _duRepositoryMock = new Mock<IDuRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllDuQueryHandler(_duRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_WhenDeliveryUnitsExist_ReturnsSuccessWithDeliveryUnits()
        {
            // Arrange
            var deliveryUnits = new List<DeliveryUnit>
            {
                new DeliveryUnit 
                { 
                    Id = 1, 
                    Name = "Engineering", 
                    Code = "ENG-001",
                    Description = "Software Development",
                    DuHeadName = "John Doe",
                    DuHeadEmail = "john@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new DeliveryUnit 
                { 
                    Id = 2, 
                    Name = "Marketing", 
                    Code = "MKT-001",
                    Description = "Marketing and Communications",
                    DuHeadName = "Jane Smith",
                    DuHeadEmail = "jane@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var duDtos = new List<DuDto>
            {
                new DuDto 
                { 
                    Id = 1, 
                    Name = "Engineering", 
                    Code = "ENG-001",
                    Description = "Software Development",
                    DuHeadName = "John Doe",
                    DuHeadEmail = "john@example.com",
                    IsActive = true,
                    ProjectCount = 5
                },
                new DuDto 
                { 
                    Id = 2, 
                    Name = "Marketing", 
                    Code = "MKT-001",
                    Description = "Marketing and Communications",
                    DuHeadName = "Jane Smith",
                    DuHeadEmail = "jane@example.com",
                    IsActive = true,
                    ProjectCount = 3
                }
            };

            var query = new GetAllDuQuery();

            _duRepositoryMock
                .Setup(x => x.GetAllWithProjectCountAsync(CancellationToken.None))
                .ReturnsAsync(deliveryUnits);

            _mapperMock
                .Setup(x => x.Map<List<DuDto>>(deliveryUnits))
                .Returns(duDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data[0].Id.Should().Be(1);
            result.Data[0].Name.Should().Be("Engineering");
            result.Data[0].Code.Should().Be("ENG-001");
            result.Data[0].ProjectCount.Should().Be(5);
            result.Data[1].Id.Should().Be(2);
            result.Data[1].Name.Should().Be("Marketing");
            result.Data[1].ProjectCount.Should().Be(3);

            _duRepositoryMock.Verify(x => x.GetAllWithProjectCountAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoDeliveryUnitsExist_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var query = new GetAllDuQuery();
            var emptyList = new List<DeliveryUnit>();

            _duRepositoryMock
                .Setup(x => x.GetAllWithProjectCountAsync(CancellationToken.None))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No projects found");
            result.Data.Should().BeEmpty();

            _duRepositoryMock.Verify(x => x.GetAllWithProjectCountAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryReturnsNull_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var query = new GetAllDuQuery();

            _duRepositoryMock
                .Setup(x => x.GetAllWithProjectCountAsync(CancellationToken.None))
                .ReturnsAsync((List<DeliveryUnit>)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No projects found");
            result.Data.Should().BeEmpty();

            _duRepositoryMock.Verify(x => x.GetAllWithProjectCountAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDeliveryUnitHasNoProjects_ReturnsZeroProjectCount()
        {
            // Arrange
            var deliveryUnits = new List<DeliveryUnit>
            {
                new DeliveryUnit 
                { 
                    Id = 1, 
                    Name = "HR", 
                    Code = "HR-001",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var duDtos = new List<DuDto>
            {
                new DuDto 
                { 
                    Id = 1, 
                    Name = "HR", 
                    Code = "HR-001",
                    IsActive = true,
                    ProjectCount = 0
                }
            };

            var query = new GetAllDuQuery();

            _duRepositoryMock
                .Setup(x => x.GetAllWithProjectCountAsync(CancellationToken.None))
                .ReturnsAsync(deliveryUnits);

            _mapperMock
                .Setup(x => x.Map<List<DuDto>>(deliveryUnits))
                .Returns(duDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(1);
            result.Data[0].ProjectCount.Should().Be(0);
        }
    }
}
