using AutoMapper;
using FluentAssertions;
using Moq;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Handlers.Projects;
using PmtAdmin.Application.Query.Projects;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pmt_Admin.Test.Handlers.Projects
{
    public class GetProjectByIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly GetProjectByIdQueryHandler _handler;

        public GetProjectByIdQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _handler = new GetProjectByIdQueryHandler(_mapperMock.Object, _projectRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProjectExists_ReturnsSuccessWithProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                Key = "TEST001",
                Description = "Test Description",
                CustomerOrgName = "Test Org",
                CustomerDomainUrl = "test.com",
                CustomerDescription = "Customer Description",
                PocEmail = "poc@test.com",
                PocPhone = "1234567890",
                ProjectManagerId = 1,
                StatusId = 1,
                DeliveryUnitId = 1,
                ProjectManagerRoleId = 1,
                IsImportedFromJira = false,
                CreatedAt = DateTime.UtcNow,
                ProjectManager = new User { Id = 1, Name = "Manager", Email = "manager@test.com" },
                Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "Active" },
                DeliveryUnit = new DeliveryUnit { Id = 1, Name = "DU Alpha", Code = "DUA001" },
                ProjectMembers = new List<ProjectMember>
                {
                    new ProjectMember
                    {
                        Id = 1,
                        UserId = 1,
                        RoleId = 1,
                        User = new User { Id = 1, Name = "Developer 1", Email = "dev1@test.com" },
                        Role = new Role { Id = 1, Name = "Developer" }
                    },
                    new ProjectMember
                    {
                        Id = 2,
                        UserId = 2,
                        RoleId = 2,
                        User = new User { Id = 2, Name = "Developer 2", Email = "dev2@test.com" },
                        Role = new Role { Id = 2, Name = "Senior Developer" }
                    }
                },
                Sprints = new List<Sprint>
                {
                    new Sprint { Id = Guid.NewGuid(), Name = "Sprint 1" },
                    new Sprint { Id = Guid.NewGuid(), Name = "Sprint 2" }
                },
                CustomFields = new List<CustomField>
                {
                    new CustomField { Id = Guid.NewGuid(), Name = "Budget", Value = "100000" },
                    new CustomField { Id = Guid.NewGuid(), Name = "Priority", Value = "High" }
                },
                Teams = new List<Team>
                {
                    new Team 
                    { 
                        Id = 1, 
                        Name = "Team Alpha", 
                        TeamMembers = new List<TeamMember>
                        {
                            new TeamMember { TeamMemberId = 1, TeamId = 1, ProjectMemberId = 1, ProjectMember = new ProjectMember { Id = 1, User = new User { Id = 1, Name = "Developer 1", Email = "dev1@test.com" } } },
                            new TeamMember { TeamMemberId = 2, TeamId = 1, ProjectMemberId = 2, ProjectMember = new ProjectMember { Id = 2, User = new User { Id = 2, Name = "Developer 2", Email = "dev2@test.com" } } }
                        }
                    },
                    new Team { Id = 2, Name = "Team Beta", TeamMembers = new List<TeamMember>() }
                }
            };

            var query = new GetProjectByIdQuery { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.GetProjectByIdWithDetailsAsync(projectId))
                .ReturnsAsync(project);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            
            var projectDto = result.Data;
            projectDto.Id.Should().Be(projectId);
            projectDto.Name.Should().Be("Test Project");
            projectDto.Key.Should().Be("TEST001");
            projectDto.Description.Should().Be("Test Description");
            projectDto.CustomerOrgName.Should().Be("Test Org");
            projectDto.CustomerDomainUrl.Should().Be("test.com");
            projectDto.PocEmail.Should().Be("poc@test.com");
            projectDto.ProjectManagerName.Should().Be("Manager");
            projectDto.StatusName.Should().Be("Active");
            projectDto.DeliveryUnitName.Should().Be("DU Alpha");
            projectDto.DeliveryUnitCode.Should().Be("DUA001");
            projectDto.TeamSize.Should().Be(2);
            projectDto.SprintCount.Should().Be(2);
            projectDto.AdditionalInformation.Should().HaveCount(2);
            projectDto.Teams.Should().HaveCount(2);

            _projectRepositoryMock.Verify(x => x.GetProjectByIdWithDetailsAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProjectDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var query = new GetProjectByIdQuery { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.GetProjectByIdWithDetailsAsync(projectId))
                .ReturnsAsync((Project?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(404);
            result.Message.Should().Be("Project not found");
            result.Data.Should().BeNull();

            _projectRepositoryMock.Verify(x => x.GetProjectByIdWithDetailsAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProjectHasNullCollections_ReturnsProjectWithEmptyCollections()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                Key = "TEST001",
                CreatedAt = DateTime.UtcNow,
                ProjectMembers = null,
                Sprints = null,
                CustomFields = null,
                Teams = null,
                Status = null,
                DeliveryUnit = null,
                ProjectManager = null
            };

            var query = new GetProjectByIdQuery { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.GetProjectByIdWithDetailsAsync(projectId))
                .ReturnsAsync(project);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().NotBeNull();
            
            var projectDto = result.Data;
            projectDto.TeamSize.Should().Be(0);
            projectDto.SprintCount.Should().Be(0);
            projectDto.AdditionalInformation.Should().BeEmpty();
            projectDto.Teams.Should().BeEmpty();
            projectDto.StatusName.Should().BeNull();
            projectDto.DeliveryUnitName.Should().BeNull();
            projectDto.ProjectManagerName.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenProjectHasCompleteData_MapsAllFieldsCorrectly()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var customFieldId1 = Guid.NewGuid();
            var customFieldId2 = Guid.NewGuid();
            
            var leadProjectMember = new ProjectMember
            {
                Id = 5,
                UserId = 5,
                User = new User { Id = 5, Name = "Team Lead", Email = "lead@test.com" },
                Role = new Role { Id = 3, Name = "Team Lead" }
            };

            var project = new Project
            {
                Id = projectId,
                Name = "Complete Project",
                Key = "COMP001",
                Description = "Complete Description",
                CustomerOrgName = "Complete Org",
                CustomerDomainUrl = "complete.com",
                CustomerDescription = "Complete Customer",
                PocEmail = "complete@test.com",
                PocPhone = "9876543210",
                ProjectManagerId = 5,
                StatusId = 2,
                DeliveryUnitId = 3,
                ProjectManagerRoleId = 2,
                IsImportedFromJira = true,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 2, 1),
                ProjectManager = new User { Id = 5, Name = "PM Name", Email = "pm@test.com" },
                Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 2, Name = "Inactive" },
                DeliveryUnit = new DeliveryUnit { Id = 3, Name = "DU Complete", Code = "DUC001" },
                ProjectMembers = new List<ProjectMember>
                {
                    new ProjectMember
                    {
                        Id = 10,
                        UserId = 10,
                        RoleId = 1,
                        User = new User { Id = 10, Name = "Team Member 1", Email = "tm1@test.com" },
                        Role = new Role { Id = 1, Name = "Tech Lead" },
                        IsOwner = true,
                        AddedAt = DateTimeOffset.UtcNow
                    }
                },
                Sprints = new List<Sprint>
                {
                    new Sprint { Id = Guid.NewGuid(), Name = "Sprint Alpha" }
                },
                CustomFields = new List<CustomField>
                {
                    new CustomField { Id = customFieldId1, Name = "Cost Center", Value = "CC-001" },
                    new CustomField { Id = customFieldId2, Name = "Region", Value = "Asia Pacific" }
                },
                Teams = new List<Team>
                {
                    new Team 
                    { 
                        Id = 5, 
                        Name = "Core Team",
                        Description = "Main development team",
                        IsActive = true,
                        LeadId = 5,
                        Lead = leadProjectMember,
                        TeamMembers = new List<TeamMember>
                        {
                            new TeamMember { TeamMemberId = 1, TeamId = 5, ProjectMemberId = 10 },
                            new TeamMember { TeamMemberId = 2, TeamId = 5, ProjectMemberId = 5 }
                        }
                    }
                }
            };

            var query = new GetProjectByIdQuery { Id = projectId };

            _projectRepositoryMock
                .Setup(x => x.GetProjectByIdWithDetailsAsync(projectId))
                .ReturnsAsync(project);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            
            var dto = result.Data;
            dto.Id.Should().Be(projectId);
            dto.Name.Should().Be("Complete Project");
            dto.Key.Should().Be("COMP001");
            dto.Description.Should().Be("Complete Description");
            dto.CustomerOrgName.Should().Be("Complete Org");
            dto.CustomerDomainUrl.Should().Be("complete.com");
            dto.CustomerDescription.Should().Be("Complete Customer");
            dto.PocEmail.Should().Be("complete@test.com");
            dto.PocPhone.Should().Be("9876543210");
            dto.ProjectManagerId.Should().Be(5);
            dto.ProjectManagerName.Should().Be("PM Name");
            dto.ProjectManagerRoleId.Should().Be(2);
            dto.StatusId.Should().Be(2);
            dto.StatusName.Should().Be("Inactive");
            dto.DeliveryUnitId.Should().Be(3);
            dto.DeliveryUnitName.Should().Be("DU Complete");
            dto.DeliveryUnitCode.Should().Be("DUC001");
            dto.IsImportedFromJira.Should().BeTrue();
            dto.CreatedAt.Should().Be(new DateTime(2024, 1, 1));
            dto.UpdatedAt.Should().Be(new DateTime(2024, 2, 1));

            // Verify custom fields
            dto.AdditionalInformation.Should().HaveCount(2);
            dto.AdditionalInformation.Should().Contain(cf => cf.Name == "Cost Center" && cf.Value == "CC-001");
            dto.AdditionalInformation.Should().Contain(cf => cf.Name == "Region" && cf.Value == "Asia Pacific");

            // Verify teams
            dto.Teams.Should().HaveCount(1);
            dto.Teams.First().Id.Should().Be(5);
            dto.Teams.First().Name.Should().Be("Core Team");
            dto.Teams.First().Description.Should().Be("Main development team");
            dto.Teams.First().IsActive.Should().BeTrue();
            dto.Teams.First().MemberCount.Should().Be(2);
            dto.Teams.First().LeadName.Should().Be("Team Lead");
        }
    }
}
