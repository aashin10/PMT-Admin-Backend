# PMT Admin Backend API - New Features Documentation

**Date:** October 29, 2025  
**Version:** 2.1  
**Branch:** feature/user-management

---

## 📋 Summary of Changes

### 1. ✅ Bulk Import Users from CSV

- **New Endpoint:** `POST /api/User/import-csv`
- **Purpose:** Import multiple users from CSV data
- **Features:**
  - Automatically infers user Type from email domain
  - Maps Status from CSV to IsActive field
  - Generates password hash and avatar URL (same as CreateUser)
  - Validates duplicates and provides detailed error reporting

### 2. ✅ Status Field in CreateUser API

- **Endpoint:** `POST /api/User` (existing, modified)
- **Change:** Now accepts `Status` field from frontend
- **Values:** "Active", "Inactive", "Suspended"
- **Mapping:** Status → IsActive (Active=true, Inactive/Suspended=false)

### 3. ✅ Paginated Users API

- **New Endpoint:** `GET /api/User/paginated`
- **Purpose:** Fetch users with pagination, sorting, and filtering
- **Features:**
  - Pagination with customizable page size
  - Sorting by name, email, type, or creation date
  - Filtering by Type and Status
  - Returns total count and page information

---

## 🔌 API Endpoints

### 1. Import Users from CSV

#### Endpoint

```http
POST /api/User/import-csv
Content-Type: application/json
```

#### Request Body

```json
{
  "users": [
    {
      "jiraId": "712020:7e79da6a-d73a-44aa-a872-02a961548405",
      "name": "Alan Jose",
      "email": "alan.jose@experionglobal.com",
      "status": "Active",
      "createdBy": 1
    },
    {
      "jiraId": "712020:ae0818cb-a311-4a09-a737-8c72d57448dc",
      "name": "FUHAD SANEEN K",
      "email": "2343@tkmce.ac.in",
      "status": "Active",
      "createdBy": 1
    },
    {
      "jiraId": "712020:b62a35a1-d272-4f51-9694-1f74f4ed4a5a",
      "name": "Aashin thomas",
      "email": "aashinthomas072@gmail.com",
      "status": "Inactive",
      "createdBy": 1
    }
  ]
}
```

#### CSV to JSON Mapping

| CSV Column  | JSON Property | Notes                                |
| ----------- | ------------- | ------------------------------------ |
| Jira_id     | jiraId        | Optional                             |
| User name   | name          | Required                             |
| email       | email         | Required                             |
| User status | status        | "Active", "Inactive", or "Suspended" |

#### Type Inference Logic

**Automatic Type Assignment:**

- If email domain is `experionglobal.com` → Type = "Internal"
- Otherwise → Type = "External"

**Examples:**
| Email | Inferred Type |
|-------|---------------|
| `alan.jose@experionglobal.com` | Internal |
| `2343@tkmce.ac.in` | External |
| `aashinthomas072@gmail.com` | External |

#### Response (Success)

```json
{
  "status": 201,
  "data": [
    {
      "id": 101,
      "name": "Alan Jose",
      "email": "alan.jose@experionglobal.com",
      "type": "Internal",
      "status": "Active",
      "created_At": "10/29/2025",
      "last_Login": null
    },
    {
      "id": 102,
      "name": "FUHAD SANEEN K",
      "email": "2343@tkmce.ac.in",
      "type": "External",
      "status": "Active",
      "created_At": "10/29/2025",
      "last_Login": null
    }
  ],
  "message": "All 2 users imported successfully from CSV"
}
```

#### Response (Partial Success)

```json
{
  "status": 201,
  "data": [
    {
      "id": 101,
      "name": "Alan Jose",
      "email": "alan.jose@experionglobal.com",
      "type": "Internal",
      "status": "Active",
      "created_At": "10/29/2025",
      "last_Login": null
    }
  ],
  "message": "1 of 3 users imported. Errors: Email already exists: duplicate@example.com; Jira ID already exists: 712020:existing-id"
}
```

#### Auto-Generated Fields

Just like the CreateUser API, the following fields are automatically generated:

