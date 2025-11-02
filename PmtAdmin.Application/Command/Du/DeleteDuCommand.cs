using MediatR;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command.Du
{
    public class DeleteDuCommand : IRequest<ApiResponse<string>>
    {
        public int Id { get; set; }

        public DeleteDuCommand(int id)
        {
            Id = id;
        }
    }
}
