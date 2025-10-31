using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Infrastructure.Context.Seeding
{
    public static class DatabaseSeeder
    {
        // Static GUIDs for all entities
        private static readonly Guid _projectId1 = new Guid("11111111-1111-1111-1111-111111111111");
        private static readonly Guid _projectId2 = new Guid("22222222-2222-2222-2222-222222222222");
        private static readonly Guid _projectId3 = new Guid("33333333-3333-3333-3333-333333333333");
        private static readonly Guid _projectId4 = new Guid("44444444-4444-4444-4444-444444444444");
        private static readonly Guid _projectId5 = new Guid("55555555-5555-5555-5555-555555555555");
        private static readonly Guid _projectId6 = new Guid("66666666-6666-6666-6666-666666666666");
        private static readonly Guid _projectId7 = new Guid("77777777-7777-7777-7777-777777777777");
        private static readonly Guid _projectId8 = new Guid("88888888-8888-8888-8888-888888888888");
        private static readonly Guid _projectId9 = new Guid("99999999-9999-9999-9999-999999999999");
        private static readonly Guid _projectId10 = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        private static readonly Guid _customFieldId1 = new Guid("00000000-0000-0000-0000-000000000049");
        private static readonly Guid _customFieldId2 = new Guid("00000000-0000-0000-0000-000000000050");
        private static readonly Guid _customFieldId3 = new Guid("00000000-0000-0000-0000-000000000051");
        private static readonly Guid _customFieldId4 = new Guid("00000000-0000-0000-0000-000000000052");
        private static readonly Guid _customFieldId5 = new Guid("00000000-0000-0000-0000-000000000053");
        // Add more as needed

        //private static readonly Guid _statusId1 = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        //private static readonly Guid _statusId2 = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc");
        //private static readonly Guid _statusId3 = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd");
        //private static readonly Guid _statusId4 = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        private static readonly int _statusId1 = 1;
        private static readonly int _statusId2 = 2;
        private static readonly int _statusId3 = 3;
        private static readonly int _statusId4 = 4;


        private static readonly Guid _boardColumnId1 = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
        private static readonly Guid _boardColumnId2 = new Guid("00000000-0000-0000-0000-000000000001");
        private static readonly Guid _boardColumnId3 = new Guid("00000000-0000-0000-0000-000000000002");
        private static readonly Guid _boardColumnId4 = new Guid("00000000-0000-0000-0000-000000000003");

        private static readonly Guid _channelId1 = new Guid("00000000-0000-0000-0000-000000000004");
        private static readonly Guid _channelId2 = new Guid("00000000-0000-0000-0000-000000000005");
        private static readonly Guid _channelId3 = new Guid("00000000-0000-0000-0000-000000000006");
        private static readonly Guid _channelId4 = new Guid("00000000-0000-0000-0000-000000000007");
        private static readonly Guid _channelId5 = new Guid("00000000-0000-0000-0000-000000000008");
        private static readonly Guid _channelId6 = new Guid("00000000-0000-0000-0000-000000000009");
        private static readonly Guid _channelId7 = new Guid("00000000-0000-0000-0000-000000000010");
        private static readonly Guid _channelId8 = new Guid("00000000-0000-0000-0000-000000000011");
        private static readonly Guid _channelId9 = new Guid("00000000-0000-0000-0000-000000000012");
        private static readonly Guid _channelId10 = new Guid("00000000-0000-0000-0000-000000000013");

        private static readonly Guid _sprintId1 = new Guid("00000000-0000-0000-0000-000000000014");
        private static readonly Guid _sprintId2 = new Guid("00000000-0000-0000-0000-000000000015");
        private static readonly Guid _sprintId3 = new Guid("00000000-0000-0000-0000-000000000016");
        private static readonly Guid _sprintId4 = new Guid("00000000-0000-0000-0000-000000000017");
        private static readonly Guid _sprintId5 = new Guid("00000000-0000-0000-0000-000000000018");

        private static readonly Guid _epicId1 = new Guid("00000000-0000-0000-0000-000000000019");
        private static readonly Guid _epicId2 = new Guid("00000000-0000-0000-0000-000000000020");
        private static readonly Guid _epicId3 = new Guid("00000000-0000-0000-0000-000000000021");
        private static readonly Guid _epicId4 = new Guid("00000000-0000-0000-0000-000000000022");
        private static readonly Guid _epicId5 = new Guid("00000000-0000-0000-0000-000000000023");
        private static readonly Guid _epicId6 = new Guid("00000000-0000-0000-0000-000000000024");
        private static readonly Guid _epicId7 = new Guid("00000000-0000-0000-0000-000000000025");
        private static readonly Guid _epicId8 = new Guid("00000000-0000-0000-0000-000000000026");
        private static readonly Guid _epicId9 = new Guid("00000000-0000-0000-0000-000000000027");
        private static readonly Guid _epicId10 = new Guid("00000000-0000-0000-0000-000000000028");

        private static readonly Guid _issueId1 = new Guid("00000000-0000-0000-0000-000000000029");
        private static readonly Guid _issueId2 = new Guid("00000000-0000-0000-0000-000000000030");
        private static readonly Guid _issueId3 = new Guid("00000000-0000-0000-0000-000000000031");
        private static readonly Guid _issueId4 = new Guid("00000000-0000-0000-0000-000000000032");
        private static readonly Guid _issueId5 = new Guid("00000000-0000-0000-0000-000000000033");
        private static readonly Guid _issueId6 = new Guid("00000000-0000-0000-0000-000000000034");
        private static readonly Guid _issueId7 = new Guid("00000000-0000-0000-0000-000000000035");
        private static readonly Guid _issueId8 = new Guid("00000000-0000-0000-0000-000000000036");
        private static readonly Guid _issueId9 = new Guid("00000000-0000-0000-0000-000000000037");
        private static readonly Guid _issueId10 = new Guid("00000000-0000-0000-0000-000000000038");
        private static readonly Guid _issueId11 = new Guid("00000000-0000-0000-0000-000000000039");
        private static readonly Guid _issueId12 = new Guid("00000000-0000-0000-0000-000000000040");
        private static readonly Guid _issueId13 = new Guid("00000000-0000-0000-0000-000000000041");
        private static readonly Guid _issueId14 = new Guid("00000000-0000-0000-0000-000000000042");
        private static readonly Guid _issueId15 = new Guid("00000000-0000-0000-0000-000000000043");

        private static readonly Guid _issueCommentId1 = new Guid("00000000-0000-0000-0000-000000000044");
        private static readonly Guid _mentionId1 = new Guid("00000000-0000-0000-0000-000000000045");
        private static readonly Guid _activityLogId1 = new Guid("00000000-0000-0000-0000-000000000046");
        private static readonly Guid _notificationId1 = new Guid("00000000-0000-0000-0000-000000000047");
        private static readonly Guid _jiraAuthId1 = new Guid("00000000-0000-0000-0000-000000000048");

        // Static dates - use fixed values, no calculations
        private static readonly DateTime _staticDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTimeOffset _staticDateOffset = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);


        //for projects
        private static readonly DateTime _projectDate1 = new DateTime(2024, 1, 5, 5, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate2 = new DateTime(2024, 3, 15, 3, 30, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate3 = new DateTime(2023, 8, 12, 9, 15, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate4 = new DateTime(2025, 5, 1, 2, 45, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate5 = new DateTime(2022, 11, 20, 11, 30, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate6 = new DateTime(2021, 6, 10, 6, 15, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate7 = new DateTime(2020, 9, 18, 9, 50, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate8 = new DateTime(2023, 2, 7, 7, 40, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate9 = new DateTime(2021, 12, 29, 12, 30, 0, DateTimeKind.Utc);
        private static readonly DateTime _projectDate10 = new DateTime(2019, 4, 12, 4, 45, 0, DateTimeKind.Utc);

        // Static dates for Sprints
        private static readonly DateTime _sprintStartDate1 = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _sprintDueDate1 = new DateTime(2024, 1, 19, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _sprintStartDate2 = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _sprintDueDate2 = new DateTime(2024, 2, 2, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _sprintStartDate3 = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _sprintDueDate3 = new DateTime(2024, 2, 14, 0, 0, 0, DateTimeKind.Utc);

        // Static DateTimeOffset for RefreshTokens
        private static readonly DateTimeOffset _refreshTokenExpiry = new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero);
        private static readonly DateTimeOffset _refreshTokenCreated = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);


        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Remove any dynamic method calls and seed everything directly
            SeedUsers(modelBuilder);
            SeedRefreshTokens(modelBuilder);

            SeedRoles(modelBuilder);
            SeedPermissions(modelBuilder);
            SeedRolePermissions(modelBuilder);
            SeedDeliveryUnits(modelBuilder);
            SeedProjectStatuses(modelBuilder);
            SeedProjectTemplates(modelBuilder);
            SeedStatuses(modelBuilder);
            SeedLabels(modelBuilder);
            SeedProjects(modelBuilder);
            SeedCustomFields(modelBuilder);
            SeedTeams(modelBuilder);
            SeedBoards(modelBuilder);
            SeedProjectMembers(modelBuilder);
            SeedTeamMembers(modelBuilder);
            SeedStarredProjects(modelBuilder);
            SeedBoardColumns(modelBuilder);
            SeedBoardBoardColumnMaps(modelBuilder);
            SeedChannels(modelBuilder);
            SeedSprints(modelBuilder);
            SeedEpics(modelBuilder);
            SeedIssues(modelBuilder);
            SeedIssueComments(modelBuilder);
            SeedMentions(modelBuilder);
            SeedActivityLogs(modelBuilder);
            SeedNotifications(modelBuilder);
            SeedAuditLogs(modelBuilder);
            SeedImportJobs(modelBuilder);
            SeedJiraAuthorizations(modelBuilder);
        }

        private static void SeedUsers(ModelBuilder modelBuilder)
        {
            var users = new List<User>
            {
                new User { Id = 1, Email = "admin@company.com", Name = "System Admin", IsActive = true, IsSuperAdmin = true, CreatedAt = _staticDate },
                new User { Id = 2, Email = "pm1@company.com", Name = "Project Manager 1", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 3, Email = "pm2@company.com", Name = "Project Manager 2", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 4, Email = "dev1@company.com", Name = "Developer 1", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 5, Email = "dev2@company.com", Name = "Developer 2", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 6, Email = "dev3@company.com", Name = "Developer 3", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 7, Email = "dev4@company.com", Name = "Developer 4", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 8, Email = "qa1@company.com", Name = "QA Engineer 1", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 9, Email = "qa2@company.com", Name = "QA Engineer 2", IsActive = true, CreatedAt = _staticDate },
                new User { Id = 10, Email = "designer1@company.com", Name = "Designer 1", IsActive = true, CreatedAt = _staticDate }
            };

            modelBuilder.Entity<User>().HasData(users);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin", Description = "System Administrator", CreatedAt = _staticDate },
                new Role { Id = 2, Name = "Project Manager", Description = "Project Manager", CreatedAt = _staticDate },
                new Role { Id = 3, Name = "Team Lead", Description = "Team Lead", CreatedAt = _staticDate },
                new Role { Id = 4, Name = "Developer", Description = "Software Developer", CreatedAt = _staticDate },
                new Role { Id = 5, Name = "QA Engineer", Description = "Quality Assurance Engineer", CreatedAt = _staticDate },
                new Role { Id = 6, Name = "Designer", Description = "UI/UX Designer", CreatedAt = _staticDate }
            };

            modelBuilder.Entity<Role>().HasData(roles);
        }

        private static void SeedPermissions(ModelBuilder modelBuilder)
        {
            var permissions = new List<Permission>
            {
                new Permission { Id = 1, Name = "project.create", Description = "Create projects" },
                new Permission { Id = 2, Name = "project.read", Description = "View projects" },
                new Permission { Id = 3, Name = "project.update", Description = "Update projects" },
                new Permission { Id = 4, Name = "project.delete", Description = "Delete projects" },
                new Permission { Id = 5, Name = "team.manage", Description = "Manage teams" },
                new Permission { Id = 6, Name = "user.manage", Description = "Manage users" }
            };

            modelBuilder.Entity<Permission>().HasData(permissions);
        }

        private static void SeedRolePermissions(ModelBuilder modelBuilder)
        {
            var rolePermissions = new List<RolePermission>
            {
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, CreatedAt = _staticDate },
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, CreatedAt = _staticDate },
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, CreatedAt = _staticDate },
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, CreatedAt = _staticDate },
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5, CreatedAt = _staticDate },
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6, CreatedAt = _staticDate },
                new RolePermission { Id = 7, RoleId = 2, PermissionId = 1, CreatedAt = _staticDate },
                new RolePermission { Id = 8, RoleId = 2, PermissionId = 2, CreatedAt = _staticDate },
                new RolePermission { Id = 9, RoleId = 2, PermissionId = 3, CreatedAt = _staticDate },
                new RolePermission { Id = 10, RoleId = 2, PermissionId = 5, CreatedAt = _staticDate }
            };

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
        }

        private static void SeedRefreshTokens(ModelBuilder modelBuilder)
        {
            var refreshTokens = new List<RefreshToken>
            {
                new RefreshToken { Id = Guid.Parse("11111111-aaaa-bbbb-cccc-111111111111"), UserId = 1, Token = "sample-refresh-token-admin", ExpiresAt = _refreshTokenExpiry, CreatedAt = _refreshTokenCreated },
                new RefreshToken { Id = Guid.Parse("22222222-aaaa-bbbb-cccc-222222222222"), UserId = 2, Token = "sample-refresh-token-pm1", ExpiresAt = _refreshTokenExpiry, CreatedAt = _refreshTokenCreated }
            };

            modelBuilder.Entity<RefreshToken>().HasData(refreshTokens);
        }



        //private static void SeedDeliveryUnits(ModelBuilder modelBuilder)
        //{
        //    var deliveryUnits = new List<DeliveryUnit>
        //    {
        //        new DeliveryUnit { Id = 1, Name = "Automotive", Code = "AUTO", Description = "Automotive Solutions", ManagerId = 2, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 2, Name = "Travel & Transportation", Code = "TNT", Description = "Travel & Transportation Solutions", ManagerId = 3, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 3, Name = "Construction Solutions", Code = "CONST", Description = "Construction Solutions", ManagerId = 2, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 4, Name = "Digital & Commerce Solutions", Code = "DCS", Description = "Digital & Commerce Solutions", ManagerId = 3, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 5, Name = "Retail & Warehouse Automation Solutions", Code = "RWA", Description = "Retail & Warehouse Automation Solutions", ManagerId = 2, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 6, Name = "Data & AI Solutions", Code = "DAI", Description = "Data & AI Solutions", ManagerId = 3, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 7, Name = "Experience Design Studio", Code = "EDS", Description = "Experience Design Studio", ManagerId = 10, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 8, Name = "Digital Technology Services", Code = "DTS", Description = "Digital Technology Services", ManagerId = 2, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 9, Name = "IOT & Embedded Solutions", Code = "IOT", Description = "IOT & Embedded Solutions", ManagerId = 3, IsActive = true, CreatedAt = _staticDate },
        //        new DeliveryUnit { Id = 10, Name = "VantX Financial Solutions", Code = "VFS", Description = "VantX Financial Solutions", ManagerId = 2, IsActive = true, CreatedAt = _staticDate }
        //    };

        //    modelBuilder.Entity<DeliveryUnit>().HasData(deliveryUnits);
        //}
        private static void SeedDeliveryUnits(ModelBuilder modelBuilder)
        {
            var deliveryUnits = new List<DeliveryUnit>
            {
                new DeliveryUnit { Id = 1, Name = "Automotive", Code = "AUTO", Description = "Automotive Solutions", DuHeadName = "Project Manager 1", DuHeadEmail = "pm1@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 2, Name = "Travel & Transportation", Code = "TNT", Description = "Travel & Transportation Solutions", DuHeadName = "Project Manager 2", DuHeadEmail = "pm2@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 3, Name = "Construction Solutions", Code = "CONST", Description = "Construction Solutions", DuHeadName = "Project Manager 1", DuHeadEmail = "pm1@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 4, Name = "Digital & Commerce Solutions", Code = "DCS", Description = "Digital & Commerce Solutions", DuHeadName = "Project Manager 2", DuHeadEmail = "pm2@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 5, Name = "Retail & Warehouse Automation Solutions", Code = "RWA", Description = "Retail & Warehouse Automation Solutions", DuHeadName = "Project Manager 1", DuHeadEmail = "pm1@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 6, Name = "Data & AI Solutions", Code = "DAI", Description = "Data & AI Solutions", DuHeadName = "Project Manager 2", DuHeadEmail = "pm2@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 7, Name = "Experience Design Studio", Code = "EDS", Description = "Experience Design Studio", DuHeadName = "Designer 1", DuHeadEmail = "designer1@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 8, Name = "Digital Technology Services", Code = "DTS", Description = "Digital Technology Services", DuHeadName = "Project Manager 1", DuHeadEmail = "pm1@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 9, Name = "IOT & Embedded Solutions", Code = "IOT", Description = "IOT & Embedded Solutions", DuHeadName = "Project Manager 2", DuHeadEmail = "pm2@company.com", IsActive = true, CreatedAt = _staticDate },
                new DeliveryUnit { Id = 10, Name = "VantX Financial Solutions", Code = "VFS", Description = "VantX Financial Solutions", DuHeadName = "Project Manager 1", DuHeadEmail = "pm1@company.com", IsActive = true, CreatedAt = _staticDate }
            };

            modelBuilder.Entity<DeliveryUnit>().HasData(deliveryUnits);
        }

        private static void SeedProjectStatuses(ModelBuilder modelBuilder)
        {
            var projectStatuses = new List<ProjectStatus>
            {
                new ProjectStatus { Id = 1, Name = "Active", Description = "Active project", CreatedAt = _staticDate },
                new ProjectStatus { Id = 2, Name = "Inactive", Description = "Inactive project", CreatedAt = _staticDate },
                new ProjectStatus { Id = 3, Name = "Completed", Description = "Completed project", CreatedAt = _staticDate }
            };

            modelBuilder.Entity<ProjectStatus>().HasData(projectStatuses);
        }

        private static void SeedProjectTemplates(ModelBuilder modelBuilder)
        {
            var templates = new List<ProjectTemplate>
            {
                new ProjectTemplate { Id = 1, Name = "Scrum" },
                new ProjectTemplate { Id = 2, Name = "Kanban" }
            };

            modelBuilder.Entity<ProjectTemplate>().HasData(templates);
        }

        private static void SeedStatuses(ModelBuilder modelBuilder)
        {
            var statuses = new List<Status>
            {
                new Status { Id = _statusId1, StatusName = "To Do" },
                new Status { Id = _statusId2, StatusName = "In Progress" },
                new Status { Id = _statusId3, StatusName = "In Review" },
                new Status { Id = _statusId4, StatusName = "Done" }
            };

            modelBuilder.Entity<Status>().HasData(statuses);
        }

        private static void SeedLabels(ModelBuilder modelBuilder)
        {
            var labels = new List<Label>
    {
        new Label { Id = 1, Name = "Bug", Colour = "#FF0000" },
        new Label { Id = 2, Name = "Feature", Colour = "#00FF00" },
        new Label { Id = 3, Name = "Improvement", Colour = "#0000FF" },
        new Label { Id = 4, Name = "Documentation", Colour = "#FFA500" }
    };

            modelBuilder.Entity<Label>().HasData(labels);
        }


        //private static void SeedProjects(ModelBuilder modelBuilder)
        //{
        //    var projects = new List<Project>
        //    {
        //        new Project { Id = _projectId1, Name = "Project 1", Key = "PROJ001", Description = "Description for Project 1", CustomerOrgName = "Customer Org 1", PocEmail = "customer1@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 1, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 1 },
        //        new Project { Id = _projectId2, Name = "Project 2", Key = "PROJ002", Description = "Description for Project 2", CustomerOrgName = "Customer Org 2", PocEmail = "customer2@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 2, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 2 },
        //        new Project { Id = _projectId3, Name = "Project 3", Key = "PROJ003", Description = "Description for Project 3", CustomerOrgName = "Customer Org 3", PocEmail = "customer3@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 3, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 1 },
        //        new Project { Id = _projectId4, Name = "Project 4", Key = "PROJ004", Description = "Description for Project 4", CustomerOrgName = "Customer Org 4", PocEmail = "customer4@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 4, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 2 },
        //        new Project { Id = _projectId5, Name = "Project 5", Key = "PROJ005", Description = "Description for Project 5", CustomerOrgName = "Customer Org 5", PocEmail = "customer5@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 5, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 1 },
        //        new Project { Id = _projectId6, Name = "Project 6", Key = "PROJ006", Description = "Description for Project 6", CustomerOrgName = "Customer Org 6", PocEmail = "customer6@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 6, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 2 },
        //        new Project { Id = _projectId7, Name = "Project 7", Key = "PROJ007", Description = "Description for Project 7", CustomerOrgName = "Customer Org 7", PocEmail = "customer7@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 7, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 1 },
        //        new Project { Id = _projectId8, Name = "Project 8", Key = "PROJ008", Description = "Description for Project 8", CustomerOrgName = "Customer Org 8", PocEmail = "customer8@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 2, DeliveryUnitId = 8, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 2 },
        //        new Project { Id = _projectId9, Name = "Project 9", Key = "PROJ009", Description = "Description for Project 9", CustomerOrgName = "Customer Org 9", PocEmail = "customer9@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 2, DeliveryUnitId = 9, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 1 },
        //        new Project { Id = _projectId10, Name = "Project 10", Key = "PROJ010", Description = "Description for Project 10", CustomerOrgName = "Customer Org 10", PocEmail = "customer10@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 3, DeliveryUnitId = 10, CreatedBy = 1, CreatedAt = _staticDate, TemplateId = 2 }
        //    };

        //    modelBuilder.Entity<Project>().HasData(projects);
        //}
        //private static void SeedProjects(ModelBuilder modelBuilder)
        //{
        //    var projects = new List<Project>
        //    {
        //        new Project { Id = _projectId1, Name = "Project 1", Key = "PROJ001", Description = "Description for Project 1", CustomerOrgName = "Customer Org 1", PocEmail = "customer1@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 1, CreatedBy = 1, CreatedAt = new DateTimeOffset(2024, 1, 5, 10, 30, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 1 },
        //        new Project { Id = _projectId2, Name = "Project 2", Key = "PROJ002", Description = "Description for Project 2", CustomerOrgName = "Customer Org 2", PocEmail = "customer2@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 2, CreatedBy = 1, CreatedAt = new DateTimeOffset(2024, 3, 15, 9, 0, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 2 },
        //        new Project { Id = _projectId3, Name = "Project 3", Key = "PROJ003", Description = "Description for Project 3", CustomerOrgName = "Customer Org 3", PocEmail = "customer3@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 3, CreatedBy = 1, CreatedAt = new DateTimeOffset(2023, 8, 12, 14, 45, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 1 },
        //        new Project { Id = _projectId4, Name = "Project 4", Key = "PROJ004", Description = "Description for Project 4", CustomerOrgName = "Customer Org 4", PocEmail = "customer4@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 4, CreatedBy = 1, CreatedAt = new DateTimeOffset(2025, 5, 1, 8, 15, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 2 },
        //        new Project { Id = _projectId5, Name = "Project 5", Key = "PROJ005", Description = "Description for Project 5", CustomerOrgName = "Customer Org 5", PocEmail = "customer5@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 5, CreatedBy = 1, CreatedAt = new DateTimeOffset(2022, 11, 20, 17, 0, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 1 },
        //        new Project { Id = _projectId6, Name = "Project 6", Key = "PROJ006", Description = "Description for Project 6", CustomerOrgName = "Customer Org 6", PocEmail = "customer6@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 6, CreatedBy = 1, CreatedAt = new DateTimeOffset(2021, 6, 10, 11, 45, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 2 },
        //        new Project { Id = _projectId7, Name = "Project 7", Key = "PROJ007", Description = "Description for Project 7", CustomerOrgName = "Customer Org 7", PocEmail = "customer7@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 7, CreatedBy = 1, CreatedAt = new DateTimeOffset(2020, 9, 18, 15, 20, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 1 },
        //        new Project { Id = _projectId8, Name = "Project 8", Key = "PROJ008", Description = "Description for Project 8", CustomerOrgName = "Customer Org 8", PocEmail = "customer8@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 2, DeliveryUnitId = 8, CreatedBy = 1, CreatedAt = new DateTimeOffset(2023, 2, 7, 13, 10, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 2 },
        //        new Project { Id = _projectId9, Name = "Project 9", Key = "PROJ009", Description = "Description for Project 9", CustomerOrgName = "Customer Org 9", PocEmail = "customer9@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 2, DeliveryUnitId = 9, CreatedBy = 1, CreatedAt = new DateTimeOffset(2021, 12, 29, 18, 0, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 1 },
        //        new Project { Id = _projectId10, Name = "Project 10", Key = "PROJ010", Description = "Description for Project 10", CustomerOrgName = "Customer Org 10", PocEmail = "customer10@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 3, DeliveryUnitId = 10, CreatedBy = 1, CreatedAt = new DateTimeOffset(2019, 4, 12, 10, 15, 0, TimeSpan.FromHours(5.5)).UtcDateTime, TemplateId = 2 }
        //    };

        //    modelBuilder.Entity<Project>().HasData(projects);
        //}
        private static void SeedProjects(ModelBuilder modelBuilder)
        {
            var projects = new List<Project>
    {
        new Project { Id = _projectId1, Name = "Project 1", Key = "PROJ001", Description = "Description for Project 1", CustomerOrgName = "Customer Org 1", PocEmail = "customer1@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 1, CreatedBy = 1, CreatedAt = _projectDate1, TemplateId = 1 },
        new Project { Id = _projectId2, Name = "Project 2", Key = "PROJ002", Description = "Description for Project 2", CustomerOrgName = "Customer Org 2", PocEmail = "customer2@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 2, CreatedBy = 1, CreatedAt = _projectDate2, TemplateId = 2 },
        new Project { Id = _projectId3, Name = "Project 3", Key = "PROJ003", Description = "Description for Project 3", CustomerOrgName = "Customer Org 3", PocEmail = "customer3@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 3, CreatedBy = 1, CreatedAt = _projectDate3, TemplateId = 1 },
        new Project { Id = _projectId4, Name = "Project 4", Key = "PROJ004", Description = "Description for Project 4", CustomerOrgName = "Customer Org 4", PocEmail = "customer4@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 4, CreatedBy = 1, CreatedAt = _projectDate4, TemplateId = 2 },
        new Project { Id = _projectId5, Name = "Project 5", Key = "PROJ005", Description = "Description for Project 5", CustomerOrgName = "Customer Org 5", PocEmail = "customer5@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 5, CreatedBy = 1, CreatedAt = _projectDate5, TemplateId = 1 },
        new Project { Id = _projectId6, Name = "Project 6", Key = "PROJ006", Description = "Description for Project 6", CustomerOrgName = "Customer Org 6", PocEmail = "customer6@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 6, CreatedBy = 1, CreatedAt = _projectDate6, TemplateId = 2 },
        new Project { Id = _projectId7, Name = "Project 7", Key = "PROJ007", Description = "Description for Project 7", CustomerOrgName = "Customer Org 7", PocEmail = "customer7@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 1, DeliveryUnitId = 7, CreatedBy = 1, CreatedAt = _projectDate7, TemplateId = 1 },
        new Project { Id = _projectId8, Name = "Project 8", Key = "PROJ008", Description = "Description for Project 8", CustomerOrgName = "Customer Org 8", PocEmail = "customer8@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 2, DeliveryUnitId = 8, CreatedBy = 1, CreatedAt = _projectDate8, TemplateId = 2 },
        new Project { Id = _projectId9, Name = "Project 9", Key = "PROJ009", Description = "Description for Project 9", CustomerOrgName = "Customer Org 9", PocEmail = "customer9@example.com", ProjectManagerId = 2, ProjectManagerRoleId = 2, StatusId = 2, DeliveryUnitId = 9, CreatedBy = 1, CreatedAt = _projectDate9, TemplateId = 1 },
        new Project { Id = _projectId10, Name = "Project 10", Key = "PROJ010", Description = "Description for Project 10", CustomerOrgName = "Customer Org 10", PocEmail = "customer10@example.com", ProjectManagerId = 3, ProjectManagerRoleId = 2, StatusId = 3, DeliveryUnitId = 10, CreatedBy = 1, CreatedAt = _projectDate10, TemplateId = 2 }
    };

            modelBuilder.Entity<Project>().HasData(projects);
        }




        private static void SeedCustomFields(ModelBuilder modelBuilder)
        {
            var customFields = new List<CustomField>
            {
                // Project 1 Custom Fields
                new CustomField { Id = _customFieldId1, ProjectId = _projectId1, Name = "Budget", Value = "$500,000" },
                new CustomField { Id = _customFieldId2, ProjectId = _projectId1, Name = "Client Priority", Value = "High" },
                new CustomField { Id = _customFieldId3, ProjectId = _projectId1, Name = "Contract Type", Value = "Fixed Price" },

                // Project 2 Custom Fields
                new CustomField { Id = _customFieldId4, ProjectId = _projectId2, Name = "Budget", Value = "$750,000" },
                new CustomField { Id = _customFieldId5, ProjectId = _projectId2, Name = "Client Priority", Value = "Medium" },

                // Project 3 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000054"), ProjectId = _projectId3, Name = "Budget", Value = "$300,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000055"), ProjectId = _projectId3, Name = "Technology Stack", Value = ".NET Core, React" },

                // Project 4 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000056"), ProjectId = _projectId4, Name = "Budget", Value = "$1,000,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000057"), ProjectId = _projectId4, Name = "Compliance Required", Value = "GDPR, HIPAA" },

                // Project 5 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000058"), ProjectId = _projectId5, Name = "Budget", Value = "$450,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000059"), ProjectId = _projectId5, Name = "Client Priority", Value = "Critical" },

                // Project 6 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000060"), ProjectId = _projectId6, Name = "Budget", Value = "$600,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000061"), ProjectId = _projectId6, Name = "Region", Value = "APAC" },

                // Project 7 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000062"), ProjectId = _projectId7, Name = "Budget", Value = "$850,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000063"), ProjectId = _projectId7, Name = "Industry", Value = "Healthcare" },

                // Project 8 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000064"), ProjectId = _projectId8, Name = "Budget", Value = "$400,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000065"), ProjectId = _projectId8, Name = "Risk Level", Value = "Low" },

                // Project 9 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000066"), ProjectId = _projectId9, Name = "Budget", Value = "$950,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000067"), ProjectId = _projectId9, Name = "Offshore Team", Value = "Yes" },

                // Project 10 Custom Fields
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000068"), ProjectId = _projectId10, Name = "Budget", Value = "$700,000" },
                new CustomField { Id = new Guid("00000000-0000-0000-0000-000000000069"), ProjectId = _projectId10, Name = "Maintenance Period", Value = "12 months" }
            };

            modelBuilder.Entity<CustomField>().HasData(customFields);
        }

        //private static void SeedTeams(ModelBuilder modelBuilder)
        //{
        //    var teams = new List<Team>
        //    {
        //        new Team { Id = 1, ProjectId = _projectId1, Name = "Team 1", Description = "Development team for Project 1", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 2, ProjectId = _projectId2, Name = "Team 2", Description = "Development team for Project 2", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 3, ProjectId = _projectId3, Name = "Team 3", Description = "Development team for Project 3", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 4, ProjectId = _projectId4, Name = "Team 4", Description = "Development team for Project 4", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 5, ProjectId = _projectId5, Name = "Team 5", Description = "Development team for Project 5", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 6, ProjectId = _projectId6, Name = "Team 6", Description = "Development team for Project 6", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 7, ProjectId = _projectId7, Name = "Team 7", Description = "Development team for Project 7", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 8, ProjectId = _projectId8, Name = "Team 8", Description = "Development team for Project 8", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 9, ProjectId = _projectId9, Name = "Team 9", Description = "Development team for Project 9", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
        //        new Team { Id = 10, ProjectId = _projectId10, Name = "Team 10", Description = "Development team for Project 10", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate }
        //    };

        //    modelBuilder.Entity<Team>().HasData(teams);
        //}
        private static void SeedTeams(ModelBuilder modelBuilder)
        {
            var teams = new List<Team>
            {
                new Team { Id = 1, ProjectId = _projectId1, Name = "Team 1", Description = "Development team for Project 1", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Backend", "API" } },
                new Team { Id = 2, ProjectId = _projectId2, Name = "Team 2", Description = "Development team for Project 2", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Frontend", "UI" } },
                new Team { Id = 3, ProjectId = _projectId3, Name = "Team 3", Description = "Development team for Project 3", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Integration", "DevOps" } },
                new Team { Id = 4, ProjectId = _projectId4, Name = "Team 4", Description = "Development team for Project 4", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Testing", "QA" } },
                new Team { Id = 5, ProjectId = _projectId5, Name = "Team 5", Description = "Development team for Project 5", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Mobile", "Flutter" } },
                new Team { Id = 6, ProjectId = _projectId6, Name = "Team 6", Description = "Development team for Project 6", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "AI", "ML" } },
                new Team { Id = 7, ProjectId = _projectId7, Name = "Team 7", Description = "Development team for Project 7", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Security", "Infra" } },
                new Team { Id = 8, ProjectId = _projectId8, Name = "Team 8", Description = "Development team for Project 8", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Data", "ETL" } },
                new Team { Id = 9, ProjectId = _projectId9, Name = "Team 9", Description = "Development team for Project 9", LeadId = 4, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Web", "React" } },
                new Team { Id = 10, ProjectId = _projectId10, Name = "Team 10", Description = "Development team for Project 10", LeadId = 5, IsActive = true, CreatedBy = 1, CreatedAt = _staticDate, Label = new List<string> { "Cloud", "AWS" } }
            };

            modelBuilder.Entity<Team>().HasData(teams);
        }


        private static void SeedBoards(ModelBuilder modelBuilder)
        {
            var boards = new List<Board>
            {
                new Board { Id = 1, ProjectId = _projectId1, TeamId = 1, Name = "Board 1", Description = "Main board for Project 1", Type = "scrum", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 2, ProjectId = _projectId2, TeamId = 2, Name = "Board 2", Description = "Main board for Project 2", Type = "kanban", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 3, ProjectId = _projectId3, TeamId = 3, Name = "Board 3", Description = "Main board for Project 3", Type = "scrum", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 4, ProjectId = _projectId4, TeamId = 4, Name = "Board 4", Description = "Main board for Project 4", Type = "kanban", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 5, ProjectId = _projectId5, TeamId = 5, Name = "Board 5", Description = "Main board for Project 5", Type = "scrum", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 6, ProjectId = _projectId6, TeamId = 6, Name = "Board 6", Description = "Main board for Project 6", Type = "kanban", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 7, ProjectId = _projectId7, TeamId = 7, Name = "Board 7", Description = "Main board for Project 7", Type = "scrum", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 8, ProjectId = _projectId8, TeamId = 8, Name = "Board 8", Description = "Main board for Project 8", Type = "kanban", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 9, ProjectId = _projectId9, TeamId = 9, Name = "Board 9", Description = "Main board for Project 9", Type = "scrum", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate },
                new Board { Id = 10, ProjectId = _projectId10, TeamId = 10, Name = "Board 10", Description = "Main board for Project 10", Type = "kanban", IsActive = true, CreatedBy = 1, CreatedAt = _staticDate }
            };

            modelBuilder.Entity<Board>().HasData(boards);
        }

        //private static void SeedProjectMembers(ModelBuilder modelBuilder)
        //{
        //    var projectMembers = new List<ProjectMember>
        //    {
        //        // Project 1 Members
        //        new ProjectMember { Id = 1, ProjectId = _projectId1, TeamId = 1, UserId = 2, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 2, ProjectId = _projectId1, TeamId = 1, UserId = 4, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 3, ProjectId = _projectId1, TeamId = 1, UserId = 5, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 4, ProjectId = _projectId1, TeamId = 1, UserId = 8, RoleId = 5, ProjectRole = "QA Engineer", AddedBy = 1, AddedAt = _staticDate },

        //        // Project 2 Members
        //        new ProjectMember { Id = 5, ProjectId = _projectId2, TeamId = 2, UserId = 3, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 6, ProjectId = _projectId2, TeamId = 2, UserId = 4, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 7, ProjectId = _projectId2, TeamId = 2, UserId = 5, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 8, ProjectId = _projectId2, TeamId = 2, UserId = 8, RoleId = 5, ProjectRole = "QA Engineer", AddedBy = 1, AddedAt = _staticDate },

        //        // Continue for other projects...
        //        new ProjectMember { Id = 9, ProjectId = _projectId3, TeamId = 3, UserId = 2, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 10, ProjectId = _projectId3, TeamId = 3, UserId = 4, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 11, ProjectId = _projectId4, TeamId = 4, UserId = 3, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 12, ProjectId = _projectId4, TeamId = 4, UserId = 5, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 13, ProjectId = _projectId5, TeamId = 5, UserId = 2, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 14, ProjectId = _projectId5, TeamId = 5, UserId = 4, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 15, ProjectId = _projectId6, TeamId = 6, UserId = 3, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 16, ProjectId = _projectId6, TeamId = 6, UserId = 5, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 17, ProjectId = _projectId7, TeamId = 7, UserId = 2, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 18, ProjectId = _projectId7, TeamId = 7, UserId = 4, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 19, ProjectId = _projectId8, TeamId = 8, UserId = 3, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 20, ProjectId = _projectId8, TeamId = 8, UserId = 5, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 21, ProjectId = _projectId9, TeamId = 9, UserId = 2, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 22, ProjectId = _projectId9, TeamId = 9, UserId = 4, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 23, ProjectId = _projectId10, TeamId = 10, UserId = 3, RoleId = 2, ProjectRole = "Project Manager", IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
        //        new ProjectMember { Id = 24, ProjectId = _projectId10, TeamId = 10, UserId = 5, RoleId = 4, ProjectRole = "Developer", AddedBy = 1, AddedAt = _staticDate }
        //    };

        //    modelBuilder.Entity<ProjectMember>().HasData(projectMembers);
        //}

        private static void SeedProjectMembers(ModelBuilder modelBuilder)
        {
            var projectMembers = new List<ProjectMember>
            {
                // Project 1 Members
                new ProjectMember { Id = 1, ProjectId = _projectId1, UserId = 2, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 2, ProjectId = _projectId1, UserId = 4, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 3, ProjectId = _projectId1, UserId = 5, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 4, ProjectId = _projectId1, UserId = 8, RoleId = 5, AddedBy = 1, AddedAt = _staticDate },

                // Project 2 Members
                new ProjectMember { Id = 5, ProjectId = _projectId2, UserId = 3, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 6, ProjectId = _projectId2, UserId = 4, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 7, ProjectId = _projectId2, UserId = 5, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 8, ProjectId = _projectId2, UserId = 8, RoleId = 5, AddedBy = 1, AddedAt = _staticDate },

                // Project 3 Members
                new ProjectMember { Id = 9, ProjectId = _projectId3, UserId = 2, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 10, ProjectId = _projectId3, UserId = 4, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 4 Members
                new ProjectMember { Id = 11, ProjectId = _projectId4, UserId = 3, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 12, ProjectId = _projectId4, UserId = 5, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 5 Members
                new ProjectMember { Id = 13, ProjectId = _projectId5, UserId = 2, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 14, ProjectId = _projectId5, UserId = 4, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 6 Members
                new ProjectMember { Id = 15, ProjectId = _projectId6, UserId = 3, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 16, ProjectId = _projectId6, UserId = 5, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 7 Members
                new ProjectMember { Id = 17, ProjectId = _projectId7, UserId = 2, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 18, ProjectId = _projectId7, UserId = 4, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 8 Members
                new ProjectMember { Id = 19, ProjectId = _projectId8, UserId = 3, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 20, ProjectId = _projectId8, UserId = 5, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 9 Members
                new ProjectMember { Id = 21, ProjectId = _projectId9, UserId = 2, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 22, ProjectId = _projectId9, UserId = 4, RoleId = 4, AddedBy = 1, AddedAt = _staticDate },

                // Project 10 Members
                new ProjectMember { Id = 23, ProjectId = _projectId10, UserId = 3, RoleId = 2, IsOwner = true, AddedBy = 1, AddedAt = _staticDate },
                new ProjectMember { Id = 24, ProjectId = _projectId10, UserId = 5, RoleId = 4, AddedBy = 1, AddedAt = _staticDate }
            };

            modelBuilder.Entity<ProjectMember>().HasData(projectMembers);
        }


        private static void SeedTeamMembers(ModelBuilder modelBuilder)
        {
            var teamMembers = new List<TeamMember>
            {
                new TeamMember { TeamMemberId = 1, TeamId = 1, ProjectMemberId = 2, CreatedAt = _staticDate },
                new TeamMember { TeamMemberId = 2, TeamId = 1, ProjectMemberId = 3, CreatedAt = _staticDate },
                new TeamMember { TeamMemberId = 3, TeamId = 1, ProjectMemberId = 4, CreatedAt = _staticDate },

                new TeamMember { TeamMemberId = 4, TeamId = 2, ProjectMemberId = 6, CreatedAt = _staticDate },
                new TeamMember { TeamMemberId = 5, TeamId = 2, ProjectMemberId = 7, CreatedAt = _staticDate },
                new TeamMember { TeamMemberId = 6, TeamId = 2, ProjectMemberId = 8, CreatedAt = _staticDate }
            };

            modelBuilder.Entity<TeamMember>().HasData(teamMembers);
        }

        private static void SeedStarredProjects(ModelBuilder modelBuilder)
        {
            var starredProjects = new List<StarredProjects>
            {
                new StarredProjects { Id = 1, ProjectId = _projectId1, UserId = 2, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 2, ProjectId = _projectId2, UserId = 3, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 3, ProjectId = _projectId3, UserId = 4, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 4, ProjectId = _projectId4, UserId = 5, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 5, ProjectId = _projectId5, UserId = 2, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 6, ProjectId = _projectId6, UserId = 3, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 7, ProjectId = _projectId7, UserId = 4, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 8, ProjectId = _projectId8, UserId = 5, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 9, ProjectId = _projectId9, UserId = 2, CreatedAt = _staticDateOffset },
                new StarredProjects { Id = 10, ProjectId = _projectId10, UserId = 3, CreatedAt = _staticDateOffset }
            };

            modelBuilder.Entity<StarredProjects>().HasData(starredProjects);
        }




        //private static void SeedBoardColumns(ModelBuilder modelBuilder)
        //{
        //    var boardColumns = new List<BoardColumn>
        //    {
        //        new BoardColumn { Id = _boardColumnId1, StatusId = _statusId1, BoardName = "To Do", BoardColor = "#FF6B6B", Position = 1 },
        //        new BoardColumn { Id = _boardColumnId2, StatusId = _statusId2, BoardName = "In Progress", BoardColor = "#4ECDC4", Position = 2 },
        //        new BoardColumn { Id = _boardColumnId3, StatusId = _statusId3, BoardName = "In Review", BoardColor = "#45B7D1", Position = 3 },
        //        new BoardColumn { Id = _boardColumnId4, StatusId = _statusId4, BoardName = "Done", BoardColor = "#96CEB4", Position = 4 }
        //    };

        //    modelBuilder.Entity<BoardColumn>().HasData(boardColumns);
        //}
        private static void SeedBoardColumns(ModelBuilder modelBuilder)
        {
            var boardColumns = new List<BoardColumn>
    {
        new BoardColumn { Id = _boardColumnId1, StatusId = 1, BoardColumnName = "To Do", BoardColor = "#FF6B6B", Position = 1 },
        new BoardColumn { Id = _boardColumnId2, StatusId = 2, BoardColumnName = "In Progress", BoardColor = "#4ECDC4", Position = 2 },
        new BoardColumn { Id = _boardColumnId3, StatusId = 3, BoardColumnName = "In Review", BoardColor = "#45B7D1", Position = 3 },
        new BoardColumn { Id = _boardColumnId4, StatusId = 4, BoardColumnName = "Done", BoardColor = "#96CEB4", Position = 4 }
    };
            modelBuilder.Entity<BoardColumn>().HasData(boardColumns);
        }

        private static void SeedBoardBoardColumnMaps(ModelBuilder modelBuilder)
        {
            var mappings = new List<BoardBoardColumnMap>
    {
        new BoardBoardColumnMap { Id = 1, BoardId = 1, BoardColumnId = _boardColumnId1, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 2, BoardId = 1, BoardColumnId = _boardColumnId2, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 3, BoardId = 1, BoardColumnId = _boardColumnId3, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 4, BoardId = 1, BoardColumnId = _boardColumnId4, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 5, BoardId = 2, BoardColumnId = _boardColumnId1, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 6, BoardId = 2, BoardColumnId = _boardColumnId2, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 7, BoardId = 2, BoardColumnId = _boardColumnId3, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 8, BoardId = 2, BoardColumnId = _boardColumnId4, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 9, BoardId = 3, BoardColumnId = _boardColumnId1, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 10, BoardId = 3, BoardColumnId = _boardColumnId2, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 11, BoardId = 3, BoardColumnId = _boardColumnId3, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 12, BoardId = 3, BoardColumnId = _boardColumnId4, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 13, BoardId = 4, BoardColumnId = _boardColumnId1, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 14, BoardId = 4, BoardColumnId = _boardColumnId2, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 15, BoardId = 4, BoardColumnId = _boardColumnId3, CreatedAt = _staticDate, UpdatedAt = _staticDate },
        new BoardBoardColumnMap { Id = 16, BoardId = 4, BoardColumnId = _boardColumnId4, CreatedAt = _staticDate, UpdatedAt = _staticDate }
    };
            modelBuilder.Entity<BoardBoardColumnMap>().HasData(mappings);
        }


        //private static void SeedChannels(ModelBuilder modelBuilder)
        //{
        //    var channels = new List<Channel>
        //    {
        //        new Channel { Id = _channelId1, TeamId = 1, },
        //        new Channel { Id = _channelId2, TeamId = 2 },
        //        new Channel { Id = _channelId3, TeamId = 3 },
        //        new Channel { Id = _channelId4, TeamId = 4 },
        //        new Channel { Id = _channelId5, TeamId = 5 },
        //        new Channel { Id = _channelId6, TeamId = 6 },
        //        new Channel { Id = _channelId7, TeamId = 7 },
        //        new Channel { Id = _channelId8, TeamId = 8 },
        //        new Channel { Id = _channelId9, TeamId = 9 },
        //        new Channel { Id = _channelId10, TeamId = 10 }
        //    };

        //    modelBuilder.Entity<Channel>().HasData(channels);
        //}
        private static void SeedChannels(ModelBuilder modelBuilder)
        {
            var channels = new List<Channel>
    {
        new Channel { Id = _channelId1, TeamId = 1, Name = "General" },
        new Channel { Id = _channelId2, TeamId = 2, Name = "Development Updates" },
        new Channel { Id = _channelId3, TeamId = 3, Name = "UI-UX Discussions" },
        new Channel { Id = _channelId4, TeamId = 4, Name = "Testing Reports" },
        new Channel { Id = _channelId5, TeamId = 5, Name = "QA Coordination" },
        new Channel { Id = _channelId6, TeamId = 6, Name = "Client Support" },
        new Channel { Id = _channelId7, TeamId = 7, Name = "Ops Daily Standup" },
        new Channel { Id = _channelId8, TeamId = 8, Name = "Research & Insights" },
        new Channel { Id = _channelId9, TeamId = 9, Name = "Marketing Campaigns" },
        new Channel { Id = _channelId10, TeamId = 10, Name = "Finance Planning" }
    };

            modelBuilder.Entity<Channel>().HasData(channels);
        }


        //private static void SeedSprints(ModelBuilder modelBuilder)
        //{
        //    var sprints = new List<Sprint>
        //    {
        //        new Sprint { Id = _sprintId1, ProjectId = _projectId1, Name = "Sprint 1.1", SprintGoal = "Goal for Sprint 1.1", StartDate = _staticDate, DueDate = _staticDate.AddDays(14), Status = "ACTIVE", StoryPoint = 20, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Sprint { Id = _sprintId2, ProjectId = _projectId3, Name = "Sprint 3.1", SprintGoal = "Goal for Sprint 3.1", StartDate = _staticDate, DueDate = _staticDate.AddDays(14), Status = "ACTIVE", StoryPoint = 20, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Sprint { Id = _sprintId3, ProjectId = _projectId5, Name = "Sprint 5.1", SprintGoal = "Goal for Sprint 5.1", StartDate = _staticDate, DueDate = _staticDate.AddDays(14), Status = "ACTIVE", StoryPoint = 20, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Sprint { Id = _sprintId4, ProjectId = _projectId7, Name = "Sprint 7.1", SprintGoal = "Goal for Sprint 7.1", StartDate = _staticDate, DueDate = _staticDate.AddDays(14), Status = "ACTIVE", StoryPoint = 20, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Sprint { Id = _sprintId5, ProjectId = _projectId9, Name = "Sprint 9.1", SprintGoal = "Goal for Sprint 9.1", StartDate = _staticDate, DueDate = _staticDate.AddDays(14), Status = "ACTIVE", StoryPoint = 20, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
        //    };

        //    modelBuilder.Entity<Sprint>().HasData(sprints);
        //}
        private static void SeedSprints(ModelBuilder modelBuilder)
        {
            var sprints = new List<Sprint>
            {
                new Sprint { Id = _sprintId1, ProjectId = _projectId1, TeamId = 1, Name = "Sprint 1 - Kickoff", SprintGoal = "Setup project structure and CI/CD", StartDate = _sprintStartDate1, DueDate = _sprintDueDate1, Status = "ACTIVE", StoryPoint = 30, CreatedBy = 1, CreatedAt = _staticDateOffset },
                new Sprint { Id = _sprintId2, ProjectId = _projectId1, TeamId = 1, Name = "Sprint 2 - Core Features", SprintGoal = "Implement authentication and dashboard", StartDate = _sprintStartDate2, DueDate = _sprintDueDate2, Status = "PLANNED", StoryPoint = 35, CreatedBy = 1, CreatedAt = _staticDateOffset },
                new Sprint { Id = _sprintId3, ProjectId = _projectId2, TeamId = 2, Name = "Sprint 1 - Discovery", SprintGoal = "Gather requirements and create wireframes", StartDate = _sprintStartDate3, DueDate = _sprintDueDate3, Status = "COMPLETED", StoryPoint = 25, CreatedBy = 1, CreatedAt = _staticDateOffset }
            };

            modelBuilder.Entity<Sprint>().HasData(sprints);
        }



        //private static void SeedEpics(ModelBuilder modelBuilder)
        //{
        //    var epics = new List<Epic>
        //    {
        //        new Epic { Id = _epicId1, ProjectId = _projectId1, Title = "Epic 1: Core Features", Description = "Core functionality for Project 1", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId2, ProjectId = _projectId2, Title = "Epic 2: Core Features", Description = "Core functionality for Project 2", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId3, ProjectId = _projectId3, Title = "Epic 3: Core Features", Description = "Core functionality for Project 3", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId4, ProjectId = _projectId4, Title = "Epic 4: Core Features", Description = "Core functionality for Project 4", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId5, ProjectId = _projectId5, Title = "Epic 5: Core Features", Description = "Core functionality for Project 5", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId6, ProjectId = _projectId6, Title = "Epic 6: Core Features", Description = "Core functionality for Project 6", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId7, ProjectId = _projectId7, Title = "Epic 7: Core Features", Description = "Core functionality for Project 7", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId8, ProjectId = _projectId8, Title = "Epic 8: Core Features", Description = "Core functionality for Project 8", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId9, ProjectId = _projectId9, Title = "Epic 9: Core Features", Description = "Core functionality for Project 9", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Epic { Id = _epicId10, ProjectId = _projectId10, Title = "Epic 10: Core Features", Description = "Core functionality for Project 10", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
        //    };

        //    modelBuilder.Entity<Epic>().HasData(epics);
        //}
        private static void SeedEpics(ModelBuilder modelBuilder)
        {
            var epics = new List<Epic>
    {
        new Epic { Id = _epicId1, ProjectId = _projectId1, Title = "Epic 1: Core Features", Description = "Core functionality for Project 1", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Backend", "Core" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId2, ProjectId = _projectId2, Title = "Epic 2: API Integration", Description = "Build and test APIs for Project 2", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "API", "Integration" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId3, ProjectId = _projectId3, Title = "Epic 3: UI Components", Description = "Design core UI for Project 3", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Frontend", "UI" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId4, ProjectId = _projectId4, Title = "Epic 4: Database Setup", Description = "Setup initial schema and relations", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Database", "Migration" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId5, ProjectId = _projectId5, Title = "Epic 5: Authentication", Description = "Implement login & roles", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Auth", "Security" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId6, ProjectId = _projectId6, Title = "Epic 6: Notifications", Description = "Push/email notifications", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Alerts", "UX" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId7, ProjectId = _projectId7, Title = "Epic 7: Performance Optimization", Description = "Optimize DB and API response", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Performance", "Backend" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId8, ProjectId = _projectId8, Title = "Epic 8: Reporting Dashboard", Description = "Build analytics and reporting UI", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Reports", "Analytics" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId9, ProjectId = _projectId9, Title = "Epic 9: Data Sync", Description = "Sync data with external systems", AssigneeId = 2, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "Sync", "Integration" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Epic { Id = _epicId10, ProjectId = _projectId10, Title = "Epic 10: Final QA", Description = "Run pre-release QA tests", AssigneeId = 3, ReporterId = 1, CreatedBy = 1, Labels = new List<string> { "QA", "Testing" }, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
    };

            modelBuilder.Entity<Epic>().HasData(epics);
        }


        //private static void SeedIssues(ModelBuilder modelBuilder)
        //{
        //    var issues = new List<Issue>
        //    {
        //        // Project 1 Issues (Scrum - with sprints)
        //        new Issue { Id = _issueId1, Key = "PROJ001-1", ProjectId = _projectId1, EpicId = _epicId1, SprintId = _sprintId1, Summary = "Issue 1 for Project 1", Title = "Task 1", Description = "Description for issue 1 in project 1", Type = "STORY", Priority = "HIGH", Status = "TODO", AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId2, Key = "PROJ001-2", ProjectId = _projectId1, EpicId = _epicId1, SprintId = _sprintId1, Summary = "Issue 2 for Project 1", Title = "Task 2", Description = "Description for issue 2 in project 1", Type = "TASK", Priority = "MEDIUM", Status = "IN_PROGRESS", AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId3, Key = "PROJ001-3", ProjectId = _projectId1, EpicId = _epicId1, SprintId = _sprintId1, Summary = "Issue 3 for Project 1", Title = "Task 3", Description = "Description for issue 3 in project 1", Type = "BUG", Priority = "MEDIUM", Status = "DONE", AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        //        // Project 2 Issues (Kanban - no sprints)
        //        new Issue { Id = _issueId4, Key = "PROJ002-1", ProjectId = _projectId2, EpicId = _epicId2, SprintId = null, Summary = "Issue 1 for Project 2", Title = "Task 1", Description = "Description for issue 1 in project 2", Type = "STORY", Priority = "HIGH", Status = "TODO", AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId5, Key = "PROJ002-2", ProjectId = _projectId2, EpicId = _epicId2, SprintId = null, Summary = "Issue 2 for Project 2", Title = "Task 2", Description = "Description for issue 2 in project 2", Type = "TASK", Priority = "MEDIUM", Status = "IN_PROGRESS", AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId6, Key = "PROJ002-3", ProjectId = _projectId2, EpicId = _epicId2, SprintId = null, Summary = "Issue 3 for Project 2", Title = "Task 3", Description = "Description for issue 3 in project 2", Type = "BUG", Priority = "MEDIUM", Status = "DONE", AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        //        // Continue for other projects...
        //        new Issue { Id = _issueId7, Key = "PROJ003-1", ProjectId = _projectId3, EpicId = _epicId3, SprintId = _sprintId2, Summary = "Issue 1 for Project 3", Title = "Task 1", Description = "Description for issue 1 in project 3", Type = "STORY", Priority = "HIGH", Status = "TODO", AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId8, Key = "PROJ003-2", ProjectId = _projectId3, EpicId = _epicId3, SprintId = _sprintId2, Summary = "Issue 2 for Project 3", Title = "Task 2", Description = "Description for issue 2 in project 3", Type = "TASK", Priority = "MEDIUM", Status = "IN_PROGRESS", AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId9, Key = "PROJ003-3", ProjectId = _projectId3, EpicId = _epicId3, SprintId = _sprintId2, Summary = "Issue 3 for Project 3", Title = "Task 3", Description = "Description for issue 3 in project 3", Type = "BUG", Priority = "MEDIUM", Status = "DONE", AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        //        new Issue { Id = _issueId10, Key = "PROJ004-1", ProjectId = _projectId4, EpicId = _epicId4, SprintId = null, Summary = "Issue 1 for Project 4", Title = "Task 1", Description = "Description for issue 1 in project 4", Type = "STORY", Priority = "HIGH", Status = "TODO", AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId11, Key = "PROJ004-2", ProjectId = _projectId4, EpicId = _epicId4, SprintId = null, Summary = "Issue 2 for Project 4", Title = "Task 2", Description = "Description for issue 2 in project 4", Type = "TASK", Priority = "MEDIUM", Status = "IN_PROGRESS", AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId12, Key = "PROJ004-3", ProjectId = _projectId4, EpicId = _epicId4, SprintId = null, Summary = "Issue 3 for Project 4", Title = "Task 3", Description = "Description for issue 3 in project 4", Type = "BUG", Priority = "MEDIUM", Status = "DONE", AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        //        new Issue { Id = _issueId13, Key = "PROJ005-1", ProjectId = _projectId5, EpicId = _epicId5, SprintId = _sprintId3, Summary = "Issue 1 for Project 5", Title = "Task 1", Description = "Description for issue 1 in project 5", Type = "STORY", Priority = "HIGH", Status = "TODO", AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId14, Key = "PROJ005-2", ProjectId = _projectId5, EpicId = _epicId5, SprintId = _sprintId3, Summary = "Issue 2 for Project 5", Title = "Task 2", Description = "Description for issue 2 in project 5", Type = "TASK", Priority = "MEDIUM", Status = "IN_PROGRESS", AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        //        new Issue { Id = _issueId15, Key = "PROJ005-3", ProjectId = _projectId5, EpicId = _epicId5, SprintId = _sprintId3, Summary = "Issue 3 for Project 5", Title = "Task 3", Description = "Description for issue 3 in project 5", Type = "BUG", Priority = "MEDIUM", Status = "DONE", AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
        //    };

        //    modelBuilder.Entity<Issue>().HasData(issues);
        //}
        private static void SeedIssues(ModelBuilder modelBuilder)
        {
            var issues = new List<Issue>
    {
        // Project 1 Issues (Scrum - with sprints)
        new Issue { Id = _issueId1, Key = "PROJ001-1", ProjectId = _projectId1, EpicId = _epicId1, SprintId = _sprintId1, Summary = "Issue 1 for Project 1", Title = "Task 1", Description = "Description for issue 1 in project 1", Type = "STORY", Priority = "HIGH", StatusId = _statusId1, AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId2, Key = "PROJ001-2", ProjectId = _projectId1, EpicId = _epicId1, SprintId = _sprintId1, Summary = "Issue 2 for Project 1", Title = "Task 2", Description = "Description for issue 2 in project 1", Type = "TASK", Priority = "MEDIUM", StatusId = _statusId2, AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId3, Key = "PROJ001-3", ProjectId = _projectId1, EpicId = _epicId1, SprintId = _sprintId1, Summary = "Issue 3 for Project 1", Title = "Task 3", Description = "Description for issue 3 in project 1", Type = "BUG", Priority = "MEDIUM", StatusId = _statusId4, AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        // Project 2 Issues (Kanban - no sprints)
        new Issue { Id = _issueId4, Key = "PROJ002-1", ProjectId = _projectId2, EpicId = _epicId2, SprintId = null, Summary = "Issue 1 for Project 2", Title = "Task 1", Description = "Description for issue 1 in project 2", Type = "STORY", Priority = "HIGH", StatusId = _statusId1, AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId5, Key = "PROJ002-2", ProjectId = _projectId2, EpicId = _epicId2, SprintId = null, Summary = "Issue 2 for Project 2", Title = "Task 2", Description = "Description for issue 2 in project 2", Type = "TASK", Priority = "MEDIUM", StatusId = _statusId2, AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId6, Key = "PROJ002-3", ProjectId = _projectId2, EpicId = _epicId2, SprintId = null, Summary = "Issue 3 for Project 2", Title = "Task 3", Description = "Description for issue 3 in project 2", Type = "BUG", Priority = "MEDIUM", StatusId = _statusId4, AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        // Project 3 Issues (Scrum)
        new Issue { Id = _issueId7, Key = "PROJ003-1", ProjectId = _projectId3, EpicId = _epicId3, SprintId = _sprintId2, Summary = "Issue 1 for Project 3", Title = "Task 1", Description = "Description for issue 1 in project 3", Type = "STORY", Priority = "HIGH", StatusId = _statusId1, AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId8, Key = "PROJ003-2", ProjectId = _projectId3, EpicId = _epicId3, SprintId = _sprintId2, Summary = "Issue 2 for Project 3", Title = "Task 2", Description = "Description for issue 2 in project 3", Type = "TASK", Priority = "MEDIUM", StatusId = _statusId2, AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId9, Key = "PROJ003-3", ProjectId = _projectId3, EpicId = _epicId3, SprintId = _sprintId2, Summary = "Issue 3 for Project 3", Title = "Task 3", Description = "Description for issue 3 in project 3", Type = "BUG", Priority = "MEDIUM", StatusId = _statusId4, AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        // Project 4 Issues (Kanban)
        new Issue { Id = _issueId10, Key = "PROJ004-1", ProjectId = _projectId4, EpicId = _epicId4, SprintId = null, Summary = "Issue 1 for Project 4", Title = "Task 1", Description = "Description for issue 1 in project 4", Type = "STORY", Priority = "HIGH", StatusId = _statusId1, AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId11, Key = "PROJ004-2", ProjectId = _projectId4, EpicId = _epicId4, SprintId = null, Summary = "Issue 2 for Project 4", Title = "Task 2", Description = "Description for issue 2 in project 4", Type = "TASK", Priority = "MEDIUM", StatusId = _statusId2, AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId12, Key = "PROJ004-3", ProjectId = _projectId4, EpicId = _epicId4, SprintId = null, Summary = "Issue 3 for Project 4", Title = "Task 3", Description = "Description for issue 3 in project 4", Type = "BUG", Priority = "MEDIUM", StatusId = _statusId4, AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },

        // Project 5 Issues (Scrum)
        new Issue { Id = _issueId13, Key = "PROJ005-1", ProjectId = _projectId5, EpicId = _epicId5, SprintId = _sprintId3, Summary = "Issue 1 for Project 5", Title = "Task 1", Description = "Description for issue 1 in project 5", Type = "STORY", Priority = "HIGH", StatusId = _statusId1, AssigneeId = 4, ReporterId = 1, StoryPoints = 3, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId14, Key = "PROJ005-2", ProjectId = _projectId5, EpicId = _epicId5, SprintId = _sprintId3, Summary = "Issue 2 for Project 5", Title = "Task 2", Description = "Description for issue 2 in project 5", Type = "TASK", Priority = "MEDIUM", StatusId = _statusId2, AssigneeId = 5, ReporterId = 1, StoryPoints = 6, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset },
        new Issue { Id = _issueId15, Key = "PROJ005-3", ProjectId = _projectId5, EpicId = _epicId5, SprintId = _sprintId3, Summary = "Issue 3 for Project 5", Title = "Task 3", Description = "Description for issue 3 in project 5", Type = "BUG", Priority = "MEDIUM", StatusId = _statusId4, AssigneeId = 8, ReporterId = 1, StoryPoints = 9, CreatedBy = 1, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
    };

            modelBuilder.Entity<Issue>().HasData(issues);
        }


        private static void SeedIssueComments(ModelBuilder modelBuilder)
        {
            var comments = new List<IssueComment>
            {
                new IssueComment { Id = _issueCommentId1, IssueId = _issueId1, AuthorId = 4, MentionId = 5, Body = "This is a sample comment mentioning another user.", CreatedBy = 4, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
            };

            modelBuilder.Entity<IssueComment>().HasData(comments);
        }

        private static void SeedMentions(ModelBuilder modelBuilder)
        {
            var mentions = new List<Mention>
            {
                new Mention { Id = _mentionId1, MentionUserId = 5, IssueCommentsId = _issueCommentId1, CreatedBy = 4, CreatedAt = _staticDateOffset, UpdatedAt = _staticDateOffset }
            };

            modelBuilder.Entity<Mention>().HasData(mentions);
        }

        private static void SeedActivityLogs(ModelBuilder modelBuilder)
        {
            var activityLogs = new List<ActivityLog>
            {
                new ActivityLog { Id = _activityLogId1, UserId = 1, EntityType = "Project", EntityId = _projectId1, Action = "CREATE", Description = "Project created successfully", CreatedAt = _staticDateOffset }
            };

            modelBuilder.Entity<ActivityLog>().HasData(activityLogs);
        }

        private static void SeedNotifications(ModelBuilder modelBuilder)
        {
            var notifications = new List<Notification>
            {
                new Notification { Id = _notificationId1, RecipientId = 4, ActorId = 1, Message = "You have been assigned to a new issue", IsRead = false, CreatedAt = _staticDateOffset }
            };

            modelBuilder.Entity<Notification>().HasData(notifications);
        }

        private static void SeedAuditLogs(ModelBuilder modelBuilder)
        {
            var auditLogs = new List<AuditLog>
            {
                new AuditLog { Id = 1, UserId = 1, Action = "USER_LOGIN", EntityType = "User", EntityId = 1, Details = "{\"ip\": \"192.168.1.1\", \"userAgent\": \"Mozilla/5.0\"}", IpAddress = "192.168.1.1", CreatedAt = _staticDate }
            };

            modelBuilder.Entity<AuditLog>().HasData(auditLogs);
        }

        private static void SeedImportJobs(ModelBuilder modelBuilder)
        {
            var importJobs = new List<ImportJob>
            {
                new ImportJob { Id = 1, Type = "JIRA", Source = "Jira Cloud", Status = "completed", StartedBy = 1, StartedAt = _staticDate, FinishedAt = _staticDate, Summary = "{\"imported\": 50, \"failed\": 2}", CreatedAt = _staticDate }
            };
            modelBuilder.Entity<ImportJob>().HasData(importJobs);
        }

        private static void SeedJiraAuthorizations(ModelBuilder modelBuilder)
        {
            var jiraAuths = new List<JiraAuthorization>
            {
                new JiraAuthorization { Id = 1, UserId = 1, ProjectId = _projectId1, BaseUrl = "https://company.atlassian.net", AccessToken = "sample_access_token", RefreshToken = "sample_refresh_token", ExpiresAt = _staticDate.AddDays(30), Scopes = "read write", CreatedAt = _staticDate }
            };

            modelBuilder.Entity<JiraAuthorization>().HasData(jiraAuths);
        }
    }
}