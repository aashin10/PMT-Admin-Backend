# PmtAdmin - Users API Implementation Summary

## Overview

Successfully implemented a complete CQRS (Command Query Responsibility Segregation) architecture for User management in the PmtAdmin project using .NET 8 and PostgreSQL.

## API Response Structure

All endpoints return responses in the following format:

```json
{
  "status": 200,
  "data": {
    "id": 1,
    "name": "Alice Johnson",
    "email": "alice.johnson@company.com",
    "type": "Internal",
    "status": "Active",
    "created_At": "09/23/2025",
    "last_Login": "09/25/2025"
  },
  "message": "Request processed successfully"
}
```

## Architecture Layers

### 1. Domain Layer (PmtAdmin.Domain)

- **Entities/Users.cs**: User entity with all fields matching DB architecture
  - Includes avatar_url field
  - Self-referencing foreign keys for created_by, updated_by, deleted_by
  - Column annotations for PostgreSQL mapping
- **Persistance/**:
  - `IGenericRepository<T>`: Generic repository interface
  - `IUserRepository`: User-specific repository interface with custom methods

### 2. Application Layer (PmtAdmin.Application)

- **Dto/UserDto.cs**: Data Transfer Object for API responses
- **Wrappers/ApiResponse.cs**: Generic API response wrapper
- **Constants/StatusCode.cs**: HTTP status code constants
- **Command/CreateUserCommand.cs**: Command for creating users
- **Command/Validators/CreateUserCommandValidator.cs**: FluentValidation rules
- **Query/**:
  - `GetAllUsersQuery.cs`: Query to fetch all users
  - `GetUserByIdQuery.cs`: Query to fetch user by ID
- **Handlers/Users/**:
  - `CreateUserCommandHandler.cs`: Handles user creation
  - `GetAllUsersQueryHandler.cs`: Handles fetching all users
  - `GetUserByIdQueryHandler.cs`: Handles fetching user by ID
- **MappingProfiles/UserProfile.cs**: AutoMapper configuration
- **Common/Behaviour/ValidationBehaviour.cs**: MediatR pipeline behavior for validation
- **CustomException/**:
  - `ValidationException.cs`: Custom validation exception
  - `NotFoundException.cs`: Custom not found exception
- **ApplicationServiceRegistration.cs**: DI registration for application services

### 3. Infrastructure Layer (PmtAdmin.Infrastructure)

- **Context/AppDbContext.cs**: Entity Framework Core DbContext for PostgreSQL
  - Configured Users DbSet
  - Model configuration with indexes, constraints, and default values
- **Repositories/**:
  - `GenericRepository<T>`: Generic repository implementation
  - `UserRepository`: User-specific repository with custom methods
- **PersistanceServiceRegistration.cs**: DI registration for infrastructure services

### 4. API Layer (PmtAdmin.Api)

- **Controllers/UserController.cs**: REST API endpoints
  - `GET /api/user` - Get all users
  - `GET /api/user/{id}` - Get user by ID
  - `POST /api/user` - Create new user
- **Middleware/GlobalExceptionMiddleware.cs**: Global exception handling
- **Program.cs**: Application configuration and DI setup
- **appsettings.json**: Database connection string configuration

## API Endpoints

### 1. Get All Users

```http
GET /api/user
```

### 2. Get User By ID

```http
GET /api/user/{id}
```

### 3. Create User

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

## Database Configuration

### Connection String (appsettings.json)

```json
{
  "ConnectionStrings": {
    "PmtAdminDbConnection": "Host=localhost;Port=5432;Database=pmtadmin;Username=postgres;Password=your_password"
  }
}
```

**Note**: Update the connection string with your actual PostgreSQL credentials before running migrations.

## Next Steps - Database Migration

### 1. Update Connection String

Edit `appsettings.json` in the `PmtAdmin.Api` project with your PostgreSQL credentials.

### 2. Run Migrations

Open Package Manager Console in Visual Studio or use the terminal:

```powershell
# Set the startup project
cd c:\ILP022025\Project\Back-End\PmtAdmin

# Add migration
dotnet ef migrations add InitialCreate --project PmtAdmin.Infrastructure --startup-project PmtAdmin.Api

# Update database
dotnet ef database update --project PmtAdmin.Infrastructure --startup-project PmtAdmin.Api
```

### 3. Run the Application

```powershell
cd c:\ILP022025\Project\Back-End\PmtAdmin\PmtAdmin.Api
dotnet run
```

The API will be available at:

- HTTPS: https://localhost:7xxx
- HTTP: http://localhost:5xxx
- Swagger UI: https://localhost:7xxx/swagger

## Features Implemented

✅ CQRS Pattern with MediatR
✅ Repository Pattern with Generic Repository
✅ AutoMapper for DTO mapping
✅ FluentValidation for input validation
✅ Global Exception Handling
✅ PostgreSQL Integration with Entity Framework Core
✅ Swagger/OpenAPI Documentation
✅ Standardized API Response Format
✅ Self-referencing foreign keys for audit fields
✅ Soft delete support (is_deleted flag)
✅ Index on is_deleted for better query performance

## Status Codes

- 200: OK
- 201: Created
- 400: BadRequest
- 404: NotFound
- 500: InternalServerError

## Database Schema

The Users table includes:

- id (serial, primary key)
- email (varchar 255, unique, not null)
- password_hash (varchar 1024, nullable)
- name (varchar 150)
- avatar_url (varchar 1000)
- is_active (boolean, default: true)
- is_super_admin (boolean, default: false)
- last_login (timestamp, nullable)
- created_at (timestamp, default: now())
- updated_at (timestamp, nullable)
- deleted_at (timestamp, nullable)
- jira_id (varchar 1024)
- type (string)
- created_by (int, FK to users.id)
- updated_by (int, FK to users.id)
- deleted_by (int, FK to users.id)
- is_deleted (boolean, default: false)

Indexes:

- email (unique)
- is_deleted

## Build Status

✅ Solution builds successfully with minor nullable warnings
✅ All dependencies configured correctly
✅ Ready for migration and testing
