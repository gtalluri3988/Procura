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

## Phase 2 – Full-Stack Deep Analysis & Production-Readiness Audit (2026-04-16)

### Frontend (React App) Overview

- **Path:** `Procura_App/FledaPlantationManagement_Frontend/`
- **Framework:** React 18.3 + Vite 6.3 + TypeScript
- **Router:** React Router v7 (~90 page/modal components, 40+ routes)
- **UI:** Shadcn/ui (47 Radix primitives) + Material UI 7 + Tailwind CSS 4.1
- **State:** Context API (LoginModalContext only) + sessionStorage (no Redux/Zustand)
- **HTTP:** Axios with Bearer token interceptor (48 API functions, 55+ endpoint constants)
- **Forms:** Manual useState validation (react-hook-form installed but largely unused)
- **Charts:** Recharts 2.15
- **PDF:** jspdf 4.1
- **Drag & Drop:** react-dnd 16

### Backend (API) Overview

- **Active Controllers:** 16 (AuthController, VendorController, TenderManagementController, BiddingController, UserController, AdminController, MasterDataController, PaymentController, AnnouncementController, ContentController, NotificationController, CategoryCodeApprovalController, SelectListController, AuthorizedCSABaseAPIController)
- **Compiled-Out Controllers:** 12 (Card, Community, Complaint, Event, Facility, Microbit, Report, Resident variants, Visitor)
- **Services:** 20+ implementations
- **Repositories:** 25+ (all extend RepositoryBase<TEntity, TDto>)
- **Entities:** 75+ EF Core entity classes
- **DTOs:** 70+ in DB/Entity/
- **Migrations:** 40+ EF Core migration files
- **AutoMapper Profiles:** 20 mapping files

---

### 1. CRITICAL ISSUES (must fix)

#### 1.1 Silent Data Loss in RepositoryBase.AddAsync() (Backend)

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

**Impact:** Data is NOT saved to database but the method returns success. Every entity creation in the entire system (vendors, tenders, bids, users, payments) can silently fail. This is the most dangerous bug in the codebase.

**Fix:** Remove the try-catch and let exceptions propagate:
```csharp
public async Task<TDto> AddAsync(TDto dto)
{
    var entity = _mapper.Map<TEntity>(dto);
    _dbSet.Add(entity);
    await _context.SaveChangesAsync(); // Let it throw
    return _mapper.Map<TDto>(entity);
}
```

**Note:** Also missing try-catch on other RepositoryBase methods (GetAllAsync line 30, GetByIdAsync line 36, UpdateAsync line 57, DeleteAsync line 66, GetUserCommunity line 75) — these should propagate exceptions naturally (no catch needed), but callers must handle errors.

---

#### 1.2 No Global Exception Handling Middleware (Backend)

**File:** `Api/Program.cs` (lines 171-196)

The middleware pipeline has **no** `app.UseExceptionHandler()` or custom exception middleware. Any unhandled exception in a controller or service returns HTTP 500 with a **raw stack trace** exposed to the client — leaking internal paths, connection strings, and class names.

**Fix — Option A: Global exception middleware (add before `app.UseCors()`):**
```csharp
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

**Fix — Option B: Global action filter (recommended — single registration):**
```csharp
// New file: Api/Filters/ApiExceptionFilter.cs
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception in {Action}",
            context.ActionDescriptor.DisplayName);
        context.Result = new ObjectResult(new CSAResponseModel<object>(
            true, "An error occurred processing your request."))
        { StatusCode = 500 };
        context.ExceptionHandled = true;
    }
}

