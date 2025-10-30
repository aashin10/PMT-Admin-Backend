# ?? Quick Reference - GetAllProjects API

## API Endpoint
```
GET /api/projects
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 10 | Items per page (10, 25, 50, max 100) |
| `searchTerm` | string? | null | Search by name, key, or manager |
| `statusIds` | List<int>? | null | Filter by multiple statuses |
| `deliveryUnitIds` | List<int>? | null | Filter by multiple delivery units |
| `projectManagerIds` | List<int>? | null | Filter by multiple managers |

## Quick Examples

### Basic
```
GET /api/projects?page=1&pageSize=10
```

### Search
```
GET /api/projects?searchTerm=Alpha
```

### Single Filter
```
GET /api/projects?statusIds=1
```

### Multi-Select
```
GET /api/projects?statusIds=1&statusIds=2&statusIds=3
```

### Combined
```
GET /api/projects?page=1&pageSize=25&searchTerm=project&statusIds=1&statusIds=2&deliveryUnitIds=1&projectManagerIds=1&projectManagerIds=2
```

## Response Format

```json
{
  "status": 200,
  "data": {
    "items": [
      {
        "id": "guid",
        "name": "Project Alpha",
        "key": "PROJ001",
        "status": { "id": 1, "name": "Active" },
        "deliveryUnit": { "id": 1, "name": "DU1", "code": "DU001" },
        "teamSize": 8,
        "projectManager": { "id": 5, "name": "John Doe" },
        "isImportedFromJira": true
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5
  },
  "message": "Retrieved 10 projects from 45 total"
}
```

## Fields Returned

| Field | Type | Description |
|-------|------|-------------|
| `id` | Guid | Project unique identifier |
| `name` | string | Project name |
| `key` | string | Project key/code |
| `status` | object | Status with id, name, description |
| `deliveryUnit` | object | DU with id, name, **code** |
| `teamSize` | int | Number of project members |
| `projectManager` | object | Manager with id, name |
| `isImportedFromJira` | bool? | Jira import flag |

## Test Commands

```bash
# Build
dotnet build

# Test
dotnet test

# Run API
dotnet run --project PmtAdmin.Api

# Test Specific
dotnet test --filter "FullyQualifiedName~GetAllProjectsQueryHandlerTests"
```

## cURL Examples

```bash
# Basic
curl -X GET "http://localhost:5000/api/projects?page=1&pageSize=10"

# Multi-select
curl -X GET "http://localhost:5000/api/projects?statusIds=1&statusIds=2&statusIds=3"

# Search + Filters
curl -X GET "http://localhost:5000/api/projects?searchTerm=Alpha&statusIds=1&deliveryUnitIds=1"
```

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 404 | Not found |
| 400 | Bad request |
| 500 | Server error |

## Test Status

? **20/20 Tests Passing**
- GetAllProjectsQueryHandlerTests: 7 ?
- GetProjectByIdQueryHandlerTests: 4 ?
- DeleteProjectCommandHandlerTests: 3 ?
- GetAllProjectStatusQueryHandlerTests: 6 ?

## Features

? Pagination (10, 25, 50 per page)
? Search (name, key, manager)
? Multi-select filters
? IsImportedFromJira field
? Total count for UI display
? Delivery unit code included

## Frontend Integration

```typescript
// React/TypeScript
const params = new URLSearchParams();
params.append('page', '1');
params.append('pageSize', '25');
statusIds.forEach(id => params.append('statusIds', id.toString()));

const response = await fetch(`/api/projects?${params}`);
const data = await response.json();

// Display
console.log(`Showing ${data.data.items.length} of ${data.data.totalCount}`);
```

## Documentation Files

- `TESTS_FIXED_SUMMARY.md` - Test results
- `TESTING_GUIDE.md` - Testing instructions  
- `MULTI_SELECT_IMPLEMENTATION.md` - Technical details
- `PMT_Admin_Postman_Collection.json` - API tests

---

**Ready for Production! ??**
