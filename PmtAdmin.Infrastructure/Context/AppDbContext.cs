using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> User { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<DeliveryUnit> DeliveryUnits { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<ProjectTemplate> ProjectTemplates { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueComment> IssueComments { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<BoardColumn> BoardColumns { get; set; }
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
            // DELIVERY UNITS CONFIGURATION
            // ============================================
            modelBuilder.Entity<DeliveryUnit>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();

                entity.HasOne(e => e.Manager)
                    .WithMany(u => u.ManagedDeliveryUnits)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ============================================
            // PROJECT STATUS CONFIGURATION
            // ============================================
            modelBuilder.Entity<ProjectStatus>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
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
            // TEAMS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Lead)
                    .WithMany(u => u.LeadTeams)
                    .HasForeignKey(e => e.LeadId)
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
                entity.HasIndex(e => new { e.ProjectId, e.TeamId, e.UserId }).IsUnique();

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.ProjectMembers)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.ProjectMembers)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.ProjectMembers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.ProjectMembers)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AddedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.AddedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================
            // SPRINTS CONFIGURATION
            // ============================================
            modelBuilder.Entity<Sprint>(entity =>
            {
                entity.HasIndex(e => e.ProjectId);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Sprints)
                    .HasForeignKey(e => e.ProjectId)
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

            //modelBuilder.SeedUsers();              // First - no dependencies
            //modelBuilder.SeedRoles();              // Second - no dependencies
            //modelBuilder.SeedDeliveryUnits();      // Third - no dependencies
            //modelBuilder.SeedProjectStatuses();    // Fourth - no dependencies
            //modelBuilder.SeedProjectTemplates();
            //modelBuilder.SeedProjects();
        }

        //public DbSet<Users> Users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Users>(entity =>
        //    {
        //        // Configure primary key
        //        entity.HasKey(e => e.Id);

        //        // Configure email unique constraint
        //        entity.HasIndex(e => e.Email)
        //              .IsUnique();

        //        // Configure index on is_deleted
        //        entity.HasIndex(e => e.Is_Deleted);

        //        // Set default values
        //        entity.Property(e => e.Is_Active)
        //              .HasDefaultValue(true);

        //        entity.Property(e => e.Is_Super_Admin)
        //              .HasDefaultValue(false);

        //        entity.Property(e => e.Created_At)
        //              .HasDefaultValueSql("CURRENT_TIMESTAMP");

        //        entity.Property(e => e.Is_Deleted)
        //              .HasDefaultValue(false);

        //        // Configure self-referencing foreign keys
        //        entity.HasOne(e => e.CreatedByUser)
        //              .WithMany()
        //              .HasForeignKey(e => e.Created_By)
        //              .OnDelete(DeleteBehavior.Restrict);

        //        entity.HasOne(e => e.UpdatedByUser)
        //              .WithMany()
        //              .HasForeignKey(e => e.Updated_By)
        //              .OnDelete(DeleteBehavior.Restrict);

        //        entity.HasOne(e => e.DeletedByUser)
        //              .WithMany()
        //              .HasForeignKey(e => e.Deleted_By)
        //              .OnDelete(DeleteBehavior.Restrict);

        //        // Configure string lengths
        //        entity.Property(e => e.Email)
        //              .HasMaxLength(255);

        //        entity.Property(e => e.Password_Hash)
        //              .HasMaxLength(1024);

        //        entity.Property(e => e.Name)
        //              .HasMaxLength(150);

        //        entity.Property(e => e.Avatar_Url)
        //              .HasMaxLength(1000);

        //        entity.Property(e => e.Jira_Id)
        //              .HasMaxLength(1024);
        //    });

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