// Register in Program.cs:
builder.Services.AddControllers(o => o.Filters.Add<ApiExceptionFilter>());
```

---

#### 1.3 70+ Controller Endpoints Have NO try-catch (Backend)

| Controller | File | Unprotected / Total | Lines With No try-catch |
|---|---|---|---|
| `TenderManagementController.cs` | `Api/Controllers/` | **24 of 24** | 42-270 (every endpoint) |
| `BiddingController.cs` | `Api/Controllers/` | **10 of 10** | 32-127 (every endpoint) |
| `VendorController.cs` | `Api/Controllers/` | **19 of ~25** | Multiple |
| `UserController.cs` | `Api/Controllers/` | **8 of 9** | Multiple |
| `AdminController.cs` | `Api/Controllers/` | **9 of 11** | Multiple |
| `CategoryCodeApprovalController.cs` | `Api/Controllers/` | **3 of 6** | Multiple |

Any database error, null reference, or business rule violation returns a raw exception with internal details to the client. Combined with no global exception middleware (1.2), this is a critical exposure.

---

#### 1.4 29 Unprotected Write/Delete Endpoints via [AllowAnonymous] (Backend)

**File:** `Api/Controllers/TenderManagementController.cs` — 19 of 24 endpoints marked `[AllowAnonymous]`:

| Line | Endpoint | HTTP Method | Issue |
|---|---|---|---|
| 48 | `UpdateTenderApplication` | POST | **Write op exposed** |
| 56 | `DeleteTenderApplication` | POST | **Delete op exposed** |
| 226 | `SaveTenderAward` | POST | **Write op exposed** |
| 235 | `SaveTenderAwardMinutes` | POST | **Write op exposed** |
| 244 | `DeleteTenderAwardMinutes` | DELETE | **Delete op exposed** |
| 263 | `SaveVendorPerformance` | POST | **Write op exposed** |
| 63, 70, 82, 95, 114, 127, 136, 146, 155, 171, 181, 207, 216 | Various GET endpoints | GET | Public read (some acceptable) |

**File:** `Api/Controllers/BiddingController.cs` — **ALL 10 of 10** endpoints marked `[AllowAnonymous]`:

| Line | Endpoint | HTTP Method | Issue |
|---|---|---|---|
| 49 | `SubmitBidding` | POST | **Write op exposed — anyone can submit bids** |
| 79 | `SaveBiddingAsset` | POST | **Write op exposed** |
| 88 | `DeleteBiddingAsset` | DELETE | **Delete op exposed** |
| 100 | `SubmitBidderAcknowledgement` | POST | **Write op exposed** |
| 111 | `VerifyTenderOpening` | POST | **Sensitive op exposed** |
| 30, 39, 58, 70, 121 | Various GET endpoints | GET | Public read |

**Impact:** Anyone on the internet can create/update/delete tenders, submit bids, and manipulate awards without authentication.

**Fix:** Remove `[AllowAnonymous]` from all write/delete endpoints. Keep it only on genuinely public read endpoints (advertisement page, public tender listing, login, vendor registration).

---

#### 1.5 JWT Secret and API Keys in appsettings.json (Security)

**File:** `Api/appsettings.json`

| Secret | Config Key | Value Exposed |
|---|---|---|
| JWT Signing Secret | `JwtSettings:Secret` | `vk2UMWJUBUCYulDocOcNEw==` |
| Payment Gateway API Key | `PaymentGateway:APIKey` | `2999d1db4dd7e2ab...` |
| Payment Merchant ID | `Ampersand:MerchantId` | `91016474` |
| Ampersand Secret Key | `Ampersand:SecretKey` | `2999d1db4dd7e2ab...` |
| SSM API Key | `SSM:APIKey` | `d49f2d60-0af1-...` |
| SSM Secret Key | `SSM:SecretKey` | `bc3b0838-b285-...` |

**Fix:** Move to User Secrets (dev) or environment variables/Azure Key Vault (production). The connection string is already correctly read from env var — apply the same pattern to all secrets.

---

#### 1.6 Frontend API Service — 29 Functions Silently Swallow Errors (Frontend)

**File:** `Procura_App/.../src/apis/apiService.ts`

29 out of 48 API functions catch errors and return `[]`, `null`, or `{}`. The UI shows empty state with no error notification. Users have no idea their save failed.

**Functions that silently return null/[] on error (MUST be fixed for save operations):**

| Function | Line | Returns | User Impact |
|---|---|---|---|
| `saveVendorProfile()` | 98 | `null` | Profile appears saved but isn't |
| `saveVendorMembers()` | 119 | `null` | Members data silently lost |
| `saveVendorFinancial()` | 174 | `null` | Financial data silently lost |
| `saveVendorCategories()` | 198 | `null` | Categories silently lost |
| `registerVendor()` | 74 | `null` | Registration silently fails |
| `requestPaymentTransaction()` | 151 | `null` | Payment silently fails |
| `getCompanyTypes()` | 24 | `[]` | Empty dropdown |
| `getCompanyEntitiesByTypeId()` | 44 | `[]` | Empty dropdown |
| `searchCompanyEntity()` | 60 | `null` | Silent search failure |
| `getSelectList()` | 86 | `null` | Empty form fields |
| `getIndustryTypes()` | 107 | `null` | Empty dropdown |
| `getVendorDetails()` | 131 | `null` | Blank profile |
| `getPaymentDetails()` | 162 | `null` | Missing payment info |
| `getAllCategories()` | 183 | `[]` | Empty category list |
| `getQuestionAnswers()` | 243 | `[]` | Blank questionnaire |
| `getVendorManagementSetting()` | 276 | `null` | Missing settings |
| `getVendorDashboard()` | 286 | `null` | Blank dashboard |
| `getVendors()` | 296 | `[]` | Empty vendor list |
| `getMasterdataCatagory()` | 306 | `null` | Missing categories |
| `getAllUsers()` | 328 | `[]` | Empty user list |
| `searchUsers()` | 338 | `[]` | Silent search failure |
| `getAllTendermanagement()` | 347 | `null` | Blank tender list |
| `getUserById()` | 369 | `null` | Blank user details |
| `getAllRoles()` | 389 | `null` | Empty roles list |
| `getRoleById()` | 421 | `null` | Blank role details |
| `getAllMenuRolePermission()` | 443 | `[]` | Empty permissions |
| `getAllMaterialBudgetList()` | 452 | `null` | Missing budget data |
| `getRolePermissionById()` | 504 | `null` | Blank permission |
| `getMaterialBudgetUpload()` | 536 | `null` | Missing upload data |

**Functions that correctly throw errors (19 — keep this pattern):**
`requestVendorCodeFromSAP`, `saveVendorExperiences`, `vendorLogin`, `staffLogin`, `saveQuestionAnswers`, `saveDeclarationStatus`, `saveHierarchyCatagoryCode`, `createUser`, `updateUserByAdmin`, `saveRole`, `saveTenderManagement`, `updateRole`, `saveMenuRolePermission`, `saveMaterialBudget`, `deleteMaterialBudget`, `saveVendorManagementSetting`, `updateRoleMenuPermission`, `getUpdateMaterialBudget`, `uploadMaterialBudgetFile`

**Fix — All save/write functions must throw errors:**
```typescript
// BEFORE (silent failure)
export const saveVendorProfile = async (vendorId: string, payload: any) => {
  try {
    const res = await apiConnector.post(`${API_URLS.SAVE_VENDOR_PROFILE}/${vendorId}`, payload);
    return res.data;
  } catch (error) {
    console.error("Error saving vendor profile:", error);
    return null; // <-- UI thinks it worked
  }
};

