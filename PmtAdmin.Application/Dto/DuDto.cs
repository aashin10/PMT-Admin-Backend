using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class DuDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? DuHeadName { get; set; }
        public string? DuHeadEmail { get; set; }
        public bool IsActive { get; set; }
        public int ProjectCount { get; set; }
    }
}
