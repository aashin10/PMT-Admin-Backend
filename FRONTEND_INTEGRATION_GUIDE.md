# PMT Admin Backend API - User Management Documentation

**Last Updated:** October 28, 2025  
**Version:** 2.0  
**Feature Branch:** feature/user-management

---

## 📋 Table of Contents

1. [Summary of Changes](#summary-of-changes)
2. [API Endpoints](#api-endpoints)
3. [Request/Response Models](#requestresponse-models)
4. [Breaking Changes](#breaking-changes)
5. [Frontend Integration Guide](#frontend-integration-guide)
6. [Examples](#examples)

---

## 🎯 Summary of Changes

### 1. **GetAllUsers - Changed from GET to POST**

- **Reason**: To support filtering by Type and Status
- **New Route**: `POST /api/User/filter`
- **Old Route**: `GET /api/User` (NO LONGER AVAILABLE)

### 2. **CreateUser - Supports Bulk Creation**

- **Route**: `POST /api/User` (same endpoint)
- **Change**: Now accepts an array of users for bulk creation
- **Auto-Generated Fields**:
  - `password_hash`: Generated from `[lastname]@experionglobal.123`
  - `avatar_url`: Generated from user's name
  - `is_super_admin`: Always set to `false`
  - `is_active`: Always set to `true`

### 3. **Enum Normalization**

- All `Type` and `Status` values are automatically normalized
- First letter capitalized, rest lowercase
- Examples: "internal" → "Internal", "ACTIVE" → "Active"

---

## 🔌 API Endpoints

### 1. Get All Users (With Filtering)

**Changed from GET to POST**

```http
POST /api/User/filter
Content-Type: application/json
```

**Request Body:**

```json
{
  "type": "Internal", // Optional: "Internal" or "External"
  "status": "Active" // Optional: "Active" or "Inactive"
}
```

**Response:**

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "John Doe",
      "email": "john.doe@example.com",
      "type": "Internal",
      "status": "Active",
      "created_At": "10/28/2025",
      "last_Login": "10/27/2025"
    }
  ],
  "message": "Request processed successfully"
}
```

**Filter Options:**
| Parameter | Type | Values | Description |
|-----------|------|--------|-------------|
| `type` | string (optional) | "Internal", "External" | Filter users by type |
| `status` | string (optional) | "Active", "Inactive" | Filter users by status |

**Notes:**

- Both filters are optional
- Can use one filter, both filters, or no filters (returns all users)
- Values are case-insensitive ("internal", "INTERNAL", "Internal" all work)

---

### 2. Create User(s) - Bulk Creation Supported

**Endpoint remains the same, but structure changed**

```http
POST /api/User
Content-Type: application/json
```

**Request Body (Single User):**

```json
{
  "users": [
    {
      "email": "jane.smith@example.com",
      "name": "Jane Smith",
      "jiraId": "JIRA-12345", // Optional
      "type": "Internal", // Optional: "Internal" or "External"
      "createdBy": 1 // Optional: ID of creating user
    }
  ]
}
```

**Request Body (Multiple Users - Bulk Creation):**

```json
{
  "users": [
    {
      "email": "user1@example.com",
      "name": "User One",
      "type": "Internal"
    },
    {
      "email": "user2@example.com",
      "name": "User Two",
      "type": "External"
    },
    {
      "email": "user3@example.com",
      "name": "User Three"
    }
  ]
}
```

**Response (Success - All Users Created):**

```json
{
  "status": 201,
  "data": [
    {
      "id": 10,
      "name": "Jane Smith",
      "email": "jane.smith@example.com",
      "type": "Internal",
      "status": "Active",
      "created_At": "10/28/2025",
      "last_Login": null
    }
  ],
  "message": "All users created successfully"
}
```

**Response (Partial Success - Some Users Failed):**

```json
{
  "status": 201,
  "data": [
    {
      "id": 11,
      "name": "User One",
      "email": "user1@example.com",
      "type": "Internal",
      "status": "Active",
      "created_At": "10/28/2025",
      "last_Login": null
    }
  ],
  "message": "1 of 2 users created. Errors: Email already exists: user2@example.com"
}
```

**Response (Error - All Users Failed):**

```json
{
  "status": 400,
  "data": null,
  "message": "Email already exists: jane.smith@example.com; Invalid email format: invalid-email"
}
```

---

### 3. Get User By ID

**No changes**

```http
GET /api/User/{id}
```

---

### 4. Delete Users

**No changes**

```http
DELETE /api/User
Content-Type: application/json
```

**Request Body:**

```json
{
  "ids": [1, 2, 3]
}
```

---

## 📦 Request/Response Models

### CreateUserDto (NEW)

```typescript
interface CreateUserDto {
  email: string; // Required
  name: string; // Required
  jiraId?: string; // Optional
  type?: string; // Optional: "Internal" or "External"
  createdBy?: number; // Optional
}
```

### CreateUserCommand (UPDATED)

```typescript
interface CreateUserCommand {
  users: CreateUserDto[]; // Array of users to create
}
```

### GetAllUsersQuery (NEW)

```typescript
interface GetAllUsersQuery {
  type?: string; // Optional: "Internal" or "External"
  status?: string; // Optional: "Active" or "Inactive"
}
```

### UserDto (Response Model)

```typescript
interface UserDto {
  id: number;
  name: string;
  email: string;
  type: string;
  status: string; // "Active" or "Inactive"
  created_At: string; // Format: "MM/dd/yyyy"
  last_Login?: string; // Format: "MM/dd/yyyy"
}
```

---

## ⚠️ Breaking Changes

### 1. GetAllUsers Endpoint

**BREAKING:** Changed from GET to POST

**Before:**

```javascript
// ❌ OLD - NO LONGER WORKS
fetch("https://localhost:7178/api/User", {
  method: "GET",
});
```

**After:**

```javascript
// ✅ NEW - REQUIRED
fetch("https://localhost:7178/api/User/filter", {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
  },
  body: JSON.stringify({
    type: "Internal", // Optional
    status: "Active", // Optional
  }),
});
```

### 2. CreateUser Request Structure

**BREAKING:** Request body structure changed

**Before:**

```json
{
  "email": "user@example.com",
  "name": "User Name",
  "passwordHash": "...", // ❌ No longer needed
  "avatarUrl": "...", // ❌ No longer needed
  "isActive": true, // ❌ No longer needed
  "isSuperAdmin": false, // ❌ No longer needed
  "jiraId": "JIRA-123",
  "type": "Internal",
  "createdBy": 1
}
```

**After:**

```json
{
  "users": [
    // ✅ Now wrapped in array
    {
      "email": "user@example.com",
      "name": "User Name",
      "jiraId": "JIRA-123", // Optional
      "type": "Internal", // Optional
      "createdBy": 1 // Optional
    }
  ]
}
```

### 3. CreateUser Response Structure

**BREAKING:** Response is now an array

**Before:**

```json
{
  "status": 201,
  "data": {
    // ❌ Single object
    "id": 1,
    "name": "User Name"
    // ...
  },
  "message": "User created successfully"
}
```

**After:**

```json
{
  "status": 201,
  "data": [
    // ✅ Array of objects
    {
      "id": 1,
      "name": "User Name"
      // ...
    }
  ],
  "message": "All users created successfully"
}
```

---

## 🚀 Frontend Integration Guide

### 1. Update GetAllUsers API Call

**Angular/TypeScript Example:**

```typescript
// OLD CODE - Remove this
getAllUsers(): Observable<ApiResponse<UserDto[]>> {
  return this.http.get<ApiResponse<UserDto[]>>(`${this.apiUrl}/User`);
}

// NEW CODE - Use this
getAllUsers(filters?: { type?: string, status?: string }): Observable<ApiResponse<UserDto[]>> {
  const body = {
    type: filters?.type || null,
    status: filters?.status || null
  };

  return this.http.post<ApiResponse<UserDto[]>>(
    `${this.apiUrl}/User/filter`,
    body
  );
}

// Usage examples:
this.usersService.getAllUsers();                                    // No filters
this.usersService.getAllUsers({ type: 'Internal' });               // Filter by type only
this.usersService.getAllUsers({ status: 'Active' });               // Filter by status only
this.usersService.getAllUsers({ type: 'Internal', status: 'Active' }); // Both filters
```

**React/JavaScript Example:**

```javascript
// OLD CODE - Remove this
const getAllUsers = async () => {
  const response = await fetch(`${API_URL}/User`);
  return response.json();
};

// NEW CODE - Use this
const getAllUsers = async (filters = {}) => {
  const response = await fetch(`${API_URL}/User/filter`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      type: filters.type || null,
      status: filters.status || null,
    }),
  });
  return response.json();
};

