using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto.DashboardDTO
{
    public class DashboardSummaryDto
    {
        public int TotalProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int OnHoldProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int TotalDeliveryUnits { get; set; }

        public List<ProjectStatusDto> ProjectStatuses { get; set; } = new();
    }
}
