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

---

## Phase 2 – Full-Stack Alignment & Production-Readiness Audit (2026-04-16)

### Frontend (React App) Overview

- **Path:** `Procura_App/FledaPlantationManagement_Frontend/`
- **Framework:** React 18.3 + Vite 6.3 + TypeScript
- **Router:** React Router v7 (~90 page/modal components)
- **UI:** Shadcn/ui (Radix) + Material UI + Tailwind CSS 4.1
- **State:** Context API + sessionStorage (no Redux)
- **HTTP:** Axios with Bearer token interceptor
- **Forms:** Manual useState validation (react-hook-form installed but largely unused)

---

### 1. CRITICAL ISSUES (must fix)

#### 1.1 No Global Exception Handling Middleware (Backend)

**File:** `Api/Program.cs` (lines 171-196)

The middleware pipeline has **no** `app.UseExceptionHandler()` or custom exception middleware. Any unhandled exception in a controller or service returns HTTP 500 with a **raw stack trace** exposed to the client.

**Fix:** Add global exception middleware:
```csharp
// In Program.cs, before app.UseCors()
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            Log.Error(error.Error, "Unhandled exception");
            await context.Response.WriteAsJsonAsync(new CSAResponseModel<object>(
                true, "An internal server error occurred. Please try again later."));
        }
    });
});
```

#### 1.2 70+ Controller Endpoints Have NO try-catch (Backend)

| Controller | Unprotected Endpoints | Total Endpoints |
|---|---|---|
| `TenderManagementController.cs` | **24 of 24** | 24 |
| `BiddingController.cs` | **10 of 10** | 10 |
| `VendorController.cs` | **19 of ~25** | ~25 |
| `UserController.cs` | **8 of 9** | 9 |
| `AdminController.cs` | **9 of 11** | 11 |
| `CategoryCodeApprovalController.cs` | **3 of 6** | 6 |

Any database error, null reference, or business rule violation returns a raw exception with internal details to the client.

#### 1.3 Silent Data Loss in RepositoryBase.AddAsync() (Backend)

**File:** `DB/Repositories/RepositoryBase.cs` (lines 42-55)

```csharp
public async Task<TDto> AddAsync(TDto dto)
{
    var entity = _mapper.Map<TEntity>(dto);
    _dbSet.Add(entity);
    try { await _context.SaveChangesAsync(); }
    catch(Exception ex) { /* EMPTY CATCH - SILENT FAILURE */ }
    return _mapper.Map<TDto>(entity);  // Returns DTO as if save succeeded!
}
```

**Impact:** Data is NOT saved to database but the method returns success. Callers have no way to know the operation failed. This is the most dangerous bug in the codebase.

**Fix:** Remove the try-catch and let exceptions propagate, or log and rethrow:
```csharp
public async Task<TDto> AddAsync(TDto dto)
{
    var entity = _mapper.Map<TEntity>(dto);
    _dbSet.Add(entity);
    await _context.SaveChangesAsync(); // Let it throw
    return _mapper.Map<TDto>(entity);
}
```

#### 1.4 Silent Email Failure (Backend)

**File:** `DB/Repositories/UserRepository.cs` (lines 127-135)

```csharp
try { await client.SendMailAsync(mailMessage); }
catch (Exception ex) { /* EMPTY - commented-out throw */ }
```

Password reset emails silently fail. The user gets a "success" response but never receives the email.

#### 1.5 Frontend Endpoint URL Typo Causing 404 (Frontend)

**File:** `Procura_App/.../src/apis/apiEndpoints.ts` (line 38)

```typescript
DELETE_MATERIAL_BUDGET: "/api/MasterData/DeleteMaterialBudge",  // Missing 't'
```

Backend route is `/api/MasterData/DeleteMaterialBudget`. This causes a **404** on every delete attempt.

#### 1.6 Inconsistent Frontend Error Handling Pattern (Frontend)

**File:** `Procura_App/.../src/apis/apiService.ts`

~30 functions silently swallow errors and return `null` or `[]`, while ~19 functions throw errors. There is no clear pattern:

| Operation Type | Returns null/[] (silent) | Throws error |
|---|---|---|
| Save operations | `saveVendorProfile`, `saveVendorMembers`, `saveVendorFinancial` | `saveVendorExperiences`, `saveQuestionAnswers`, `saveDeclarationStatus` |
| Get operations | `getVendorDetails`, `getAllUsers`, `getVendorDashboard` | (none) |
| Login | (none) | `vendorLogin`, `staffLogin` |

**Impact:** Callers cannot distinguish between "no data found" and "API error". Users see blank pages with no error feedback.

