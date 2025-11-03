using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command.CustomFields
{
    public class CreateCustomFieldCommand : IRequest<ApiResponse<CustomFieldDTO>>
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