| Field            | Generation Logic                                                         | Example                                                   |
| ---------------- | ------------------------------------------------------------------------ | --------------------------------------------------------- |
| `password_hash`  | `[lastname]@experionglobal.123` → BCrypt hash                            | Jose@experionglobal.123                                   |
| `avatar_url`     | `https://avatar.iran.liara.run/username?username=[firstname]+[lastname]` | https://avatar.iran.liara.run/username?username=Alan+Jose |
| `is_super_admin` | Always `false`                                                           | false                                                     |
| `is_active`      | Mapped from Status ("Active" → true)                                     | true                                                      |
| `is_deleted`     | Always `false`                                                           | false                                                     |

---

### 2. Create User (Updated)

#### Endpoint

```http
POST /api/User
Content-Type: application/json
```

#### Request Body (NEW - with Status field)

```json
{
  "users": [
    {
      "email": "john.doe@experionglobal.com",
      "name": "John Doe",
      "jiraId": "JIRA-12345",
      "type": "Internal",
      "status": "Active",
      "createdBy": 1
    }
  ]
}
```

#### Changes from Previous Version

**ADDED:**

- `status` field: "Active", "Inactive", or "Suspended"

**REMOVED (Auto-Generated):**

- `passwordHash` - Now auto-generated
- `avatarUrl` - Now auto-generated
- `isSuperAdmin` - Now auto-generated (always false)
- `isActive` - Now derived from Status field

#### Status Mapping

| Frontend Status | Backend IsActive | Description        |
| --------------- | ---------------- | ------------------ |
| "Active"        | `true`           | User can log in    |
| "Inactive"      | `false`          | User cannot log in |
| "Suspended"     | `false`          | User cannot log in |

#### Request Model

```typescript
interface CreateUserDto {
  email: string; // Required
  name: string; // Required
  jiraId?: string; // Optional
  type?: string; // Optional: "Internal" or "External"
  status?: string; // NEW - Optional: "Active", "Inactive", "Suspended"
  createdBy?: number; // Optional
}

interface CreateUserCommand {
  users: CreateUserDto[]; // Array of users
}
```

---

### 3. Get Users with Pagination (NEW)

#### Endpoint

```http
GET /api/User/paginated?pageNumber=1&pageSize=10&sortBy=name&sortOrder=asc&type=Internal&status=Active
```

#### Query Parameters

| Parameter    | Type   | Default | Description         | Valid Values                         |
| ------------ | ------ | ------- | ------------------- | ------------------------------------ |
| `pageNumber` | int    | 1       | Current page number | >= 1                                 |
| `pageSize`   | int    | 10      | Items per page      | 1-100                                |
| `sortBy`     | string | "name"  | Field to sort by    | "name", "email", "type", "createdat" |
| `sortOrder`  | string | "asc"   | Sort direction      | "asc", "desc"                        |
| `type`       | string | null    | Filter by user type | "Internal", "External"               |
| `status`     | string | null    | Filter by status    | "Active", "Inactive", "Suspended"    |

#### Example Requests

**Basic Pagination (No Filters):**

```http
GET /api/User/paginated?pageNumber=1&pageSize=20
```

**Filter by Type:**

```http
GET /api/User/paginated?pageNumber=1&pageSize=10&type=Internal
```

**Filter by Status:**

```http
GET /api/User/paginated?pageNumber=1&pageSize=10&status=Active
```

**Filter by Both:**

```http
GET /api/User/paginated?pageNumber=1&pageSize=10&type=Internal&status=Active
```

**Sort by Email Descending:**

```http
GET /api/User/paginated?pageNumber=1&pageSize=10&sortBy=email&sortOrder=desc
```

**Sort by Creation Date:**

```http
GET /api/User/paginated?pageNumber=2&pageSize=25&sortBy=createdat&sortOrder=desc
```

#### Response Structure

```json
{
  "status": 200,
  "data": {
    "data": [
      {
        "id": 1,
        "name": "Alan Jose",
        "email": "alan.jose@experionglobal.com",
        "type": "Internal",
        "status": "Active",
        "created_At": "10/29/2025",
        "last_Login": "10/28/2025"
      },
      {
        "id": 2,
        "name": "Aswin Raj",
        "email": "aswin.raj@experionglobal.com",
        "type": "Internal",
        "status": "Active",
        "created_At": "10/29/2025",
        "last_Login": null
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalRecords": 25,
    "totalPages": 3,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "message": "Users retrieved successfully"
}
```

#### Response Model

