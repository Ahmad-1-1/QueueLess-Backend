# QueueLess — Backend Development Foundation
## Work Log & Current Implementation State

**Project:** QueueLess — Smart Digital Queue Management System  
**Track:** Backend  
**Role:** Backend Developer 01 / Backend Team Leader  
**Task:** FAD Task 04 — Development Foundation  
**Current milestone:** Repository + Visual Studio backend foundation completed and committed locally.

---

## 1. Purpose

This document records everything completed so far while starting the QueueLess Backend implementation.

It is a working reference for continuing implementation without losing:
- approved business decisions,
- Git/GitHub workflow,
- Visual Studio structure,
- architecture decisions,
- problems encountered and fixes,
- current Git state,
- next implementation steps.

It does not replace the approved SRS, ERD, API Contract, or SignalR Events documents.

---

# 2. Source of Truth

Implementation must follow the current project documents:

- `QueueLess_SRS_Draft_v1_4.md`
- `01_ERD.md`
- `02_API_Contract.md`
- `03_SignalR_Events.md`
- FAD Task 04 requirements
- Latest team-approved decisions

The SRS covers Authentication, Business Discovery, Queue Management, Ticket Management, Notifications, Staff, Platform Administration, Queue Engine, Business Rules, and MVP decisions.

---

# 3. Task 04 Backend Scope

Task 04 is a **Development Foundation** task, not the complete backend.

### Backend Developer 01 — Architecture & Database

Required foundation:
1. Backend architecture.
2. Main modules.
3. Main entities.
4. Database tables.
5. Relationships.
6. Primary keys.
7. Foreign keys.
8. Important fields.
9. Authentication requirements.
10. Authorization requirements.
11. ERD/database diagram.
12. Database structure based on the SRS.
13. Initial backend project structure.

### Backend Developer 02 — API Foundation

Required foundation:
1. API project structure.
2. Controllers.
3. Services.
4. Repositories.
5. Models/DTOs.
6. Middleware.
7. Configuration.
8. Endpoint planning from SRS/API Contract.
9. 1–2 important working POC endpoints.
10. Request validation.
11. Processing.
12. Structured responses.
13. Basic errors.
14. API documentation/Postman.

### Backend deliverables

- Backend Architecture
- Database Design
- ERD
- API Structure
- API Documentation
- 1–2 Working Endpoints

---

# 4. What We Are NOT Building Yet

Not required as part of the foundation task:

- Complete authentication module
- Complete business module
- Complete queue engine
- Complete ticket module
- Complete notification system
- Complete SignalR implementation
- All production endpoints
- Full production database seeding
- Full Flutter/mobile integration
- Complete unit/integration test suite
- Production deployment

Goal:

> Build a clean, understandable, SRS-aligned backend foundation that the team can confidently continue from.

---

# 5. Approved Business Decisions

## 5.1 Ticket Exit / Cancellation

Decision:

> Exit is implemented as ticket cancellation, but cancellation is allowed only while the customer has at least 10 people ahead.

```text
Waiting Ticket
      |
      +-- People Ahead >= 10 --> Cancelled
      |
      +-- People Ahead < 10 --> Cannot Cancel
```

Examples:

| People Ahead | Can Cancel? |
|---:|:---:|
| 15 | Yes |
| 10 | Yes |
| 9 | No |
| 5 | No |
| 3 | No |
| 0 / Serving | No |

Transition:

```text
Waiting -> Cancelled
```

This is a state change, not physical ticket deletion.

The rule must remain consistent in SRS, ERD, Ticket status, API Contract, application logic, validation, and error responses.

---

## 5.2 Skip

Staff can Skip, but Staff does not select an arbitrary ticket.

The Queue Engine controls progression.

```text
Current Serving Ticket
        |
       Skip
        |
        v
     Skipped
        |
        v
Next Waiting Ticket
        |
        v
     Serving
```

Rules:
- Skip acts on the current serving ticket.
- Staff cannot select a random ticket.
- Staff cannot reorder the queue.
- Staff cannot directly edit ticket state.
- Queue Engine controls progression.
- Queue positions are recalculated after progression.
- Relevant real-time events are generated.
- Required notifications are generated.

---

## 5.3 Notifications

Approved thresholds:

```text
10 people remaining
        |
        v
5 people remaining
        |
        v
3 people remaining
        |
        v
Serving
```

Serving is a separate notification/event.

Do not use older notes that mention `2`; the approved SRS decision is `10 / 5 / 3`.

---

# 6. Other Important Queue Rules

## One Active Ticket

A customer can have at most one active ticket.

