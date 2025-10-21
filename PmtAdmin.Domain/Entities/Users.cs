using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password_Hash { get; set; }
        public string? Name { get; set; }
        public bool Is_Active { get; set; } = true;
        public bool Is_Super_Admin { get; set; } = true;
        public DateTime LastLogin { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; }
        public DateTime Deleted_At { get; set; }
        public int Created_By { get; set; }
        public int Updated_By { get; set; }
        public int Deleted_By { get; set; }
        public bool Is_Deleted { get; set; } = false;

    }
}
