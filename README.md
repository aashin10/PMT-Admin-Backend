# PmtAdmin - User Management API

A complete User Management API built with .NET 8, implementing CQRS pattern with PostgreSQL database.

## 🚀 Features

- ✅ **CQRS Architecture** - Separation of Commands and Queries using MediatR
- ✅ **Repository Pattern** - Generic repository with user-specific extensions
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Comprehensive input validation
- ✅ **Global Exception Handling** - Centralized error management
- ✅ **PostgreSQL Integration** - Using Entity Framework Core
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Soft Delete** - Data preservation with is_deleted flag
- ✅ **Audit Fields** - Self-referencing created_by, updated_by, deleted_by

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## 🏗️ Project Structure

```
PmtAdmin/
│
├── PmtAdmin.Api/                    # API Layer
│   ├── Controllers/
│   │   └── UserController.cs        # REST API endpoints
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs
│   ├── Program.cs                   # Application entry point
│   └── appsettings.json            # Configuration
│
├── PmtAdmin.Application/            # Application Layer (CQRS)
│   ├── Command/
│   │   ├── CreateUserCommand.cs
│   │   └── Validators/
│   │       └── CreateUserCommandValidator.cs
│   ├── Query/
│   │   ├── GetAllUsersQuery.cs
│   │   └── GetUserByIdQuery.cs
│   ├── Handlers/Users/
│   │   ├── CreateUserCommandHandler.cs
│   │   ├── GetAllUsersQueryHandler.cs
│   │   └── GetUserByIdQueryHandler.cs
│   ├── Dto/
│   │   └── UserDto.cs
│   ├── Wrappers/
│   │   └── ApiResponse.cs
│   ├── MappingProfiles/
│   │   └── UserProfile.cs
│   └── ApplicationServiceRegistration.cs
│
├── PmtAdmin.Domain/                 # Domain Layer
│   ├── Entities/
│   │   └── Users.cs                # User entity
│   └── Persistance/
│       ├── IGenericRepository.cs
│       └── IUserRepository.cs
│
└── PmtAdmin.Infrastructure/         # Infrastructure Layer
    ├── Context/
    │   └── AppDbContext.cs         # EF Core DbContext
    ├── Repositories/
    │   ├── GenericRepository.cs
    │   └── UserRepository.cs
    └── PersistanceServiceRegistration.cs
```

## 🔧 Setup Instructions

### 1. Clone/Navigate to the project

```powershell
cd c:\ILP022025\Project\Back-End\PmtAdmin
```

### 2. Configure Database Connection

Edit `PmtAdmin.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PmtAdminDbConnection": "Host=localhost;Port=5432;Database=pmtadmin;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 3. Restore Dependencies

```powershell
dotnet restore
```

### 4. Create Database Migration

```powershell
dotnet ef migrations add InitialCreate --project PmtAdmin.Infrastructure --startup-project PmtAdmin.Api
```

### 5. Update Database

```powershell
dotnet ef database update --project PmtAdmin.Infrastructure --startup-project PmtAdmin.Api
```

### 6. Run the Application

```powershell
cd PmtAdmin.Api
dotnet run
```

The API will be available at:

- Swagger UI: `https://localhost:7xxx/swagger`
- API Base URL: `https://localhost:7xxx/api`

## 📡 API Endpoints

### Get All Users

```http
GET /api/user
```

**Response:**

```json
{
  "status": 200,
  "data": [
    {
      "id": 1,
      "name": "Alice Johnson",
      "email": "alice.johnson@company.com",
      "type": "Internal",
      "status": "Active",
      "created_At": "10/21/2025",
      "last_Login": "10/25/2025"
    }
  ],
  "message": "Request processed successfully"
}
```

### Get User by ID

```http
GET /api/user/{id}
```

### Create User

```http
POST /api/user
Content-Type: application/json

{
  "email": "user@example.com",
  "password_Hash": "hashed_password",
  "name": "John Doe",
  "avatar_Url": "https://example.com/avatar.jpg",
  "is_Active": true,
  "is_Super_Admin": false,
  "jira_Id": "JIRA-123",
  "type": "Internal",
  "created_By": 1
}
```