// Usage examples:
await getAllUsers(); // No filters
await getAllUsers({ type: "Internal" }); // Filter by type only
await getAllUsers({ status: "Active" }); // Filter by status only
await getAllUsers({ type: "Internal", status: "Active" }); // Both filters
```

### 2. Update CreateUser API Call

**Angular/TypeScript Example:**

```typescript
// Interface updates
interface CreateUserDto {
  email: string;
  name: string;
  jiraId?: string;
  type?: string;
  createdBy?: number;
}

interface CreateUserCommand {
  users: CreateUserDto[];
}

// OLD CODE - Remove this
createUser(user: any): Observable<ApiResponse<UserDto>> {
  return this.http.post<ApiResponse<UserDto>>(`${this.apiUrl}/User`, user);
}

// NEW CODE - Use this
createUser(user: CreateUserDto): Observable<ApiResponse<UserDto[]>> {
  const command: CreateUserCommand = {
    users: [user]  // Wrap in array
  };

  return this.http.post<ApiResponse<UserDto[]>>(
    `${this.apiUrl}/User`,
    command
  );
}

// NEW - Bulk creation method
createUsers(users: CreateUserDto[]): Observable<ApiResponse<UserDto[]>> {
  const command: CreateUserCommand = {
    users: users
  };

  return this.http.post<ApiResponse<UserDto[]>>(
    `${this.apiUrl}/User`,
    command
  );
}

