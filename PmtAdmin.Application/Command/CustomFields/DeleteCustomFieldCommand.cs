using MediatR;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command.CustomFields
{
    public class DeleteCustomFieldCommand : IRequest<ApiResponse<string>>
    {
        public Guid Id { get; set; }
    }
}
