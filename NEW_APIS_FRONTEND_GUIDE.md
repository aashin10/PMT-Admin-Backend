# New User Management APIs - Frontend Integration

This document covers the **NEW** APIs added for User Management (October 29, 2025).

---

## New APIs Added

1. ✅ **Bulk Import Users** - `POST /api/user/bulk-import`
2. ✅ **Paginated User List** - `POST /api/user/list`
3. ✅ **Updated Create User** - Now accepts `status` field

---

## 1. Bulk Import Users API

### Endpoint

```
POST /api/user/bulk-import
```

### Request Body

```typescript
interface BulkImportRequest {
  users: {
    jiraId?: string;
    name: string;
    email: string;
    status?: "Active" | "Inactive" | "Suspended";
  }[];
  createdBy?: number;
}
```

### Example

```json
{
  "users": [
    {
      "jiraId": "712020:7e79da6a-d73a-44aa-a872-02a961548405",
      "name": "Alan Jose",
      "email": "alan.jose@experionglobal.com",
      "status": "Active"
    }
  ]
}
```

### Response

```typescript
interface BulkImportResponse {
  data: {
    successCount: number;
    duplicateCount: number;
    errorCount: number;
    totalProcessed: number;
    errors: string[];
    duplicates: string[];
    createdUsers: UserDto[];
  };
  message: string;
  statusCode: number;
  succeeded: boolean;
}
```

### Features

- ✅ Auto-infers type from email domain (`@experionglobal.com` = Internal)
- ✅ Converts "Suspended" to "Inactive"
- ✅ Detects duplicates by email and JiraId
- ✅ Returns detailed summary
- ✅ Max 1000 users per request

---

## 2. Paginated User List API

### Endpoint

```
POST /api/user/paginated
```

### Request Body

```typescript
interface PaginationRequest {
  page?: number; // Default: 1
  pageSize?: number; // Default: 10, Max: 100
  sortBy?: string; // "name" | "email" | "type" | "status" | "createdat"
  sortOrder?: string; // "asc" | "desc"
  type?: string; // "Internal" | "External"
  status?: string; // "Active" | "Inactive"
  searchTerm?: string; // Search by name or email
}
```

### Example

```json
{
  "page": 1,
  "pageSize": 10,
  "sortBy": "name",
  "sortOrder": "asc",
  "type": "Internal",
  "status": "Active",
  "searchTerm": "alan"
}
```

### Response

```typescript
interface PaginatedResponse {
  data: {
    users: UserDto[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
    metadata: {
      sortBy: string;
      sortOrder: string;
      type: string | null;
      status: string | null;
      searchTerm: string | null;
    };
  };
  message: string;
  statusCode: number;
  succeeded: boolean;
}
```

---

## 3. Updated Create User API

### Endpoint

```
POST /api/user
```

### NEW: Status Field Added

```typescript
interface CreateUserDto {
  email: string;
  name: string;
  jiraId?: string;
  type?: string;
  status?: "Active" | "Inactive" | "Suspended"; // ← NEW FIELD
  createdBy?: number;
}
```

### Example

```json
{
  "users": [
    {
      "email": "alan.jose@experionglobal.com",
      "name": "Alan Jose",
      "jiraId": "712020:test",
      "type": "Internal",
      "status": "Active",
      "createdBy": 1
    }
  ]
}
```

---

## React/TypeScript Integration

### Bulk Import Hook

```typescript
import { useState } from "react";
import axios from "axios";

export const useBulkImport = () => {
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);

  const importUsers = async (users: any[]) => {
    setLoading(true);
    try {
      const response = await axios.post("/api/user/bulk-import", { users });
      setResult(response.data.data);
      return response.data.data;
    } catch (error) {
      console.error("Import failed:", error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  return { importUsers, loading, result };
};
```

### Pagination Hook

```typescript
import { useState, useEffect } from "react";
import axios from "axios";

export const usePaginatedUsers = () => {
  const [users, setUsers] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchUsers = async (params: any) => {
    setLoading(true);
    try {
      const response = await axios.post("/api/user/list", {
        page,
        pageSize: 10,
        ...params,
      });

      const data = response.data.data;
      setUsers(data.users);
      setTotalPages(data.totalPages);

      return data;
    } catch (error) {
      console.error("Fetch failed:", error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  return {
    users,
    page,
    setPage,
    totalPages,
    loading,
    fetchUsers,
  };
};
```

### CSV Import Component

```typescript
import React from "react";
import Papa from "papaparse";
import { useBulkImport } from "./hooks/useBulkImport";

export const CSVImportComponent: React.FC = () => {
  const { importUsers, loading, result } = useBulkImport();

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    Papa.parse(file, {
      header: true,
      complete: async (results) => {
        const users = results.data.map((row: any) => ({
          jiraId: row.jiraId,
          name: row.name,
          email: row.email,
          status: row.status || "Active",
        }));

        const importResult = await importUsers(users);
        alert(
          `Success: ${importResult.successCount}, Errors: ${importResult.errorCount}`
        );
      },
    });
  };

  return (
    <div>
      <input
        type="file"
        accept=".csv"
        onChange={handleFileChange}
        disabled={loading}
      />
      {loading && <p>Importing...</p>}
      {result && (
        <div>
          <p>Imported: {result.successCount}</p>
          <p>Duplicates: {result.duplicateCount}</p>
          <p>Errors: {result.errorCount}</p>
        </div>
      )}
    </div>
  );
};
```

---

## Angular Integration

### User Service

```typescript
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: "root" })
export class UserService {
  constructor(private http: HttpClient) {}

  bulkImport(users: any[]) {
    return this.http.post("/api/user/bulk-import", { users });
  }

  getUsersPaginated(params: any) {
    return this.http.post("/api/user/paginated", params);
  }

  createUser(user: any) {
    return this.http.post("/api/user", { users: [user] });
  }
}
```

---

## CSV Format

```csv
jiraId,name,email,status
712020:id1,Alan Jose,alan.jose@experionglobal.com,Active
712020:id2,John Doe,john.doe@external.com,Inactive
,Jane Smith,jane@example.com,Active
```

---

## Error Handling

```typescript
try {
  const result = await bulkImportUsers(users);

  // Check for partial failures
  if (result.errorCount > 0) {
    console.warn("Some users failed:", result.errors);
  }

  if (result.duplicateCount > 0) {
    console.info("Duplicates skipped:", result.duplicates);
  }
} catch (error) {
  // Handle API error
  console.error("API call failed:", error);
}
```

---

## Quick Reference

| Feature        | Endpoint                | Method |
| -------------- | ----------------------- | ------ |
| Bulk Import    | `/api/user/bulk-import` | POST   |
| Paginated List | `/api/user/paginated`   | POST   |
| Create User    | `/api/user`             | POST   |
| Filter Users   | `/api/user/filter`      | POST   |
| Get by ID      | `/api/user/{id}`        | GET    |
| Delete Users   | `/api/user`             | DELETE |

---

## Notes

- **Type Inference**: If not provided, inferred from email domain
- **Status Mapping**: "Suspended" → "Inactive"
- **Max Bulk**: 1000 users per request
- **Max Page Size**: 100 users per page
- **Date Format**: "MM/dd/yyyy"