// Usage examples:
// Single user
this.usersService.createUser({
  email: 'user@example.com',
  name: 'User Name',
  type: 'Internal'
}).subscribe(response => {
  const createdUser = response.data[0];  // ⚠️ Note: data is now an array
  console.log('Created user:', createdUser);
});

// Multiple users
this.usersService.createUsers([
  { email: 'user1@example.com', name: 'User One', type: 'Internal' },
  { email: 'user2@example.com', name: 'User Two', type: 'External' }
]).subscribe(response => {
  console.log(`Created ${response.data.length} users`);
  console.log('Message:', response.message);
});
```

**React/JavaScript Example:**

```javascript
// OLD CODE - Remove this
const createUser = async (user) => {
  const response = await fetch(`${API_URL}/User`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(user),
  });
  return response.json();
};

// NEW CODE - Use this
const createUser = async (user) => {
  const response = await fetch(`${API_URL}/User`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      users: [user], // Wrap in array
    }),
  });
  return response.json();
};

// NEW - Bulk creation
const createUsers = async (users) => {
  const response = await fetch(`${API_URL}/User`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      users: users,
    }),
  });
  return response.json();
};

// Usage examples:
// Single user
const result = await createUser({
  email: "user@example.com",
  name: "User Name",
  type: "Internal",
});
const createdUser = result.data[0]; // ⚠️ Note: data is now an array

// Multiple users
const result = await createUsers([
  { email: "user1@example.com", name: "User One", type: "Internal" },
  { email: "user2@example.com", name: "User Two", type: "External" },
]);
console.log(`Created ${result.data.length} users`);
```

---

## 💡 Examples

### Example 1: Filter Users by Type and Status

```javascript
// Get all internal, active users
const response = await fetch("https://localhost:7178/api/User/filter", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    type: "Internal",
    status: "Active",
  }),
});

const result = await response.json();
console.log("Active internal users:", result.data);
```

### Example 2: Create Single User with Minimal Data

```javascript
const response = await fetch("https://localhost:7178/api/User", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    users: [
      {
        email: "john.doe@example.com",
        name: "John Doe",
      },
    ],
  }),
});