```text
Customer
   |
   +---- ONE active Ticket maximum
```

## Independent Queue Per Service

```text
Business
   |
   +-- Service A -> Queue A
   +-- Service B -> Queue B
   +-- Service C -> Queue C
```

One service's tickets must not affect another service's queue.

## One Staff Per Service

Approved relationship:

```text
Staff -> exactly one assigned Service
```

This must remain consistent across ERD, database, authorization, dashboard, and API.

## Queue Engine Authority

Queue progression is controlled by the Queue Engine.

Staff actions such as Next/Skip are commands, not arbitrary direct state edits.

---

# 7. Repository Decision

Backend uses a **separate GitHub repository** from Flutter.

Conceptually:

```text
QueueLess Backend   -> Backend repository
QueueLess Flutter   -> Flutter repository
QueueLess Frontend  -> Separate repository if applicable
```

We chose separate repositories rather than a Backend + Flutter monorepo.

### Actual Backend repository name

```text
FAD-QuLessService
```

---

# 8. GitHub Workflow We Followed

Repository was created **before** implementation.

Workflow:

```text
Create GitHub Repository
        |
        v
Clone Repository
        |
        v
Create Visual Studio Solution
        |
        v
Create Projects
        |
        v
Add References
        |
        v
Build
        |
        v
Commit
        |
        v
Push
        |
        v
Continue Development
```

We will not build everything locally and upload it at the end.

---

# 9. Local Repository

Repository root:

```text
E:\1-.net internship\FAD-QuLessService
```

Current conceptual structure:

```text
FAD-QuLessService/
|
+-- .git/
+-- .github/
+-- .gitignore
+-- README.md
|
+-- QueueLess/
    |
    +-- QueueLess.sln
    +-- QueueLess.API/
    +-- QueueLess.Application/
    +-- QueueLess.Domain/
    +-- QueueLess.Infrastructure/
```

The `.vs` folder is a Visual Studio cache and is ignored by Git.

---

# 10. Visual Studio Solution

Used:

**Visual Studio 2022**

Solution:

```text
QueueLess
```

Solution file:

```text
QueueLess.sln
```

The solution currently contains four projects.

---

# 11. Projects Created

```text
QueueLess.API
QueueLess.Application
QueueLess.Domain
QueueLess.Infrastructure
```

All use:

```text
.NET 8.0
```

---

# 12. QueueLess.Domain

Type:

```text
Class Library
```

Responsibility:
- Domain entities
- Enums
- Domain rules
- Domain relationships
- Domain concepts

Dependency:

```text
None
```

Domain must not depend on API, Infrastructure, or Application.

---

# 13. QueueLess.Application

Type:

```text
Class Library
```

Responsibility:
- Application use cases
- Application services
- Interfaces/abstractions
- Application workflows
- Validation/use-case coordination

Dependency:

```text
Application -> Domain
```

---

# 14. QueueLess.Infrastructure

Type:

```text
Class Library
```

Responsibility:
- Persistence
- EF Core
- Database access
- Repository implementations
- External technical integrations

Dependencies:

```text
Infrastructure -> Application
Infrastructure -> Domain
```

---

# 15. QueueLess.API

Type:

```text
ASP.NET Core Web API
```

Configuration:
- .NET 8
- Authentication: None for initial foundation
- HTTPS enabled
- OpenAPI enabled
- Controllers enabled

Contains normal API foundation such as:

```text
Controllers/
Program.cs
appsettings.json
Properties/
```

Generated `WeatherForecast` files were deleted because they are unrelated to QueueLess.

---

# 16. Dependency Direction

Established architecture:

```text
                 QueueLess.API
                      |
                      v
            QueueLess.Application
                      |
                      v
                QueueLess.Domain

       QueueLess.Infrastructure
              |             |
              v             v
       QueueLess.Application Domain
```

Exact references:

| Project | References |
|---|---|
| QueueLess.Domain | None |
| QueueLess.Application | QueueLess.Domain |
| QueueLess.Infrastructure | QueueLess.Application, QueueLess.Domain |
| QueueLess.API | QueueLess.Application, QueueLess.Infrastructure |

Forbidden directions:

```text
Domain -> Application
Domain -> Infrastructure
Domain -> API
Application -> Infrastructure
Application -> API
Infrastructure -> API
```

---

# 17. Initial Cleanup

Removed generated files:

```text
WeatherForecast.cs
WeatherForecastController.cs
Class1.cs
```

This was done so the solution contains QueueLess-related foundation only.

---

# 18. Build Verification

After creating all four projects and references:

```text
Build -> Build Solution
```

The solution built successfully.