```typescript
interface PaginatedResponse<T> {
  data: T[]; // Array of items for current page
  pageNumber: number; // Current page number
  pageSize: number; // Items per page
  totalRecords: number; // Total items across all pages
  totalPages: number; // Total number of pages
  hasPreviousPage: boolean; // Can go to previous page
  hasNextPage: boolean; // Can go to next page
}

interface ApiResponse<T> {
  status: number;
  data: T;
  message: string;
}
```

#### Pagination Calculations

```typescript
// Frontend pagination helper
function getPaginationInfo(response: PaginatedResponse<UserDto>) {
  return {
    currentPage: response.pageNumber,
    totalPages: response.totalPages,
    totalItems: response.totalRecords,
    itemsPerPage: response.pageSize,
    startItem: (response.pageNumber - 1) * response.pageSize + 1,
    endItem: Math.min(
      response.pageNumber * response.pageSize,
      response.totalRecords
    ),
    canGoPrevious: response.hasPreviousPage,
    canGoNext: response.hasNextPage,
  };
}

// Example: "Showing 11-20 of 25 users"
const info = getPaginationInfo(response);
console.log(
  `Showing ${info.startItem}-${info.endItem} of ${info.totalItems} users`
);
```

---

## 🔄 Comparison: Filter vs Paginated API

### POST /api/User/filter (Existing)

- **Returns:** All matching users (no pagination)
- **Use Case:** When you need all users at once
- **Filters:** Type, Status
- **Response:** Array of users
- **Max Results:** Unlimited (could be slow with many users)

### GET /api/User/paginated (NEW)

- **Returns:** One page of users at a time
- **Use Case:** Large datasets, user lists with pagination UI
- **Filters:** Type, Status
- **Sorting:** By name, email, type, or creation date
- **Response:** Paginated response with metadata
- **Max Results:** Configurable (1-100 per page)

**Recommendation:** Use `/paginated` for user listing pages, use `/filter` for dropdowns or when you need all users.

---

## 🎨 Frontend Integration Examples

### Import Users from CSV (Angular)

```typescript
// users-api.service.ts
interface CsvUserDto {
  jiraId?: string;
  name: string;
  email: string;
  status: string;
  createdBy?: number;
}

interface ImportUsersFromCsvCommand {
  users: CsvUserDto[];
}

importUsersFromCsv(csvData: CsvUserDto[]): Observable<ApiResponse<UserDto[]>> {
  const command: ImportUsersFromCsvCommand = {
    users: csvData
  };

  return this.http.post<ApiResponse<UserDto[]>>(
    `${this.apiUrl}/User/import-csv`,
    command
  );
}

// CSV parsing example
parseCsvToUsers(csvContent: string): CsvUserDto[] {
  const lines = csvContent.split('\n');
  const headers = lines[0].split('\t'); // Tab-separated

  return lines.slice(1).map(line => {
    const values = line.split('\t');
    return {
      jiraId: values[0],
      name: values[1],
      email: values[2],
      status: values[3],
      createdBy: 1 // Current user ID
    };
  });
}

// Usage in component
onCsvFileSelected(event: any) {
  const file = event.target.files[0];
  const reader = new FileReader();

  reader.onload = (e: any) => {
    const csvContent = e.target.result;
    const users = this.parseCsvToUsers(csvContent);

    this.usersApi.importUsersFromCsv(users).subscribe({
      next: (response) => {
        console.log(`Imported ${response.data.length} users`);
        this.showSuccess(response.message);
      },
      error: (error) => {
        this.showError(error.message);
      }
    });
  };

  reader.readAsText(file);
}
```

### Create User with Status (Angular)

```typescript
// Update existing createUser method
createUser(userData: {
  email: string;
  name: string;
  jiraId?: string;
  type?: string;
  status?: string;  // NEW
}): Observable<ApiResponse<UserDto[]>> {
  const command: CreateUserCommand = {
    users: [userData]
  };

  return this.http.post<ApiResponse<UserDto[]>>(
    `${this.apiUrl}/User`,
    command
  );
}

// Usage in component
onCreateUser(form: any) {
  this.usersApi.createUser({
    email: form.email,
    name: form.name,
    type: form.type,
    status: form.status || 'Active',  // Default to Active
    jiraId: form.jiraId
  }).subscribe({
    next: (response) => {
      const user = response.data[0];
      console.log('User created:', user);
    }
  });
}
```

### Paginated Users API (Angular)

