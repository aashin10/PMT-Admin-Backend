namespace PmtAdmin.Infrastructure.Models
{
    public class JiraImportModels
    {
        public class JiraProject
        {
            public string? Id { get; set; }
            public string? Key { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public JiraUser? Lead { get; set; }
            public Dictionary<string, string>? Roles { get; set; }

        }

        public class JiraUser
        {
            public string AccountId { get; set; }
            public string DisplayName { get; set; }
        }

        public class JiraBoard
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Type { get; set; }
        }

        public class JiraSprint
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? State { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime? CompleteDate { get; set; }

        }
    }
}