Confirmed:
- All four projects compile.
- References are valid.
- Dependency direction is valid.
- API project is valid.
- Foundation is ready for the next phase.

---

# 19. GitIgnore Problem We Encountered

First commit attempt failed because Git tried to access Visual Studio cache files:

```text
QueueLess/.vs/...
FileContentIndex/...
```

`.vs` is local Visual Studio state and should not be committed.

---

# 20. GitIgnore Fix

Created root file:

```text
E:\1-.net internship\FAD-QuLessService\.gitignore
```

Important rules:

```gitignore
# Visual Studio
.vs/
**/.vs/

# Build output
[Bb]in/
[Oo]bj/

# User-specific files
*.user
*.userosscache
*.suo

# Visual Studio cache
*.VC.db
*.VC.VC.opendb

# Rider
.idea/

# Test results
TestResults/

# OS files
.DS_Store
Thumbs.db
```

We removed the existing `.vs` cache folders, reopened Visual Studio, and Git Changes then showed only real project files.

---

# 21. Initial Commit

After fixing `.gitignore`, the foundation was committed successfully.

Commit message:

```text
chore: initialize backend solution
```

Current working tree:

```text
0 changes
```

This means the local working tree is clean after the initial commit.

---

# 22. Current Git State

Branch:

```text
main
```

Working tree:

```text
0 changes
```

Initial commit exists locally.

Next action is to verify/push it to GitHub if not already pushed.

---

# 23. Future Git Commit Strategy

Use small meaningful commits.

Examples:

```text
chore: initialize backend solution
chore: add domain application infrastructure api projects
feat: add initial domain entities
feat: add database configurations
chore: configure ef core
feat: add authentication foundation
feat: add api response model
feat: add global exception middleware
feat: implement register endpoint
docs: add backend architecture documentation
docs: add ERD
docs: update README
```

Avoid one giant final commit.

---

# 24. Team Git Workflow

Once feature work starts:

```text
main
 |
 +-- feature/backend-architecture
 +-- feature/database-foundation
 +-- feature/api-foundation
 +-- feature/auth-poc
```

Recommended workflow:

```text
Pull latest main
      |
      v
Create/continue feature branch
      |
      v
Implement small change
      |
      v
Build + test
      |
      v
Commit
      |
      v
Push branch
      |
      v
Pull Request
      |
      v
Backend Lead Review
      |
      v
Merge
```

Backend Team Leader reviews changes before merging into `main`.

---

# 25. Backend Team Division

## Backend Developer 01 — Team Leader

Primary:
- Backend architecture
- Domain structure
- Database foundation
- ERD review
- Entity/relationship review
- Authentication requirements
- Authorization requirements
- Queue business-rule review
- Architecture documentation
- Final backend technical review
- Coordination with Developer 02

## Backend Developer 02

Primary:
- API project structure
- Controllers
- DTOs
- Services
- Repositories
- Middleware
- Configuration
- Endpoint planning
- 1–2 working POC endpoints
- API documentation/Postman

Both developers must coordinate because API implementation depends on the Domain/Database contracts.

---

# 26. Current Domain Concepts to Verify Before Coding

The foundation plan identifies concepts around:

```text
User
Business
BusinessCategory
Service
Staff
Queue
Ticket
Notification
WorkingHours
PlatformAdmin
```

These are NOT to be implemented blindly.

Before creating entities, compare every entity against:

```text
SRS
ERD
API Contract
SignalR Events
```

Do not invent an entity just because it sounds useful.

---

# 27. Queue/Ticket Requirements

The SRS includes requirements for:

- Join Queue
- Queue Eligibility Validation
- Working Hours Validation
- Prevent Multiple Active Tickets
- Generate Digital Ticket
- Assign Queue Number
- Queue Position
- Estimated Waiting Time
- Track Active Ticket
- Queue Engine Progression
- Next Customer
- Skip Customer
- Automatic Queue Progression
- Active Ticket
- Ticket Information
- Ticket Status
- Ticket Updates
- Exit Active Ticket

These will drive the Domain/Application design.

---

# 28. Notification Requirements

The SRS includes:

- Queue Join Notification
- Queue Progress Notifications
- 10 people remaining
- 5 people remaining
- 3 people remaining
- Serving Notification
- Skip Notification
- Completion Notification
- Real-Time Updates

SignalR behavior must be checked against `03_SignalR_Events.md` before implementation.

---

# 29. Staff Requirements

Staff requirements include:

- Staff Login
- Current Serving Ticket
- Assigned Service
- Queue Summary
- Next Action
- Skip Action
- Real-Time Dashboard Updates
- Queue Availability

