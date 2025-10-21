using Newtonsoft.Json;

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

        public class JiraIssue
        {
            public string Id { get; set; }

            public string Key { get; set; }

            public string Summary { get; set; }

            public JiraUser Assignee { get; set; }

            public List<String> Labels { get; set; }

            public JiraUser Reporter { get; set; }

            public JiraUser Creator { get; set; }

            public List<JiraComment> Comment { get; set; }

            public JiraTeam Team { get; set; }
            public DateTime UpdatedAt { get; set; }

            public JiraPriority Priority { get; set; }

            public JiraStatus Status { get; set; }

        }

        public class JiraEpic : JiraIssue
        {

        }

        public class JiraStatus
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class JiraPriority
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class JiraTeam
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public string Titile { get; set; }
        }

        public class JiraComment
        {
            public int Id { get; set; }

            public JiraUser Author { get; set; }

            public string Body { get; set; }

            [JsonProperty("created")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("updated")]
            public DateTime UpdatedAt { get; set; }

        }
    }


}