```typescript
// users-api.service.ts
interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  type?: string;
  status?: string;
}

getUsersPaginated(params: PaginationParams = {}): Observable<ApiResponse<PaginatedResponse<UserDto>>> {
  const queryParams = new HttpParams()
    .set('pageNumber', params.pageNumber?.toString() || '1')
    .set('pageSize', params.pageSize?.toString() || '10')
    .set('sortBy', params.sortBy || 'name')
    .set('sortOrder', params.sortOrder || 'asc');

  // Add optional filters
  let finalParams = queryParams;
  if (params.type) {
    finalParams = finalParams.set('type', params.type);
  }
  if (params.status) {
    finalParams = finalParams.set('status', params.status);
  }

  return this.http.get<ApiResponse<PaginatedResponse<UserDto>>>(
    `${this.apiUrl}/User/paginated`,
    { params: finalParams }
  );
}

// Component usage
export class UsersListComponent implements OnInit {
  users: UserDto[] = [];
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  totalRecords = 0;
  sortBy = 'name';
  sortOrder: 'asc' | 'desc' = 'asc';
  filterType: string | null = null;
  filterStatus: string | null = null;

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.usersApi.getUsersPaginated({
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder,
      type: this.filterType || undefined,
      status: this.filterStatus || undefined
    }).subscribe({
      next: (response) => {
        const paginated = response.data;
        this.users = paginated.data;
        this.currentPage = paginated.pageNumber;
        this.totalPages = paginated.totalPages;
        this.totalRecords = paginated.totalRecords;
      }
    });
  }

  onPageChange(page: number) {
    this.currentPage = page;
    this.loadUsers();
  }

  onSortChange(column: string) {
    if (this.sortBy === column) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortOrder = 'asc';
    }
    this.currentPage = 1; // Reset to first page
    this.loadUsers();
  }

  onFilterChange() {
    this.currentPage = 1; // Reset to first page
    this.loadUsers();
  }
}
```

### React Example

```javascript
// usersApi.js
export const getUsersPaginated = async ({
  pageNumber = 1,
  pageSize = 10,
  sortBy = "name",
  sortOrder = "asc",
  type = null,
  status = null,
}) => {
  const params = new URLSearchParams({
    pageNumber: pageNumber.toString(),
    pageSize: pageSize.toString(),
    sortBy,
    sortOrder,
  });

  if (type) params.append("type", type);
  if (status) params.append("status", status);

  const response = await fetch(
    `${API_URL}/User/paginated?${params.toString()}`
  );
  return response.json();
};

// UsersListComponent.jsx
function UsersList() {
  const [users, setUsers] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [sortBy, setSortBy] = useState("name");
  const [sortOrder, setSortOrder] = useState("asc");

  useEffect(() => {
    loadUsers();
  }, [currentPage, sortBy, sortOrder]);

  const loadUsers = async () => {
    const response = await getUsersPaginated({
      pageNumber: currentPage,
      pageSize: 10,
      sortBy,
      sortOrder,
    });

    setUsers(response.data.data);
    setTotalPages(response.data.totalPages);
  };

  return (
    <div>
      <table>
        <thead>
          <tr>
            <th onClick={() => handleSort("name")}>Name</th>
            <th onClick={() => handleSort("email")}>Email</th>
            <th onClick={() => handleSort("type")}>Type</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.name}</td>
              <td>{user.email}</td>
              <td>{user.type}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={setCurrentPage}
      />
    </div>
  );
}
```

---

## ✅ Validation Rules

### Import CSV Users

| Field    | Required | Max Length | Validation                           |
| -------- | -------- | ---------- | ------------------------------------ |
| `email`  | ✅ Yes   | 255        | Must be valid email format           |
| `name`   | ✅ Yes   | 150        | Cannot be empty                      |
| `jiraId` | ❌ No    | 1024       | Must be unique (if provided)         |
| `status` | ❌ No    | 50         | "Active", "Inactive", or "Suspended" |

### Create User (Updated)

| Field       | Required | Max Length | Validation                                 |
| ----------- | -------- | ---------- | ------------------------------------------ |
| `email`     | ✅ Yes   | 255        | Must be valid email format, must be unique |
| `name`      | ✅ Yes   | 150        | Cannot be empty                            |
| `jiraId`    | ❌ No    | 1024       | Must be unique (if provided)               |
| `type`      | ❌ No    | 50         | "Internal" or "External"                   |
| `status`    | ❌ No    | 50         | "Active", "Inactive", or "Suspended"       |
| `createdBy` | ❌ No    | -          | Must be valid user ID (if provided)        |

