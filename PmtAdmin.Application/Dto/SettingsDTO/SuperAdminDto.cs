using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto.SettingsDTO
{
    public class SuperAdminDto
    {
        public int Id { get; set; }
        public bool IsSuperAdmin { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
