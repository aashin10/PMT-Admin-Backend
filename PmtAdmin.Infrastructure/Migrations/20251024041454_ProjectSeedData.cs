using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PmtAdmin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProjectSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "delivery_units",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "project_statuses",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "project_statuses",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "project_statuses",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "project_template",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "project_template",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "project_template",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "project_template",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "delivery_units",
                columns: new[] { "id", "code", "created_at", "description", "is_active", "manager_id", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, "ENG", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Engineering", null },
                    { 2, "PD", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Product Development", null },
                    { 3, "DS", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Digital Services", null },
                    { 4, "CS", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Cloud Solutions", null },
                    { 5, "DA", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Data Analytics", null },
                    { 6, "ES", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Enterprise Systems", null },
                    { 7, "MS", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Mobile Solutions", null },
                    { 8, "INF", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, "Infrastructure", null }
                });

            migrationBuilder.InsertData(
                table: "project_statuses",
                columns: new[] { "id", "created_at", "description", "name" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Project is currently active", "Active" },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Project is temporarily inactive", "Inactive" },
                    { 3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Project has been completed", "Completed" }
                });

            migrationBuilder.InsertData(
                table: "project_template",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Scrum Template" },
                    { 2, "Kanban Template" },
                    { 3, "Waterfall Template" },
                    { 4, "Agile Template" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "description", "metadata", "name", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manages projects", null, "Project Manager", null },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Technical leadership", null, "Tech Lead", null },
                    { 3, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manages delivery", null, "Delivery Manager", null }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar_url", "created_at", "created_by", "deleted_at", "deleted_by", "email", "is_active", "is_deleted", "is_super_admin", "jira_id", "last_login", "name", "password_hash", "type", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user1@pmtadmin.com", true, false, true, null, null, "User 1", null, null, null, null },
                    { 2, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user2@pmtadmin.com", true, false, false, null, null, "User 2", null, null, null, null },
                    { 3, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user3@pmtadmin.com", true, false, false, null, null, "User 3", null, null, null, null },
                    { 4, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user4@pmtadmin.com", true, false, false, null, null, "User 4", null, null, null, null },
                    { 5, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user5@pmtadmin.com", true, false, false, null, null, "User 5", null, null, null, null },
                    { 6, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user6@pmtadmin.com", true, false, false, null, null, "User 6", null, null, null, null },
                    { 7, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user7@pmtadmin.com", true, false, false, null, null, "User 7", null, null, null, null },
                    { 8, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user8@pmtadmin.com", true, false, false, null, null, "User 8", null, null, null, null },
                    { 9, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user9@pmtadmin.com", true, false, false, null, null, "User 9", null, null, null, null },
                    { 10, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "user10@pmtadmin.com", true, false, false, null, null, "User 10", null, null, null, null }
                });
        }
    }
}
