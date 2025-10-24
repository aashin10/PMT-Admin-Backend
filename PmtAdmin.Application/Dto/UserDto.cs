using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? Created_At { get; set; }
        public string? Last_Login { get; set; }
    }
}
