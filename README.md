# QueueLess Backend

QueueLess is a queue management platform designed to help customers discover businesses, view available services, join queues, and track their queue status without waiting physically at the location.

This repository contains the backend REST API built with **ASP.NET Core .NET 8**, following a layered architecture with clear separation of concerns.

---

## Project Status

### Implemented

- .NET 8 Web API
- Layered architecture
- SQL Server database
- Entity Framework Core
- EF Core migrations
- Repository pattern
- Unit of Work
- JWT authentication
- Password hashing
- OTP infrastructure
- Email service
- Global exception handling
- JWT token blacklist after logout
- Swagger/OpenAPI
- Business categories
- Business discovery
- Business search and filtering
- Business details
- Home page API
- Location-based business recommendations
- Distance calculation using latitude/longitude
- Business popularity score
- Picture URL resolution
- Database seeding

### Planned / In Progress

- Queue joining workflow
- Active ticket workflow
- Real-time queue tracking
- SignalR queue events
- Customer notifications
- Staff queue management
- Business management
- Platform administration

---

# Architecture

QueueLess follows a layered architecture:

```text
QueueLess
│
├── QueueLess.API
├── QueueLess.Application
├── QueueLess.Domain
└── QueueLess.Infrastructure
```

### Dependency Flow

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ├── Application
 └── Domain
```

The architecture keeps business logic independent from infrastructure and framework-specific concerns.

---

# Project Structure

## QueueLess.API

Responsible for exposing HTTP endpoints and configuring the application.

Contains:

- Controllers
- Middleware
- Authentication configuration
- Authorization
- Swagger configuration
- Dependency Injection
- HTTP request pipeline

Example:

```text
Controllers/
├── AuthController.cs
├── UsersController.cs
├── BusinessesController.cs
├── HomeController.cs
└── ...
```

## QueueLess.Application

Contains application-level business logic and contracts.

```text
Application/
├── DTOs/
├── Interfaces/
└── Services/
```

Examples:

- `IHomeService`
- `IBusinessRepository`
- `IUserRepository`
- `IAuthService`
- `ITokenService`
- `HomeService`
- `UserService`
- `AuthService`

## QueueLess.Domain

Contains the core domain entities and enums.

```text
Domain/
├── Entities/
└── Enums/
```

Examples:

- User
- Business
- BusinessCategory
- Service
- WorkingHours
- Staff
- PlatformAdmin
- Ticket
- Notification
- OtpRequest

The Domain layer does not depend on the API or Infrastructure layers.

## QueueLess.Infrastructure

Contains database and infrastructure implementations.

```text
Infrastructure/
├── Persistence/
│   ├── Configurations/
│   ├── Repositories/
│   ├── QueueLessDbContext.cs
│   └── DbSeeder.cs
├── Migrations/
└── Security/
```

Infrastructure is responsible for:

- Entity Framework Core
- SQL Server
- Repository implementations
- Database configurations
- Database migrations
- Data seeding
- Password hashing
- JWT token generation
- Email infrastructure

---

# Database

QueueLess uses:

- SQL Server
- Entity Framework Core

The main database context is:

```csharp
QueueLessDbContext
```

Example DbSets:

```csharp
public DbSet<User> Users => Set<User>();
public DbSet<Business> Businesses => Set<Business>();
public DbSet<Service> Services => Set<Service>();
public DbSet<Ticket> Tickets => Set<Ticket>();
public DbSet<Notification> Notifications => Set<Notification>();
```

---

# Authentication

QueueLess uses JWT Bearer Authentication.

Authentication flow:

```text
Register / Login
      ↓
Authentication
      ↓
JWT Token
      ↓
Authenticated Requests
```

JWT configuration includes:

- Issuer
- Audience
- Secret key
- Token lifetime validation
- Signing key validation

Swagger is configured to support Bearer authentication.

---

# Home API

The Home API provides the data required by the application's home screen.

Endpoint:

```http
GET /api/v1/home
```

The endpoint supports customer location information through:

```text
latitude
longitude
```

Example:

```http
GET /api/v1/home?latitude=30.7865&longitude=31.0004
```

The response contains:

```json
{
  "userLocation": {
    "latitude": 30.7865,
    "longitude": 31.0004
  },
  "categories": [],
  "popularServices": [],
  "recommendedServices": []
}
```

---

# Location-Based Recommendations

The Home API uses the customer's coordinates to determine nearby businesses.

Business entities contain:

```text
Latitude
Longitude
```

The application calculates the geographic distance between:

```text
Customer Location
        ↓