// AFTER (proper error propagation)
export const saveVendorProfile = async (vendorId: string, payload: any) => {
  try {
    const res = await apiConnector.post(`${API_URLS.SAVE_VENDOR_PROFILE}/${vendorId}`, payload);
    return res.data;
  } catch (error) {
    console.error("Error saving vendor profile:", error);
    throw error; // <-- Let caller show toast.error()
  }
};
```

---

#### 1.7 Silent Email Failure (Backend)

**File:** `DB/Repositories/UserRepository.cs` (lines 127-135)

```csharp
try { await client.SendMailAsync(mailMessage); }
catch (Exception ex) { /* EMPTY - commented-out throw */ }
```

Password reset emails silently fail. The user gets a "success" response but never receives the email.

---

#### 1.8 Frontend Endpoint URL Typos Causing Failures (Frontend)

**File:** `Procura_App/.../src/apis/apiEndpoints.ts`

| Line | Constant | Current Value | Issue |
|---|---|---|---|
| 38 | `DELETE_MATERIAL_BUDGET` | `"/api/MasterData/DeleteMaterialBudge"` | Missing trailing "t" — causes 404 |
| 37 | `SAVE_METERIAL_BUDGET` | — | "METERIAL" should be "MATERIAL" |
| 54 | `UPDATE_METERIAL_BUDGET` | — | "METERIAL" should be "MATERIAL" |
| 41 | `VENDER_MANAGEMNT_SETTING` | — | "VENDER MANAGEMNT" should be "VENDOR_MANAGEMENT" |

---

#### 1.9 No React Error Boundary (Frontend)

**File:** `Procura_App/.../src/app/App.tsx` (lines 1-11)

If any component throws during render, the entire app white-screens with no recovery. No `ErrorBoundary` component exists anywhere in the codebase.

**Fix — Create `src/app/components/ErrorBoundary.tsx`:**
```tsx
import { Component, ReactNode } from "react";

interface Props { children: ReactNode; }
interface State { hasError: boolean; }

export class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false };
  static getDerivedStateFromError(): State { return { hasError: true }; }
  componentDidCatch(error: Error) { console.error("React Error Boundary caught:", error); }
  render() {
    if (this.state.hasError) {
      return (
        <div className="flex items-center justify-center h-screen">
          <div className="text-center">
            <h1 className="text-2xl font-bold">Something went wrong</h1>
            <button onClick={() => this.setState({ hasError: false })}
              className="mt-4 px-4 py-2 bg-blue-600 text-white rounded">Try Again</button>
          </div>
        </div>
      );
    }
    return this.props.children;
  }
}
```

**Wrap in App.tsx:**
```tsx
<ErrorBoundary>
  <LoginModalProvider>
    <RouterProvider router={router} />
  </LoginModalProvider>
</ErrorBoundary>
```

---

#### 1.10 No Axios 401 Response Interceptor — No Logout / Token Expiry Handling (Frontend)

**File:** `Procura_App/.../src/apis/apiService.ts`

Token lifetime is 60 minutes. When it expires, API calls silently fail (return 401). There is no response interceptor to detect this, no redirect to login, and no logout mechanism.

**Fix — Add response interceptor in `apiService.ts`:**
```typescript
apiConnector.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      sessionStorage.removeItem("authToken");
      sessionStorage.removeItem("vendorId");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);
