# Fresh Database Setup Guide

## Overview

This guide will help you set up a completely fresh database with all tables, relationships, and seeded data.

## Prerequisites

- PostgreSQL installed and running
- Database credentials ready

---

## Step-by-Step Instructions

### 1. Create New PostgreSQL Database

Open PostgreSQL (pgAdmin or psql command line) and create a new database:

```sql
CREATE DATABASE pmt_admin_new;
```

Or use the existing database name if you prefer. The connection string is configured in `appsettings.json`.

---

### 2. Update Connection String (Already Done)

The connection string in `appsettings.json` has been updated to:

```json
"PmtAdminDbConnection": "Host=localhost;Port=5432;Database=pmt_admin_new;Username=postgres;Password=experion@123"
```

**If you want to use a different database:**

- Update the `Database` parameter in the connection string
- Update the `Username` and `Password` if needed

---

### 3. Delete Old Migrations (Already Done)

All old migration files have been removed from:

```
PmtAdmin.Infrastructure\Migrations\
```

---

### 4. Create Initial Migration

**In Package Manager Console (Visual Studio):**

```powershell
Add-Migration InitialCreate -Project PmtAdmin.Infrastructure -StartupProject PmtAdmin.Api
```

This will create a new migration with ALL your entities including:

- Users table with Status column
- Projects, Teams, Boards
- Sprints, Epics, Issues
- All relationships and constraints
- Seeded data from DatabaseSeeder.cs

---

### 5. Apply Migration and Seed Data

**Option A: Using Package Manager Console**

```powershell
Update-Database -Project PmtAdmin.Infrastructure -StartupProject PmtAdmin.Api
```

**Option B: Run the Application**
The application is configured to auto-migrate on startup:

- Press F5 or Ctrl+F5 to run the application
- The database will be created automatically with all seeded data

---

## What Gets Created Automatically

### Tables Created:

1. **users** - With Status column ("Active", "Inactive", "Suspended")
2. **roles** - User roles and permissions
3. **permissions** - System permissions
4. **role_permissions** - Role-permission mappings
5. **delivery_units** - Delivery units with DU heads
6. **project_statuses** - Project status definitions
7. **project_templates** - Scrum/Kanban templates
8. **statuses** - Issue statuses
9. **projects** - Project definitions
10. **custom_fields** - Project custom fields
11. **teams** - Team definitions
12. **boards** - Scrum/Kanban boards
13. **project_members** - Project team members
14. **board_columns** - Board column configurations
15. **channels** - Team communication channels
16. **sprints** - Sprint definitions
17. **epics** - Epic definitions
18. **issues** - Issues/tasks
19. **issue_comments** - Issue comments
20. **mentions** - User mentions
21. **activity_logs** - Activity tracking
22. **notifications** - User notifications
23. **audit_logs** - Audit trail
24. **import_jobs** - Import job tracking
25. **jira_authorizations** - Jira integration auth

### Seeded Data Includes:

**Users (10 records):**

- Sarah Johnson (Super Admin, Internal, Active)
- Michael Chen (PM, Internal, Active)
- Emily Rodriguez (PM, Internal, Active)
- David Kumar (Developer, Internal, Active)
- Jessica Anderson (Developer, Internal, Inactive)
- Robert Wilson (Developer, External, Active)
- Amanda Taylor (Developer, Internal, Suspended)
- James Patel (QA, Internal, Active)
- Maria Garcia (QA, External, Active)
- Christopher Lee (Designer, Internal, Inactive)

**Roles:** Admin, Project Manager, Team Lead, Developer, QA Engineer, Designer

**Permissions:** project.create, project.read, project.update, project.delete, team.manage, user.manage

**Delivery Units (10):** Automotive, Travel & Transportation, Construction Solutions, etc.

**Projects (10):** PROJ001 through PROJ010 with teams, boards, and custom fields

**Sprints (5):** One active sprint per Scrum project

**Epics (10):** One epic per project

**Issues (15):** 3 issues per first 5 projects with different statuses

---

## Verification Steps

After migration completes, verify the setup:

### 1. Check Database Tables

```sql
-- Connect to pmt_admin_new database
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;
```

### 2. Verify Users Table Has Status Column

```sql
SELECT id, name, email, type, status, is_active
FROM users;
```

Expected output: 10 users with Status values ("Active", "Inactive", "Suspended")

### 3. Test API Endpoints

**Test User Filter API:**

```http
POST http://localhost:5000/api/User/filter
Content-Type: application/json

{
  "status": "Active"
}
```

**Test User Pagination API:**

```http
GET http://localhost:5000/api/User/paginated?pageNumber=1&pageSize=10&status=Active
```

**Test CSV Import API:**

```http
POST http://localhost:5000/api/User/import-csv
Content-Type: application/json

{
  "users": [
    {
      "jiraId": "TEST001",
      "name": "Test User",
      "email": "test@experionglobal.com",
      "status": "Active",
      "createdBy": 1
    }
  ]
}
```

---

## Troubleshooting

### Issue: Migration fails with "database already exists"

**Solution:**

```sql
DROP DATABASE pmt_admin_new;
CREATE DATABASE pmt_admin_new;
```

Then run the migration again.

### Issue: "Connection string not initialized"

**Solution:** Check that `PmtAdminDbConnection` matches the name in `PersistanceServiceRegistration.cs`

### Issue: Status column not showing in database

**Solution:** Check that the migration file includes `AddColumn` for status field. Delete migrations and recreate if needed.

### Issue: Seeded data not appearing

**Solution:** Check `AppDbContext.cs` - ensure `DatabaseSeeder.SeedData(modelBuilder);` is called in `OnModelCreating`

---

## Rollback Instructions

If you need to start over:

1. **Drop the database:**

   ```sql
   DROP DATABASE pmt_admin_new;
   ```

2. **Delete migrations folder contents:**

   ```powershell
   Remove-Item "PmtAdmin.Infrastructure\Migrations\*" -Recurse -Force
   ```

3. **Follow setup steps again from Step 1**

---

## Next Steps

After successful database setup:

1. ✅ All tables created with proper relationships
2. ✅ 10 users seeded with Status field
3. ✅ 10 projects with teams and boards
4. ✅ Sample epics, sprints, and issues

You can now:

- Test all User APIs (filter, pagination, CSV import)
- Use Status field with 3 values: "Active", "Inactive", "Suspended"
- Build frontend integration
- Add more custom seeding data if needed

---

## Configuration Files Modified

1. ✅ `appsettings.json` - Connection string updated to `pmt_admin_new`
2. ✅ `Program.cs` - Auto-migration enabled on startup
3. ✅ `DatabaseSeeder.cs` - Updated with realistic user data including Status
4. ✅ `User.cs` - Status column property added
5. ✅ All migrations cleared for fresh start

---

## Support

If you encounter any issues:

1. Check PostgreSQL is running
2. Verify connection string credentials
3. Check migration files are generated correctly
4. Review console output for specific error messages
