# Database Migration Script - Add Status Column

## PostgreSQL Migration

```sql
-- ========================================
-- Migration: Add Status Column to Users Table
-- Date: October 29, 2025
-- Description: Add support for 3 statuses: Active, Inactive, Suspended
-- ========================================

-- Step 1: Add status column (nullable, default 'Active')
ALTER TABLE users
ADD COLUMN status VARCHAR(50) DEFAULT 'Active';

-- Step 2: Populate existing records based on is_active field
UPDATE users
SET status = CASE
    WHEN is_active = true THEN 'Active'
    ELSE 'Inactive'
END
WHERE status IS NULL OR status = 'Active';

-- Step 3: Add check constraint to ensure only valid statuses
ALTER TABLE users
ADD CONSTRAINT chk_users_status
CHECK (status IN ('Active', 'Inactive', 'Suspended'));

-- Step 4: Add index on status for better query performance
CREATE INDEX idx_users_status ON users(status);

-- Step 5: Verify the migration
SELECT
    status,
    is_active,
    COUNT(*) as count
FROM users
WHERE is_deleted = false
GROUP BY status, is_active
ORDER BY status;

-- Expected output:
-- status     | is_active | count
-- -----------+-----------+-------
-- Active     | true      | X
-- Inactive   | false     | Y
-- Suspended  | false     | 0  (initially)
```

## Rollback Script (if needed)

```sql
-- ========================================
-- Rollback: Remove Status Column
-- ========================================

-- Step 1: Drop index
DROP INDEX IF EXISTS idx_users_status;

-- Step 2: Drop check constraint
ALTER TABLE users
DROP CONSTRAINT IF EXISTS chk_users_status;

-- Step 3: Drop status column
ALTER TABLE users
DROP COLUMN IF EXISTS status;
```

## Entity Framework Core Migration

### Generate Migration

```powershell
# Navigate to solution directory
cd c:\ILP022025\Project\Back-End\PmtAdmin

# Generate migration
dotnet ef migrations add AddStatusColumnToUsers `
  --project PmtAdmin.Infrastructure `
  --startup-project PmtAdmin.Api `
  --context AppDbContext

# Review the generated migration file
# It should be in: PmtAdmin.Infrastructure/Migrations/[timestamp]_AddStatusColumnToUsers.cs
```

### Apply Migration

```powershell
# Apply to database
dotnet ef database update `
  --project PmtAdmin.Infrastructure `
  --startup-project PmtAdmin.Api `
  --context AppDbContext

# Verify migration was applied
dotnet ef migrations list `
  --project PmtAdmin.Infrastructure `
  --startup-project PmtAdmin.Api
```

### Rollback Migration (if needed)

```powershell
# Rollback to previous migration
dotnet ef database update [PreviousMigrationName] `
  --project PmtAdmin.Infrastructure `
  --startup-project PmtAdmin.Api

# Or remove migration entirely
dotnet ef migrations remove `
  --project PmtAdmin.Infrastructure `
  --startup-project PmtAdmin.Api
```

## Verification Queries

### Check Column Was Added

```sql
SELECT
    column_name,
    data_type,
    character_maximum_length,
    column_default,
    is_nullable
FROM information_schema.columns
WHERE table_name = 'users'
  AND column_name = 'status';

-- Expected output:
-- column_name | data_type        | character_maximum_length | column_default | is_nullable
-- ------------+------------------+--------------------------+----------------+-------------
-- status      | character varying| 50                       | 'Active'       | YES
```

### Check Data Migration

```sql
-- Verify all users have a status
SELECT COUNT(*) as total_users,
       COUNT(status) as users_with_status,
       COUNT(*) - COUNT(status) as users_without_status
FROM users;

-- Should show: users_without_status = 0

-- Verify status distribution
SELECT
    status,
    COUNT(*) as count,
    ROUND(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER(), 2) as percentage
FROM users
WHERE is_deleted = false
GROUP BY status
ORDER BY count DESC;
```

### Check Constraint

```sql
-- Verify constraint exists
SELECT
    conname as constraint_name,
    pg_get_constraintdef(oid) as constraint_definition
FROM pg_constraint
WHERE conname = 'chk_users_status';

-- Expected output:
-- constraint_name   | constraint_definition
-- ------------------+------------------------------------------------
-- chk_users_status  | CHECK ((status)::text = ANY (ARRAY['Active'::character varying, 'Inactive'::character varying, 'Suspended'::character varying]::text[]))
```

### Test Constraint

```sql
-- This should succeed
INSERT INTO users (email, name, status)
VALUES ('test@example.com', 'Test User', 'Suspended');

-- This should fail
INSERT INTO users (email, name, status)
VALUES ('test2@example.com', 'Test User 2', 'InvalidStatus');
-- ERROR: new row violates check constraint "chk_users_status"

-- Clean up test data
DELETE FROM users WHERE email LIKE 'test%@example.com';
```

## Post-Migration Steps

### 1. Update Application Configuration

```json
// appsettings.json - No changes needed
// Connection string remains the same
```

### 2. Restart Application

```powershell
# Stop current instance (if running)
# Ctrl+C in the terminal where it's running

# Start fresh
dotnet run --project PmtAdmin.Api
```

### 3. Test API Endpoints

```powershell
# Test create user with each status
Invoke-RestMethod -Uri "https://localhost:7178/api/User" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"users":[{"email":"test.active@example.com","name":"Test Active","status":"Active"}]}'

Invoke-RestMethod -Uri "https://localhost:7178/api/User" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"users":[{"email":"test.suspended@example.com","name":"Test Suspended","status":"Suspended"}]}'

# Test filter by status
Invoke-RestMethod -Uri "https://localhost:7178/api/User/filter" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"status":"Suspended"}'
```

## Troubleshooting

### Issue: Migration Fails with "column already exists"

**Solution:**

```sql
-- Check if column already exists
SELECT column_name FROM information_schema.columns
WHERE table_name = 'users' AND column_name = 'status';

-- If it exists, skip to Step 2 (populate data)
```

### Issue: Check constraint fails to add

**Solution:**

```sql
-- Check for invalid existing data
SELECT DISTINCT status FROM users;

-- Clean up invalid statuses
UPDATE users
SET status = 'Active'
WHERE status NOT IN ('Active', 'Inactive', 'Suspended')
   OR status IS NULL;

-- Then add constraint
```

### Issue: EF Core migration generates incorrect SQL

**Solution:**

- Manually edit the migration file
- Or use raw SQL migration:

```csharp
migrationBuilder.Sql(@"
    ALTER TABLE users ADD COLUMN status VARCHAR(50) DEFAULT 'Active';
    UPDATE users SET status = CASE WHEN is_active THEN 'Active' ELSE 'Inactive' END;
");
```

## Migration Checklist

- [ ] Backup database before migration
- [ ] Run migration script in test environment first
- [ ] Verify all existing users have a status
- [ ] Check constraint is properly enforced
- [ ] Index is created for performance
- [ ] Application starts without errors
- [ ] API endpoints work with all 3 statuses
- [ ] Frontend can filter by "Suspended"
- [ ] Document migration completion date
- [ ] Update team on migration status

---

**Migration Ready to Execute!**  
Review the script, backup your database, then run the migration.
