using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Infrastructure.Context.Seeding;

namespace PmtAdmin.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Suppress non-deterministic model warnings
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        // DbSets
        public DbSet<User> User { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<DeliveryUnit> DeliveryUnits { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<ProjectTemplate> ProjectTemplates { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<Team> Teams { get; set; }
        public ICollection<StarredProjects> StarredProjects { get; set; }

        public DbSet<Board> Boards { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }

        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueComment> IssueComments { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<BoardColumn> BoardColumns { get; set; }
        public DbSet<BoardBoardColumnMap> BoardBoardColumnMaps { get; set; }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ImportJob> ImportJobs { get; set; }
        public DbSet<JiraAuthorization> JiraAuthorizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // USERS CONFIGURATION
            // ============================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();

                // Self-referencing relationships
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.DeletedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // ROLES & PERMISSIONS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // REFRESH TOKENS CONFIGURATION
            // ============================================
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Token).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany() // If you want to add navigation, you can later use: .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Token)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ReplacedByToken)
                    .HasMaxLength(500);
            });


            // ============================================
            // DELIVERY UNITS CONFIGURATION
            // ============================================
            //modelBuilder.Entity<DeliveryUnit>(entity =>
            //{
            //    entity.HasIndex(e => e.Code).IsUnique();

            //    entity.HasOne(e => e.Manager)
            //        .WithMany(u => u.ManagedDeliveryUnits)
            //        .HasForeignKey(e => e.ManagerId)
            //        .OnDelete(DeleteBehavior.SetNull);
            //});

            modelBuilder.Entity<DeliveryUnit>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // ============================================
            // PROJECT STATUS CONFIGURATION
            // ============================================
            modelBuilder.Entity<ProjectStatus>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // ============================================
            // Label CONFIGURATION
            // ============================================
            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique(); // optional
            });


            // ============================================
            // PROJECTS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.Key).IsUnique();
                entity.HasIndex(e => e.StatusId);

                entity.HasOne(e => e.ProjectManager)
                    .WithMany(u => u.ManagedProjects)
                    .HasForeignKey(e => e.ProjectManagerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ProjectManagerRole)
                    .WithMany(r => r.ProjectManagerRoles)
                    .HasForeignKey(e => e.ProjectManagerRoleId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Status)
                    .WithMany(s => s.Projects)
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.DeliveryUnit)
                    .WithMany(d => d.Projects)
                    .HasForeignKey(e => e.DeliveryUnitId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Template)
                    .WithMany(t => t.Projects)
                    .HasForeignKey(e => e.TemplateId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // CUSTOM FIELDS CONFIGURATION
            // ============================================
            modelBuilder.Entity<CustomField>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.CustomFields)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // TEAMS CONFIGURATION
            // ============================================

            //modelBuilder.Entity<Team>(entity =>
            //{
            //    entity.HasIndex(e => e.ProjectId);

            //    entity.HasOne(e => e.Project)
            //        .WithMany(p => p.Teams)
            //        .HasForeignKey(e => e.ProjectId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(e => e.Lead)
            //        .WithMany(u => u.LeadTeams)
            //        .HasForeignKey(e => e.LeadId)
            //        .OnDelete(DeleteBehavior.SetNull);

            //    entity.HasOne(e => e.Creator)
            //        .WithMany()
            //        .HasForeignKey(e => e.CreatedBy)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(e => e.Updater)
            //        .WithMany()
            //        .HasForeignKey(e => e.UpdatedBy)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});
            modelBuilder.Entity<Team>(entity =>
            {
                // ?? Index for faster lookup by Project
                entity.HasIndex(e => e.ProjectId);

                // ?? Project ? Teams (1:N)
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ?? Lead ? Teams (optional, ProjectMember as Lead)
                entity.HasOne(e => e.Lead)
                    .WithMany()
                    .HasForeignKey(e => e.LeadId)
                    .OnDelete(DeleteBehavior.SetNull);

                // ?? Creator ? Teams (optional, ProjectMember as Creator)
                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // ?? Updater ? Teams (optional, ProjectMember as Updater)
                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // ?? Labels column as JSONB (optional but recommended for PostgreSQL)
                entity.Property(e => e.Label)
                    .HasColumnName("label")
                    .HasColumnType("jsonb");
            });

            // ============================================
            // Starred CONFIGURATION
            // ============================================
            modelBuilder.Entity<StarredProjects>(entity =>
            {
                // Unique index to ensure a user can star a project only once
                entity.HasIndex(e => new { e.ProjectId, e.UserId }).IsUnique();

                // Relationships
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.StarredProjects)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.StarredProjects)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            // ============================================
            // BOARDS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasIndex(e => e.TeamId);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Boards)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.Boards)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // PROJECT MEMBERS CONFIGURATION
            // ============================================

            modelBuilder.Entity<ProjectMember>(entity =>
            {
                // Unique index so that a user cannot be added twice to the same project
                entity.HasIndex(e => new { e.ProjectId, e.UserId }).IsUnique();

                // Project ? ProjectMembers (1:N)
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.ProjectMembers)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // User ? ProjectMembers (1:N)
                entity.HasOne(e => e.User)
                    .WithMany(u => u.ProjectMembers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Role ? ProjectMembers (1:N)
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.ProjectMembers)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                // AddedBy (User reference)
                entity.HasOne(e => e.Users) // this is the navigation property for AddedBy
                    .WithMany()
                    .HasForeignKey(e => e.AddedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //modelBuilder.Entity<ProjectMember>(entity =>
            //{
            //    entity.HasIndex(e => new { e.ProjectId, e.TeamId, e.UserId }).IsUnique();

            //    entity.HasOne(e => e.Project)
            //        .WithMany(p => p.ProjectMembers)
            //        .HasForeignKey(e => e.ProjectId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(e => e.Team)
            //        .WithMany(t => t.ProjectMembers)
            //        .HasForeignKey(e => e.TeamId)
            //        .OnDelete(DeleteBehavior.SetNull);

            //    entity.HasOne(e => e.User)
            //        .WithMany(u => u.ProjectMembers)
            //        .HasForeignKey(e => e.UserId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(e => e.Role)
            //        .WithMany(r => r.ProjectMembers)
            //        .HasForeignKey(e => e.RoleId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(e => e.AddedByUser)
            //        .WithMany()
            //        .HasForeignKey(e => e.AddedBy)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});


            // ============================================
            // TEAM MEMBERS CONFIGURATION
            // ============================================
            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.HasKey(e => e.TeamMemberId);

                entity.HasIndex(e => new { e.TeamId, e.ProjectMemberId }).IsUnique();

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.TeamMembers)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProjectMember)
                    .WithMany()
                    .HasForeignKey(e => e.ProjectMemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // ============================================
            // SPRINTS CONFIGURATION
            // ============================================

            //modelBuilder.Entity<Sprint>(entity =>
            //{
            //    entity.HasIndex(e => e.ProjectId);

            //    entity.HasOne(e => e.Project)
            //        .WithMany(p => p.Sprints)
            //        .HasForeignKey(e => e.ProjectId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(e => e.Creator)
            //        .WithMany()
            //        .HasForeignKey(e => e.CreatedBy)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(e => e.Updater)
            //        .WithMany()
            //        .HasForeignKey(e => e.UpdatedBy)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});
            // ============================================
            // SPRINTS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Sprint>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.ProjectId);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Sprints)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.Sprints)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // ============================================
            // EPICS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Epic>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Epics)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Assignee)
                    .WithMany()
                    .HasForeignKey(e => e.AssigneeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Reporter)
                    .WithMany()
                    .HasForeignKey(e => e.ReporterId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // ISSUES CONFIGURATION
            // ============================================
            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasIndex(e => e.Key).IsUnique();
                entity.HasIndex(e => e.ProjectId);
                entity.HasIndex(e => e.SprintId);
                entity.HasIndex(e => e.EpicId);

                // Check constraint for story_points >= 0
                entity.HasCheckConstraint("CK_Issues_StoryPoints", "story_points >= 0");

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Epic)
                    .WithMany(ep => ep.Issues)
                    .HasForeignKey(e => e.EpicId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Sprint)
                    .WithMany(s => s.Issues)
                    .HasForeignKey(e => e.SprintId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ParentIssue)
                    .WithMany(i => i.ChildIssues)
                    .HasForeignKey(e => e.ParentIssueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Assignee)
                    .WithMany()
                    .HasForeignKey(e => e.AssigneeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Reporter)
                    .WithMany()
                    .HasForeignKey(e => e.ReporterId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // ISSUE COMMENTS CONFIGURATION
            // ============================================
            modelBuilder.Entity<IssueComment>(entity =>
            {
                entity.HasIndex(e => e.IssueId);

                entity.HasOne(e => e.Issue)
                    .WithMany(i => i.IssueComments)
                    .HasForeignKey(e => e.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Author)
                    .WithMany()
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MentionedUser)
                    .WithMany()
                    .HasForeignKey(e => e.MentionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // MENTIONS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Mention>(entity =>
            {
                entity.HasOne(e => e.MentionedUser)
                    .WithMany()
                    .HasForeignKey(e => e.MentionUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.IssueComment)
                    .WithMany(ic => ic.Mentions)
                    .HasForeignKey(e => e.IssueCommentsId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // BOARD COLUMNS CONFIGURATION
            // ============================================
            modelBuilder.Entity<BoardColumn>(entity =>
            {
                entity.HasOne(e => e.Status)
                    .WithMany(s => s.BoardColumns)
                    .HasForeignKey(e => e.StatusId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // BOARD ? BOARD COLUMN MAP CONFIGURATION
            // ============================================
            modelBuilder.Entity<BoardBoardColumnMap>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Board relationship
                entity.HasOne(e => e.Board)
                    .WithMany(b => b.BoardBoardColumnMaps)
                    .HasForeignKey(e => e.BoardId)
                    .OnDelete(DeleteBehavior.Cascade); // or SetNull if you prefer

                // BoardColumn relationship
                entity.HasOne(e => e.BoardColumn)
                    .WithMany(bc => bc.BoardBoardColumnMaps)
                    .HasForeignKey(e => e.BoardColumnId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // CHANNELS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasOne(e => e.Team)
                    .WithMany(t => t.Channels)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // MESSAGES CONFIGURATION
            // ============================================
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(e => e.Channel)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MentionedUser)
                    .WithMany()
                    .HasForeignKey(e => e.MentionUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // ACTIVITY LOG CONFIGURATION
            // ============================================
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // NOTIFICATION CONFIGURATION
            // ============================================
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(e => e.Recipient)
                    .WithMany()
                    .HasForeignKey(e => e.RecipientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Actor)
                    .WithMany()
                    .HasForeignKey(e => e.ActorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Activity)
                    .WithMany(a => a.Notifications)
                    .HasForeignKey(e => e.ActivityId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // AUDIT LOGS CONFIGURATION
            // ============================================
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasIndex(e => new { e.EntityType, e.EntityId });

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // IMPORT JOBS CONFIGURATION
            // ============================================
            modelBuilder.Entity<ImportJob>(entity =>
            {
                entity.HasOne(e => e.StartedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.StartedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // JIRA AUTHORIZATION CONFIGURATION
            // ============================================
            modelBuilder.Entity<JiraAuthorization>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.JiraAuthorizations)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // SEED DATA
            // ============================================
            DatabaseSeeder.SeedData(modelBuilder);
        }
    }
}