using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public string? CustomerOrgName { get; set; }
        public string? CustomerDomainUrl { get; set; }
        public string? CustomerDescription { get; set; }
        public string? PocEmail { get; set; }
        public string? PocPhone { get; set; }
        public int? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }
        public int? ProjectManagerRoleId { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public int? DeliveryUnitId { get; set; }
        public string? DeliveryUnitName { get; set; }
        public string? DeliveryUnitCode { get; set; }
        public int TeamSize { get; set; }
        public int SprintCount { get; set; }
        public List<CustomFieldDTO> AdditionalInformation { get; set; } = new List<CustomFieldDTO>();
        public List<TeamDTO> Teams { get; set; } = new List<TeamDTO>();
        public List<TeamMemberDTO> TeamMembers { get; set; } = new List<TeamMemberDTO>();
        public bool? IsImportedFromJira { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}