#### 1.7 JWT Secret in appsettings.json (Security)

**File:** `Api/appsettings.json`

```json
"JwtSettings": { "Secret": "vk2UMWJUBUCYulDocOcNEw==" }
```

JWT signing secret is committed to source control. Also, payment gateway API keys and SSM keys are in the same file.

**Fix:** Move to User Secrets (dev) or environment variables/Azure Key Vault (production). The connection string is already correctly read from env var — apply the same pattern to JWT and API keys.

---

### 2. IMPROVEMENTS (should fix)

#### 2.1 Inconsistent Backend Error Response Format

Controllers return errors in 4 different formats:
- `ErrorMessage { message = "..." }` — AuthController
- Raw `ex.Message` string — VendorController, UserController
- `new { Error = ex.Message }` — CategoryCodeApprovalController
- `CSAResponseModel<T>(true, error)` — documented standard but rarely used

**Fix:** Standardize all error responses through `CSAResponseModel<T>`.

#### 2.2 N+1 Query Issue in BiddingRepository (Backend)

**File:** `DB/Repositories/BiddingRepository.cs` (lines 57-89)

`GetBiddingDetailAsync()` makes **6 separate DB queries** for a single detail request:
1. TenderApplications (with Include)
2. TenderAdvertisementSettings
3. SiteLevel
4. BiddingAssets
5. TenderVendorSubmissions
6. BidderSubmissionItems

**Fix:** Combine into 1-2 queries using `.Include()` / `.ThenInclude()` joins.

#### 2.3 No Pagination in Any Repository List Method (Backend)

**All** `GetAll*Async()` methods load entire tables into memory. At 1000+ drivers/vendors, this will cause significant memory pressure and slow response times.

**Affected:** `BiddingRepository`, `TenderRepository`, `VendorRepository`, `UserRepository`, `CategoryCodeApprovalRepository`, `DropdownRepository`

**Fix:** Add `PageNumber`/`PageSize` parameters:
```csharp
public async Task<PaginatedResult<T>> GetAllAsync(int page = 1, int pageSize = 20)
{
    var query = _dbSet.AsQueryable();
    var total = await query.CountAsync();
    var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    return new PaginatedResult<T>(items, total, page, pageSize);
}
```

#### 2.4 Multiple Sequential API Calls on Page Mount (Frontend)

**File:** `VendorProfilePage.tsx` (lines 60-90) — 3 separate useEffect hooks make independent API calls:
- `getCompanyTypes()`
- `getSelectList("state,country")`
- `getIndustryTypes()`

**Fix:** Batch with `Promise.all()`:
```typescript
useEffect(() => {
  Promise.all([getCompanyTypes(), getSelectList("state,country"), getIndustryTypes()])
    .then(([types, lists, industries]) => { /* set state */ });
}, []);
```

#### 2.5 Duplicate Utility Functions Across 8+ Pages (Frontend)

| Function | Duplicated In |
|---|---|
| `isValidEmail()` | VendorProfilePage, VendorFinancialPage, VendorMembersPage, VendorExperiencesPage, VendorCategoryCodePage, UserProfileContent, SSMPaymentPage, RegisterVendorPage |
| `convertToBase64()` | VendorProfilePage, VendorFinancialPage, VendorMembersPage, VendorExperiencesPage |
| JWT decode pattern | 6+ pages |

**Fix:** Extract to shared utilities:
- `src/utils/validators.ts`
- `src/utils/fileHelpers.ts`
- `src/hooks/useAuth.ts`

#### 2.6 Async Method Without Await (Backend)

**File:** `DB/Repositories/DropdownRepository.cs` (lines 15-80)

`GetDropdownDataAsync()` is marked `async` but uses synchronous `.ToList()` instead of `.ToListAsync()` in all branches.

#### 2.7 Redundant Retry Logic (Backend)

**File:** `DB/Repositories/UserRepository.cs` (lines 21-28)

```csharp
public async Task<User?> GetUserByUsernameAsync(string username)
{
    try { return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username); }
    catch (Exception ex) { return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username); }
}
```

Retries the exact same query — if the DB is down, this always fails identically.

#### 2.8 Missing Loading States (Frontend)

Pages with async data fetching (`VendorProfilePage`, `VendorMembersPage`, `VendorFinancialPage`, `VendorExperiencesPage`) show blank/empty forms while data loads. No loading spinners or skeleton UI.

#### 2.9 Too Many Endpoints Marked [AllowAnonymous] (Backend)

