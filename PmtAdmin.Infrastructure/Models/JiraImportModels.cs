using Newtonsoft.Json;

namespace PmtAdmin.Infrastructure.Models
{
    public class JiraImportModels
    {
        // ========================================
        // PROJECT
        // ========================================
        public class JiraProject
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("key")]
            public string? Key { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("lead")]
            public JiraUser? Lead { get; set; }

            [JsonProperty("projectTypeKey")]
            public string? ProjectTypeKey { get; set; }

            [JsonProperty("simplified")]
            public bool? Simplified { get; set; }

            [JsonProperty("style")]
            public string? Style { get; set; } // classic, next-gen

            [JsonProperty("isPrivate")]
            public bool? IsPrivate { get; set; }

            [JsonProperty("roles")]
            public Dictionary<string, string>? Roles { get; set; }

            [JsonProperty("avatarUrls")]
            public JiraAvatarUrls? AvatarUrls { get; set; }

            [JsonProperty("projectCategory")]
            public JiraProjectCategory? ProjectCategory { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // USER
        // ========================================
        public class JiraUser
        {
            [JsonProperty("accountId")]
            public string? AccountId { get; set; }

            [JsonProperty("emailAddress")]
            public string? EmailAddress { get; set; }

            [JsonProperty("displayName")]
            public string? DisplayName { get; set; }

            [JsonProperty("active")]
            public bool? Active { get; set; }

            [JsonProperty("timeZone")]
            public string? TimeZone { get; set; }

            [JsonProperty("accountType")]
            public string? AccountType { get; set; }

            [JsonProperty("avatarUrls")]
            public JiraAvatarUrls? AvatarUrls { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // BOARD
        // ========================================
        public class JiraBoard
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("type")]
            public string? Type { get; set; } // scrum, kanban

            [JsonProperty("self")]
            public string? Self { get; set; }

            [JsonProperty("location")]
            public JiraBoardLocation? Location { get; set; }
        }

        public class JiraBoardLocation
        {
            [JsonProperty("projectId")]
            public int? ProjectId { get; set; }

            [JsonProperty("displayName")]
            public string? DisplayName { get; set; }

            [JsonProperty("projectName")]
            public string? ProjectName { get; set; }

            [JsonProperty("projectKey")]
            public string? ProjectKey { get; set; }

            [JsonProperty("projectTypeKey")]
            public string? ProjectTypeKey { get; set; }

            [JsonProperty("avatarURI")]
            public string? AvatarUri { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }
        }

        // ========================================
        // SPRINT
        // ========================================
        public class JiraSprint
        {
            [JsonProperty("id")]
            public int? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("state")]
            public string? State { get; set; } // future, active, closed

            [JsonProperty("startDate")]
            public DateTime? StartDate { get; set; }

            [JsonProperty("endDate")]
            public DateTime? EndDate { get; set; }

            [JsonProperty("completeDate")]
            public DateTime? CompleteDate { get; set; }

            [JsonProperty("originBoardId")]
            public int? OriginBoardId { get; set; }

            [JsonProperty("goal")]
            public string? Goal { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // ISSUE
        // ========================================
        public class JiraIssue
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("key")]
            public string? Key { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }

            // Basic Fields
            public string? Summary { get; set; }
            public string? Description { get; set; }
            public string? IssueType { get; set; }

            // Relationships
            public JiraUser? Assignee { get; set; }
            public JiraUser? Reporter { get; set; }
            public JiraUser? Creator { get; set; }

            // Sprint & Epic
            public JiraSprint? Sprint { get; set; }
            public string? EpicKey { get; set; }
            public string? EpicName { get; set; }

            // Parent Issue (for subtasks)
            public JiraIssueParent? Parent { get; set; }
            public string? ParentKey { get; set; }

            // Status & Priority
            public JiraStatus? Status { get; set; }
            public JiraPriority? Priority { get; set; }

            // Labels & Components
            public List<string>? Labels { get; set; }
            public List<JiraComponent>? Components { get; set; }

            // Dates
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DueDate { get; set; }
            public DateTime? ResolutionDate { get; set; }

            // Story Points & Time Tracking
            public decimal? StoryPoints { get; set; }
            public int? TimeEstimate { get; set; }
            public int? TimeSpent { get; set; }
            public int? TimeRemaining { get; set; }

            // Comments & Attachments
            public List<JiraComment>? Comment { get; set; }
            public List<JiraAttachment>? Attachments { get; set; }

            // Custom Fields
            public JiraTeam? Team { get; set; }
            public string? Environment { get; set; }

            // Workflow
            public JiraResolution? Resolution { get; set; }
            public List<JiraWorklog>? Worklogs { get; set; }

            // Sprint information (custom field)
            public string? SprintName { get; set; }
        }

