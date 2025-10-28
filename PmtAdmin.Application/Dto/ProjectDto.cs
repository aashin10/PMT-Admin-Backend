using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class ProjectDto
    {
        public string Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public string? CustomerOrgName { get; set; }
        public string? CustomerDomainUrl { get; set; }
        public string? PocEmail { get; set; }
        public string? PocPhone { get; set; }
        public int? ProjectManagerId { get; set; }
        public int? ProjectManagerRoleId { get; set; }
        public int? StatusId { get; set; }
        public int? DeliveryUnitId { get; set; }
        public int? TemplateId { get; set; }
    }
}
