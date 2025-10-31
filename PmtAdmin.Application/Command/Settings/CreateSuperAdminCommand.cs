using MediatR;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command.Settings
{
    public class CreateSuperAdminCommand : IRequest<ApiResponse<SuperAdminDto>>
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Indicates if this admin is a super admin (optional)
        public bool IsSuperAdmin { get; set; } = false;

        // Optional: default active status
        public bool IsActive { get; set; } = true;
    }
}