```

---

### 2. IMPROVEMENTS (should fix)

#### 2.1 No Pagination in Any Repository List Method (Backend — Performance)

**All** `GetAll*Async()` methods load entire tables into memory. At 1000+ vendors/tenders, this causes significant memory pressure and slow response times.

**RepositoryBase.GetAllAsync()** — `DB/Repositories/RepositoryBase.cs` (lines 30-34):
```csharp
public async Task<IEnumerable<TDto>> GetAllAsync()
{
    var entities = await _dbSet.ToListAsync();  // LOADS ALL RECORDS
    return _mapper.Map<IEnumerable<TDto>>(entities);
}
```

**Other affected methods:**
- `VendorRepository.GetVendorListAsync()` (line 1225) — loads all vendors with Include
- `VendorRepository.GetVendorDashboardAsync()` (lines 1275-1298) — loads all vendors into memory then counts
- `VendorRepository.GetVendorListAsync(VendorSearchRequest)` (line 1300) — filters but no Skip/Take
- `BiddingRepository.GetActiveBiddingListAsync()` (lines 19-54) — loads ALL TenderApplications into memory then filters client-side
- `TenderRepository.GetAllTenderApplicationsAsync()` — loads all tenders

**Fix — Add pagination to RepositoryBase:**
```csharp
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public async Task<PagedResult<TDto>> GetPagedAsync(int page = 1, int pageSize = 20)
{
    var query = _dbSet.AsQueryable();
    var total = await query.CountAsync();
    var entities = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    return new PagedResult<TDto>
    {
        Items = _mapper.Map<IEnumerable<TDto>>(entities),
        TotalCount = total, Page = page, PageSize = pageSize
    };
}
```

---

#### 2.2 BiddingRepository — 6 DB Roundtrips for Single Detail Page (Backend — Performance)

**File:** `DB/Repositories/BiddingRepository.cs` (lines 57-127)

`GetBiddingDetailAsync()` makes **6 separate DB queries** for a single detail request:
1. Line 59: TenderApplications with 2 includes
2. Line 66: TenderAdvertisementSettings (separate query)
3. Line 70: SiteLevel (separate query after nullable check)
4. Line 74: BiddingAssets (separate query)
5. Line 80: TenderVendorSubmissions (separate query)
6. Line 86: BidderSubmissionItems (separate query)

**Fix:** Combine into 1-2 queries using `.Include()` / `.ThenInclude()` joins.

---

#### 2.3 Client-Side Filtering in GetActiveBiddingListAsync() (Backend — Performance)

**File:** `DB/Repositories/BiddingRepository.cs` (lines 19-54)

Loads ALL TenderApplications into memory (line 23: `.ToListAsync()`), creates a dictionary of ALL TenderAdvertisementSettings (line 29-31), then filters with LINQ-to-Objects in memory (lines 33-52). At 1000+ tenders, this is a memory bomb.

**Fix:** Move filtering to the database with `.Where()` before `.ToListAsync()`.

---

#### 2.4 VendorRepository.GetVendorFullDetailsAsync() — 12 Include Chains (Backend — Performance)

**File:** `DB/Repositories/VendorRepository.cs` (lines 250-274)

Chains 12 `.Include()` operations with nested `.ThenInclude()`, creating a massive query with potential cartesian product. Duplicated includes (VendorFinancial appears 3 times, VendorCategories appears 3 times).

**Fix:** Split into separate queries for distinct entity aggregates, or use `.AsSplitQuery()`.

---

#### 2.5 VendorService.ValidateCategoryChangeAsync() — Double Query (Backend — Performance)

**File:** `BusinessLogic/Services/VendorService.cs` (lines 535-616)

Calls `GetVendorByIdAsync()` at line 543 (scalar load), then calls `GetVendorFullDetailsAsync()` at line 599 (massive 12-include load). The second query is only used to count distinct categories (lines 602-605).

**Fix:** Replace the full details load with a targeted count query.

---

#### 2.6 Missing Database Indexes (Backend — Performance)

**File:** `DB/Model/ProcuraDbContext.cs` (lines 175-263) — No explicit performance indexes beyond primary keys.

**Critical missing indexes:**

| Table | Column(s) | Used By |
|---|---|---|
| `Vendors` | `VendorCodeStatus` | GetVendorDashboard, search endpoints |
| `Vendors` | `StateId` | GetVendors filter |
| `Vendors` | `CompanyEntityTypeId` | Search filter |
| `TenderVendorSubmissions` | `TenderId, VendorId` (composite) | Bidding lookups |
| `TenderAdvertisementSettings` | `TenderId` | BiddingRepository |
| `BiddingAssets` | `TenderId` | GetBiddingAssets |
| `QuestionAnswers` | `VendorId` | GetQuestionAnswers |
| `VendorCategories` | `VendorId` | Category navigation |
| `TenderApplications` | `TenderCategoryId` | Foreign key access |
| `TenderApplications` | `JobCategoryId` | Filter |
| `TenderApplications` | `ApplicationLevelId` | Filter |
| `CategoryCodeApprovals` | `VendorId` | Filter |

**Fix — Add migration with indexes:**
```csharp
migrationBuilder.CreateIndex("IX_Vendors_VendorCodeStatus", "Vendors", "VendorCodeStatus");
migrationBuilder.CreateIndex("IX_Vendors_StateId", "Vendors", "StateId");
migrationBuilder.CreateIndex("IX_TenderVendorSubmissions_TenderId_VendorId",
    "TenderVendorSubmissions", new[] { "TenderId", "VendorId" });
