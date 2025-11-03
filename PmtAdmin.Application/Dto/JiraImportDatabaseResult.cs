using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Application.Dto
{
    public class JiraImportDatabaseResult
    {
        public List<User> Users { get; set; }

        public List<Project> Projects { get; set; }

        public List<OperationResult> Results { get; set; }


    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
