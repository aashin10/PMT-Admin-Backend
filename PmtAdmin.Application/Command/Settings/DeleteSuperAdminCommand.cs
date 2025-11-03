using MediatR;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command.Settings
{
    public class DeleteSuperAdminCommand : IRequest<ApiResponse<SuperAdminDto>>
    {
        public int Id { get; set; }
        public int? DeletedBy { get; set; }
    }
}
