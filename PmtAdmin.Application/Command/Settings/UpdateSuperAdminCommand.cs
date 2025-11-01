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
    public class UpdateSuperAdminCommand : IRequest<ApiResponse<SuperAdminDto>>
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        // Optional flag to update super admin privilege
        //public bool? IsSuperAdmin { get; set; }

        // Optional flag to update active status
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }

    }
}
