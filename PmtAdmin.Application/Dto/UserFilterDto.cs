using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class UserFilterDto
    {
        public string? Type { get; set; }  // "Internal" or "External"
        public string? Status { get; set; }  // "Active" or "Inactive"
    }
}
