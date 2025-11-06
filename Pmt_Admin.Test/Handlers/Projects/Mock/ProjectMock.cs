using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pmt_Admin.Test.Handlers.Projects.Mock
{
    public static class ProjectMock
    {
        public static List<Project> GetProjects()
        {
            var user1 = new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
            var user2 = new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" };

            return new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Project Alpha",
                    Description = "Description for Project Alpha",
                    ProjectManager = user1,
                    ProjectManagerId = user1.Id,
                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "In Progress" },
                    DeliveryUnit = new DeliveryUnit { Id = 1, Name = "Digital" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProjectMembers = new List<ProjectMember> { new ProjectMember() }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Project Beta",
                    Description = "Description for Project Beta",
                    ProjectManager = user2,
                    ProjectManagerId = user2.Id,
                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 2, Name = "Completed" },
                    DeliveryUnit = new DeliveryUnit { Id = 2, Name = "Engineering" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProjectMembers = new List<ProjectMember> { new ProjectMember(), new ProjectMember() }
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Project Gamma",
                    Description = "Description for Project Gamma",
                    ProjectManager = user1, // John Doe is also managing this project
                    ProjectManagerId = user1.Id,
                    Status = new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "In Progress" },
                    DeliveryUnit = new DeliveryUnit { Id = 1, Name = "Digital" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProjectMembers = new List<ProjectMember>()
                }
            };
        }

        public static List<ProjectManagerInfo> GetProjectManagers()
        {
            var projects = GetProjects();
            return projects
                .Where(p => p.ProjectManager != null)
                .Select(p => p.ProjectManager)
                .DistinctBy(pm => pm.Id)
                .Select(pm => new ProjectManagerInfo
                {
                    Id = pm.Id,
                    Name = pm.Name
                }).ToList();
        }
    }
}
