# QueueLess Backend

## Overview

QueueLess is a Smart Digital Queue Management System that allows customers to join queues remotely, track their position in real-time, and receive notifications as they get closer to being served.

This repository contains the **Backend API** built with ASP.NET Core 8.0 following Clean Architecture principles.

---

## Architecture

The solution follows **Clean Architecture** (also known as Onion Architecture), separating concerns into four distinct layers:

```
QueueLess.API          ← Presentation layer (Controllers, Middleware, DI wiring)
       ↓
QueueLess.Application  ← Business logic (Services, Interfaces, DTOs)
       ↓
QueueLess.Domain       ← Core domain (Entities, Enums, Business rules)

QueueLess.Infrastructure ← Data access (EF Core, Repositories, Security)
       ↓ (implements Application interfaces)
```

### Dependency Direction

| Project | References |
|---|---|
| `QueueLess.Domain` | None |
| `QueueLess.Application` | `QueueLess.Domain` |
| `QueueLess.Infrastructure` | `QueueLess.Application`, `QueueLess.Domain` |
| `QueueLess.API` | `QueueLess.Application`, `QueueLess.Infrastructure` |

---

## Project Structure

```
FAD-QuLessService/
├── .gitignore
├── README.md
└── QueueLess/
    ├── QueueLess.sln
    ├── QueueLess.API/
    │   ├── Controllers/
    │   │   └── AuthController.cs
    │   ├── Middleware/
    │   │   └── GlobalExceptionMiddleware.cs
    │   ├── Models/
    │   │   └── ApiResponse.cs
    │   ├── Program.cs
    │   └── appsettings.json
    ├── QueueLess.Application/
    │   ├── DTOs/
    │   │   └── AuthDtos.cs
    │   ├── Interfaces/
    │   │   ├── IAuthService.cs
    │   │   ├── IPasswordHasher.cs
    │   │   ├── ITokenService.cs
    │   │   ├── IUnitOfWork.cs
    │   │   └── IUserRepository.cs
    │   └── Services/
    │       └── AuthService.cs
    ├── QueueLess.Domain/
    │   ├── Entities/
    │   │   ├── Business.cs
    │   │   ├── BusinessCategory.cs
    │   │   ├── Notification.cs
    │   │   ├── PlatformAdmin.cs
    │   │   ├── Service.cs
    │   │   ├── Staff.cs
    │   │   ├── Ticket.cs
    │   │   ├── User.cs
    │   │   └── WorkingHours.cs
    │   └── Enums/
    │       ├── Role.cs
    │       └── TicketStatus.cs
    └── QueueLess.Infrastructure/
        ├── Persistence/
        │   ├── Configurations/
        │   │   └── EntityConfigurations.cs
        │   ├── Repositories/
        │   │   ├── UnitOfWork.cs
        │   │   └── UserRepository.cs
        │   └── QueueLessDbContext.cs
        └── Security/
            ├── PasswordHasher.cs
            └── TokenService.cs
```

---

## Technologies

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8.0 |
| ORM | Entity Framework Core 8.0 |
| Database | SQL Server / LocalDB |
| Authentication | JWT Bearer Tokens (`System.IdentityModel.Tokens.Jwt 8.0`) |
| Password Hashing | PBKDF2 / SHA-256 (`Rfc2898DeriveBytes` — built-in .NET) |
| API Documentation | Swagger / OpenAPI (Swashbuckle) |

---

## Database

### Entities

| Entity | Description |
|---|---|
| `User` | All system users (Customer, Staff, PlatformAdmin) |
| `Business` | A business registered on the platform |
| `BusinessCategory` | Category of a business (e.g. Bank, Clinic) |
| `Service` | A service offered by a business |
| `WorkingHours` | Operating hours per service per day |
| `Staff` | Staff member assigned to exactly one service |
| `PlatformAdmin` | Platform administrator |
| `Ticket` | A customer's queue ticket for a specific service |
| `Notification` | Queue progress notifications sent to customers |

### Key Business Constraints

- **One active ticket per customer** — enforced by a unique partial index on `Ticket.CustomerId` filtered to `Status IN ('Waiting', 'Serving')`.
- **One staff per service** — enforced by a unique FK relationship between `Staff` and `Service`.
- **Cancellation rule** — a ticket may only be cancelled when there are 10 or more people ahead.
- **Notification thresholds** — customers are notified at 10, 5, and 3 people remaining, and again when serving begins.

---

## Authentication & Authorization

- Login is via **Mobile Number + Password**.
- Passwords are hashed using **PBKDF2 with SHA-256**, 100,000 iterations, 16-byte salt (no external dependencies).
- Successful login returns a signed **JWT token**.
- Roles: `Customer`, `Staff`, `PlatformAdmin`.

---

## API Documentation

Swagger UI is available in development mode at:

```
https://localhost:<port>/swagger
```

### POC Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Public | Register a new customer account |
| `POST` | `/api/v1/auth/login` | Public | Login and receive a JWT token |

### Standard Response Shape

**Success:**
```json
{
  "userId": "guid",
  "mobileNumber": "01000000000",
  "fullName": "John Doe"
}
```

**Error:**
```json
{
  "statusCode": 400,
  "error": "ValidationError",
  "message": "One or more validation errors occurred.",
  "details": ["Mobile number is required."]
}
```

---

## Running the Project

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server or LocalDB

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/Ahmad-1-1/QueueLess-Backend.git
   cd QueueLess-Backend/QueueLess
   ```

2. Update the connection string in `QueueLess.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QueueLessDb;Trusted_Connection=True;"
   }
   ```

3. Apply the database migration:
   ```bash
   dotnet ef database update --project QueueLess.Infrastructure --startup-project QueueLess.API
   ```

4. Run the API:
   ```bash
   dotnet run --project QueueLess.API
   ```

5. Open Swagger at `https://localhost:<port>/swagger`

---

## Environment Configuration

> ⚠️ **Never commit real secrets.** The `Jwt:SecretKey` in `appsettings.json` is a placeholder for development only. Use environment variables or `dotnet user-secrets` for production secrets.

```bash
# Set secrets locally
dotnet user-secrets set "Jwt:SecretKey" "your-real-secret-key" --project QueueLess.API
```

---

## Team Contribution

| Role | Responsibility |
|---|---|
| Backend Developer 01 (Team Lead) | Architecture, Domain, Database, EF Core, Auth foundation |
| Backend Developer 02 | API structure, Controllers, DTOs, Services, Repositories, POC endpoints |

---

## Current Task Status

**Task 04 — Development Foundation:** ✅ Completed

- [x] Backend Architecture (Clean Architecture / 4-layer)
- [x] Domain Model (9 entities, 2 enums)
- [x] Database Design (EF Core Fluent API configurations)
- [x] EF Core Migration generated (`InitialMigration`)
- [x] Authentication Foundation (PBKDF2 + JWT)
- [x] API Structure (Controllers, Middleware, DI, Swagger)
- [x] POC Endpoints: `POST /api/v1/auth/register`, `POST /api/v1/auth/login`
- [x] Global Exception Handling Middleware
- [x] Standard API Response Model