Business Location
```

The calculated distance is represented as:

```text
DistanceKm
```

This allows the application to prioritize businesses based on proximity.

---

# Business Popularity

Businesses contain:

```text
PopularityScore
```

The score can be used when ranking recommended businesses.

Recommendation logic can consider:

```text
Distance
+
Popularity
+
Business status
+
Category
+
Search criteria
```

This allows the Home screen to provide more useful recommendations.

---

# Business Discovery

The Business API supports retrieving businesses with optional filters.

Supported filters include:

- Category
- Search term
- Location

Examples:

```http
GET /api/v1/businesses
```

```http
GET /api/v1/businesses?category=Hospital
```

```http
GET /api/v1/businesses?search=clinic
```

```http
GET /api/v1/businesses?location=Tanta
```

Filters can also be combined.

---

# Business Details

A specific business can be retrieved using its ID.

```http
GET /api/v1/businesses/{id}
```

Business details can include:

- Business information
- Category
- Services
- Working hours

---

# Picture Resolver

QueueLess uses a dedicated picture resolution service instead of spreading image fallback logic throughout the application.

Abstraction:

```csharp
IPictureResolver
```

Implementation:

```csharp
PictureResolver
```

Configuration:

```csharp
PictureOptions
```

This keeps image URL handling centralized and makes it easier to change image sources later without modifying business logic.

The Home service uses the resolver for image URLs used by:

- Categories
- Popular services
- Recommended businesses

---

# Repository Pattern

Database access is separated from application business logic through repository interfaces.

Example:

```csharp
IBusinessRepository
```

Implementation:

```csharp
BusinessRepository
```

The repository provides operations such as:

```csharp
GetCategoriesAsync()
GetRecommendedBusinessesAsync(...)
GetByIdAsync(...)
AddCategoryAsync(...)
AddBusinessAsync(...)
```

This keeps Entity Framework Core implementation details inside the Infrastructure layer.

---

# Unit of Work

QueueLess uses a Unit of Work abstraction:

```csharp
IUnitOfWork
```

This provides a centralized way to persist database changes.

---

# Database Seeding

The application contains:

```csharp
DbSeeder
```

The seeder creates initial development data such as:

- Business categories
- Businesses
- Services
- Related data

---

# Entity Configurations

Entity Framework Core configurations are separated from the DbContext using:

```csharp
IEntityTypeConfiguration<T>
```

Examples include:

```text
UserConfiguration
BusinessCategoryConfiguration
BusinessConfiguration
ServiceConfiguration
WorkingHoursConfiguration
StaffConfiguration
PlatformAdminConfiguration
TicketConfiguration
NotificationConfiguration
```

This keeps the DbContext clean and makes database constraints easier to maintain.

---

# Queue System

The main QueueLess customer journey is designed around:

```text
Login
  ↓
Browse Businesses
  ↓
Browse Services
  ↓
Join Queue
  ↓
Active Ticket
  ↓
Live Queue Tracking
  ↓
Served
```

A customer can have at most one active ticket at a time.

Active ticket states include:

```text
Waiting
Serving
```

The active ticket is intended to be displayed in its dedicated ticket/tracking screen rather than as a permanent item on the Home screen.

---

# Notifications

QueueLess is designed to notify customers about important queue changes.

Notification thresholds include:

```text
10 people remaining
5 people remaining
3 people remaining
Serving
```

Notifications are associated with:

```text
User
Ticket
```

---

# Real-Time Queue Updates

SignalR is planned for real-time queue communication.

Expected flow:

```text
Staff changes queue
        ↓
Backend
        ↓
SignalR Event
        ↓
Customer Application
        ↓
Updated Queue Position
```

The goal is to allow the customer application to receive queue updates without continuously polling the API.

---

# Technologies

| Technology | Purpose |
|---|---|
| .NET 8 | Backend framework |
| ASP.NET Core Web API | REST API |
| Entity Framework Core | ORM |
| SQL Server | Relational database |
| JWT | Authentication |
| Swagger / OpenAPI | API documentation |
| SignalR | Real-time communication |
| C# | Programming language |
| Git / GitHub | Version control |

---

# API Testing

Swagger is available during development.

Run the API and open:

```text
https://localhost:<port>/swagger
```

Swagger can be used to:

- Explore endpoints
- Send requests
- Test query parameters
- Test authentication
- Inspect JSON responses

---

# EF Core Migrations

Database changes are managed through Entity Framework Core migrations.

Create a migration:

```powershell
Add-Migration MigrationName
```

Apply migrations:

```powershell
Update-Database
```

Migration files are stored under:

```text
QueueLess.Infrastructure/Migrations
```

---

# Running the Project

## 1. Clone the repository

```bash
git clone <repository-url>
```

## 2. Open the solution

Open:

```text
QueueLess.sln
```

using Visual Studio.

## 3. Configure the database

Update the connection string in:

```text
QueueLess.API/appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

## 4. Configure JWT

Configure:

```json
{
  "Jwt": {
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "..."
  }
}
```

## 5. Apply migrations

```powershell
Update-Database
```

## 6. Run the API

Run the project using Visual Studio.

Swagger should then be available at:

```text
https://localhost:<port>/swagger
```

---

# Current API Examples

### Home

```http
GET /api/v1/home
```

Example:

```http
GET /api/v1/home?latitude=30.7865&longitude=31.0004
```

### Businesses

```http
GET /api/v1/businesses
```

### Business Details

```http
GET /api/v1/businesses/{id}
```

---

# Future Improvements

The backend will continue expanding toward the complete QueueLess workflow.

Planned areas include:

- Queue creation
- Ticket management
- Real-time queue tracking
- SignalR events
- Staff operations
- Business owner operations
- Notifications
- Appointment/queue rules
- Platform administration
- More advanced recommendation logic
- Production-ready image storage
- Production deployment

---

# Development Principles

The project follows several software engineering principles:

- Separation of Concerns
- Dependency Injection
- Repository Pattern
- Unit of Work
- DTO Pattern
- SOLID principles
- Layered Architecture
- Centralized Exception Handling
- Configuration-based infrastructure

The goal is to keep the backend maintainable, testable, and ready for future feature expansion.

---

# QueueLess

QueueLess aims to replace physical waiting with a smarter digital queue experience.

```text
Discover
   ↓
Choose Service
   ↓
Join Queue
   ↓
Track Queue
   ↓
Get Served
```
