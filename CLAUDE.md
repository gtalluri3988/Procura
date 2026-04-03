# Procura – Developer Guide for Claude

## Project Overview

**Procura** is a procurement and vendor management platform for managing the full tender lifecycle: vendor registration, tender creation/approval, bidding, evaluation, award, and payment processing.

- **Framework:** .NET 7.0, ASP.NET Core Web API
- **Language:** C# 11 (nullable reference types enabled, implicit usings)
- **Database:** SQL Server via Entity Framework Core 7.0 (Code-First)
- **Auth:** JWT Bearer tokens
- **Mapping:** AutoMapper 12
- **Logging:** Serilog (console + daily rolling file at `Logs/app_log.txt`)
- **Docs:** Swagger/Swashbuckle (available in both Development and Production)

---

## Before Making Code Changes
- Always read all relevant markdown (.md) files before making any code changes.
- Follow project architecture, coding standards, and documented guidelines strictly.
- Do not introduce changes that conflict with existing documentation.
- If documentation is unclear, ask for clarification before proceeding.


## Change Logging (CHANGES.md)

- Update `CHANGES.md` only for meaningful code changes (feature, bug fix, refactor).
- Each entry must include:
  - File/Module
  - Change Summary
  - **Requirement (WHY the change was made)**
- Do not log trivial changes (formatting, minor renaming).
- Follow consistent format.

Example:

| Timestamp | File | Action | Requirement |
|----------|------|--------|------------|
| 2026-03-30 | TruckService.cs | Added validation | Prevent invalid truck data from API |


## Architecture Rules

- Follow clean architecture principles
- Do not mix UI, business logic, and data access
- Keep controllers thin, move logic to services
- Use proper layering (Controller → Service → Repository)


## Code Quality

- Write clean, readable, and maintainable code
- Use meaningful variable and method names
- Avoid duplication (follow DRY principle)
- Add comments only when necessary

## Validation & Error Handling

- Validate all inputs (UI + API)
- Never trust client input
- Return proper error messages
- Handle edge cases and null values

## Performance

- Optimize database queries
- Use async/await properly
- Do not load unnecessary data

## After Coding

- Review your own code
- Check for bugs and edge cases
- Ensure consistency with project standards
- Suggest improvements if needed





## Solution Structure

```
Procura/
├── Api/                        # ASP.NET Core Web API (Procura.csproj)
│   ├── Controllers/            # 26 controllers (14 active, 12 compiled out)
│   ├── Helpers/                # Email, Signature, AmpersandClient
│   ├── Models/                 # Request/response DTOs
│   ├── Program.cs              # DI wiring, middleware pipeline
│   └── appsettings.json        # JWT, payment gateway, SSM config
├── BusinessLogic/              # Application services (BusinessLogic.csproj)
│   ├── Interfaces/             # Service contracts (26+ interfaces)
│   ├── Services/               # Service implementations (30+)
│   ├── Models/                 # Business-layer DTOs
│   └── startup.cs              # Extension method: AddBusinessLogic()
├── DB/                         # Data access layer (DB.csproj)
│   ├── Model/                  # EF Core DbContext + 75+ entity classes
│   ├── Entity/                 # DTO / mapping helper classes
│   ├── Repositories/           # 25+ repositories + RepositoryBase<TEntity,TDto>
│   ├── Profiles/               # 20 AutoMapper profiles
│   ├── Migrations/             # EF Core migration files
│   ├── ReleaseScripts/         # Generated SQL release/rollback scripts
│   ├── Program.cs              # Extension method: AddDBProject() / AddRepository()
│   └── MigrationInfo.txt       # Migration workflow reference
└── Procura.sln
```

---

## Architecture

Three-tier layered architecture with strict dependency direction:

```
Api (Controllers)
  └─ depends on → BusinessLogic (Services)
                    └─ depends on → DB (Repositories → DbContext)
```

**Key patterns:**
- **Repository Pattern** – `RepositoryBase<TEntity, TDto>` provides generic CRUD; specific repos extend it with custom queries.
- **Service Layer** – All business logic lives in `BusinessLogic/Services/`. Controllers call services only.
- **DTO Pattern** – Entities never cross layer boundaries raw; AutoMapper profiles handle mapping in `DB/Profiles/`.
- **DI via extension methods** – `AddBusinessLogic()` (in `BusinessLogic/startup.cs`) and `AddDBProject()` (in `DB/Program.cs`) register all dependencies. Most service registrations happen directly in `Api/Program.cs`.
- **Base Controller** – `AuthorizedCSABaseAPIController` provides JWT claim extraction (`userid`) and common logging.

---

## Dependency Injection Registration

All DI is wired in `Api/Program.cs`. The pattern for adding a new feature:

1. Create entity in `DB/Model/`
2. Create DTO in `DB/Entity/`
3. Create AutoMapper profile in `DB/Profiles/`
4. Create repository interface in `DB/Repositories/Interfaces/` and implementation in `DB/Repositories/`
5. Register repository in `DB/Program.cs` → `AddRepository()`
6. Create service interface in `BusinessLogic/Interfaces/` and implementation in `BusinessLogic/Services/`
7. Register service in `Api/Program.cs` with `builder.Services.AddScoped<IXService, XService>()`
8. Create controller in `Api/Controllers/`

---

## Database & Migrations

**Connection string** is read from the environment variable `ProcuraConnection` (not from appsettings):

```csharp
var connectionString = Environment.GetEnvironmentVariable("ProcuraConnection");
```

