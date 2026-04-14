# Change Log

| Timestamp | File | Action | Requirement |
|-----------|------|---------|-------------|
| 2026-04-11 | DB/Repositories/Interfaces/IRolePermissionRepository.cs | Added `GeteRolePermissionAsync(int PermissionId)` to interface | Method existed in the implementation but was missing from the interface, causing a compiler contract violation |
| 2026-04-11 | BusinessLogic/Interfaces/IRoleMenuPermissionService.cs | Added `GetRolePermissionAsync(int permissionId)` to service interface | Expose get-by-id capability through the service layer |
| 2026-04-11 | BusinessLogic/Services/RoleMenuPermissionService.cs | Implemented `GetRolePermissionAsync` delegating to repository | Wire service layer to repository for get-by-id permission lookup |
| 2026-04-11 | Api/Controllers/AdminController.cs | Added `GetRolePermissionById` GET endpoint | Expose `GET /api/Admin/GetRolePermissionById?permissionId={id}` so clients can fetch a single role menu permission |
| 2026-04-10 | DB/Repositories/VendorRepository.cs | Added base64-to-file save logic for `VendorBank.Attachment` in `SaveFinancialAsync` | Bank statement attachment must be saved as a physical file under `C:\VendorUploads\` and only the file path stored in the DB, consistent with how `LatestBankStatementPath` is handled |
| 2026-04-10 | DB/Model/VendorBank.cs | Added `BankBranch`, `BankBranchAddress`, `Balance`, `FixedDeposit`, `Attachment` fields | Vendor registration Financial step requires Bank Branch, Bank Branch Address, Balance (RM last 3 months), Fixed Deposit (RM), and bank statement attachment as per the UI screen |
| 2026-04-06 | DB/Profiles/UserProfile.cs | Fixed AutoMapper mappings for `User → UserDTO`: resolved `int SiteOffice → State SiteOffice` type crash; added explicit mappings for `Email`, `Mobile`, `SiteOfficeId`, `SiteOfficeName`, navigation property ignores on reverse map | `GetUserById` threw `AutoMapperMappingException: Missing type map Int32 → State` because `User.SiteOffice` (int FK) and `UserDTO.SiteOffice` (State nav property) share a name but have incompatible types |
| 2026-04-03 | DB/Entity/UserDTO.cs | Added `SiteLevelName`, `SiteOfficeName`, `DesignationName` string properties | User list API response was missing site level, site office, and designation data |
| 2026-04-03 | DB/Repositories/UserRepository.cs | Updated all 4 user query projections to map `SiteLevelId`, `SiteOfficeId`, `DesignationId`, and name fields; added `Include(x => x.Designation)` to all queries | User list API response was missing site level, site office, and designation data |
