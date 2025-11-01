using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Application.Dto
{
    public class JiraImportDatabaseResult
    {
        public List<User> Users { get; set; }

        public List<Project> Projects { get; set; }

        public bool ImportFailed { get; set; }
    }
}