**Response:**

```json
{
  "status": 201,
  "data": {
    "id": 2,
    "name": "John Doe",
    "email": "user@example.com",
    "type": "Internal",
    "status": "Active",
    "created_At": "10/21/2025",
    "last_Login": null
  },
  "message": "User created successfully"
}
```

## 🗄️ Database Schema

**Users Table:**

| Column         | Type          | Constraints      | Description                              |
| -------------- | ------------- | ---------------- | ---------------------------------------- |
| id             | serial        | PRIMARY KEY      | Auto-incrementing ID                     |
| email          | varchar(255)  | UNIQUE, NOT NULL | User email                               |
| password_hash  | varchar(1024) | NULLABLE         | Hashed password                          |
| name           | varchar(150)  |                  | User full name                           |
| avatar_url     | varchar(1000) |                  | Profile picture URL                      |
| is_active      | boolean       | DEFAULT true     | Active status                            |
| is_super_admin | boolean       | DEFAULT false    | Admin flag                               |
| last_login     | timestamp     | NULLABLE         | Last login time                          |
| created_at     | timestamp     | DEFAULT now()    | Creation timestamp                       |
| updated_at     | timestamp     | NULLABLE         | Last update time                         |
| deleted_at     | timestamp     | NULLABLE         | Soft delete time                         |
| jira_id        | varchar(1024) |                  | Jira user ID                             |
| type           | varchar       |                  | User type (Internal/External/Contractor) |
| created_by     | int           | FK → users.id    | Created by user                          |
| updated_by     | int           | FK → users.id    | Updated by user                          |
| deleted_by     | int           | FK → users.id    | Deleted by user                          |
| is_deleted     | boolean       | DEFAULT false    | Soft delete flag                         |

**Indexes:**

- `email` (unique)
- `is_deleted`

## 🧪 Testing with Sample Data

After running migrations, you can insert test data:

```sql
-- See sample_data.sql for complete script
INSERT INTO users (email, name, type, is_active, is_super_admin, created_at, is_deleted)
VALUES
    ('alice.johnson@company.com', 'Alice Johnson', 'Internal', true, true, CURRENT_TIMESTAMP, false),
    ('bob.smith@company.com', 'Bob Smith', 'Internal', true, false, CURRENT_TIMESTAMP, false);
```

## 📚 Technologies Used

- **.NET 8** - Framework
- **Entity Framework Core 9** - ORM
- **PostgreSQL** - Database
- **MediatR** - CQRS implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Validation
- **Swashbuckle** - Swagger/OpenAPI

## 🔐 Status Codes

- `200` - OK (Success)
- `201` - Created (Resource created successfully)
- `400` - Bad Request (Validation error)
- `404` - Not Found (Resource not found)
- `500` - Internal Server Error

## 📝 Response Format

All API responses follow this structure:

```json
{
  "status": 200,
  "data": {
    /* response data */
  },
  "message": "Request processed successfully"
}
```

**Error Response:**

```json
{
  "status": 400,
  "data": null,
  "message": "Validation error",
  "errors": ["Email is required", "Name cannot exceed 150 characters"]
}
```

## 🚧 Future Enhancements

- [ ] Update User endpoint (PUT)
- [ ] Soft Delete User endpoint (DELETE)
- [ ] User search and filtering
- [ ] Pagination support
- [ ] JWT Authentication
- [ ] Role-based authorization
- [ ] Logging with Serilog
- [ ] Unit & Integration tests
- [ ] Docker containerization

## 📖 Documentation

- [Quick Start Guide](QUICK_START.md) - Step-by-step setup
- [Implementation Summary](IMPLEMENTATION_SUMMARY.md) - Architecture details
- [Sample Data](sample_data.sql) - Test data script

## 👥 Contributing

This is an internal project. For questions or issues, contact the development team.

## 📄 License

Internal use only - ILP022025 Project

---

**Created:** October 21, 2025  
**Project:** PmtAdmin Backend  
**Framework:** .NET 8 with CQRS Pattern
