using PmtAdmin.Domain.Entities;
using System.Collections.Generic;

namespace Pmt_Admin.Test.Handlers.ProjectStatus.Mock
{
    public static class ProjectStatusMock
    {
        public static List<PmtAdmin.Domain.Entities.ProjectStatus> GetProjectStatuses()
        {
            return new List<PmtAdmin.Domain.Entities.ProjectStatus>
            {
                new PmtAdmin.Domain.Entities.ProjectStatus { Id = 1, Name = "In Progress" },
                new PmtAdmin.Domain.Entities.ProjectStatus { Id = 2, Name = "Completed" },
                new PmtAdmin.Domain.Entities.ProjectStatus { Id = 3, Name = "On Hold" }
            };
        }
    }
}
