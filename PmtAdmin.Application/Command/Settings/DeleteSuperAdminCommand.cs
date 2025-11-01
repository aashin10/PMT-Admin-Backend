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
    public class DeleteSuperAdminCommand : IRequest<ApiResponse<SuperAdminDto>>
    {
        public int Id { get; set; }
        public int? DeletedBy { get; internal set; }
    }
}
