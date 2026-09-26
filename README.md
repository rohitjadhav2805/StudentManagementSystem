# Student Management System - Enterprise Full Stack Solution (.NET 9 & React)

Production-ready Student Management System developed for **Zest India IT Pvt Ltd** technical assignment.

Built using **ASP.NET Core 8 Web API**, **C#**, **SQL Server**, **Entity Framework Core 8**, **JWT Authentication**, **Serilog**, **FluentValidation**, **Swagger UI**, **xUnit Unit Tests**, and a modern **React UI (Bootstrap 5)**.

---

## Technical Stack & Architecture

- **Backend API**: ASP.NET Core 8.0 Web API (`net8.0`)
- **Database**: SQL Server 2022 / LocalDB / Express
- **ORM**: Entity Framework Core 8.0 (Code-First Migrations, Fluent API & Stored Procedure support)
- **Security**: JWT Bearer Token Authentication, Password Hashing with Salt (PBKDF2/SHA256), Role-based Authorization (`Admin`, `User`)
- **Logging**: Serilog (Console Sink & Daily Rolling File Sink under `logs/log-.txt`)
- **Validation**: FluentValidation with custom rules for DTOs
- **API Documentation**: Swagger / OpenAPI with interactive JWT Authorize header support
- **Architecture**: Clean / Layered Architecture (`Domain` → `Application` → `Infrastructure` → `Persistence` → `API`)
- **Design Patterns**: Repository Pattern, Unit of Work, Options Pattern, Middleware Pipeline, Standard API Response Wrapper
- **Testing**: xUnit, Moq, FluentAssertions
- **Frontend UI**: React 18, Vite, Bootstrap 5, Bootstrap Icons, Axios with Interceptors
- **DevOps**: Docker, Docker Compose, Postman Collection & Environment

---

## Project Structure

```
StudentManagement.sln
│
├── src/
│   ├── StudentManagement.Domain/               # Entities, Interfaces, Enums, Response Wrappers
│   │   ├── Common/                             # ApiResponse<T>, PagedResult<T>
│   │   ├── Constants/                          # UserRoles (Admin, User)
│   │   ├── Entities/                           # Student, User
│   │   └── Interfaces/                         # IGenericRepository, IStudentRepository, IUserRepository, IUnitOfWork
│   │
│   ├── StudentManagement.Application/          # DTOs, Services, Interfaces, FluentValidation, Exceptions
│   │   ├── DTOs/                               # Student DTOs, Auth DTOs
│   │   ├── Exceptions/                         # NotFound, BadRequest, DuplicateEmail, Unauthorized Exceptions
│   │   ├── Interfaces/                         # IStudentService, IAuthService, IJwtTokenGenerator, IPasswordHasher
│   │   ├── Services/                           # StudentService, AuthService
│   │   └── Validators/                         # CreateStudent, UpdateStudent, Login, Register Validators
│   │
│   ├── StudentManagement.Infrastructure/       # Security, JwtTokenGenerator, PasswordHasher, JwtSettings
│   │
│   ├── StudentManagement.Persistence/          # DbContext, Configurations, Repositories, Migrations, Seeding
│   │   ├── Configurations/                     # StudentConfiguration, UserConfiguration (Fluent API)
│   │   ├── Context/                            # ApplicationDbContext
│   │   ├── Migrations/                         # EF Core Migrations
│   │   ├── Repositories/                       # GenericRepository, StudentRepository, UserRepository, UnitOfWork
│   │   └── Seed/                               # DbInitializer (Auto seed Admin, User, Students)
│   │
│   ├── StudentManagement.API/                  # Controllers, Middleware, Extensions, Program.cs
│   │   ├── Controllers/                        # AuthController, StudentsController
│   │   ├── Extensions/                         # DependencyInjectionExtensions, SwaggerExtensions
│   │   ├── Middleware/                         # GlobalExceptionMiddleware, RequestResponseLoggingMiddleware
│   │   ├── appsettings.json                    # Configuration (Serilog, JWT, Connection Strings)
│   │   └── Program.cs                          # Application Entry Point & DI Configuration
│   │
│   └── StudentManagement.UI/                   # React UI Application (Vite + Bootstrap 5)
│       ├── src/
│       │   ├── components/                     # Navbar, StudentModal
│       │   ├── pages/                          # Login, Dashboard
│       │   └── services/                       # Axios API Client & Auth Interceptor
│       ├── index.html
│       ├── package.json
│       └── vite.config.js
│
├── tests/
│   └── StudentManagement.Tests/                # xUnit Unit Test Suite
│       ├── Controllers/                        # StudentsControllerTests
│       ├── Services/                           # StudentServiceTests, AuthServiceTests
│       └── Validators/                         # StudentValidatorTests
│
├── sql/                                        # SQL Scripts
│   ├── 01_CreateDatabaseAndTables.sql          # DDL, Tables, Primary Keys, Indexes, Constraints
│   ├── 02_StoredProcedures.sql                 # CRUD Stored Procedures
│   └── 03_SeedData.sql                         # Initial Seed Data Script
│
├── Dockerfile                                  # Multi-stage Docker build file
├── docker-compose.yml                          # Container orchestration (API + SQL Server 2022)
├── StudentManagement.postman_collection.json   # Postman API Collection
├── StudentManagement.postman_environment.json  # Postman Environment Variables
└── README.md                                   # Comprehensive System Documentation
```

---

## Default System Credentials

