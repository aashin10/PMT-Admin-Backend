# Get User By ID API - Frontend Integration Guide

This document provides comprehensive details for integrating the Get User By ID API in the frontend, including endpoint, request/response formats, error handling, and usage recommendations.

---

## 1. Overview

The Get User By ID API retrieves detailed information about a specific user by their unique ID. This is typically used for viewing user profiles, editing user details, or validating user existence.

---

## 2. API Endpoint

**URL:**

```
GET /api/user/{id}
```

**Path Parameters:**

- `id` (required): The unique integer ID of the user to retrieve

---

## 3. Request Example

**HTTP Request:**

```
GET /api/user/123
```

No request body is required.

---

## 4. Response Format

### Success Response (200 OK)

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
  "message": "Request processed successfully"
}
```

**Fields:**

- `id`: User's unique ID
- `name`: Full name
- `email`: Email address
- `type`: "Internal" or "External"
- `status`: "Active" or "Inactive" (derived from `isActive`)
- `avatarUrl`: URL to user's avatar image
- `jiraId`: Jira identifier
- `created_At`: Date user was created
- `last_Login`: Last login date

---

## 5. Error Responses

### User Not Found (404)

```json
{
  "status": 404,
  "data": null,
  "message": "User not found"
}
```

### Invalid ID (400)

```json
{
  "status": 400,
  "data": null,
  "message": "Invalid user ID"
}
```

---

## 6. Frontend Usage Guidelines

- Always validate the user ID before making the request (must be a positive integer)
- Handle 404 errors gracefully (show "User not found" message)
- Use the returned data to populate user profile or edit forms
- Do not expose sensitive fields (e.g., passwordHash)
- Map `type` and `status` fields to user-friendly labels in the UI
- Use `avatarUrl` for profile images; provide a default if missing

---

## 7. Example Frontend Implementation (React/TypeScript)

```typescript
import axios from "axios";

const getUserById = async (userId: number) => {
  try {
    const response = await axios.get(`/api/user/${userId}`);
    if (response.data.status === 200) {
      return response.data.data;
    } else {
      throw new Error(response.data.message);
    }
  } catch (error) {
    if (error.response?.status === 404) {
      notifyError("User not found");
    } else if (error.response?.status === 400) {
      notifyError("Invalid user ID");
    } else {
      notifyError("Failed to fetch user");
    }
    throw error;
  }
};

// Usage example:
getUserById(123).then((user) => {
  // Populate UI with user details
});
```

---

## 8. UI Recommendations

- Show loading indicator while fetching user data
- Display error messages for not found or invalid ID
- Use profile image from `avatarUrl` or fallback to default
- Display all user fields in a readable format
- Disable edit actions if user is not found

---

## 9. Security Considerations

- Do not expose sensitive backend fields in the UI
- Validate user permissions before showing edit options
- Ensure only authorized users can view certain user details

---

## 10. Related APIs

- **PUT /api/user/{id}** - Update user details
- **GET /api/user/paginated** - List users with pagination
- **POST /api/user** - Create a new user

---

For further questions, refer to the API documentation or contact the backend team.
