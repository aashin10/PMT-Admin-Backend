using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Context.Seeding
{
    public static class ProjectStatusSeedData
    {
        public static List<ProjectStatus> GetProjectStatuses()
        {
            return new List<ProjectStatus>
            {
                new ProjectStatus { Id = 1, Name = "Active", Description = "Project is currently active", CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatus { Id = 2, Name = "Inactive", Description = "Project is temporarily inactive", CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatus { Id = 3, Name = "Completed", Description = "Project has been completed", CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            };
        }

        public static void SeedProjectStatuses(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectStatus>().HasData(GetProjectStatuses());
        }
    }
}
