# Change Log

| Timestamp | File | Action | Requirement |
|-----------|------|---------|-------------|
| 2026-04-06 | DB/Profiles/UserProfile.cs | Fixed AutoMapper mappings for `User → UserDTO`: resolved `int SiteOffice → State SiteOffice` type crash; added explicit mappings for `Email`, `Mobile`, `SiteOfficeId`, `SiteOfficeName`, navigation property ignores on reverse map | `GetUserById` threw `AutoMapperMappingException: Missing type map Int32 → State` because `User.SiteOffice` (int FK) and `UserDTO.SiteOffice` (State nav property) share a name but have incompatible types |
| 2026-04-03 | DB/Entity/UserDTO.cs | Added `SiteLevelName`, `SiteOfficeName`, `DesignationName` string properties | User list API response was missing site level, site office, and designation data |
| 2026-04-03 | DB/Repositories/UserRepository.cs | Updated all 4 user query projections to map `SiteLevelId`, `SiteOfficeId`, `DesignationId`, and name fields; added `Include(x => x.Designation)` to all queries | User list API response was missing site level, site office, and designation data |