Set this before running: `$env:ProcuraConnection = "Server=...;Database=...;..."`

**All EF Core commands must be run from the `DB/` project directory** (or with `--project DB`):

```bash
# Add a migration
dotnet ef migrations add <MigrationName> --project DB --startup-project Api

# Apply to database
dotnet ef database update --project DB --startup-project Api

# List migrations
dotnet ef migrations list --project DB --startup-project Api

# Remove last migration (if not yet applied)
dotnet ef migrations remove --project DB --startup-project Api
```

**Generating release/rollback SQL scripts** (see `DB/MigrationInfo.txt` for full details):

```bash
# From DB/ directory
dotnet ef migrations script -i \
  -o "DB/ReleaseScripts/<release>/<id>/release.sql" \
  <FROM_MIGRATION> <TO_MIGRATION>

dotnet ef migrations script -i \
  -o "DB/ReleaseScripts/<release>/<id>/rollback.sql" \
  <TO_MIGRATION> <FROM_MIGRATION>
```

Use `dotnet ef migrations list` to identify the last two migration names.

**Data migrations** (INSERT/UPDATE scripts): Add `migrationBuilder.Sql(...)` calls inside the migration's `Up()` method; corresponding deletes go in `Down()`.

---

## Authentication & Authorization

- JWT tokens are issued by `AuthController` on successful login.
- Token claims: `userid`, `roleid`, `Name` (username).
- Token lifetime: 60 minutes (configurable in `appsettings.json` → `JwtSettings:ExpiryMinutes`).
- Protect endpoints with `[Authorize]`; use `[AllowAnonymous]` for public endpoints.
- Extract current user in repositories/services via: `_httpContextAccessor.HttpContext?.User?.FindFirst("userid")?.Value`

---

## API Conventions

- Route template: `[Route("api/[controller]/[action]")]`
- Response wrapper: `CSAResponseModel<T>` for all API responses.
- Error format: `ErrorMessage` model.
- JSON enums serialized as strings (`JsonStringEnumConverter` registered globally).
- Multipart upload limit: 100 MB.
- Static files served from `C:\VendorUploads` at `/VendorUploads`.
- CORS: AllowAll policy (any origin, method, header) – **do not restrict in dev**.
- HTTPS redirect is currently commented out; do not re-enable without team discussion.

---

## Disabled/Compiled-Out Code

The following features are compiled out via `<Compile Remove>` in the `.csproj` files. **Do not delete these files** — they are intentionally preserved for future use:

- Controllers: Card, Community, Complaint, Event, Facility, Microbit, Report, Resident (all variants), Visitor
- Matching interfaces and services in BusinessLogic are also compiled out.

To re-enable a feature: remove the `<Compile Remove>` entry from `Api/Procura.csproj` and `BusinessLogic/BusinessLogic.csproj`.

---

## External Integrations

| Integration | Purpose | Config Key |
|---|---|---|
| Ampersand Pay | Payment gateway | `appsettings.json` → `Ampersand` |
| SSM Search API | Company profile lookup | `appsettings.json` → `SSM` |
| SAP (`ZBAPI_CREATEVENDOR`) | Vendor creation in SAP | `useSimulator = true` flag in `Program.cs` |

**SAP simulator mode** is on by default (`useSimulator = true` in `Program.cs:93`). Switch to `false` and configure `RealZBAPI_CREATEVENDORClient` for production.

---

## Key Domain Areas

| Area | Controller | Service | Repository |
|---|---|---|---|
| Authentication | `AuthController` | `IUserService` | `IUserRepository` |
| Vendor onboarding | `VendorController` | `IVendorService` | `IVendorRepository` |
| Tender lifecycle | `TenderManagementController` | `ITenderService` | `ITenderRepository` |
| Bidding | `BiddingController` | `IBiddingService` | `IBiddingRepository` |
| Payments | `PaymentController` | `IPaymentService` | `IPaymentRepository` |
| Master data / lookups | `MasterDataController` | `IMasterDataService` | `DropdownRepository` |
| Announcements | `AnnouncementController` | `IAnnouncementService` | `IAnnouncementRepository` |
| User / Role / Menu | `UserController` | `IUserService`, `IRoleService`, `IMenuService` | `IUserRepository`, `IRoleRepository` |

---

## File Upload

- Upload path: `C:\VendorUploads\` (configured in `appsettings.json` → `FileSettings:UploadPath`)
- Served statically at: `https://<host>/VendorUploads/<filename>`
- Max size: 100 MB (FormOptions multipart limit)

---

## DataProtection Keys

Keys are persisted to `C:\Keys` (configured in `Program.cs`). Ensure this directory exists on the server. Application name is `"MyApp"`.

---

## Logging

- Provider: Serilog
- Sinks: Console + File
- Log file path: `Logs/app_log.txt` (daily rolling, e.g. `app_log20260402.txt`)
- EF Core SQL logging: enabled via `LogTo(Console.WriteLine, LogLevel.Information)` – visible in console during development.

---

## Build Notes

- No automated test projects exist in this solution.
- `Api/Properties/PublishProfiles/FolderProfile.pubxml` targets Release publish to `bin/Release/net7.0/publish/`.
- `Logs/` folder is excluded from build output via `.csproj` configuration.
- User secrets ID: `9a22af4c-61d8-4931-a0b9-6b737e066f2d` (for local dev sensitive config).
- Swagger is enabled in **both** Development and Production (`app.Environment.IsProduction() || app.Environment.IsDevelopment()`).
