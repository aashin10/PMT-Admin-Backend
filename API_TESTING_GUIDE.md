# API Testing Guide - User Management

Quick reference for testing the new User Management APIs.

---

## 1. Bulk Import Users

**Endpoint:** `POST /api/user/bulk-import`

**Postman/HTTP Request:**

```http
POST https://localhost:5001/api/user/bulk-import
Content-Type: application/json

{
  "users": [
    {
      "jiraId": "712020:7e79da6a-d73a-44aa-a872-02a961548405",
      "name": "Alan Jose",
      "email": "alan.jose@experionglobal.com",
      "status": "Active"
    },
    {
      "jiraId": "712020:ae0818cb-a311-4a09-a737-8c72d57448dc",
      "name": "John Doe",
      "email": "john.doe@external.com",
      "status": "Inactive"
    },
    {
      "name": "Jane Smith",
      "email": "jane.smith@experionglobal.com",
      "status": "Suspended"
    }
  ],
  "createdBy": 1
}
```

**Expected Result:**

- Alan Jose: Type = "Internal" (from email), Status = Active
- John Doe: Type = "External" (from email), Status = Inactive
- Jane Smith: Type = "Internal" (from email), Status = Inactive (Suspended converted)

---

## 2. Create User (Updated with Status)

**Endpoint:** `POST /api/user`

**Postman/HTTP Request:**

```http
POST https://localhost:5001/api/user
Content-Type: application/json

{
  "users": [
    {
      "email": "test.user@experionglobal.com",
      "name": "Test User",
      "jiraId": "712020:test-user-id",
      "type": "Internal",
      "status": "Active",
      "createdBy": 1
    }
  ]
}
```

**Test Cases:**

1. With status "Active" → IsActive = true
2. With status "Inactive" → IsActive = false
3. With status "Suspended" → IsActive = false
4. Without status field → Default to Active (backward compatible)
5. Without type field → Infers from email domain

---

## 3. Paginated User List

**Endpoint:** `POST /api/user/paginated`

### Test Case 1: Basic Pagination

```http
POST https://localhost:5001/api/user/paginated
Content-Type: application/json

{
  "page": 1,
  "pageSize": 10
}
```

### Test Case 2: With Sorting

```http
POST https://localhost:5001/api/user/paginated
Content-Type: application/json

{
  "page": 1,
  "pageSize": 10,
  "sortBy": "name",
  "sortOrder": "asc"
}
```

### Test Case 3: With Filtering

```http
POST https://localhost:5001/api/user/paginated
Content-Type: application/json

{
  "page": 1,
  "pageSize": 10,
  "type": "Internal",
  "status": "Active"
}
```

### Test Case 4: With Search

```http
POST https://localhost:5001/api/user/paginated
Content-Type: application/json

{
  "page": 1,
  "pageSize": 10,
  "searchTerm": "alan"
}
```

### Test Case 5: Complete Example

```http
POST https://localhost:5001/api/user/paginated
Content-Type: application/json

{
  "page": 1,
  "pageSize": 20,
  "sortBy": "createdat",
  "sortOrder": "desc",
  "type": "Internal",
  "status": "Active",
  "searchTerm": "jose"
}
```

**Valid sortBy values:**

- `name` - Sort by user name
- `email` - Sort by email address
- `type` - Sort by user type
- `status` - Sort by status
- `createdat` - Sort by creation date

**Valid sortOrder values:**

- `asc` - Ascending order
- `desc` - Descending order

**Valid type values:**

- `Internal` - Internal users
- `External` - External users

**Valid status values:**

- `Active` - Active users
- `Inactive` - Inactive users

---

## PowerShell Testing Commands

### 1. Bulk Import Users

```powershell
$headers = @{
    "Content-Type" = "application/json"
}

$body = @{
    users = @(
        @{
            jiraId = "712020:test-id-1"
            name = "Alan Jose"
            email = "alan.jose@experionglobal.com"
            status = "Active"
        },
        @{
            jiraId = "712020:test-id-2"
            name = "John Doe"
            email = "john.doe@external.com"
            status = "Inactive"
        }
    )
    createdBy = 1
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:5001/api/user/bulk-import" -Method Post -Headers $headers -Body $body
```

