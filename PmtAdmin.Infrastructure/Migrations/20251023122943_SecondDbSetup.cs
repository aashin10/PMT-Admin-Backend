using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PmtAdmin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondDbSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_is_deleted",
                table: "users");

            migrationBuilder.AlterColumn<bool>(
                name: "is_super_admin",
                table: "users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "activity_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_activity_log_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    action = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entity_id = table.Column<int>(type: "integer", nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "custom_fields",
                columns: table => new
                {
                    projectid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_custom_fields", x => x.projectid);
                });

            migrationBuilder.CreateTable(
                name: "delivery_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    manager_id = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_delivery_units_users_manager_id",
                        column: x => x.manager_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "import_jobs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    started_by = table.Column<int>(type: "integer", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    summary = table.Column<string>(type: "jsonb", nullable: true),
                    details = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_import_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_import_jobs_users_started_by",
                        column: x => x.started_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_template",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_template", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_id = table.Column<int>(type: "integer", nullable: false),
                    actor_id = table.Column<int>(type: "integer", nullable: true),
                    activity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_activity_log_activity_id",
                        column: x => x.activity_id,
                        principalTable: "activity_log",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notification_users_actor_id",
                        column: x => x.actor_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_notification_users_recipient_id",
                        column: x => x.recipient_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    customer_org_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    customer_domain_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    customer_description = table.Column<string>(type: "text", nullable: true),
                    poc_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    poc_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    project_manager_id = table.Column<int>(type: "integer", nullable: true),
                    project_manager_role_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    delivery_unit_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    isimportedfromjira = table.Column<bool>(type: "boolean", nullable: true),
                    template_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                    table.ForeignKey(
                        name: "FK_projects_delivery_units_delivery_unit_id",
                        column: x => x.delivery_unit_id,
                        principalTable: "delivery_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_project_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "project_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_project_template_template_id",
                        column: x => x.template_id,
                        principalTable: "project_template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_roles_project_manager_role_id",
                        column: x => x.project_manager_role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_projects_users_project_manager_id",
                        column: x => x.project_manager_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_projects_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    permission_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "board_columns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: true),
                    board_name = table.Column<string>(type: "text", nullable: true),
                    board_color = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_board_columns", x => x.id);
                    table.ForeignKey(
                        name: "FK_board_columns_status_status_id",
                        column: x => x.status_id,
                        principalTable: "status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "epics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    assignee_id = table.Column<int>(type: "integer", nullable: true),
                    reporter_id = table.Column<int>(type: "integer", nullable: true),
                    labels = table.Column<string>(type: "jsonb", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epics", x => x.id);
                    table.ForeignKey(
                        name: "FK_epics_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_epics_users_assignee_id",
                        column: x => x.assignee_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_epics_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_epics_users_reporter_id",
                        column: x => x.reporter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_epics_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jira_authorizations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    base_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    access_token = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scopes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jira_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_jira_authorizations_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_jira_authorizations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sprints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    sprint_goal = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    story_point = table.Column<decimal>(type: "numeric", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sprints", x => x.id);
                    table.ForeignKey(
                        name: "FK_sprints_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sprints_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sprints_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    lead_id = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_teams_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teams_users_lead_id",
                        column: x => x.lead_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_teams_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "text", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    epic_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sprint_id = table.Column<Guid>(type: "uuid", nullable: true),
                    parent_issue_id = table.Column<Guid>(type: "uuid", nullable: true),
                    summary = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    assignee_id = table.Column<int>(type: "integer", nullable: true),
                    reporter_id = table.Column<int>(type: "integer", nullable: true),
                    story_points = table.Column<int>(type: "integer", nullable: false),
                    labels = table.Column<string>(type: "jsonb", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issues", x => x.id);
                    table.CheckConstraint("CK_Issues_StoryPoints", "story_points >= 0");
                    table.ForeignKey(
                        name: "FK_issues_epics_epic_id",
                        column: x => x.epic_id,
                        principalTable: "epics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_issues_issues_parent_issue_id",
                        column: x => x.parent_issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_issues_sprints_sprint_id",
                        column: x => x.sprint_id,
                        principalTable: "sprints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_issues_users_assignee_id",
                        column: x => x.assignee_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_issues_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issues_users_reporter_id",
                        column: x => x.reporter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_issues_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "boards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boards", x => x.id);
                    table.ForeignKey(
                        name: "FK_boards_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_boards_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_boards_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_boards_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channel", x => x.id);
                    table.ForeignKey(
                        name: "FK_channel_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    project_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_owner = table.Column<bool>(type: "boolean", nullable: false),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    added_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_members_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_members_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_members_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_project_members_users_added_by",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issue_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    mention_id = table.Column<int>(type: "integer", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issue_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_issue_comments_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_issue_comments_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_issue_comments_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issue_comments_users_mention_id",
                        column: x => x.mention_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_issue_comments_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messeges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: true),
                    body = table.Column<string>(type: "text", nullable: true),
                    mention_user_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messeges", x => x.id);
                    table.ForeignKey(
                        name: "FK_messeges_channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messeges_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messeges_users_mention_user_id",
                        column: x => x.mention_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_messeges_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mentions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    mention_user_id = table.Column<int>(type: "integer", nullable: true),
                    issue_comments_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mentions", x => x.id);
                    table.ForeignKey(
                        name: "FK_mentions_issue_comments_issue_comments_id",
                        column: x => x.issue_comments_id,
                        principalTable: "issue_comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mentions_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mentions_users_mention_user_id",
                        column: x => x.mention_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_mentions_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_log_user_id",
                table: "activity_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_entity_type_entity_id",
                table: "audit_logs",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_board_columns_status_id",
                table: "board_columns",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_boards_created_by",
                table: "boards",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_boards_project_id",
                table: "boards",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_boards_team_id",
                table: "boards",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_boards_updated_by",
                table: "boards",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_channel_team_id",
                table: "channel",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_units_code",
                table: "delivery_units",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_delivery_units_manager_id",
                table: "delivery_units",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_epics_assignee_id",
                table: "epics",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "IX_epics_created_by",
                table: "epics",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_epics_project_id",
                table: "epics",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_epics_reporter_id",
                table: "epics",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "IX_epics_updated_by",
                table: "epics",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_import_jobs_started_by",
                table: "import_jobs",
                column: "started_by");

            migrationBuilder.CreateIndex(
                name: "IX_issue_comments_author_id",
                table: "issue_comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_issue_comments_created_by",
                table: "issue_comments",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_issue_comments_issue_id",
                table: "issue_comments",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "IX_issue_comments_mention_id",
                table: "issue_comments",
                column: "mention_id");

            migrationBuilder.CreateIndex(
                name: "IX_issue_comments_updated_by",
                table: "issue_comments",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_issues_assignee_id",
                table: "issues",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "IX_issues_created_by",
                table: "issues",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_issues_epic_id",
                table: "issues",
                column: "epic_id");

            migrationBuilder.CreateIndex(
                name: "IX_issues_key",
                table: "issues",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_issues_parent_issue_id",
                table: "issues",
                column: "parent_issue_id");

            migrationBuilder.CreateIndex(
                name: "IX_issues_project_id",
                table: "issues",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_issues_reporter_id",
                table: "issues",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "IX_issues_sprint_id",
                table: "issues",
                column: "sprint_id");

            migrationBuilder.CreateIndex(
                name: "IX_issues_updated_by",
                table: "issues",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_jira_authorizations_project_id",
                table: "jira_authorizations",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_jira_authorizations_user_id",
                table: "jira_authorizations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mentions_created_by",
                table: "mentions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_mentions_issue_comments_id",
                table: "mentions",
                column: "issue_comments_id");

            migrationBuilder.CreateIndex(
                name: "IX_mentions_mention_user_id",
                table: "mentions",
                column: "mention_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_mentions_updated_by",
                table: "mentions",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_messeges_channel_id",
                table: "messeges",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_messeges_created_by",
                table: "messeges",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_messeges_mention_user_id",
                table: "messeges",
                column: "mention_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_messeges_updated_by",
                table: "messeges",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_notification_activity_id",
                table: "notification",
                column: "activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_actor_id",
                table: "notification",
                column: "actor_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_recipient_id",
                table: "notification",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_members_added_by",
                table: "project_members",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "IX_project_members_project_id_team_id_user_id",
                table: "project_members",
                columns: new[] { "project_id", "team_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_members_role_id",
                table: "project_members",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_members_team_id",
                table: "project_members",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_members_user_id",
                table: "project_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_statuses_name",
                table: "project_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_created_by",
                table: "projects",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_projects_delivery_unit_id",
                table: "projects",
                column: "delivery_unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_key",
                table: "projects",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_project_manager_id",
                table: "projects",
                column: "project_manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_project_manager_role_id",
                table: "projects",
                column: "project_manager_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_status_id",
                table: "projects",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_template_id",
                table: "projects",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_updated_by",
                table: "projects",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_role_id_permission_id",
                table: "role_permissions",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sprints_created_by",
                table: "sprints",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_sprints_project_id",
                table: "sprints",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_sprints_updated_by",
                table: "sprints",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_teams_created_by",
                table: "teams",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_teams_lead_id",
                table: "teams",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_teams_project_id",
                table: "teams",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_teams_updated_by",
                table: "teams",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "board_columns");

            migrationBuilder.DropTable(
                name: "boards");

            migrationBuilder.DropTable(
                name: "custom_fields");

            migrationBuilder.DropTable(
                name: "import_jobs");

            migrationBuilder.DropTable(
                name: "jira_authorizations");

            migrationBuilder.DropTable(
                name: "mentions");

            migrationBuilder.DropTable(
                name: "messeges");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "project_members");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "status");

            migrationBuilder.DropTable(
                name: "issue_comments");

            migrationBuilder.DropTable(
                name: "channel");

            migrationBuilder.DropTable(
                name: "activity_log");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "issues");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "epics");

            migrationBuilder.DropTable(
                name: "sprints");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "delivery_units");

            migrationBuilder.DropTable(
                name: "project_statuses");

            migrationBuilder.DropTable(
                name: "project_template");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.AlterColumn<bool>(
                name: "is_super_admin",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_users_is_deleted",
                table: "users",
                column: "is_deleted");
        }
    }
}
