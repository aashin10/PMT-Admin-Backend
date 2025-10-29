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
        public bool? IsImportedFromJira { get; set; }
        public string? Status { get; set; }
        public string? DeliveryUnitName { get; set; }
        public string? ProjectManagerName { get; set; }
        public int TeamSize { get; set; }
    }

    public class ProjectDetailsDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public int TotalSprintCount { get; set; }
        public int TotalTeamMemberSize { get; set; }
        public CustomerDetailsDTO CustomerDetails { get; set; }
        public ProjectInfoDTO ProjectInfo { get; set; }
        public List<CustomFieldDTO> AdditionalInformation { get; set; }
        public List<TeamDTO> Teams { get; set; }
    }

    public class CustomerDetailsDTO
    {
        public string? OrganizationName { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? PocEmail { get; set; }
        public string? PocPhone { get; set; }
    }

    public class ProjectInfoDTO
    {
        public string? ProjectManagerName { get; set; }
        public string? DeliveryUnit { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int TeamSize { get; set; }
    }

    public class CustomFieldDTO
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }

    public class TeamDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int MemberCount { get; set; }
        public List<TeamMemberDTO> Members { get; set; }
    }

    public class TeamMemberDTO
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}