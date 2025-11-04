using MediatR;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command.Settings
{
    public class UpdateSuperAdminCommand : IRequest<ApiResponse<SuperAdminDto>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