        // ========================================
        // EPIC
        // ========================================
        public class JiraEpic
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("key")]
            public string? Key { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("summary")]
            public string? Summary { get; set; }

            [JsonProperty("done")]
            public bool? Done { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }

            // Additional Epic fields
            public string? Description { get; set; }
            public JiraStatus? Status { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? DueDate { get; set; }
            public string? Color { get; set; }
        }

        // ========================================
        // STATUS
        // ========================================
        public class JiraStatus
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("iconUrl")]
            public string? IconUrl { get; set; }

            [JsonProperty("statusCategory")]
            public JiraStatusCategory? StatusCategory { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        public class JiraStatusCategory
        {
            [JsonProperty("id")]
            public int? Id { get; set; }

            [JsonProperty("key")]
            public string? Key { get; set; } // new, indeterminate, done

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("colorName")]
            public string? ColorName { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // PRIORITY
        // ========================================
        public class JiraPriority
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("iconUrl")]
            public string? IconUrl { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // RESOLUTION
        // ========================================
        public class JiraResolution
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // TEAM
        // ========================================
        public class JiraTeam
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("title")]
            public string? Title { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // COMMENT
        // ========================================
        public class JiraComment
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("author")]
            public JiraUser? Author { get; set; }

            [JsonProperty("body")]
            public string? Body { get; set; }

            [JsonProperty("created")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("updated")]
            public DateTime? UpdatedAt { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }

            [JsonProperty("updateAuthor")]
            public JiraUser? UpdateAuthor { get; set; }
        }

        // ========================================
        // ATTACHMENT
        // ========================================
        public class JiraAttachment
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("filename")]
            public string? Filename { get; set; }

            [JsonProperty("author")]
            public JiraUser? Author { get; set; }

            [JsonProperty("created")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("size")]
            public long? Size { get; set; }

            [JsonProperty("mimeType")]
            public string? MimeType { get; set; }

            [JsonProperty("content")]
            public string? Content { get; set; }

            [JsonProperty("thumbnail")]
            public string? Thumbnail { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // WORKLOG
        // ========================================
        public class JiraWorklog
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("author")]
            public JiraUser? Author { get; set; }

            [JsonProperty("comment")]
            public string? Comment { get; set; }

            [JsonProperty("created")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("updated")]
            public DateTime? UpdatedAt { get; set; }

            [JsonProperty("started")]
            public DateTime? Started { get; set; }

            [JsonProperty("timeSpent")]
            public string? TimeSpent { get; set; }

            [JsonProperty("timeSpentSeconds")]
            public int? TimeSpentSeconds { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // COMPONENT
        // ========================================
        public class JiraComponent
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("lead")]
            public JiraUser? Lead { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // ISSUE PARENT (for subtasks)
        // ========================================
        public class JiraIssueParent
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("key")]
            public string? Key { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }

            [JsonProperty("fields")]
            public JiraIssueParentFields? Fields { get; set; }
        }

        public class JiraIssueParentFields
        {
            [JsonProperty("summary")]
            public string? Summary { get; set; }

            [JsonProperty("status")]
            public JiraStatus? Status { get; set; }

            [JsonProperty("priority")]
            public JiraPriority? Priority { get; set; }

            [JsonProperty("issuetype")]
            public JiraIssueType? IssueType { get; set; }
        }

        // ========================================
        // ISSUE TYPE
        // ========================================
        public class JiraIssueType
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("iconUrl")]
            public string? IconUrl { get; set; }

            [JsonProperty("subtask")]
            public bool? Subtask { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // PROJECT CATEGORY
        // ========================================
        public class JiraProjectCategory
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // AVATAR URLS
        // ========================================
        public class JiraAvatarUrls
        {
            [JsonProperty("48x48")]
            public string? Size48 { get; set; }

            [JsonProperty("24x24")]
            public string? Size24 { get; set; }

            [JsonProperty("16x16")]
            public string? Size16 { get; set; }

            [JsonProperty("32x32")]
            public string? Size32 { get; set; }
        }

        // ========================================
        // ROLE
        // ========================================
        public class JiraRole
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("actors")]
            public List<JiraRoleActor>? Actors { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        public class JiraRoleActor
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("displayName")]
            public string? DisplayName { get; set; }

            [JsonProperty("type")]
            public string? Type { get; set; } // atlassian-user-role-actor, atlassian-group-role-actor

            [JsonProperty("actorUser")]
            public JiraUser? ActorUser { get; set; }

            [JsonProperty("actorGroup")]
            public JiraGroup? ActorGroup { get; set; }
        }

        public class JiraGroup
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("groupId")]
            public string? GroupId { get; set; }

            [JsonProperty("self")]
            public string? Self { get; set; }
        }

        // ========================================
        // PROJECT DATA (Main Container)
        // ========================================
        public class JiraProjectData
        {
            public JiraProject? Project { get; set; }
            public List<BoardWithDetails>? Boards { get; set; }
            public List<JiraUser>? ProjectMembers { get; set; }
            public Dictionary<string, List<JiraUser>>? RoleMembers { get; set; }

            // Import Metadata
            public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
            public string? ImportedBy { get; set; }
            public string? SourceBaseUrl { get; set; }
        }

        public class BoardWithDetails
        {
            public JiraBoard? BoardInfo { get; set; }
            public List<JiraEpic>? Epics { get; set; }
            public List<JiraIssue>? Issues { get; set; }
            public List<JiraSprint>? Sprints { get; set; }
            public List<JiraRole>? Roles { get; set; }

            // Board Statistics
            public int TotalIssues => Issues?.Count ?? 0;
            public int TotalEpics => Epics?.Count ?? 0;
            public int TotalSprints => Sprints?.Count ?? 0;
            public int OpenIssues => Issues?.Count(i => i.Status?.StatusCategory?.Key != "done") ?? 0;
            public int ClosedIssues => Issues?.Count(i => i.Status?.StatusCategory?.Key == "done") ?? 0;
        }

        // ========================================
        // IMPORT STATISTICS
        // ========================================
        public class ImportStatistics
        {
            public int TotalProjects { get; set; }
            public int TotalBoards { get; set; }
            public int TotalSprints { get; set; }
            public int TotalEpics { get; set; }
            public int TotalIssues { get; set; }
            public int TotalComments { get; set; }
            public int TotalUsers { get; set; }

            public int FailedProjects { get; set; }
            public int FailedBoards { get; set; }
            public int FailedIssues { get; set; }

            public List<string> Errors { get; set; } = new List<string>();
            public List<string> Warnings { get; set; } = new List<string>();

            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;

            public bool IsSuccess => FailedProjects == 0 && FailedBoards == 0 && FailedIssues == 0;
        }

        // ========================================
        // FIELD MAPPING CONFIGURATION
        // ========================================
        public class JiraFieldMapping
        {
            public string JiraFieldId { get; set; }
            public string JiraFieldName { get; set; }
            public string TargetField { get; set; }
            public string DataType { get; set; } // string, number, date, user, etc.
            public bool IsCustomField { get; set; }
            public string DefaultValue { get; set; }
        }

        // ========================================
        // IMPORT CONFIGURATION
        // ========================================
        public class JiraImportConfiguration
        {
            public bool ImportAttachments { get; set; } = false;
            public bool ImportComments { get; set; } = true;
            public bool ImportWorklogs { get; set; } = false;
            public bool ImportSubtasks { get; set; } = true;
            public bool ImportLinkedIssues { get; set; } = false;
            public bool ImportClosedSprints { get; set; } = true;

            public List<string> IssueTypesToImport { get; set; } = new List<string>();
            public List<string> StatusesToImport { get; set; } = new List<string>();

            public Dictionary<string, JiraFieldMapping> CustomFieldMappings { get; set; } =
                new Dictionary<string, JiraFieldMapping>();

            public int MaxIssuesPerRequest { get; set; } = 50;
            public int MaxRetries { get; set; } = 3;
            public int RetryDelaySeconds { get; set; } = 2;
        }
    }
}