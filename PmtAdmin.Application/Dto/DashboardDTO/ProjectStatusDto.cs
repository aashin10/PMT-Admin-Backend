using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto.DashboardDTO
{
    public class ProjectStatusDto
    {
        public string DeliveryUnit { get; set; }
        public int InProgress { get; set; }
        public int OnHold { get; set; }
        public int Completed { get; set; }
        public int Total { get; set; }
    }
}
