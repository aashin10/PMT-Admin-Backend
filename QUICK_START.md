# Quick Start Guide - PmtAdmin Users API

## Prerequisites

1. PostgreSQL installed and running
2. .NET 8 SDK installed
3. Visual Studio 2022 or VS Code

## Step 1: Configure Database Connection

Edit `PmtAdmin.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PmtAdminDbConnection": "Host=localhost;Port=5432;Database=pmtadmin;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  }
}
```

Replace:

- `YOUR_USERNAME` with your PostgreSQL username
- `YOUR_PASSWORD` with your PostgreSQL password

## Step 2: Create Database Migration

Open PowerShell and navigate to the PmtAdmin folder:

```powershell
cd c:\ILP022025\Project\Back-End\PmtAdmin
```

### Using .NET CLI (Recommended)

```powershell
# Add migration
dotnet ef migrations add InitialCreate --project PmtAdmin.Infrastructure --startup-project PmtAdmin.Api

# Update database
dotnet ef database update --project PmtAdmin.Infrastructure --startup-project PmtAdmin.Api
```

### Using Package Manager Console (Visual Studio)

1. Open Visual Studio
2. Tools → NuGet Package Manager → Package Manager Console
3. Set Default Project to `PmtAdmin.Infrastructure`
4. Run:

```powershell
Add-Migration InitialCreate
Update-Database
```

## Step 3: Run the Application

```powershell
cd PmtAdmin.Api
dotnet run
```

Or press F5 in Visual Studio.

## Step 4: Test the API

The application will start with Swagger UI automatically.

Navigate to: `https://localhost:7xxx/swagger` (port number will be shown in console)

### Available Endpoints:

1. **GET /api/user** - Get all users
2. **GET /api/user/{id}** - Get user by ID
3. **POST /api/user** - Create a new user

### Sample POST Request:

```json
{
  "email": "alice.johnson@company.com",
  "password_Hash": "$2a$10$abcdefghijklmnopqrstuvwxyz",
  "name": "Alice Johnson",
  "avatar_Url": "https://example.com/avatar.jpg",
  "is_Active": true,
  "is_Super_Admin": false,
  "jira_Id": "JIRA-001",
  "type": "Internal",
  "created_By": null
}
```

### Expected Response:

```json
{
  "status": 201,
  "data": {
    "id": 1,
    "name": "Alice Johnson",
    "email": "alice.johnson@company.com",
    "type": "Internal",
    "status": "Active",
    "created_At": "10/21/2025",
    "last_Login": null
  },
  "message": "User created successfully"
}
```

## Troubleshooting

### Error: "No DbContext was found"

- Make sure you're in the correct directory
- Verify the startup project is `PmtAdmin.Api`

### Error: "Connection refused"

- Check if PostgreSQL is running
- Verify connection string credentials
- Ensure PostgreSQL is listening on port 5432

### Error: "Database does not exist"

- The migration will create the database automatically
- Or manually create database: `CREATE DATABASE pmtadmin;`

### Build Errors

If you see build errors after creating files:

1. Close and reopen Visual Studio/VS Code
2. Rebuild the solution: `dotnet build`
3. Restore packages: `dotnet restore`

## Project Structure

```
PmtAdmin/
├── PmtAdmin.Api/            # API Layer (Controllers, Middleware)
├── PmtAdmin.Application/    # Business Logic (CQRS, Handlers, DTOs)
├── PmtAdmin.Domain/         # Entities and Interfaces
└── PmtAdmin.Infrastructure/ # Data Access (DbContext, Repositories)
```

## Next Features to Implement

- Update User (PUT endpoint)
- Delete User (Soft delete)
- Pagination and filtering
- Authentication & Authorization
- User search functionality
- Audit logging

## Notes

- The API uses soft delete (is_deleted flag) instead of hard delete
- Dates are formatted as MM/dd/yyyy in responses
- Email must be unique in the database
- All timestamps are stored in UTC