### 2. Create User with Status

```powershell
$headers = @{
    "Content-Type" = "application/json"
}

$body = @{
    users = @(
        @{
            email = "test.user@experionglobal.com"
            name = "Test User"
            jiraId = "712020:test-user"
            type = "Internal"
            status = "Active"
            createdBy = 1
        }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://localhost:5001/api/user" -Method Post -Headers $headers -Body $body
```

### 3. Paginated User List

```powershell
$headers = @{
    "Content-Type" = "application/json"
}

$body = @{
    page = 1
    pageSize = 10
    sortBy = "name"
    sortOrder = "asc"
    type = "Internal"
    status = "Active"
    searchTerm = "alan"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/user/paginated" -Method Post -Headers $headers -Body $body
```

---

## cURL Commands (for Bash/Linux)

### 1. Bulk Import Users

```bash
curl -X POST https://localhost:5001/api/user/bulk-import \
  -H "Content-Type: application/json" \
  -d '{
    "users": [
      {
        "jiraId": "712020:test-id-1",
        "name": "Alan Jose",
        "email": "alan.jose@experionglobal.com",
        "status": "Active"
      }
    ],
    "createdBy": 1
  }'
```

### 2. Create User with Status

```bash
curl -X POST https://localhost:5001/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "users": [
      {
        "email": "test.user@experionglobal.com",
        "name": "Test User",
        "type": "Internal",
        "status": "Active",
        "createdBy": 1
      }
    ]
  }'
```

### 3. Paginated User List

```bash
curl -X POST https://localhost:5001/api/user/paginated \
  -H "Content-Type: application/json" \
  -d '{
    "page": 1,
    "pageSize": 10,
    "sortBy": "name",
    "sortOrder": "asc"
  }'
```

---

## Expected Responses

### Successful Bulk Import

```json
{
  "data": {
    "successCount": 3,
    "duplicateCount": 0,
    "errorCount": 0,
    "totalProcessed": 3,
    "errors": [],
    "duplicates": [],
    "createdUsers": [...]
  },
  "message": "Processed 3 users. Successfully imported: 3",
  "statusCode": 200,
  "succeeded": true
}
```

### Bulk Import with Errors

```json
{
  "data": {
    "successCount": 2,
    "duplicateCount": 1,
    "errorCount": 1,
    "totalProcessed": 4,
    "errors": ["Invalid email format: invalid-email"],
    "duplicates": ["Email already exists: duplicate@example.com"],
    "createdUsers": [...]
  },
  "message": "Processed 4 users. Successfully imported: 2. Duplicates skipped: 1. Errors: 1",
  "statusCode": 200,
  "succeeded": true
}
```

### Successful Pagination

```json
{
  "data": {
    "users": [...],
    "totalCount": 50,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false,
    "metadata": {
      "sortBy": "name",
      "sortOrder": "asc",
      "type": "Internal",
      "status": "Active",
      "searchTerm": null
    }
  },
  "message": "Retrieved 10 users (Page 1 of 5)",
  "statusCode": 200,
  "succeeded": true
}
```

---

## Validation Error Examples

### Invalid Page Size

```json
{
  "data": null,
  "message": "PageSize cannot exceed 100",
  "statusCode": 400,
  "succeeded": false
}
```

### Invalid Status

```json
{
  "data": null,
  "message": "Status must be 'Active', 'Inactive', or 'Suspended'",
  "statusCode": 400,
  "succeeded": false
}
```

### Empty Users List

```json
{
  "data": null,
  "message": "Users list cannot be empty",
  "statusCode": 400,
  "succeeded": false
}
```

---

## Running the API

1. Navigate to the API project directory:

```powershell
cd c:\ILP022025\Project\Back-End\PmtAdmin\PmtAdmin.Api
```

2. Run the application:

```powershell
dotnet run
```

3. Access Swagger UI:

```
https://localhost:5001/swagger
```

4. Test endpoints using Swagger UI or the commands above

---

## Notes

- Replace `localhost:5001` with your actual API URL
- Ensure the database connection is configured
- Run migrations if needed: `dotnet ef database update`
- Check logs for detailed error information
- Use Swagger UI for interactive testing and documentation
