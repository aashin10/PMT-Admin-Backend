using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;

namespace PmtAdmin.Application.Command.Projects
{
    public class UpdateProjectCommand : IRequest<ApiResponse<ProjectDTO>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Key { get; set; }
        public string? Description { get; set; }
        public string? CustomerOrgName { get; set; }
        public string? CustomerDomainUrl { get; set; }
        public string? CustomerDescription { get; set; }
        public string? PocEmail { get; set; }
        public string? PocPhone { get; set; }
        public int? ProjectManagerId { get; set; }
        public int? ProjectManagerRoleId { get; set; }
        public int? StatusId { get; set; }
        public int? DeliveryUnitId { get; set; }
        public int? CreatedBy { get; set; }
        public string? Metadata { get; set; }
        public int? TemplateId { get; set; }
        public List<CustomFieldDTO>? CustomFields { get; set; }
    }
}