Several sensitive endpoints are publicly accessible:
- `TenderManagementController` — ALL endpoints are `[AllowAnonymous]` (save, update, delete tenders)
- `BiddingController` — ALL endpoints are `[AllowAnonymous]`
- Many `VendorController` endpoints that should require auth

**Fix:** Remove `[AllowAnonymous]` from write/admin operations. Keep it only for: login, vendor registration, and public tender listings.

---

### 3. GOOD PRACTICES FOUND

- **Clean three-tier architecture:** Controller -> Service -> Repository with proper separation
- **RepositoryBase<TEntity, TDto>** generic pattern avoids CRUD boilerplate
- **AutoMapper profiles** in dedicated `/Profiles` directory for clean entity-DTO mapping
- **JWT Bearer auth** with proper token validation parameters (issuer, audience, lifetime, signing key)
- **CSAResponseModel<T>** wrapper exists as a standardized response format (needs consistent adoption)
- **Serilog** configured with daily rolling file logging
- **Connection string from environment variable** — good security practice
- **Multi-step vendor registration** with step tracking (`VendorRegistrationStep` enum) is well-designed
- **Axios request interceptor** for automatic Bearer token injection on all API calls
- **Shadcn/ui component library** with ~60 reusable components in `app/components/ui/`
- **React Router v7** with nested layout routes for master dashboard
- **Sonner toast notifications** for user feedback
- **Compiled-out code** properly preserved for future use (not deleted)

---

### 4. MISSING FEATURES

| Feature | Impact | Priority |
|---|---|---|
| **Global exception middleware** | Stack traces exposed to clients | P0 |
| **Request validation middleware** | Invalid data reaches DB layer | P0 |
| **API rate limiting** | Vulnerable to brute force / DDoS | P1 |
| **Retry/circuit breaker for external APIs** (Ampersand, SSM, SAP) | Silent failures on 3rd-party outage | P1 |
| **Frontend route guards** | Unauthenticated users can access protected pages | P1 |
| **Centralized TypeScript types** | No shared type definitions for API contracts | P2 |
| **Frontend error boundary** | Unhandled React errors crash entire app | P2 |
| **Audit logging** | No record of who changed what | P2 |
| **Automated tests** (unit + integration) | No test projects exist | P2 |
| **Health check endpoint** | No `/health` for monitoring | P3 |
| **Response compression** | Large payloads not compressed | P3 |
| **API versioning** | No version prefix strategy | P3 |

---

### 5. ENDPOINT ALIGNMENT — Frontend vs Backend Mismatches

| Issue | Frontend | Backend | Severity |
|---|---|---|---|
| **URL typo** | `DeleteMaterialBudge` | `DeleteMaterialBudget` | CRITICAL (404) |
| **Spelling** | `GetVendorDetilsById` | `GetVendorDetilsById` | LOW (both have same typo — consistent but wrong) |
| **Unused backend endpoints** | Not in `apiEndpoints.ts` | PaymentController (6 endpoints), AdminController (4), MasterDataController (3), VendorController (5) | INFO |

---

### 6. ARCHITECTURE DIAGRAM

```
+---------------------------------------------------+
|                   FRONTEND (React)                 |
|  Procura_App/FledaPlantationManagement_Frontend/   |
|                                                   |
|  [Pages]  -->  [apiService.ts]  -->  [Axios]      |
|  ~90 pages      57 API functions     interceptor   |
|  Context API    apiEndpoints.ts      Bearer JWT    |
|  SessionStorage                                    |
+---------------------------------------------------+
           |  HTTPS (REST JSON)  |
           v                     v
+---------------------------------------------------+
|                BACKEND API (.NET 7)                |
|                  Procura/Api/                      |
|                                                   |
|  [Controllers] --> [Services] --> [Repositories]  |
|  14 active         30+ impls      25+ repos       |
|  JWT Auth          Business       RepositoryBase   |
|  CSAResponseModel  Logic          AutoMapper       |
+---------------------------------------------------+
           |                     |
           v                     v
+------------------+   +---------------------+
|    SQL Server    |   |  External Services   |
|  (EF Core 7.0)  |   |                     |
|  75+ entities    |   |  - Ampersand Pay    |
|  40+ migrations  |   |  - SSM Search API   |
|                  |   |  - SAP ZBAPI        |
+------------------+   +---------------------+
```

---

### Phase 2 Development Rules

- Do NOT modify existing pages unless for route mapping or critical fixes
- New features go in new page components
- Backend changes should not break existing API contracts
- All new endpoints must include try-catch error handling
- All new endpoints must use `CSAResponseModel<T>` response wrapper
- All new repository methods must use async properly
