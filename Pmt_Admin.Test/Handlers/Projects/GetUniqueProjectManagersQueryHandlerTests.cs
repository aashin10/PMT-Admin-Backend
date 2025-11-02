using MediatR;
using Moq;
using FluentAssertions;
using PmtAdmin.Application.Handlers.Projects;
using PmtAdmin.Application.Query.Projects;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Projects
{
    public class GetUniqueProjectManagersQueryHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly GetUniqueProjectManagersQueryHandler _handler;

        public GetUniqueProjectManagersQueryHandlerTests()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _handler = new GetUniqueProjectManagersQueryHandler(_projectRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProjectManagersExist_ReturnsUniqueList()
        {
            // Arrange
            var projectManagers = new List<ProjectManagerInfo>
            {
                new ProjectManagerInfo { Id = 1, Name = "John Doe" },
                new ProjectManagerInfo { Id = 2, Name = "Jane Smith" },
                new ProjectManagerInfo { Id = 3, Name = "Bob Johnson" }
            };

            var query = new GetUniqueProjectManagersQuery();

            _projectRepositoryMock
                .Setup(x => x.GetUniqueProjectManagersAsync())
                .ReturnsAsync(projectManagers);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.Should().Contain(pm => pm.Name == "John Doe");
            result.Data.Should().Contain(pm => pm.Name == "Jane Smith");
            result.Data.Should().Contain(pm => pm.Name == "Bob Johnson");
            result.Message.Should().Be("Retrieved 3 unique project managers");

            _projectRepositoryMock.Verify(x => x.GetUniqueProjectManagersAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoProjectManagers_ReturnsEmptyList()
        {
            // Arrange
            var projectManagers = new List<ProjectManagerInfo>();
            var query = new GetUniqueProjectManagersQuery();

            _projectRepositoryMock
                .Setup(x => x.GetUniqueProjectManagersAsync())
                .ReturnsAsync(projectManagers);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No project managers found");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();

            _projectRepositoryMock.Verify(x => x.GetUniqueProjectManagersAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProjectManagersIsNull_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetUniqueProjectManagersQuery();

            _projectRepositoryMock
                .Setup(x => x.GetUniqueProjectManagersAsync())
                .ReturnsAsync((IReadOnlyList<ProjectManagerInfo>?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Message.Should().Be("No project managers found");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEmpty();

            _projectRepositoryMock.Verify(x => x.GetUniqueProjectManagersAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var query = new GetUniqueProjectManagersQuery();

            _projectRepositoryMock
                .Setup(x => x.GetUniqueProjectManagersAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handler.Handle(query, CancellationToken.None));

            _projectRepositoryMock.Verify(x => x.GetUniqueProjectManagersAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSingleProjectManager_ReturnsSingleManager()
        {
            // Arrange
            var projectManagers = new List<ProjectManagerInfo>
            {
                new ProjectManagerInfo { Id = 1, Name = "John Doe" }
            };

            var query = new GetUniqueProjectManagersQuery();

            _projectRepositoryMock
                .Setup(x => x.GetUniqueProjectManagersAsync())
                .ReturnsAsync(projectManagers);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(1);
            result.Data.First().Id.Should().Be(1);
            result.Data.First().Name.Should().Be("John Doe");
            result.Message.Should().Be("Retrieved 1 unique project managers");
        }
    }
}
