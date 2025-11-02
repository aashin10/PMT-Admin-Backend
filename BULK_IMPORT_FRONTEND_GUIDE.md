# Bulk Import Users - Frontend Integration Guide

This document provides a comprehensive explanation of the bulk import functionality for users, including request/response formats, error handling, skipping logic, and notification strategies for frontend implementation.

---

## 1. Overview

The bulk import API allows the frontend to send a list of users (parsed from a CSV or other source) for batch creation. The backend processes each user, validates data, checks for duplicates, and applies business rules (such as skipping suspended users). The response contains detailed results for success, errors, duplicates, and skipped users, enabling the frontend to provide clear feedback to the user.

---

## 2. API Endpoint

**URL:**

```
POST /api/user/bulk-import
```

**Content-Type:**

```
application/json
```

---

## 3. Request Format

```json
{
  "users": [
    {
      "jiraId": "712020:abc123",
      "name": "Jane Doe",
      "email": "jane.doe@experionglobal.com",
      "status": "Active"
    },
    {
      "jiraId": "712020:def456",
      "name": "John Smith",
      "email": "john.smith@external.com",
      "status": "Suspended"
    }
  ],
  "createdBy": 1
}
```

**Fields:**

- `jiraId` (optional): Jira identifier for the user
- `name` (required): Full name of the user
- `email` (required): Email address (must be valid format)
- `status` (optional): "Active", "Inactive", or "Suspended"
- `createdBy` (optional): ID of the user performing the import

---

## 4. Backend Processing Logic

For each user in the request:

1. **Skip Suspended Users:**
   - If `status` is "Suspended" (case-insensitive), the user is **not imported**.
   - The user is added to the `Skipped` list in the response, with a message: `Suspended user skipped: {Name} ({Email})`.
2. **Validate Required Fields:**
   - If `name` is missing or empty, the user is skipped and an error is added: `Name is required for user with email: {Email}`.
   - If `email` is missing or empty, the user is skipped and an error is added: `Email is required for user: {Name}`.
   - If `email` is not a valid format, the user is skipped and an error is added: `Invalid email format: {Email}`.
3. **Check for Duplicates:**
   - If a user with the same email already exists, the user is skipped and a duplicate message is added: `Email already exists: {Email}`.
   - If a user with the same Jira ID already exists, the user is skipped and a duplicate message is added: `Jira ID already exists: {JiraId} for user: {Name}`.
4. **Type Inference:**
   - If the email domain is `@experionglobal.com`, type is set to "Internal"; otherwise, "External".
5. **Status Mapping:**
   - "Active" → IsActive = true
   - "Inactive" → IsActive = false
   - "Suspended" → Skipped (not imported)
6. **Password & Avatar Generation:**
   - Password: `[lastname]@experionglobal.123`
   - Avatar URL: Generated from name
7. **User Creation:**
   - If all validations pass, the user is created and added to the `CreatedUsers` list.
   - If an exception occurs during creation, an error is added: `Error creating user {Name}: {ExceptionMessage}`.

---

## 5. Response Format

```json
{
  "data": {
    "successCount": 1,
    "duplicateCount": 1,
    "skippedCount": 1,
    "errorCount": 1,
    "totalProcessed": 4,
    "errors": ["Name is required for user with email: john.smith@external.com"],
    "duplicates": ["Email already exists: jane.doe@experionglobal.com"],
    "skipped": ["Suspended user skipped: John Smith (john.smith@external.com)"],
    "createdUsers": [
      {
        "id": 123,
        "name": "Jane Doe",
        "email": "jane.doe@experionglobal.com",
        "type": "Internal",
        "status": "Active",
        "created_At": "10/30/2025",
        "last_Login": null
      }
    ]
  },
  "message": "Processed 4 users. Successfully imported: 1. Suspended users skipped: 1. Duplicates skipped: 1. Errors: 1",
  "statusCode": 200,
  "succeeded": true
}
```

