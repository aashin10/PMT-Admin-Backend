using Newtonsoft.Json;

namespace PmtAdmin.Domain.Models
{
    public static class JiraImportModels
    {
        public class JiraProject
        {
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

            public string Goal { get; set; }

        }

        public class JiraIssue
        {

            public int Id { get; set; }

            public string Key { get; set; }

            public string Summary { get; set; }

            public string Description { get; set; }

            public JiraUser Assignee { get; set; }

            public List<String> Labels { get; set; }

            public JiraUser Reporter { get; set; }

            public JiraSprint Sprint { get; set; }

            public JiraUser Creator { get; set; }


            public List<JiraComment> Comment { get; set; }

            public JiraTeam Team { get; set; }
            public DateTimeOffset UpdatedAt { get; set; }

            public int StoryPoints { get; set; }

            public JiraPriority Priority { get; set; }

            public JiraIssueType IssueType { get; set; }

            public JiraStatus Status { get; set; }

            public JiraEpic Epic { get; set; }

            public DateTime? DueDate { get; set; }

            public DateTime? StartDate { get; set; }



        }

        public class JiraIssueType
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        public class JiraEpic
        {
            public int Id { get; set; }

            public string Key { get; set; }

            public string Summary { get; set; }

            public string Description { get; set; }


        }

        public class JiraStatus
        {
            public int Id { get; set; }
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

            public string Title { get; set; }
        }

        public class JiraComment
        {

            public string Id { get; set; }
            public JiraUser Author { get; set; }

            public string Body { get; set; }

            [JsonProperty("created")]
            public DateTimeOffset CreatedAt { get; set; }

            [JsonProperty("updated")]
            public DateTimeOffset UpdatedAt { get; set; }

        }

        public class JiraProjectData
        {
            public JiraProject Project { get; set; }
            public List<BoardWithDetails> Boards { get; set; }

            public Dictionary<string, List<JiraUser>> UsersByRole { get; set; }
        }

        public class BoardWithDetails
        {
            public JiraBoard BoardInfo { get; set; }
            public List<JiraEpic> Epics { get; set; }
            public List<JiraIssue> Issues { get; set; }
            public List<JiraSprint> Sprints { get; set; }

            public List<JiraRole> Roles { get; set; }

        }

        public class JiraRole
        {
            public int Id { get; set; }
        }
    }
}
