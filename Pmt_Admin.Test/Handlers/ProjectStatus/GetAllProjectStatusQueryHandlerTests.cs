using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.ProjectStatus;
using PmtAdmin.Application.Query.Status;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.ProjectStatus
{
    public class GetAllProjectStatusQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IProjectStatusRepository> _projectStatusRepositoryMock;
        private readonly GetAllProjectStatusQueryHandler _handler;

        public GetAllProjectStatusQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _projectStatusRepositoryMock = new Mock<IProjectStatusRepository>();
            _handler = new GetAllProjectStatusQueryHandler(_mapperMock.Object, _projectStatusRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenStatusesExist_ReturnsSuccessWithStatuses()
        {
            // Arrange
            var statuses = new List<PmtAdmin.Domain.Entities.ProjectStatus>
            {
                new PmtAdmin.Domain.Entities.ProjectStatus
                {
                    Id = 1,
                    Name = "Active",
                    Description = "Active project",
                    CreatedAt = DateTime.UtcNow
                },
                new PmtAdmin.Domain.Entities.ProjectStatus
                {
                    Id = 2,
                    Name = "Inactive",
                    Description = "Inactive project",
                    CreatedAt = DateTime.UtcNow
                },
                new PmtAdmin.Domain.Entities.ProjectStatus
                {
                    Id = 3,
                    Name = "Completed",
                    Description = "Completed project",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var statusDtos = new List<ProjectStatusDto>
            {
                new ProjectStatusDto
                {
                    Id = 1,
                    Name = "Active",
                    Description = "Active project"
                },
                new ProjectStatusDto
                {
                    Id = 2,
                    Name = "Inactive",
                    Description = "Inactive project"
                },
                new ProjectStatusDto
                {
                    Id = 3,
                    Name = "Completed",
                    Description = "Completed project"
                }
            };

            var query = new GetAllProjectStatusQuery();

            _projectStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(statuses);

            _mapperMock
                .Setup(x => x.Map<List<ProjectStatusDto>>(statuses))
                .Returns(statusDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.Should().Contain(s => s.Name == "Active");
            result.Data.Should().Contain(s => s.Name == "Inactive");
            result.Data.Should().Contain(s => s.Name == "Completed");

            _projectStatusRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _mapperMock.Verify(x => x.Map<List<ProjectStatusDto>>(statuses), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoStatusesExist_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var statuses = new List<PmtAdmin.Domain.Entities.ProjectStatus>();
            var query = new GetAllProjectStatusQuery();

            _projectStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(statuses);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No project statuses found");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();

            _projectStatusRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _mapperMock.Verify(x => x.Map<List<ProjectStatusDto>>(It.IsAny<List<PmtAdmin.Domain.Entities.ProjectStatus>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenStatusesIsNull_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var query = new GetAllProjectStatusQuery();

            _projectStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync((IReadOnlyList<PmtAdmin.Domain.Entities.ProjectStatus>?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No project statuses found");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();

            _projectStatusRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSingleStatus_ReturnsSingleStatus()
        {
            // Arrange
            var statuses = new List<PmtAdmin.Domain.Entities.ProjectStatus>
            {
                new PmtAdmin.Domain.Entities.ProjectStatus
                {
                    Id = 1,
                    Name = "Active",
                    Description = "Active project",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var statusDtos = new List<ProjectStatusDto>
            {
                new ProjectStatusDto
                {
                    Id = 1,
                    Name = "Active",
                    Description = "Active project"
                }
            };

            var query = new GetAllProjectStatusQuery();

            _projectStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(statuses);

            _mapperMock
                .Setup(x => x.Map<List<ProjectStatusDto>>(statuses))
                .Returns(statusDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(1);
            result.Data.First().Id.Should().Be(1);
            result.Data.First().Name.Should().Be("Active");
            result.Data.First().Description.Should().Be("Active project");
        }

        [Fact]
        public async Task Handle_WhenMapperReturnsNull_ReturnsSuccessWithEmptyList()
        {
            // Arrange
            var statuses = new List<PmtAdmin.Domain.Entities.ProjectStatus>
            {
                new PmtAdmin.Domain.Entities.ProjectStatus
                {
                    Id = 1,
                    Name = "Active",
                    Description = "Active project",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var query = new GetAllProjectStatusQuery();

            _projectStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(statuses);

            _mapperMock
                .Setup(x => x.Map<List<ProjectStatusDto>>(statuses))
                .Returns((List<ProjectStatusDto>?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var query = new GetAllProjectStatusQuery();

            _projectStatusRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
                await _handler.Handle(query, CancellationToken.None));

            _projectStatusRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }
    }
}