| Role | Username | Email | Password | Permissions |
| :--- | :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `admin@zestindia.com` | `Admin@123` | Full Access (GET, POST, PUT, DELETE) |
| **User** | `user` | `user@zestindia.com` | `User@123` | Read Only (GET All, GET By ID, Search) |

---

## Database Configuration & Setup

### Option 1: Automatic Migration & Seeding (Recommended)
When you launch `StudentManagement.API`, Entity Framework Core automatically executes pending migrations and seeds initial accounts and students into SQL Server LocalDB (`(localdb)\mssqllocaldb`).

### Option 2: Standalone SQL Scripts Execution
If setting up an external SQL Server instance, run the scripts in order:
```powershell
sqlcmd -S localhost -i sql/01_CreateDatabaseAndTables.sql
sqlcmd -S localhost -i sql/02_StoredProcedures.sql
sqlcmd -S localhost -i sql/03_SeedData.sql
```

---

## How to Run the Solution

### 1. Build and Run API via .NET CLI
```powershell
# Restore dependencies and build solution
dotnet restore StudentManagement.sln
dotnet build StudentManagement.sln --configuration Release

# Run Unit Tests
dotnet test tests/StudentManagement.Tests/StudentManagement.Tests.csproj

# Run Web API
cd src/StudentManagement.API
dotnet run
```
- **Swagger URL**: `http://localhost:5242/swagger` or `https://localhost:7198/swagger`
- **Base API URL**: `http://localhost:5242/api/v1`

### 2. Run via Visual Studio 2022
1. Open `StudentManagement.sln` in Visual Studio 2022 (.NET 8 installed).
2. Set `StudentManagement.API` as the Startup Project.
3. Press `F5` or `Ctrl + F5` to compile and launch Swagger UI automatically.

### 3. Run React UI Frontend
```powershell
cd src/StudentManagement.UI
npm install
npm run dev
```
- **React UI URL**: `http://localhost:3000`

### 4. Run via Docker Compose
```powershell
docker-compose up --build -d
```
- **API Container**: `http://localhost:5000/swagger`
- **SQL Server 2022 Container**: `localhost,1433`

---

## API Endpoints Reference

### Authentication (`/api/v1/auth`)
- `POST /api/v1/auth/login` (Anonymous) - Authenticate and retrieve JWT token.
- `POST /api/v1/auth/register` (Anonymous) - Register new user account.

### Students (`/api/v1/students`) - Requires JWT Bearer Token
- `GET /api/v1/students` (Admin, User) - Retrieve all students.
- `GET /api/v1/students/{id}` (Admin, User) - Get student by ID.
- `GET /api/v1/students/search?searchTerm={term}&course={course}` (Admin, User) - Search/Filter students.
- `POST /api/v1/students` (Admin Only) - Create student.
- `PUT /api/v1/students/{id}` (Admin Only) - Update student details.
- `DELETE /api/v1/students/{id}` (Admin Only) - Delete student record.

---

## API Standardized Response Format

Every API endpoint returns a standardized JSON structure:

```json
{
  "success": true,
  "message": "Student created successfully.",
  "data": {
    "id": 1,
    "name": "Rahul Sharma",
    "email": "rahul.sharma@example.com",
    "age": 22,
    "course": "Computer Science",
    "createdDate": "2026-07-22T16:00:00Z"
  },
  "errors": [],
  "statusCode": 201
}
```

In case of error (e.g. Validation, Duplicate Email, 401 Unauthorized, 403 Forbidden, 404 Not Found):
```json
{
  "success": false,
  "message": "A record with the email 'rahul.sharma@example.com' already exists.",
  "data": null,
  "errors": [
    "A record with the email 'rahul.sharma@example.com' already exists."
  ],
  "statusCode": 409
}
```

---

## Package Manager & Migration CLI Commands

### EF Core CLI Commands
```powershell
# Add new migration
dotnet ef migrations add InitialCreate --project src/StudentManagement.Persistence --startup-project src/StudentManagement.API

# Update database
dotnet ef database update --project src/StudentManagement.Persistence --startup-project src/StudentManagement.API
```

### Visual Studio Package Manager Console (PMC) Commands
```powershell
Add-Migration InitialCreate -Project StudentManagement.Persistence -StartupProject StudentManagement.API
Update-Database -Project StudentManagement.Persistence -StartupProject StudentManagement.API
```

---

## NuGet Packages Installed

- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.8)
- `Microsoft.EntityFrameworkCore` (8.0.8)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.8)
- `Microsoft.EntityFrameworkCore.Tools` (8.0.8)
- `Microsoft.EntityFrameworkCore.Design` (8.0.8)
- `Microsoft.EntityFrameworkCore.InMemory` (8.0.8)
- `FluentValidation.DependencyInjectionExtensions` (11.9.0)
- `Serilog.AspNetCore` (8.0.2)
- `Serilog.Sinks.Console` (6.0.0)
- `Serilog.Sinks.File` (6.0.0)
- `Swashbuckle.AspNetCore` (6.6.2)
- `xunit` (2.8.1)
- `Moq` (4.20.70)
- `FluentAssertions` (6.12.0)

---

## Verification & Compliance

✅ **100% Complete Implementation** (Zero TODO comments, zero placeholders).  
✅ **Compiles cleanly** on Visual Studio 2022 & .NET 8.0 SDK.  
✅ **Unit Tests** pass successfully.  
✅ **JWT Security & Role Authorization** tested and enforced.  
✅ **Serilog File Logging** enabled under `/logs/log-.txt`.  