Staff must not:
- Reorder the queue.
- Recall customers.
- Select arbitrary tickets.
- Directly edit ticket state.
- Manage businesses/services/staff unless authorized as PlatformAdmin.

---

# 30. Authentication / Authorization Foundation

Approved authentication decision:

```text
Mobile Number + Password
```

Roles:

```text
Customer
Staff
PlatformAdmin
```

Future foundation should define:
- User identity
- Password hashing
- Authentication flow
- JWT/session strategy according to API Contract
- Role claims
- Authentication middleware
- Protected endpoints

Task 04 does not require every production authentication feature.

---

# 31. API Foundation

API structure must follow the approved API Contract.

Potential categories:

```text
/api/v1/auth/...
/api/v1/businesses/...
/api/v1/services/...
/api/v1/queues/...
/api/v1/tickets/...
/api/v1/notifications/...
/api/v1/staff/...
/api/v1/admin/...
```

These are categories only.

Do not invent duplicate routes. Exact routes must come from the approved API Contract.

---

# 32. POC Endpoint Requirement

Task 04 requires at least 1–2 important working endpoints.

A POC endpoint must:

1. Receive request.
2. Validate input.
3. Process request.
4. Return structured response.
5. Handle basic errors.

Possible examples from the foundation plan:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
```

Final choice must follow the current API Contract and be coordinated with Developer 02.

Goal: prove architecture quality, not endpoint quantity.

---

# 33. Standard API Response

The POC should use the approved API Contract response format.

Example shape from the foundation plan:

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": {}
}
```

Do not create a separate response format only for Task 04.

Exact property names must follow the current API Contract.

---

# 34. Basic Error Handling

Foundation should prepare consistent handling for:

- Validation errors
- Not Found
- Unauthorized
- Forbidden
- Bad Request
- Conflict
- Unexpected server errors

A global exception middleware can be prepared in API.

HTTP-specific handling should not be placed inside Domain entities.

---

# 35. Next Phase — Domain + Database

Before writing entities, review:

```text
SRS
  |
  v
ERD
  |
  v
API Contract
  |
  v
SignalR Events
  |
  v
Domain Design
```

For every entity/table establish:
- Name
- Purpose
- Primary key
- Foreign keys
- Required fields
- Optional fields
- Relationships
- Unique constraints
- Important indexes
- Business constraints

Actual names/fields must follow the approved ERD/API Contract.

---

# 36. EF Core — Planned

After Domain review:

```text
Configure EF Core
       |
       v
Create DbContext
       |
       v
Entity Configurations
       |
       v
PKs
       |
       v
FKs
       |
       v
Relationships
       |
       v
Indexes / Unique Constraints
       |
       v
Active Ticket Constraint
       |
       v
Migration
```

Do not overbuild production seed data in Task 04.

---

# 37. ERD Review — Planned

Check ERD against:
- SRS v1.4
- Cancellation rule
- Ticket status values
- Staff -> Service relationship
- Queue -> Service relationship
- Ticket -> Queue/Service relationships
- Notification relationships
- One-active-ticket constraint

Eventually place final ERD in repository documentation, for example:

```text
docs/
└── ERD/
    ├── QueueLess-ERD.png
    └── QueueLess-ERD.md
```

---

# 38. API Documentation — Planned

Documentation should contain:
- Endpoint
- HTTP method
- Authentication requirement
- Request
- Response
- Status codes
- Validation rules
- Error examples

Postman collection can be stored as:

```text
docs/
└── API/
    └── QueueLess.postman_collection.json
```

---

# 39. README — Planned

Recommended sections:

```text
# QueueLess Backend
## Overview
## Architecture
## Project Structure
## Technologies
## Database
## Authentication & Authorization
## API Documentation
## Running the Project
## Environment Configuration
## POC Endpoints
## Team Contribution
## Current Task Status
```

---

# 40. Environment & Secrets

Never commit:
- Database passwords
- JWT secrets
- API keys
- Production credentials
- Local credentials

Use safe configuration placeholders and appropriate local-secret mechanisms.

---

# 41. Target Repository Structure

Current:

```text
FAD-QuLessService/
|
+-- .git/
+-- .github/
+-- .gitignore
+-- README.md
|
+-- QueueLess/
    |
    +-- QueueLess.sln
    +-- QueueLess.API/
    +-- QueueLess.Application/
    +-- QueueLess.Domain/
    +-- QueueLess.Infrastructure/
```

Later, when documentation/tests actually exist:

