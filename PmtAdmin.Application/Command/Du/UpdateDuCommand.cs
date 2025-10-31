using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command.Du
{
    public class UpdateDuCommand : IRequest<ApiResponse<DuDto>>
    {
        public int Id { get; set; }                 // Internal DU Id (from DB)
        public string? Name { get; set; }          // e.g. "Engineering"
        public string? Code { get; set; }          // e.g. "ENG-001" (non-editable in UI)
        public string? Description { get; set; }     // e.g. "Software Development & Architecture"
        public string? HeadName { get; set; }        // e.g. "Sarah Chen"
        public string? HeadEmail { get; set; }       // e.g. "sarah.chen@company.com"
    }
}
