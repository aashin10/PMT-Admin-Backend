using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto.DashboardDTO
{
    public class ChartDataDto
    {
        public List<ChartPointDto> Monthly { get; set; } = new();
        public List<ChartPointDto> Quarterly { get; set; } = new();
        public List<ChartPointDto> Yearly { get; set; } = new();
        public List<ChartPointDto> Last5Years { get; set; } = new();
        public List<ChartPointDto> AllTime { get; set; } = new();
    }
}
