namespace BACKEND_CQRS.Application.Dto
{
    /// <summary>
    /// User information DTO returned by the /me endpoint
    /// </summary>
    public class UserInfoDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}