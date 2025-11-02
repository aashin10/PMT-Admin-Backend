# Update User API - Frontend Integration Guide

This document provides detailed information about the Update User API endpoint for frontend implementation.

---

## 1. Overview

The Update User API allows updating specific fields of an existing user. **Name and Email are NOT editable** to maintain data integrity. All other user fields can be updated through this endpoint.

---

## 2. API Endpoint

**URL:**

```
PUT /api/user/{id}
```

**Content-Type:**

```
application/json
```

**Path Parameters:**

- `id` (required): The ID of the user to update

---

## 3. Request Format

```json
{
  "jiraId": "712020:abc123",
  "type": "Internal",
  "isActive": true,
  "updatedBy": 1
}
```

**Editable Fields:**

- `jiraId` (optional, string, max 1024 chars): Jira identifier for the user
- `type` (optional, string): User type - must be "Internal" or "External"
- `isActive` (optional, boolean): Whether the user is active
- `updatedBy` (optional, integer): ID of the user performing the update

**Non-Editable Fields:**

- `name`: Cannot be changed
- `email`: Cannot be changed
- `passwordHash`: Cannot be changed directly (use password reset API instead)
- `isSuperAdmin`: Cannot be changed (requires separate admin management API)
- `avatarUrl`: Cannot be changed through this API

**Note:** All fields in the request are optional. Only include the fields you want to update.

---

## 4. Validation Rules

### Field Validations:

1. **User ID (Path Parameter):**

   - Must be greater than 0
   - User must exist in the database
   - User must not be deleted (soft delete check)

2. **JiraId:**

   - Maximum 1024 characters
   - Must be unique (cannot match another user's Jira ID)
   - Can be set to empty string to clear the value

3. **Type:**

   - Must be exactly "Internal" or "External" (case-sensitive)
   - If provided, cannot be empty

4. **IsActive:**
   - Boolean value (true/false)

---

## 5. Response Format

### Success Response (200 OK):

```json
{
  "status": 200,
  "data": {
    "id": 123,
    "name": "Jane Doe",
    "email": "jane.doe@experionglobal.com",
    "type": "Internal",
    "status": "Active",
    "avatarUrl": "https://example.com/avatar.jpg",
    "jiraId": "712020:abc123",
    "created_At": "10/29/2025",
    "last_Login": "10/30/2025"
  },
  "message": "User updated successfully"
}
```

---

## 6. Error Responses

### User Not Found (404):

```json
{
  "status": 404,
  "data": null,
  "message": "User with ID 123 not found"
}
```

### User Deleted (404):

```json
{
  "status": 404,
  "data": null,
  "message": "User with ID 123 has been deleted"
}
```

### Duplicate Jira ID (409):

```json
{
  "status": 409,
  "data": null,
  "message": "Jira ID '712020:abc123' already exists for another user"
}
```

### Validation Error (400):

```json
{
  "status": 400,
  "data": null,
  "message": "Validation failed",
  "errors": [
    "User ID must be greater than 0",
    "Type must be either 'Internal' or 'External'",
    "Avatar URL cannot exceed 1000 characters"
  ]
}
```

---

## 7. Frontend Implementation Guidelines

### Partial Updates:

Only send the fields you want to update. For example, to only update the status:

```json
{
  "isActive": false,
  "updatedBy": 1
}
```

### Clearing Values:

To clear optional fields like `jiraId`, send an empty string:

```json
{
  "jiraId": "",
  "updatedBy": 1
}
```

### Type Field Mapping:

When displaying in the UI, map the type field:

- "Internal" → Show "Internal User"
- "External" → Show "External User"

### Status Field Mapping:

The `isActive` field represents the user's status:

- `isActive: true` → Display as "Active"
- `isActive: false` → Display as "Inactive"

---

## 8. Example Frontend Implementation (React/TypeScript)

```typescript
import axios from "axios";

interface UpdateUserRequest {
  jiraId?: string;
  type?: "Internal" | "External";
  isActive?: boolean;
  updatedBy?: number;
}

const updateUser = async (userId: number, updates: UpdateUserRequest) => {
  try {
    const response = await axios.put(`/api/user/${userId}`, updates);

    if (response.data.status === 200) {
      notifySuccess(response.data.message);
      return response.data.data;
    }
  } catch (error) {
    if (error.response?.status === 404) {
      notifyError("User not found or has been deleted");
    } else if (error.response?.status === 409) {
      notifyError("Jira ID already exists for another user");
    } else if (error.response?.status === 400) {
      notifyError("Validation failed: " + error.response.data.message);
    } else {
      notifyError("Failed to update user");
    }
    throw error;
  }
};

// Example usage:
updateUser(123, {
  isActive: false,
  type: "External",
  updatedBy: 1,
});
```

---

## 9. UI Recommendations

### Edit Form:

- **Disable** name, email, and avatarUrl fields in the edit form
- Show a tooltip explaining why these fields cannot be edited
- Validate type dropdown to only allow "Internal" or "External"
- Use a toggle/checkbox for isActive
- Add character count indicator for JiraId (1024)

### Confirmation:

- Show confirmation dialog before saving changes
- Display a summary of changes being made
- Highlight which fields are being updated

### Error Handling:

- Display field-specific validation errors inline
- Show a toast notification for duplicate Jira ID errors
- Provide retry option if update fails due to network issues

---

## 10. Security Considerations

- The `updatedBy` field should be automatically populated from the logged-in user's session
- The `isSuperAdmin` field is intentionally excluded from this API for security reasons
- Super admin privileges should be managed through a separate, more secure admin management API
- Do not expose the `passwordHash` field in any edit form
- Implement role-based access control to restrict who can update users

---

## 11. Testing Scenarios

1. **Successful Update:** Update user type from "Internal" to "External"
2. **Partial Update:** Update only the `isActive` field
3. **Clear Field:** Set `jiraId` to empty string
4. **Duplicate Jira ID:** Try to update with an existing Jira ID
5. **Invalid User ID:** Try to update a non-existent user
6. **Deleted User:** Try to update a user that has been soft-deleted
7. **Invalid Type:** Try to set type to "Other" (should fail validation)
8. **Max Length:** Try to exceed character limit for JiraId

---

## 12. Related APIs

- **GET /api/user/{id}** - Retrieve user details before editing
- **GET /api/user/paginated** - List users with pagination
- **POST /api/user** - Create a new user
- **DELETE /api/user** - Delete users (soft delete)

---

For further questions, refer to the API documentation or contact the backend team.
