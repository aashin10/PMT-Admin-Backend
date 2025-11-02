using System.Collections.Generic;

namespace PmtAdmin.Application.Dto
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Metadata { get; set; }
        public int UserCount { get; set; }
        public string? CreatedAt { get; set; }
        public List<PermissionDto>? Permissions { get; set; }
    }

    public class PermissionDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}