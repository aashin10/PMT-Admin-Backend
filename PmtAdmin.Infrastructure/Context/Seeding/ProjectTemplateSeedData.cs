using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Context.Seeding
{
    public static class ProjectTemplateSeedData
    {
        public static List<ProjectTemplate> GetProjectTemplates()
        {
            return new List<ProjectTemplate>
            {
                new ProjectTemplate { Id = 1, Name = "Scrum Template" },
                new ProjectTemplate { Id = 2, Name = "Kanban Template" },
                new ProjectTemplate { Id = 3, Name = "Waterfall Template" },
                new ProjectTemplate { Id = 4, Name = "Agile Template" }
            };
        }

        public static void SeedProjectTemplates(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectTemplate>().HasData(GetProjectTemplates());
        }
    }
}