**Fields:**

- `successCount`: Number of users successfully imported
- `duplicateCount`: Number of users skipped due to duplicate email or Jira ID
- `skippedCount`: Number of users skipped due to "Suspended" status
- `errorCount`: Number of users skipped due to validation or creation errors
- `totalProcessed`: Total number of users processed
- `errors`: List of error messages for failed users
- `duplicates`: List of duplicate messages
- `skipped`: List of skipped user messages ("Suspended" status)
- `createdUsers`: List of successfully created users (see UserDto)
- `message`: Summary message for notifications
- `statusCode`: HTTP status code
- `succeeded`: Indicates if the request was processed

---

## 6. Frontend Notification Strategy

The frontend should process the response and notify the user for each category:

### Success Notification

- Show the number of users successfully imported
- Display details of created users if needed

### Skipped Notification

- Show the number of users skipped due to "Suspended" status
- Display the `skipped` messages for details

### Duplicate Notification

- Show the number of users skipped due to duplicates
- Display the `duplicates` messages for details

### Error Notification

- Show the number of users skipped due to errors
- Display the `errors` messages for details

### Example Notification Logic

```typescript
if (response.succeeded) {
  showSuccess(`Imported: ${response.data.successCount}`);
  if (response.data.skippedCount > 0) {
    showWarning(`Suspended users skipped: ${response.data.skippedCount}`);
    showDetails(response.data.skipped);
  }
  if (response.data.duplicateCount > 0) {
    showWarning(`Duplicates skipped: ${response.data.duplicateCount}`);
    showDetails(response.data.duplicates);
  }
  if (response.data.errorCount > 0) {
    showError(`Errors: ${response.data.errorCount}`);
    showDetails(response.data.errors);
  }
} else {
  showError(response.message);
}
```

---

## 7. CSV Format for Import

**CSV Headers:**

```
jiraId,name,email,status
```

**CSV Example:**

```csv
jiraId,name,email,status
712020:abc123,Jane Doe,jane.doe@experionglobal.com,Active
,John Smith,john.smith@external.com,Suspended
,John Doe,john.doe@external.com,
```

---

## 8. Edge Cases & Recommendations

- **Max Users:** Limit bulk import to 1000 users per request
- **Field Lengths:**
  - Name: max 150 chars
  - Email: max 255 chars
  - JiraId: max 1024 chars
- **Status Values:** Only "Active", "Inactive", "Suspended" are valid
- **Type Inference:** If not provided, inferred from email domain
- **Date Format:** All dates in "MM/dd/yyyy"
- **Frontend Validation:** Validate CSV before sending to backend for better UX
- **Show Details:** Always display skipped, duplicate, and error details to the user

---

## 9. Example Frontend Implementation (React/TypeScript)

```typescript
import axios from "axios";

const bulkImportUsers = async (users: any[], createdBy?: number) => {
  const response = await axios.post("/api/user/bulk-import", {
    users,
    createdBy,
  });
  const data = response.data.data;

  // Notifications
  if (data.successCount > 0) {
    notifySuccess(`Imported: ${data.successCount}`);
  }
  if (data.skippedCount > 0) {
    notifyWarning(`Suspended users skipped: ${data.skippedCount}`);
    showDetails(data.skipped);
  }
  if (data.duplicateCount > 0) {
    notifyWarning(`Duplicates skipped: ${data.duplicateCount}`);
    showDetails(data.duplicates);
  }
  if (data.errorCount > 0) {
    notifyError(`Errors: ${data.errorCount}`);
    showDetails(data.errors);
  }
};
```

---

## 10. Summary

- The bulk import API provides detailed feedback for every user processed
- "Suspended" users are **skipped** and not imported
- All errors, duplicates, and skipped users are reported in the response
- Frontend should notify the user for each category and show details
- Always validate CSV before sending to backend for best results

---

For further questions, refer to the API documentation or contact the backend team.