```text
FAD-QuLessService/
|
+-- .git/
+-- .github/
+-- .gitignore
+-- README.md
|
+-- QueueLess/
    |
    +-- QueueLess.sln
    +-- QueueLess.API/
    +-- QueueLess.Application/
    +-- QueueLess.Domain/
    +-- QueueLess.Infrastructure/
    |
    +-- docs/
    |   +-- ERD/
    |   +-- Architecture/
    |   +-- API/
    |
    +-- tests/
```

Do not create empty folders just for appearance.

---

# 42. Current Checklist

## Repository

- [x] GitHub repository created
- [x] Backend repository separated from Flutter
- [x] Repository cloned
- [x] README exists
- [x] `.gitignore` added
- [x] `.vs` ignored
- [x] Initial commit completed locally
- [ ] Initial commit pushed/verified on GitHub

## Architecture

- [x] Visual Studio solution created
- [x] Domain project created
- [x] Application project created
- [x] Infrastructure project created
- [x] API project created
- [x] Project references added
- [x] Dependency direction established
- [x] Generated template files removed
- [x] Solution builds successfully

## Domain

- [ ] Entities finalized from ERD
- [ ] Enums finalized
- [ ] Relationships finalized
- [ ] Domain constraints implemented

## Database

- [ ] EF Core configured
- [ ] DbContext created
- [ ] Entity configurations created
- [ ] PKs/FKs configured
- [ ] Relationships configured
- [ ] Indexes/unique constraints configured
- [ ] Active-ticket constraint configured
- [ ] Migration generated

## Authentication / Authorization

- [ ] Authentication foundation
- [ ] Roles
- [ ] Authorization policies
- [ ] JWT/configuration as required

## API

- [ ] Controllers structure
- [ ] DTOs
- [ ] Services
- [ ] Repositories
- [ ] Middleware
- [ ] Configuration
- [ ] Standard response
- [ ] Validation
- [ ] Error handling
- [ ] 1–2 POC endpoints
- [ ] Swagger/Postman verification

## Documentation

- [ ] Backend architecture document
- [ ] Final ERD
- [ ] API documentation
- [ ] Postman collection
- [ ] README update
- [ ] Setup instructions
- [ ] POC instructions

---

# 43. Exact Next Execution Order

```text
CURRENT POINT
     |
     v
1. Push initial commit to GitHub / verify it
     |
     v
2. Review SRS + ERD + API Contract + SignalR Events
     |
     v
3. Freeze Domain model
     |
     v
4. Create Domain entities
     |
     v
5. Create enums
     |
     v
6. Add relationships
     |
     v
7. Build
     |
     v
8. Configure EF Core
     |
     v
9. Create DbContext
     |
     v
10. Create entity configurations
     |
     v
11. Configure PK/FK/relationships/indexes
     |
     v
12. Verify migration
     |
     v
13. Authentication/Authorization foundation
     |
     v
14. Coordinate API foundation with Developer 02
     |
     v
15. Select 1–2 POC endpoints
     |
     v
16. Implement POC
     |
     v
17. Swagger/Postman
     |
     v
18. Documentation
     |
     v
19. Final review
```

---

# 44. Implementation Rule

Before changing an entity, endpoint, database relationship, or queue behavior:

**Check the approved source documents first.**

Priority:

```text
Approved SRS
      |
      v
Approved ERD
      |
      v
Approved API Contract
      |
      v
Approved SignalR Events
      |
      v
Implementation
```

If a conflict is discovered, stop and resolve the contract before coding.

---

# 45. Current Milestone

Completed:

> **Backend Repository + Visual Studio Architecture Foundation**

Current achievement:

```text
GitHub Repository
       |
       v
Local Clone
       |
       v
QueueLess Solution
       |
       +---- Domain
       +---- Application
       +---- Infrastructure
       +---- API
       |
       v
Project References
       |
       v
Build Successful
       |
       v
Template Cleanup
       |
       v
.gitignore Fix
       |
       v
Initial Commit
       |
       v
0 Local Changes
```

The next milestone is:

> **Domain Model + Database Foundation**

---

# 46. References

This work log is based on:

- `QueueLess_SRS_Draft_v1_4.md`
- `01_ERD.md`
- `02_API_Contract.md`
- `03_SignalR_Events.md`
- `QueueLess_Task04_Backend_Implementation_Plan.md`
- FAD Task 04 requirements
- Latest team-approved decisions made during implementation planning

---

# 47. Final Goal

The goal is NOT:

```text
Write as much backend code as possible.
```

The goal is:

```text
Clean
+
SRS-aligned
+
ERD-aligned
+
API-contract-aligned
+
Maintainable
+
Team-ready
Backend Foundation
```

Every next implementation step should preserve that goal.
