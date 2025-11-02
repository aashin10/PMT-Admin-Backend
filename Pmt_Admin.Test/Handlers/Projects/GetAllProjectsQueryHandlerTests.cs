//using AutoMapper;
//using FluentAssertions;
//using Moq;
//using PmtAdmin.Application.Dto;
//using PmtAdmin.Application.Handlers.Projects;
//using PmtAdmin.Application.Query.Projects;
//using PmtAdmin.Domain.Entities;
//using PmtAdmin.Domain.Persistance;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace Pmt_Admin.Test.Handlers.Projects
//{
//    public class GetAllProjectsQueryHandlerTests
//    {
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly Mock<IProjectRepository> _projectRepositoryMock;
//        private readonly GetAllProjectsQueryHandler _handler;

//        public GetAllProjectsQueryHandlerTests()
//        {
//            _mapperMock = new Mock<IMapper>();
//            _projectRepositoryMock = new Mock<IProjectRepository>();
//            _handler = new GetAllProjectsQueryHandler(_mapperMock.Object, _projectRepositoryMock.Object);
//        }

        [Fact]
        public async Task Handle_WhenProjectsExist_ReturnsSuccessWithProjects()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var projects = new List<Project>
            {
                new Project
                {
                    Id = projectId,
                    Name = "Test Project",
                    Key = "TEST001",
                    Description = "Test Description",
                    StatusId = 1,
                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "Active" },
                    DeliveryUnitId = 1,
                    DeliveryUnit = new DeliveryUnit { Id = 1, Name = "DU1", Code = "DU001" },
                    ProjectManagerId = 1,
                    ProjectManager = new User { Id = 1, Name = "John Doe" },
                    IsImportedFromJira = true,
                    CreatedAt = DateTime.UtcNow,
                    ProjectMembers = new List<ProjectMember>
                    {
                        new ProjectMember
                        {
                            Id = 1,
                            UserId = 1,
                            RoleId = 1,
                            User = new User { Id = 1, Name = "User 1", Email = "user1@test.com" },
                            Role = new Role { Id = 1, Name = "Developer" }
                        }
                    }
                }
            };

//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, null, null, null, null))
//                .ReturnsAsync((projects, 1));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Status.Should().Be(200);
//            result.Data.Should().NotBeNull();
//            result.Data.Items.Should().HaveCount(1);
//            result.Data.TotalCount.Should().Be(1);
//            result.Data.Page.Should().Be(1);
//            result.Data.PageSize.Should().Be(10);
            
//            var projectDto = result.Data.Items.First();
//            projectDto.Id.Should().Be(projectId);
//            projectDto.Name.Should().Be("Test Project");
//            projectDto.Key.Should().Be("TEST001");
//            projectDto.Status.Should().NotBeNull();
//            projectDto.Status!.Name.Should().Be("Active");
//            projectDto.DeliveryUnit.Should().NotBeNull();
//            projectDto.DeliveryUnit!.Code.Should().Be("DU001");
//            projectDto.TeamSize.Should().Be(1);
//            projectDto.IsImportedFromJira.Should().BeTrue();

//            _projectRepositoryMock.Verify(x => x.GetProjectsForTableAsync(1, 10, null, null, null, null), Times.Once);
//        }

//        [Fact]
//        public async Task Handle_WhenNoProjectsExist_ReturnsSuccessWithEmptyList()
//        {
//            // Arrange
//            var projects = new List<Project>();
//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, null, null, null, null))
//                .ReturnsAsync((projects, 0));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Status.Should().Be(200);
//            result.Message.Should().Be("No projects found");
//            result.Data.Should().NotBeNull();
//            result.Data.Items.Should().BeEmpty();
//            result.Data.TotalCount.Should().Be(0);

//            _projectRepositoryMock.Verify(x => x.GetProjectsForTableAsync(1, 10, null, null, null, null), Times.Once);
//        }

//        [Fact]
//        public async Task Handle_WhenSearchTermProvided_ReturnsFilteredProjects()
//        {
//            // Arrange
//            var projectId = Guid.NewGuid();
//            var projects = new List<Project>
//            {
//                new Project
//                {
//                    Id = projectId,
//                    Name = "Alpha Project",
//                    Key = "ALPHA001",
//                    ProjectMembers = new List<ProjectMember>(),
//                    IsImportedFromJira = false,
//                    CreatedAt = DateTime.UtcNow
//                }
//            };

//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10,
//                SearchTerm = "Alpha"
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, "Alpha", null, null, null))
//                .ReturnsAsync((projects, 1));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Status.Should().Be(200);
//            result.Data.Items.Should().HaveCount(1);
//            result.Data.Items.First().Name.Should().Be("Alpha Project");
//            result.Data.Items.First().IsImportedFromJira.Should().BeFalse();

//            _projectRepositoryMock.Verify(x => x.GetProjectsForTableAsync(1, 10, "Alpha", null, null, null), Times.Once);
//        }

//        [Fact]
//        public async Task Handle_WhenSingleFiltersProvided_ReturnsFilteredProjects()
//        {
//            // Arrange
//            var projects = new List<Project>
//            {
//                new Project
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Filtered Project",
//                    Key = "FILT001",
//                    StatusId = 1,
//                    DeliveryUnitId = 2,
//                    ProjectManagerId = 3,
//                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "Active" },
//                    DeliveryUnit = new DeliveryUnit { Id = 2, Name = "DU2", Code = "DU002" },
//                    ProjectManager = new User { Id = 3, Name = "Manager" },
//                    ProjectMembers = new List<ProjectMember>(),
//                    IsImportedFromJira = true,
//                    CreatedAt = DateTime.UtcNow
//                }
//            };

