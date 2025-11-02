using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto.DashboardDTO
{
    public class ChartPointDto
    {
        public string Period { get; set; } = default!;
        public int Projects { get; set; }
    }
}