const result = await response.json();
// Password will be: Doe@experionglobal.123
// Avatar URL will be: https://avatar.iran.liara.run/username?username=John+Doe
// is_super_admin will be: false
// is_active will be: true
```

### Example 3: Bulk Create Users

```javascript
const response = await fetch("https://localhost:7178/api/User", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    users: [
      {
        email: "alice.johnson@example.com",
        name: "Alice Johnson",
        type: "Internal",
        jiraId: "JIRA-001",
      },
      {
        email: "bob.williams@example.com",
        name: "Bob Williams",
        type: "External",
        jiraId: "JIRA-002",
      },
      {
        email: "charlie.brown@example.com",
        name: "Charlie Brown",
        type: "Internal",
      },
    ],
  }),
});

const result = await response.json();
console.log(`Created ${result.data.length} users`);
console.log("Message:", result.message);
```

### Example 4: Handle Partial Success

```javascript
const response = await fetch("https://localhost:7178/api/User", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    users: [
      { email: "new.user@example.com", name: "New User" },
      { email: "existing.user@example.com", name: "Existing User" }, // Already exists
    ],
  }),
});

const result = await response.json();
if (result.status === 201 && result.data.length < 2) {
  console.log("Some users failed to create");
  console.log("Success count:", result.data.length);
  console.log("Error details:", result.message);
  // Output: "1 of 2 users created. Errors: Email already exists: existing.user@example.com"
}
```

---

## 🔐 Password Generation Logic

The backend automatically generates passwords using the following pattern:

**Pattern:** `[lastname]@experionglobal.123`

**Examples:**
| Full Name | Generated Password |
|-----------|-------------------|
| John Doe | `Doe@experionglobal.123` |
| Alice Johnson | `Johnson@experionglobal.123` |
| Bob | `Bob@experionglobal.123` |
| Mary Jane Watson | `Jane@experionglobal.123` |

**Rules:**

- Uses the **second word** from the name (if available)
- If only one word, uses that word
- If more than two words, uses only the second word
- Password is hashed using BCrypt before storage

---

## 🖼️ Avatar URL Generation Logic

The backend automatically generates avatar URLs using the following pattern:

**Pattern:** `https://avatar.iran.liara.run/username?username=[firstname]+[lastname]`

**Examples:**
| Full Name | Generated Avatar URL |
|-----------|---------------------|
| John Doe | `https://avatar.iran.liara.run/username?username=John+Doe` |
| Alice Johnson | `https://avatar.iran.liara.run/username?username=Alice+Johnson` |
| Bob | `https://avatar.iran.liara.run/username?username=Bob` |
| Mary Jane Watson | `https://avatar.iran.liara.run/username?username=Mary+Jane` |

**Rules:**

- Uses **first two words** from the name
- If only one word, uses that word
- If more than two words, ignores the rest
- Words are joined with `+` symbol

---

## ✅ Validation Rules

### Email

- **Required**: Yes
- **Format**: Must be valid email format
- **Unique**: Must not already exist in database
- **Max Length**: 255 characters

### Name

- **Required**: Yes
- **Max Length**: 150 characters

### JiraId

- **Required**: No
- **Unique**: Must not already exist (if provided)
- **Max Length**: 1024 characters

### Type

- **Required**: No
- **Valid Values**: "Internal", "External" (case-insensitive)
- **Max Length**: 50 characters
- **Auto-Normalized**: First letter capitalized

---

## 🐛 Error Handling

### Common Error Messages

| Error Message            | Reason                          | Solution                |
| ------------------------ | ------------------------------- | ----------------------- |
| "Name is required"       | Name field is missing or empty  | Provide a valid name    |
| "Email is required"      | Email field is missing or empty | Provide a valid email   |
| "Invalid email format"   | Email format is incorrect       | Use valid email format  |
| "Email already exists"   | Email is already in database    | Use a different email   |
| "Jira ID already exists" | Jira ID is already in database  | Use a different Jira ID |

### Error Response Format

```json
{
  "status": 400,
  "data": null,
  "message": "Email already exists: user@example.com; Invalid email format: bad-email"
}
```

---

## 📞 Support

For questions or issues:

- **Backend Team Lead**: [Your Name]
- **API Documentation**: https://localhost:7178/swagger (if available)
- **Repository**: PMT-Admin-Backend
- **Branch**: feature/user-management

---

**End of Documentation**