### Paginated Query

| Parameter    | Min | Max | Default |
| ------------ | --- | --- | ------- |
| `pageNumber` | 1   | -   | 1       |
| `pageSize`   | 1   | 100 | 10      |

---

## 🐛 Common Errors

### Import CSV

**Error:** "Email already exists: user@example.com"

- **Cause:** User with this email already exists in database
- **Solution:** Remove duplicate or update existing user

**Error:** "Jira ID already exists: JIRA-123"

- **Cause:** User with this Jira ID already exists
- **Solution:** Use unique Jira ID or leave empty

**Error:** "Invalid email format: invalid-email"

- **Cause:** Email doesn't match standard format
- **Solution:** Correct email format in CSV

### Create User

**Error:** "Status must be 'Active', 'Inactive', or 'Suspended'"

- **Cause:** Invalid status value provided
- **Solution:** Use one of the valid status values

### Paginated Query

**Error:** No pagination metadata returned

- **Cause:** Using wrong endpoint (used `/filter` instead of `/paginated`)
- **Solution:** Use `GET /api/User/paginated`

---

## 📊 Performance Considerations

### Import CSV

- **Batch Size:** No hard limit, but recommend < 1000 users per request
- **Processing:** Each user is validated individually
- **Errors:** Partial success supported (some succeed, some fail)

### Paginated API

- **Page Size Limit:** Maximum 100 items per page
- **Recommended:** 10-50 items per page for optimal performance
- **Sorting:** Indexed on name, email, type, created_at

---

## 🔧 Testing Examples

### Postman Collection

**Import CSV Users:**

```http
POST https://localhost:7178/api/User/import-csv
Content-Type: application/json

{
  "users": [
    {
      "jiraId": "TEST-001",
      "name": "Test User",
      "email": "test@experionglobal.com",
      "status": "Active"
    }
  ]
}
```

**Create User with Status:**

```http
POST https://localhost:7178/api/User
Content-Type: application/json

{
  "users": [
    {
      "email": "newuser@example.com",
      "name": "New User",
      "type": "External",
      "status": "Inactive"
    }
  ]
}
```

**Get Paginated Users:**

```http
GET https://localhost:7178/api/User/paginated?pageNumber=1&pageSize=20&sortBy=name&sortOrder=asc&type=Internal&status=Active
```

---

## 📝 Migration Notes

### For Frontend Developers

**If you're using CreateUser API:**

1. ✅ Add `status` field to your user creation forms
2. ✅ Update request model to include `status?: string`
3. ✅ Set default value to "Active" if not specified
4. ✅ Remove any code setting `isActive`, `passwordHash`, or `avatarUrl`

**If you're implementing user listing:**

1. ✅ Use new `/paginated` endpoint for large lists
2. ✅ Add pagination controls (page number, page size)
3. ✅ Add sorting controls (sort by, sort order)
4. ✅ Keep existing `/filter` endpoint for dropdowns

**If you need CSV import:**

1. ✅ Parse CSV with tab-separated values
2. ✅ Map CSV columns to JSON properties
3. ✅ Call `/import-csv` endpoint
4. ✅ Handle partial success scenarios

---

## 🎯 Summary

### New Endpoints

- ✅ `POST /api/User/import-csv` - Bulk import from CSV
- ✅ `GET /api/User/paginated` - Get users with pagination

### Modified Endpoints

- ✅ `POST /api/User` - Now accepts `status` field

### Key Features

- ✅ Automatic Type inference from email domain
- ✅ Status field support ("Active", "Inactive", "Suspended")
- ✅ Pagination with configurable page size (1-100)
- ✅ Sorting by name, email, type, or creation date
- ✅ Filtering by Type and Status in paginated API

### Auto-Generated Fields (All User Creation)

- ✅ Password: `[lastname]@experionglobal.123` → BCrypt hash
- ✅ Avatar URL: `https://avatar.iran.liara.run/username?username=[firstname]+[lastname]`
- ✅ IsSuperAdmin: Always `false`
- ✅ IsDeleted: Always `false`

---

**Documentation Complete**  
**Build Status:** ✅ All features implemented and tested  
**Ready for Frontend Integration**
