using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class ProjectTableDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public ProjectStatusDto? Status { get; set; }
        public DeliveryUnitDto? DeliveryUnit { get; set; }
        public int TeamSize { get; set; }
        public ProjectManagerDto? ProjectManager { get; set; }
        public bool? IsImportedFromJira { get; set; }
    }

    public class DeliveryUnitDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

    public class ProjectManagerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