//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10,
//                StatusIds = new List<int> { 1 },
//                DeliveryUnitIds = new List<int> { 2 },
//                ProjectManagerIds = new List<int> { 3 }
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, null, 
//                    It.Is<List<int>>(list => list != null && list.Count == 1 && list[0] == 1),
//                    It.Is<List<int>>(list => list != null && list.Count == 1 && list[0] == 2),
//                    It.Is<List<int>>(list => list != null && list.Count == 1 && list[0] == 3)))
//                .ReturnsAsync((projects, 1));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Status.Should().Be(200);
//            result.Data.Items.Should().HaveCount(1);
//            result.Data.Items.First().Status!.Id.Should().Be(1);
//            result.Data.Items.First().DeliveryUnit!.Id.Should().Be(2);
//            result.Data.Items.First().ProjectManager!.Id.Should().Be(3);

//            _projectRepositoryMock.Verify(x => x.GetProjectsForTableAsync(1, 10, null, 
//                It.Is<List<int>>(list => list != null && list.Count == 1 && list[0] == 1),
//                It.Is<List<int>>(list => list != null && list.Count == 1 && list[0] == 2),
//                It.Is<List<int>>(list => list != null && list.Count == 1 && list[0] == 3)), Times.Once);
//        }

//        [Fact]
//        public async Task Handle_WhenMultipleFiltersProvided_ReturnsFilteredProjects()
//        {
//            // Arrange
//            var projects = new List<Project>
//            {
//                new Project
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Project 1",
//                    Key = "PROJ001",
//                    StatusId = 1,
//                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "Active" },
//                    ProjectMembers = new List<ProjectMember>(),
//                    IsImportedFromJira = true,
//                    CreatedAt = DateTime.UtcNow
//                },
//                new Project
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Project 2",
//                    Key = "PROJ002",
//                    StatusId = 2,
//                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 2, Name = "Inactive" },
//                    ProjectMembers = new List<ProjectMember>(),
//                    IsImportedFromJira = false,
//                    CreatedAt = DateTime.UtcNow
//                }
//            };

//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10,
//                StatusIds = new List<int> { 1, 2 }
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, null, 
//                    It.Is<List<int>>(list => list != null && list.Count == 2 && list.Contains(1) && list.Contains(2)),
//                    null, null))
//                .ReturnsAsync((projects, 2));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Status.Should().Be(200);
//            result.Data.Items.Should().HaveCount(2);
//            result.Data.TotalCount.Should().Be(2);

//            _projectRepositoryMock.Verify(x => x.GetProjectsForTableAsync(1, 10, null, 
//                It.Is<List<int>>(list => list != null && list.Count == 2 && list.Contains(1) && list.Contains(2)),
//                null, null), Times.Once);
//        }

//        [Fact]
//        public async Task Handle_WhenMultipleDeliveryUnitsAndManagers_ReturnsFilteredProjects()
//        {
//            // Arrange
//            var projects = new List<Project>
//            {
//                new Project
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Multi-Filter Project",
//                    Key = "MFP001",
//                    DeliveryUnitId = 1,
//                    ProjectManagerId = 5,
//                    DeliveryUnit = new DeliveryUnit { Id = 1, Name = "DU1", Code = "DU001" },
//                    ProjectManager = new User { Id = 5, Name = "Manager 1" },
//                    ProjectMembers = new List<ProjectMember>(),
//                    IsImportedFromJira = true,
//                    CreatedAt = DateTime.UtcNow
//                }
//            };

//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10,
//                DeliveryUnitIds = new List<int> { 1, 2, 3 },
//                ProjectManagerIds = new List<int> { 5, 6 }
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, null, null,
//                    It.Is<List<int>>(list => list != null && list.Count == 3),
//                    It.Is<List<int>>(list => list != null && list.Count == 2)))
//                .ReturnsAsync((projects, 1));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Status.Should().Be(200);
//            result.Data.Items.Should().HaveCount(1);
//            result.Data.Items.First().DeliveryUnit!.Code.Should().Be("DU001");
//            result.Data.Items.First().ProjectManager!.Name.Should().Be("Manager 1");

//            _projectRepositoryMock.Verify(x => x.GetProjectsForTableAsync(1, 10, null, null,
//                It.Is<List<int>>(list => list != null && list.Count == 3),
//                It.Is<List<int>>(list => list != null && list.Count == 2)), Times.Once);
//        }

//        [Fact]
//        public async Task Handle_WhenIsImportedFromJiraIsNull_ReturnsProjectWithNullValue()
//        {
//            // Arrange
//            var projectId = Guid.NewGuid();
//            var projects = new List<Project>
//            {
//                new Project
//                {
//                    Id = projectId,
//                    Name = "Project Without Jira Flag",
//                    Key = "NOWJ001",
//                    ProjectMembers = new List<ProjectMember>(),
//                    IsImportedFromJira = null,
//                    CreatedAt = DateTime.UtcNow
//                }
//            };

//            var query = new GetAllProjectsQuery
//            {
//                Page = 1,
//                PageSize = 10
//            };

//            _projectRepositoryMock
//                .Setup(x => x.GetProjectsForTableAsync(1, 10, null, null, null, null))
//                .ReturnsAsync((projects, 1));

//            // Act
//            var result = await _handler.Handle(query, CancellationToken.None);

//            // Assert
//            result.Should().NotBeNull();
//            result.Data.Items.First().IsImportedFromJira.Should().BeNull();
//        }
//    }
//}