// ... etc
```

---

#### 2.7 Inconsistent Backend Error Response Format

Controllers return errors in 4 different formats:
- `ErrorMessage { message = "..." }` — AuthController
- Raw `ex.Message` string — VendorController, UserController
- `new { Error = ex.Message }` — CategoryCodeApprovalController
- `CSAResponseModel<T>(true, error)` — documented standard but rarely used

**Fix:** Standardize all error responses through `CSAResponseModel<T>`.

---

#### 2.8 CORS: AllowAll in Production (Backend — Security)

**File:** `Api/Program.cs` (lines 165-167, 191)

```csharp
policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
```

Allows any website to call the API. Must restrict to frontend domain before production deployment.

---

#### 2.9 Multiple Sequential API Calls on Page Mount (Frontend — Performance)

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

---

#### 2.10 Duplicate Utility Functions Across 8+ Pages (Frontend — Code Quality)

| Function | Duplicated In |
|---|---|
| `isValidEmail()` | VendorProfilePage, VendorFinancialPage, VendorMembersPage, VendorExperiencesPage, VendorCategoryCodePage, UserProfileContent, SSMPaymentPage, RegisterVendorPage |
| `convertToBase64()` | VendorProfilePage, VendorFinancialPage, VendorMembersPage, VendorExperiencesPage |
| JWT decode pattern | LoginPage, RegisterVendorPage, VendorDashboardPage, VendorProfilePage, CompanyDashboardPage, Sidebar |

**Fix:** Extract to shared utilities:
- `src/utils/validators.ts` — email, phone, field validation
- `src/utils/fileHelpers.ts` — base64 conversion, file upload
- `src/hooks/useAuth.ts` — token decode, vendorId extraction, auth state

---

#### 2.11 Async Method Without Await (Backend)

**File:** `DB/Repositories/DropdownRepository.cs` (lines 15-80)

`GetDropdownDataAsync()` is marked `async` but uses synchronous `.ToList()` instead of `.ToListAsync()` in all branches. This blocks the thread pool.

---

#### 2.12 Redundant Retry Logic (Backend)

**File:** `DB/Repositories/UserRepository.cs` (lines 21-28)

```csharp
public async Task<User?> GetUserByUsernameAsync(string username)
{
    try { return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username); }
    catch (Exception ex) { return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username); }
}
```

Retries the exact same query — if the DB is down, this always fails identically.

---

#### 2.13 Missing Loading States on Data-Fetching Pages (Frontend — UX)

| Page | Loading State | Status |
|---|---|---|
| `TenderManagementContent.tsx` | `loading` state (line 29) + "Loading..." message | GOOD |
| `UserListing.tsx` | `loading` state (line 41) + "Loading users..." message | GOOD |
| `VendorManagementDashboard.tsx` | `isLoadingDetails` (line 47) — opacity reduction only | WEAK — no spinner |
| `VendorProfilePage.tsx` | None | MISSING — blank form while loading |
| `VendorMembersPage.tsx` | None | MISSING |
| `VendorFinancialPage.tsx` | None | MISSING |
| `VendorExperiencesPage.tsx` | None | MISSING |
| `BidderUserListing.tsx` | None | MISSING |

---

#### 2.14 No Debouncing on Search Inputs (Frontend — Performance)

| Page | Search Input | Pattern |
|---|---|---|
| `UserListing.tsx` (line 294) | Search button | Direct API call on click — no debounce |
| `VendorManagementDashboard.tsx` (line 449) | Search button | Direct API call — can trigger rapid-fire |
| `TenderManagementPage.tsx` (line 104) | Search button | Placeholder — not implemented |
| `BidderUserListing.tsx` (line 117) | Search button | Not implemented |

**Fix:** Add `useCallback` + `setTimeout` debounce or lodash `debounce` for text-input search fields.

---

#### 2.15 46 Console Statements in Production Code (Frontend — Code Quality)

**Files with console.log/console.error left in production code:**

| File | Line(s) | Most Critical |
|---|---|---|
| `Sidebar.tsx` | 534 | `console.log("No route defined for ID:", id)` — leaks route info |
| `LoginPage.tsx` | 21, 126, 175 | Error logging in production |
| `TenderManagementContent.tsx` | 70, 80, 179, 181 | Mix of log/error/warn |
| `CategoryCodeContent.tsx` | 144, 154, 281, 324 | Mix of log/error/warn |
| `UserListing.tsx` | 77, 98, 116, 141, 187 | Error logging |
| `RegisterVendorPage.tsx` | 1420, 1782, 1788 | Error logging |
| `VendorFinancialPage.tsx` | Multiple | Error logging |
| `VendorMembersPage.tsx` | Multiple | Error logging |
| `VendorManagementDashboard.tsx` | 60, 77 | Error logging |
| `VendorCategoryCodePage.tsx` | 307 | Log statement |
| `VendorExperiencesPage.tsx` | 180 | Log statement |
| `ESGQuestionnairePage.tsx` | 16 | Log statement |
| `LoginModal.tsx` | 37 | `console.log("Forgot password clicked")` |
| `BidderUserListing.tsx` | 26 | Error logging |

**Fix:** Remove all `console.log` statements or wrap in environment check.

---

#### 2.16 Extensive Use of TypeScript `any` Type (Frontend — Code Quality)

22+ files use `: any[]` or `as any` extensively. Worst offenders:

| File | Line(s) | Usage Count |
|---|---|---|
| `VendorManagementDashboard.tsx` | 35, 36, 38, 40, 45, 52 | 6 `any` declarations |
| `TenderManagementContent.tsx` | 52, 61 | `map((doc: any)`, `map((crit: any)` |
| `VendorListingForm.tsx` | Throughout | Multiple `any[]` |
| `UserListing.tsx` | 127 | `payload: any = {}` |

**Fix:** Create `src/types/api.ts` with proper TypeScript interfaces for all API responses and request payloads.

---

#### 2.17 Hardcoded Colors and Magic Numbers (Frontend — Code Quality)

| Category | Examples | Files |
|---|---|---|
| Hex colors | `bg-[#B84521]`, `bg-[#D1D1D1]`, `text-[#1A3A5C]` | 15+ occurrences across pages |
| Heights | `max-h-[500px]`, `max-h-[90vh]` | VendorManagementDashboard (3x), RegisterVendorPage (6x) |
| Default values | `setMinCapital(50)`, `setNegLimit(2)` | TenderManagementContent (lines 23-24) |
| Repeated strings | `"- Select -"` placeholder | TenderManagementPage (lines 42, 50, 64, 77, 90) |

**Fix:** Centralize colors in `theme.css` or `tailwind.config.ts`. Extract magic numbers to named constants.

---

#### 2.18 Route Title Placeholders Not Interpolated (Frontend)

**File:** `Procura_App/.../src/app/routes.tsx` (lines 139-159)

```typescript
title="{declaration_name}"  // Literal string, not interpolated — 6 occurrences
```

---

### 3. GOOD PRACTICES FOUND

- **Clean three-tier architecture:** Controller -> Service -> Repository with strict dependency direction
- **RepositoryBase<TEntity, TDto>** generic pattern avoids CRUD boilerplate across 25+ repositories
- **AutoMapper profiles** in dedicated `DB/Profiles/` directory (20 mapping files) for clean entity-DTO mapping
- **JWT Bearer auth** with proper token validation parameters (issuer, audience, lifetime, signing key)
- **CSAResponseModel<T>** wrapper exists as standardized response format (needs consistent adoption)
- **Serilog** configured with daily rolling file logging + console
- **Connection string from environment variable** — good security practice (env var `ProcuraConnection`)
- **Multi-step vendor registration** with step tracking (`VendorRegistrationStep` enum: Profile, Members, Financial, Categories, Experiences, Declaration, Payment) is well-designed
- **Axios request interceptor** for automatic Bearer token injection on all API calls (`apiService.ts:10-16`)
- **Centralized API endpoint constants** — all URLs in one file (`apiEndpoints.ts`)
- **Shadcn/ui component library** with 47 Radix UI primitives in `app/components/ui/`
- **React Router v7** with nested layout routes for master dashboard with sidebar
- **Sonner toast notifications** for user feedback (configured in `Layout.tsx`)
- **URL-driven wizard state** — `RegisterVendorPage` uses `useSearchParams()` for browser back/forward support
- **Compiled-out code** properly preserved for future use via `<Compile Remove>` (12 controllers, not deleted)
- **BaseEntity** with audit fields (CreatedDate, ModifiedDate, CreatedBy) across all entities
- **Role-based access enum** — `Roles.cs` defines SystemAdmin, Vendor, BusinessUser
- **Daily rolling log files** — Serilog writes to `Logs/app_log{date}.txt`
- **No TODO/FIXME comments** — codebase is clean of temporary scaffolding

---

### 4. MISSING FEATURES

| Feature | Impact | Priority | Effort |
|---|---|---|---|
| **Global exception middleware** | Stack traces exposed to clients | P0 | 30 min |
| **Request validation middleware** | Invalid data reaches DB layer | P0 | 2 hrs |
| **Logout mechanism** | No way to clear session/token — user stuck until browser close | P1 | 1 hr |
| **Token refresh / expiry handling** | 60-min token expires silently; no 401 interceptor | P1 | 30 min |
| **API rate limiting** | Vulnerable to brute force / DDoS | P1 | 2 hrs |
| **Retry/circuit breaker for external APIs** (Ampersand, SSM, SAP) | Silent failures on 3rd-party outage | P1 | 3 hrs |
| **Frontend route guards** | Unauthenticated users can access protected pages via URL | P1 | 2 hrs |
| **Centralized TypeScript types** | No shared type definitions for API contracts | P2 | 4 hrs |
| **Frontend error boundary** | Unhandled React errors crash entire app | P2 | 30 min |
| **Audit logging** | BaseEntity has CreatedBy but no update history tracking | P2 | 4 hrs |
| **Automated tests** (unit + integration) | No test projects exist | P2 | Ongoing |
| **File upload type validation** | 100MB limit set but no file type validation | P2 | 1 hr |
| **Background email processor** | `EmailNotificationQueue` entity exists but no worker processes it | P2 | 4 hrs |
| **Health check endpoint** | No `/health` for monitoring/load balancer probes | P3 | 30 min |
| **Response compression** | Large payloads not compressed | P3 | 15 min |
| **API versioning** | No version prefix strategy | P3 | 2 hrs |

---

### 5. ENDPOINT ALIGNMENT — Frontend vs Backend Mismatches

| Issue | Frontend (apiEndpoints.ts) | Backend (Controller) | Severity |
|---|---|---|---|
| **URL typo** | `DeleteMaterialBudge` (line 38) | `DeleteMaterialBudget` | CRITICAL (404) |
| **Spelling** | `GetVendorDetilsById` | `GetVendorDetilsById` | LOW (both have same typo — consistent but wrong) |
| **Constant typo** | `SAVE_METERIAL_BUDGET` (line 37) | Endpoint URL is correct | LOW (JS const name only) |
| **Constant typo** | `UPDATE_METERIAL_BUDGET` (line 54) | Endpoint URL is correct | LOW (JS const name only) |
| **Constant typo** | `VENDER_MANAGEMNT_SETTING` (line 41) | Endpoint URL is correct | LOW (JS const name only) |
| **Duplicate endpoint** | `GET_ALL_MASTERDATA_CATAGORY` + `GET_ALL_CATEGORIES` | Same endpoint | LOW (redundant constant) |
| **Unused backend endpoints** | Not in `apiEndpoints.ts` | PaymentController (6), AdminController (4), MasterDataController (3), VendorController (5), NotificationController (4) | INFO |

---

### 6. ARCHITECTURE DIAGRAM

```
┌─────────────────────────────────────────────────────────────────────┐
│                        BROWSER (React 18 + Vite)                    │
│                                                                     │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────┐  ┌────────────┐ │
│  │  Pages (86)  │  │ Components   │  │ Context   │  │ Routes     │ │
│  │              │  │ (57 + 47 UI) │  │ (Login    │  │ (40+)      │ │
│  │ Vendor (11)  │  │              │  │  Modal)   │  │            │ │
│  │ Tender (18)  │  │ Layout       │  │           │  │ / (public) │ │
│  │ Opening (4)  │  │ Header       │  │ session   │  │ /vendor-*  │ │
│  │ Eval (5)     │  │ Footer       │  │ Storage   │  │ /master-*  │ │
│  │ Award (4)    │  │ Sidebar      │  │ (JWT +    │  │ /submission│ │
│  │ Admin (10)   │  │ VendorHeader │  │  vendorId)│  │ /tender-*  │ │
│  │ Bidding (4)  │  │ VendorTabs   │  │           │  │ /* (404)   │ │
│  │ Modals (10)  │  │              │  │           │  │            │ │
│  │ Payment (4)  │  │ Shadcn/Radix │  │           │  │            │ │
│  │ Other (16)   │  │ (47 prims)   │  │           │  │            │ │
│  └──────┬───────┘  └──────────────┘  └───────────┘  └────────────┘ │
│         │                                                           │
│  ┌──────▼───────────────────────────────────────────────────────┐   │
│  │  apiService.ts (48 functions)   ← Axios + Bearer interceptor │   │
│  │  apiEndpoints.ts (55+ URLs)     ← VITE_API_BASE_URL env var  │   │
│  │  ⚠ 29 funcs silently swallow errors (return null/[])         │   │
│  │  ⚠ No 401 response interceptor                               │   │
│  └──────┬───────────────────────────────────────────────────────┘   │
└─────────┼───────────────────────────────────────────────────────────┘
          │  HTTP/JSON (CORS: AllowAll ⚠)
          ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     .NET 7 Web API (Api/)                           │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │  MIDDLEWARE PIPELINE:                                        │   │
│  │  Swagger → StaticFiles → CORS → Authorization → Routing     │   │
│  │  ⚠ MISSING: ExceptionHandler, RateLimiting, ResponseCache   │   │
│  └──────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  ┌──────────────────────┐  ┌─────────────────┐  ┌───────────────┐  │
│  │  Controllers (16)    │  │  Auth            │  │  Helpers      │  │
│  │  ├ Auth              │  │  ├ JWT Bearer    │  │  ├ Ampersand  │  │
│  │  ├ Vendor (24 ep)    │  │  ├ 60-min expiry │  │  ├ Email      │  │
│  │  ├ TenderMgmt (24)   │  │  ├ Role claims   │  │  ├ Signature  │  │
│  │  ├ Bidding (10)      │  │  │  (userid,     │  │  └ Network    │  │
│  │  ├ User (9)          │  │  │   roleid,     │  │               │  │
│  │  ├ MasterData (12)   │  │  │   Name)       │  └───────────────┘  │
│  │  ├ Payment (6)       │  │  └ 3 roles:      │                     │
│  │  ├ Admin (11)        │  │    SystemAdmin,   │                     │
│  │  ├ Announcement (5)  │  │    Vendor,        │                     │
│  │  ├ Content (5)       │  │    BusinessUser   │                     │
│  │  ├ Notification (4)  │  └─────────────────┘                      │
│  │  ├ CategoryCode (6)  │                                           │
│  │  └ SelectList (1)    │                                           │
│  │  ⚠ 70+ endpoints     │                                           │
│  │    have no try-catch  │                                           │
│  │  ⚠ 29+ [AllowAnon]   │                                           │
│  │    on write ops       │                                           │
│  └──────────┬───────────┘                                           │
│             ▼                                                       │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │  BusinessLogic Layer (20+ Services)                          │   │
│  │  IVendorService, ITenderService, IBiddingService,            │   │
│  │  IUserService, IPaymentService, ISAPServices,                │   │
│  │  IRoleService, IMenuService, INotificationService,           │   │
│  │  ICategoryCodeApprovalService, IMasterDataService,           │   │
│  │  IAnnouncementService, IContentService, IDropDownService     │   │
│  └──────────┬───────────────────────────────────────────────────┘   │
│             ▼                                                       │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │  Data Access Layer (DB/)                                     │   │
│  │  RepositoryBase<T,TDto> ⚠ (AddAsync has silent catch)       │   │
│  │  ├ VendorRepository     ├ TenderRepository                  │   │
│  │  ├ BiddingRepository    ├ UserRepository                    │   │
│  │  ├ PaymentRepository    ├ AnnouncementRepository            │   │
│  │  ├ CategoryCodeApprovalRepository  ├ DropdownRepository     │   │
│  │  └ AutoMapper Profiles (20 files)                           │   │
│  │  ⚠ No pagination in any GetAll method                       │   │
│  │  ⚠ No custom database indexes                               │   │
│  └──────────┬───────────────────────────────────────────────────┘   │
└─────────────┼───────────────────────────────────────────────────────┘
              ▼
┌──────────────────────┐  ┌────────────────────┐  ┌──────────────────┐
│    SQL Server        │  │  Ampersand Pay     │  │  SAP System      │
│    (EF Core 7.0)     │  │  (Payment Gateway) │  │  (Vendor Codes)  │
│                      │  │                    │  │                  │
│  ProcuraDbContext     │  │  Sandbox:          │  │  ZBAPI Client    │
│  150+ DbSets         │  │  stg-ipg.ampersand │  │  ⚠ Simulator ON  │
│  75+ Entity classes   │  │  pay.com           │  │  (Program.cs:93) │
│  40+ Migrations      │  │  Webhook: POST     │  │                  │
│                      │  │  /api/payment/     │  │                  │
│  Env var:            │  │  webhook           │  │                  │
│  ProcuraConnection   │  │                    │  │                  │
└──────────────────────┘  └────────────────────┘  └──────────────────┘
              │
              │           ┌────────────────────┐  ┌──────────────────┐
              │           │  SSM Search API    │  │  File Storage    │
              │           │  (Company Lookup)  │  │                  │
              │           │                    │  │  C:\VendorUploads│
              │           │  Sandbox:          │  │  Served at:      │
              │           │  cidp.ssmsearch.com│  │  /VendorUploads  │
              │           │                    │  │  Max: 100 MB     │
              │           └────────────────────┘  └──────────────────┘
              │
              ▼
┌──────────────────────┐
│  Logs                │
│  Serilog → Console   │
│  Serilog → File      │
│  Logs/app_log{date}  │
│  .txt (daily rolling)│
└──────────────────────┘
```

---

### 7. PRIORITY ACTION PLAN

| Priority | Issue | Section | Effort | Impact |
|---|---|---|---|---|
| **P0** | Fix RepositoryBase.AddAsync() silent catch | 1.1 | 5 min | Prevents silent data loss across entire system |
| **P0** | Add global exception filter | 1.2 | 30 min | Prevents stack trace leaks to clients |
| **P0** | Remove [AllowAnonymous] from write endpoints | 1.4 | 1 hr | Prevents unauthorized data manipulation |
| **P0** | Move secrets out of appsettings.json | 1.5 | 1 hr | Security compliance |
| **P1** | Make all frontend save functions throw errors | 1.6 | 2 hrs | Users see actual save failures |
| **P1** | Add 401 response interceptor | 1.10 | 15 min | Proper auth expiry handling |
| **P1** | Add ErrorBoundary to App.tsx | 1.9 | 30 min | Prevents white-screen app crashes |
| **P1** | Fix endpoint URL typo (DeleteMaterialBudge) | 1.8 | 5 min | Fix silent 404 on delete |
| **P1** | Add pagination to list endpoints | 2.1 | 4 hrs | Required for 1000+ scale |
| **P2** | Add database indexes | 2.6 | 2 hrs | Query performance at scale |
| **P2** | Consolidate BiddingRepository queries (6→1-2) | 2.2 | 2 hrs | 6x fewer DB roundtrips |
| **P2** | Restrict CORS to frontend domain | 2.8 | 15 min | Security hardening |
| **P2** | Standardize error response format | 2.7 | 3 hrs | Consistent API contract |
| **P2** | Extract duplicate utility functions | 2.10 | 2 hrs | Code maintainability |
| **P3** | Remove console.log statements | 2.15 | 1 hr | Clean production output |
| **P3** | Add TypeScript interfaces for API | 2.16 | 4 hrs | Type safety |
| **P3** | Add loading skeletons to pages | 2.13 | 2 hrs | Better UX |
| **P3** | Add search debouncing | 2.14 | 1 hr | Reduce API load |
| **P3** | Fix hardcoded colors/magic numbers | 2.17 | 2 hrs | Maintainability |

---

### Phase 2 Development Rules

- Do NOT modify existing pages unless for route mapping or critical fixes
- New features go in new page components
- Backend changes should not break existing API contracts
- All new endpoints must include try-catch error handling
- All new endpoints must use `CSAResponseModel<T>` response wrapper
- All new API service functions in frontend must throw errors (never return null/[] on failure)
- All new repository methods must use async properly (`.ToListAsync()` not `.ToList()`)
- All new list endpoints must support pagination (PageNumber/PageSize)
- Do not add [AllowAnonymous] to write/delete endpoints without explicit approval
