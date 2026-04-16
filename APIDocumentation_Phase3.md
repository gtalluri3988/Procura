# PROCURA – Phase 3 API Documentation: Screen-to-API Deep Mapping

**Version:** 1.0  
**Date:** 2026-04-16  
**Base URL:** `https://localhost:7225` (configurable per environment)  
**Framework:** ASP.NET Core 7.0 Web API  
**Client:** FELDA Plantation Management Sdn. Bhd. (FPMSB)

---

## TABLE OF CONTENTS

1. [Common Standards](#1-common-standards)
2. [Module 1 – Tender Opening (Admin Portal)](#module-1--tender-opening-admin-portal)
3. [Module 2 – Tender Evaluation (Admin Portal)](#module-2--tender-evaluation-admin-portal)
4. [Module 3 – Tender Award (Admin Portal)](#module-3--tender-award-admin-portal)
5. [Module 4 – Bidding (Vendor Portal)](#module-4--bidding-vendor-portal)
6. [Screen-wise Mapping Summary Table](#screen-wise-mapping-summary-table)
7. [Critical Gaps](#critical-gaps)
8. [Improvements](#improvements)
9. [Unused APIs](#unused-apis)
10. [Missing APIs](#missing-apis)
11. [Validation & Edge Cases](#validation--edge-cases)
12. [Recommended API Contract (DTO Structure)](#recommended-api-contract-dto-structure)
13. [Phase 3 API Summary Table](#phase-3-api-summary-table)
14. [Navigation Flows](#navigation-flows)

---

## 1. Common Standards

*(Same as Phase 1/2 — see `API_DOCUMENTATION.md` Section 1 for full details)*

- **Route Pattern:** `/api/{Controller}/{Action}`
- **Response Wrapper:** `CSAResponseModel<T>` → `{ "error": false, "errors": [], "data": { ... } }`
- **Auth Header:** `Authorization: Bearer <jwt_token>`
- **Content-Type:** `application/json` (or `multipart/form-data` for file uploads)
- **Enums:** Serialized as strings

---

## MODULE 1 – TENDER OPENING (ADMIN PORTAL)

### Screen 1.1 – Tender Opening Listing

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_41_47.png`  
**Description:** List of tenders in the Opening phase. Filter by Status dropdown. Click Reference ID link to open tender details.

**UI Elements:** Status filter (dropdown), Search button, Table (No., Reference ID link, Project Name, Start Date, End Date, Status).

#### API 1.1.1 – Get Tender Opening List

**Method:** GET  
**URL:** `/api/TenderManagement/GetTenderOpeningList`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `referenceId` | string | No | Filter by reference ID |
| `projectName` | string | No | Filter by project name |

**Response:**
```json
[
  {
    "id": 45,
    "referenceId": "2026/B020",
    "projectName": "Supply, Delivery, and Distribution of Compound and Straight Fertilizers...",
    "startDate": "2026-01-29T08:11:54",
    "endDate": "2026-01-29T08:11:54",
    "tenderStatus": "New"
  }
]
```

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | No. | *(row index)* | int | Generated client-side |
| 2 | Reference ID | `referenceId` | string | Clickable link → navigates to Opening Detail |
| 3 | Project Name | `projectName` | string | |
| 4 | Start Date | `startDate` | DateTime | Format: `dd/MM/yyyy HH:mm:ss` |
| 5 | End Date | `endDate` | DateTime | Format: `dd/MM/yyyy HH:mm:ss` |
| 6 | Status | `tenderStatus` | string | Values: New, In Progress, Completed |

**Backend DTO:** `TenderOpeningListDto` → `DB/Entity/TenderOpeningDto.cs`

**Data Contract Issues:**
- Screen shows Status filter dropdown but API has no `status` filter parameter — filtering must be done client-side or by reference/project name search
- Screen column "Status" maps to `tenderStatus` (not `status`) — consistent naming recommended

---

### Screen 1.2 – Tender Opening Appointment

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_47_16.png` *(shared with Evaluation)*  
**Description:** Committee member's appointment notification. Shows tender details and instructions to proceed with the opening process.

**UI Elements:** "TENDER EVALUATION APPOINMENT" header (note: same template used for opening), Reference ID, Project Name, appointment message text, Start Date, End Date, instruction text. Buttons: Back, Proceed.

#### API 1.2.1 – Get Tender Opening Detail

**Method:** GET  
**URL:** `/api/TenderManagement/GetTenderOpeningDetail?tenderId={id}`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tenderId` | int | Yes | Tender application ID |

**Response:**
```json
{
  "id": 45,
  "referenceId": "T1823004",
  "projectName": "Supply, Delivery, and Distribution of Compound and Straight Fertilizers for FELDA Plantation Management Sdn. Bhd. Estates in the Central and Southern Regions",
  "startDate": "2026-02-03T00:00:00",
  "endDate": "2026-02-10T00:00:00"
}
```

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Reference ID | `referenceId` | string | Display only |
| 2 | Project Name | `projectName` | string | Display only (bold, full text) |
| 3 | Start Date | `startDate` | DateTime | Format: `dd/MM/yyyy` |
| 4 | End Date | `endDate` | DateTime | Format: `dd/MM/yyyy` |

**Backend DTO:** `TenderOpeningDetailDto` → `DB/Entity/TenderOpeningDto.cs`

**Data Contract Issues:**
- Screen shows `committeeRole` text ("member of the Tender Evaluation Committee") but the DTO does not include a `committeeRole` field — this is likely hardcoded on the frontend

---

### Screen 1.3 – Tender Opening Summary Tab

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_42_41.png`  
**Description:** "Summary" tab showing tender details and list of vendors who submitted bids. Includes Print and Verify buttons.

**UI Elements:** Two tabs (Summary / Progress). Reference ID, Project Name (display). Info table: Closing Date, Closing Time, Type, Category Code, Estimation Cost (RM), Validity. Vendor table: Bil, Vendor Name (clickable), Bumi / Non Bumi Status, Registration Expiry, Offered Price (RM). Buttons: Print, Verify.

#### API 1.3.1 – Get Tender Opening Page (Summary)

**Method:** GET  
**URL:** `/api/TenderManagement/GetTenderOpeningPage?tenderId={id}`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tenderId` | int | Yes | Tender application ID |

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T1234567",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN MENEBANG DAN MEMANGKAS POKOK BAHAYA DI KAWASAN TEMPAT MELETAK KENDERAAN DI TAMAN RIMBA BUKIT MERTAJAM, PULAU PINANG.",
  "summary": {
    "closingDate": "2024-03-12T00:00:00",
    "closingTime": "12:00 PM",
    "type": "SERVICE",
    "categoryCode": "010101 AND 010304",
    "estimationCost": 500000.00,
    "validity": "90 DAYS"
  },
  "vendors": [
    {
      "bil": 1,
      "vendorName": "Vendor 1",
      "bumiStatus": "Bumiputera",
      "registrationExpiry": "2023-03-09T00:00:00",
      "offeredPrice": 500000.00
    },
    {
      "bil": 2,
      "vendorName": "Vendor 2",
      "bumiStatus": "Bumiputera",
      "registrationExpiry": "2023-03-12T00:00:00",
      "offeredPrice": 450000.00
    },
    {
      "bil": 3,
      "vendorName": "Vendor 3",
      "bumiStatus": "Non Bumiputera",
      "registrationExpiry": "2023-03-23T00:00:00",
      "offeredPrice": 570000.00
    }
  ]
}
```

**Screen → API Field Mapping (Summary Section):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Reference ID | `referenceId` | string | Display only |
| 2 | Project Name | `projectName` | string | Display only (bold) |
| 3 | Closing Date | `summary.closingDate` | DateTime | Format: `dd/MM/yyyy` |
| 4 | Closing Time | `summary.closingTime` | string | Format: `hh:mm AM/PM` |
| 5 | Type | `summary.type` | string | e.g., "SERVICE" |
| 6 | Category Code | `summary.categoryCode` | string | Multiple codes joined with "AND" |
| 7 | Estimation Cost (RM) | `summary.estimationCost` | decimal | Format: `#,##0.00` |
| 8 | Validity | `summary.validity` | string | e.g., "90 DAYS" |

**Screen → API Field Mapping (Vendor Table):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Bil | `vendors[].bil` | int | Sequential number |
| 2 | Vendor Name | `vendors[].vendorName` | string | Clickable → opens Vendor Details tab |
| 3 | Bumi / Non Bumi Status | `vendors[].bumiStatus` | string | "Bumiputera" or "Non Bumiputera" |
| 4 | Registration Expiry | `vendors[].registrationExpiry` | DateTime | Format: `dd/MM/yyyy` |
| 5 | Offered Price (RM) | `vendors[].offeredPrice` | decimal | Format: `#,##0.00` |

**Backend DTOs:**
- `TenderOpeningPageDto` → `DB/Entity/TenderOpeningDto.cs`
- `TenderOpeningSummaryRowDto` → `DB/Entity/TenderOpeningDto.cs`
- `TenderOpeningVendorDto` → `DB/Entity/TenderOpeningDto.cs`

#### API 1.3.2 – Verify Tender Opening

**Method:** POST  
**URL:** `/api/Bidding/VerifyTenderOpening`  
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "remarks": "All bids received and verified."
}
```

**Screen → API Field Mapping (Verify Action):**

| # | UI Field | API Request Field | Type | Notes |
|---|----------|-------------------|------|-------|
| 1 | *(Verify button click)* | `tenderId` | int | From current tender context |
| 2 | *(optional remarks)* | `remarks` | string? | Verification comments |

**Backend DTO:** `VerifyTenderOpeningDto` → `DB/Entity/BiddingDto.cs`

**Success Response (200):** `"Tender opening verified successfully"`

---

### Screen 1.4 – Tender Opening Progress Tab

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_43_17.png`  
**Description:** "Progress" tab showing bid submission progress for each vendor with detailed vendor information.

**UI Elements:** Two tabs (Summary / Progress). Table: Bil, Company Reg. No (clickable link), Vendor Name, Type, State, District, Vendor Status, Submission Date / Time, Receipt No., Progress (color-coded: Pending=yellow, Completed=green).

#### API 1.4.1 – Get Tender Opening Progress

**Method:** GET  
**URL:** `/api/Bidding/GetTenderOpeningProgress?tenderId={id}`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tenderId` | int | Yes | Tender application ID |

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T1234567",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "openingStatus": "Pending",
  "verifiedAt": null,
  "verifiedByName": null,
  "remarks": null,
  "totalBidsReceived": 3,
  "bidProgress": [
    {
      "bil": 1,
      "vendorName": "Vendor 1 Sdn. Bhd",
      "offeredPrice": 500000.00,
      "openingStatus": "Pending"
    }
  ]
}
```

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Match Status |
|---|----------|--------------------|------|--------------|
| 1 | Bil | `bidProgress[].bil` | int | MATCHED |
| 2 | Company Reg. No | **NOT IN API** | string | MISSING |
| 3 | Vendor Name | `bidProgress[].vendorName` | string | MATCHED |
| 4 | Type | **NOT IN API** | string | MISSING |
| 5 | State | **NOT IN API** | string | MISSING |
| 6 | District | **NOT IN API** | string | MISSING |
| 7 | Vendor Status | **NOT IN API** | string | MISSING |
| 8 | Submission Date / Time | **NOT IN API** | DateTime | MISSING |
| 9 | Receipt No. | **NOT IN API** | string | MISSING |
| 10 | Progress | `bidProgress[].openingStatus` | string | PARTIAL — API has "Pending"/"Passed"/"Failed"; UI shows "Pending"/"Completed" |

**Backend DTO:** `TenderOpeningProgressDto` / `TenderOpeningBidProgressDto` → `DB/Entity/BiddingDto.cs`

> **CRITICAL GAP:** The `TenderOpeningBidProgressDto` only has 4 fields (`Bil`, `VendorName`, `OfferedPrice`, `OpeningStatus`), but the screen requires 10 fields. 6 fields are MISSING from the API response. See [Critical Gaps](#critical-gaps) section for required DTO changes.

---

### Screen 1.5 – Tender Opening: Vendor Details Tab

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_43_32.png`  
**Description:** Deep-dive vendor details view accessed by clicking a vendor name in the Opening Progress tab. Shows three sub-tabs: Vendor Details, Submitted Documents, Summary. This screen shows full vendor profile, financial details, category codes (FPMSB, MOF, CIDB), and experiences — all read-only.

**UI Elements — Vendor Details Sub-Tab:**
- **Company Info:** Company Type, Company Name, Registration No., Vendor ID, Date of Incorporation, Address, Postcode, State, City, Country, Office Phone No., Fax No., Email Address, Website, Type of Industry, Business Coverage Area, Company's PIC Name, PIC Mobile No., PIC Email Address
- **Financial Details:** Paid-Up Capital (RM), Authorized Capital (RM), Working Capital (RM), Liquid Capital (RM), Asset Balance (RM)
- **Category Code — FPMSB:** Table (Category, Sub-Category, Activities)
- **Category Code — MOF:** Table (Category, Sub-Category, Sub-Category Division)
- **Category Code — CIDB:** Table (Category, Code, Description)
- **Experiences:** Table (No., Project Name, Organization, Project Value RM, Status, Completion Year, Attachment link)
- Button: Back

#### API 1.5.1 – Get Vendor Details by ID

**Method:** GET  
**URL:** `/api/Vendor/GetVendorDetilsById?vendorId={id}`  
**Authentication:** Required

**Note:** This endpoint returns a limited vendor profile. For the full view shown in this screen, the frontend must make MULTIPLE API calls.

**Screen → API Endpoint Mapping:**

| UI Section | API Endpoint | Method |
|------------|-------------|--------|
| Company Info (Profile) | `/api/Vendor/GetVendorDetilsById?vendorId={id}` | GET |
| Financial Details | `/api/Vendor/Financial?vendorId={id}` | GET *(read)* |
| Category Codes | `/api/Vendor/Categories?vendorId={id}` | GET *(read)* |
| Experiences | `/api/Vendor/Experiences?vendorId={id}` | GET *(read)* |

**Screen → API Field Mapping (Company Info Section):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Company Type | `companyType` / `companyEntityTypeId` | string/int | Dropdown display |
| 2 | Company Name | `companyName` | string | |
| 3 | Registration No. | `registrationNo` / `rocNumber` | string | |
| 4 | Vendor ID | `vendorCode` | string | e.g., "VEN2667001" |
| 5 | Date of Incorporation | `dateOfIncorporation` | DateTime | Format: `dd-mm-yyyy` |
| 6 | Address | `address` | string | Multiline |
| 7 | Postcode | `postcode` | string | |
| 8 | State | `stateId` → dropdown lookup | int | Requires SelectList API |
| 9 | City | `city` | string | |
| 10 | Country | `countryId` → dropdown lookup | int | Requires SelectList API |
| 11 | Office Phone No. | `officePhoneNo` | string | |
| 12 | Fax No. | `faxNo` | string | |
| 13 | Email Address | `emailAddress` / `email` | string | |
| 14 | Website | `website` | string | |
| 15 | Type of Industry | `industryTypeId` → dropdown | int | Requires SelectList API |
| 16 | Business Coverage Area | `businessCoverageArea` | string | |
| 17 | Company's PIC Name | `picName` | string | |
| 18 | PIC Mobile No. | `picMobileNo` | string | |
| 19 | PIC Email Address | `picEmailAddress` / `picEmail` | string | |

**Screen → API Field Mapping (Financial Section):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Paid-Up Capital (RM) | `paidUpCapital` | decimal | |
| 2 | Authorized Capital (RM) | `authorizedCapital` | decimal | |
| 3 | Working Capital (RM) | `workingCapital` | decimal | |
| 4 | Liquid Capital (RM) | `liquidCapital` | decimal | |
| 5 | Asset Balance (RM) | `assetBalance` | decimal | |

**Screen → API Field Mapping (Category Codes — FPMSB):**

| # | UI Column | API Response Field | Type |
|---|-----------|-------------------|------|
| 1 | Category | `category.description` | string |
| 2 | Sub-Category | `subCategory.description` | string |
| 3 | Activities | `activity.description` | string |

**Screen → API Field Mapping (Experiences):**

| # | UI Column | API Response Field | Type |
|---|-----------|-------------------|------|
| 1 | No. | *(row index)* | int |
| 2 | Project Name | `projectName` | string |
| 3 | Organization | `organization` | string |
| 4 | Project Value (RM) | `projectValue` | decimal |
| 5 | Status | `status` | string |
| 6 | Completion Year | `completionYear` | int? |
| 7 | Attachment | `attachmentPath` | string |

**Backend DTOs:**
- `VendorProfileDto` → `DB/Entity/VendorProfileDto.cs`
- `VendorFinancialDto` → `DB/Entity/VendorFinancialDto.cs`
- `VendorCategoryDto` → `DB/Entity/VendorCategoryDto.cs`
- `VendorExperienceDto` → `DB/Entity/VendorExperienceDto.cs`

> **IMPROVEMENT:** No single composite endpoint exists to return all vendor details for the tender opening context. Frontend must make 4 separate API calls. See [Improvements](#improvements) for recommended composite DTO.

---

### Screen 1.6 – Tender Opening: Submitted Documents Tab

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_43_45.png`  
**Description:** Shows documents submitted by a vendor for this tender. Includes two-level validation workflow (1st Validation, 2nd Validation) with a Validate action button per document.

**UI Elements:** Vendor name header ("Teguh Maju Sdn. Bhd."). Table: No, Document Name, Attachment (link to PDF), Submission (Submitted / Not Applicable), 1st Validation (color-coded: Pending=yellow, Passed=green, Failed=red), 2nd Validation (Pending), Action (Validate button). Button: Back.

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Match Status |
|---|----------|--------------------|------|--------------|
| 1 | No | *(row index)* | int | N/A |
| 2 | Document Name | **NO API** | string | MISSING |
| 3 | Attachment | **NO API** | string (URL) | MISSING |
| 4 | Submission | **NO API** | string | MISSING |
| 5 | 1st Validation | **NO API** | string | MISSING |
| 6 | 2nd Validation | **NO API** | string | MISSING |
| 7 | Action (Validate) | **NO API** | button | MISSING |

> **CRITICAL GAP:** No API endpoint or DTO exists for the Submitted Documents tab or the document validation workflow. This entire feature needs to be built. See [Missing APIs](#missing-apis) for required endpoints and DTOs.

---

### Screen 1.7 – Document Validation Popup

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_45_55.png`  
**Description:** Modal popup triggered by clicking the "Validate" button on a submitted document. Shows two validation levels with the validator's name, a Result dropdown (Pass/Fail), and a Remarks text area.

**UI Elements:**
- **1st Validation (Mohamad Ali bin Ismail):** Result dropdown (-Select- / Passed / Failed), Remarks text area
- **2nd Validation (Abdul Rahim):** Result dropdown (-Select- / Passed / Failed), Remarks text area
- Buttons: Close, Save

**Screen → API Field Mapping:**

| # | UI Field | API Request Field | Type | Match Status |
|---|----------|-------------------|------|--------------|
| 1 | 1st Validation — Validator Name | **NO API** | string | MISSING |
| 2 | 1st Validation — Result | **NO API** | string | MISSING |
| 3 | 1st Validation — Remarks | **NO API** | string | MISSING |
| 4 | 2nd Validation — Validator Name | **NO API** | string | MISSING |
| 5 | 2nd Validation — Result | **NO API** | string | MISSING |
| 6 | 2nd Validation — Remarks | **NO API** | string | MISSING |

> **CRITICAL GAP:** No API endpoint or DTO exists for saving document validation results. This is a two-level approval workflow that needs a complete backend implementation. See [Missing APIs](#missing-apis).

---

### Screen 1.8 – Tender Opening: Summary / Price Tab

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_44_41.png`  
**Description:** Third sub-tab showing vendor's pricing breakdown with material-level detail. Supports negotiation rounds — shows original submission and each negotiation summary.

**UI Elements:** Vendor name header ("Teguh Maju Sdn. Bhd."). Materials table: No., Material Code, Material Group, Material Description, Quantity, Unit, Price Per Unit (RM) (editable), Total (RM). Totals: Vendor Total Price Without SST (RM), FPMSB Estimation Price (RM). Negotiation Summary section (numbered, timestamped): Same material table structure. Button: Back.

**Screen → API Field Mapping:**

| # | UI Field | Likely API Source | Type | Match Status |
|---|----------|-------------------|------|--------------|
| 1 | Material Code | `jobScope[].materialCode` | string | FROM TenderApplication |
| 2 | Material Group | `jobScope[].materialGroupDescription` | string | FROM TenderApplication |
| 3 | Material Description | `jobScope[].shortTask` | string | FROM TenderApplication |
| 4 | Quantity | `jobScope[].quantity` | int | FROM TenderApplication |
| 5 | Unit | `jobScope[].unit` | string | FROM TenderApplication |
| 6 | Price Per Unit (RM) | **PARTIAL** | decimal | Vendor's submitted price — needs bid item mapping |
| 7 | Total (RM) | **COMPUTED** | decimal | `quantity × pricePerUnit` |
| 8 | Vendor Total Price Without SST | **PARTIAL** | decimal | Sum of all totals |
| 9 | FPMSB Estimation Price | `jobScope[].subTotal` SUM | decimal | FROM TenderApplication |
| 10 | Negotiation Summary | **NO API** | object | MISSING — no negotiation history endpoint |

**Backend Notes:**
- Material details come from `TenderApplicationDto.jobScope` (already in `GetTenderApplicationById`)
- Vendor's per-material pricing requires mapping between `BidderSubmissionItem` and `TenderJobScope`
- Negotiation summary (multiple rounds) is not currently supported by any API

> **CRITICAL GAP:** No dedicated endpoint returns vendor pricing breakdown with material-level detail in the tender opening context. The current `BiddingAssetDto` is designed for asset-based bidding (vehicles), not material/service-based pricing. Negotiation history tracking is also missing. See [Missing APIs](#missing-apis).

---

## MODULE 2 – TENDER EVALUATION (ADMIN PORTAL)

### Screen 2.1 – Tender Evaluation Listing

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_46_57.png`  
**Description:** List of tenders in the Evaluation phase. Same layout as Tender Opening Listing.

**UI Elements:** Status filter (dropdown), Search button, Table (No., Reference ID link, Project Name, Start Date, End Date, Status).

**Frontend Note:** No separate evaluation listing endpoint exists. Reuse `GET /api/TenderManagement/GetTenderOpeningList` and filter by evaluation-phase status on the client side, OR use `GetAllTenderApplications` with status filter.

**Screen → API Field Mapping:** *(Same as Screen 1.1)*

| # | UI Field | API Response Field | Type |
|---|----------|--------------------|------|
| 1 | No. | *(row index)* | int |
| 2 | Reference ID | `referenceId` | string |
| 3 | Project Name | `projectName` | string |
| 4 | Start Date | `startDate` | DateTime |
| 5 | End Date | `endDate` | DateTime |
| 6 | Status | `tenderStatus` | string |

> **IMPROVEMENT:** A dedicated `GetTenderEvaluationList` endpoint with evaluation-specific statuses would be cleaner than reusing the opening list.

---

### Screen 2.2 – Tender Evaluation Appointment

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_47_16.png`  
**Description:** Evaluation committee member's appointment notification. Same template as Opening Appointment (Screen 1.2).

**UI Elements:** Header "TENDER EVALUATION APPOINMENT", Reference ID, Project Name, appointment text, Start Date, End Date, instruction text. Buttons: Back, Proceed.

**API Used:** `GET /api/TenderManagement/GetTenderOpeningDetail?tenderId={id}` *(shared with Opening)*

**Screen → API Field Mapping:** *(Same as Screen 1.2)*

---

### Screen 2.3 – Tender Evaluation Details

**Screen File:** `Screenshot 2026-04-03 071432.png` + `Screenshot 2026-04-16 115322.png`  
**Description:** Main evaluation page with four sections: Technical Evaluation, Financial Evaluation, Recommendation, and Vendor Offer popup. Clicking a Tenderer Code opens the Technical Scoring popup. Clicking Vendor Offer Price opens the Vendor Offer Summary popup.

**UI Elements:**
- **Technical Evaluation:** Table (Tenderer Code link, Tender Opening Status (color-coded), Technical Evaluation Status, Result, Ranking). Download Report button.
- **Financial Evaluation:** Table (Tenderer Code, Capital Liquidation RM, Asset Balance RM, Final Amount RM, Min. Capital Required RM, Result (color-coded)). Download Report button.
- **Recommendation:** Table (Tenderer Code, Tender Opening Status, Technical Evaluation Status, Financial Evaluation Status, Vendor Offer Price (clickable link), FPMSB Estimation Price RM, Difference %). Recommended Tenderer dropdown, Reason text area. Buttons: Back, Save.

#### API 2.3.1 – Get Tender Evaluation Page

**Method:** GET  
**URL:** `/api/TenderManagement/GetTenderEvaluationPage?tenderId={id}`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tenderId` | int | Yes | Tender application ID |

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "S11621009",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "technicalRows": [
    {
      "vendorId": 101,
      "tendererCode": "1/3",
      "tenderOpeningStatus": "Pending",
      "technicalEvaluationStatus": "Pending",
      "result": "Pending",
      "ranking": 1
    }
  ],
  "financialRows": [
    {
      "vendorId": 101,
      "tendererCode": "1/3",
      "capitalLiquidation": 299132.37,
      "assetBalance": 542597.89,
      "finalAmount": 542597.89,
      "minCapitalRequired": 33905.65,
      "result": "Passed"
    }
  ],
  "recommendation": {
    "tenderId": 45,
    "recommendedVendorId": null,
    "reason": null,
    "rows": [
      {
        "vendorId": 101,
        "tendererCode": "1/3",
        "tenderOpeningStatus": "Pending",
        "technicalEvaluationStatus": "Pending",
        "financialEvaluationStatus": "Pending",
        "vendorOfferPrice": 40000.00,
        "fpmsEstimationPrice": 100000.00,
        "differencePercent": 60.0
      }
    ],
    "vendorOptions": [
      { "vendorId": 101, "tendererCode": "1/3" },
      { "vendorId": 102, "tendererCode": "2/3" }
    ]
  }
}
```

**Screen → API Field Mapping (Technical Evaluation):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Tenderer Code | `technicalRows[].tendererCode` | string | Clickable link → opens scoring popup |
| 2 | Tender Opening Status | `technicalRows[].tenderOpeningStatus` | string | Color-coded: Pending=yellow, Passed=green, Failed=red |
| 3 | Technical Evaluation Status | `technicalRows[].technicalEvaluationStatus` | string | Same color coding |
| 4 | Result | `technicalRows[].result` | string | Pending / Passed / Failed |
| 5 | Ranking | `technicalRows[].ranking` | int | Auto-computed from scores |

**Screen → API Field Mapping (Financial Evaluation):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Tenderer Code | `financialRows[].tendererCode` | string | |
| 2 | Capital Liquidation (RM) | `financialRows[].capitalLiquidation` | decimal | From vendor financial data |
| 3 | Asset Balance (RM) | `financialRows[].assetBalance` | decimal | From vendor financial data |
| 4 | Final Amount (RM) | `financialRows[].finalAmount` | decimal | `MIN(capitalLiquidation, assetBalance)` |
| 5 | Min. Capital Required (RM) | `financialRows[].minCapitalRequired` | decimal | From master data settings |
| 6 | Result | `financialRows[].result` | string | Passed if finalAmount >= minCapitalRequired |

**Screen → API Field Mapping (Recommendation):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Tenderer Code | `recommendation.rows[].tendererCode` | string | |
| 2 | Tender Opening Status | `recommendation.rows[].tenderOpeningStatus` | string | |
| 3 | Technical Evaluation Status | `recommendation.rows[].technicalEvaluationStatus` | string | |
| 4 | Financial Evaluation Status | `recommendation.rows[].financialEvaluationStatus` | string | |
| 5 | Vendor Offer Price (RM) | `recommendation.rows[].vendorOfferPrice` | decimal | Clickable → opens Vendor Offer popup |
| 6 | FPMSB Estimation Price (RM) | `recommendation.rows[].fpmsEstimationPrice` | decimal | |
| 7 | Difference (%) | `recommendation.rows[].differencePercent` | decimal | `(estimation - offer) / estimation × 100` |
| 8 | Recommended Tenderer | `recommendation.recommendedVendorId` | int? | Dropdown populated from `vendorOptions` |
| 9 | Reason | `recommendation.reason` | string? | Text area |

**Backend DTOs:**
- `TenderEvaluationPageDto` → `DB/Entity/TenderEvaluationDto.cs`
- `TenderTechnicalEvalRowDto` → `DB/Entity/TenderEvaluationDto.cs`
- `TenderFinancialEvalRowDto` → `DB/Entity/TenderEvaluationDto.cs`
- `TenderRecommendationPageDto` → `DB/Entity/TenderEvaluationDto.cs`
- `TenderRecommRowDto` → `DB/Entity/TenderEvaluationDto.cs`

---

### Screen 2.4 – Technical Evaluation Scoring Popup

**Screen File:** `Screenshot 2026-04-03 071432.png` (right-side overlay)  
**Description:** Popup shown when clicking a Tenderer Code in the Technical Evaluation section. Allows scoring each evaluation criterion on a 0-5 scale with remarks.

**UI Elements:** Reference ID, Project Name, Tenderer Code (display). Criteria table: No., Criteria, Weightage, Score (0-5) dropdown, Total, Remarks (text input). Summary: Total Score, Passing Marks, Result (auto-computed). Buttons: Back, Save.

#### API 2.4.1 – Get Technical Evaluation Popup

**Method:** GET  
**URL:** `/api/TenderManagement/GetTechnicalEvaluationPopup?tenderId={id}&vendorId={vendorId}`  
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "referenceId": "S11621009",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "tendererCode": "1/3",
  "criteria": [
    { "specificationId": 1, "specification": "Ketersediaan Pekerja",   "weightage": 30, "score": 4, "total": 24, "remarks": "manpower yang mecukupi" },
    { "specificationId": 2, "specification": "Ketersediaan Jentera",   "weightage": 25, "score": 4, "total": 20, "remarks": "spec jentera terbaru dan memuaskan" },
    { "specificationId": 3, "specification": "Ketersediaan Peralatan", "weightage": 20, "score": 3, "total": 12, "remarks": "peralatan lengkap cuma outdated" },
    { "specificationId": 4, "specification": "Ketersediaan Racun",     "weightage": 15, "score": 4, "total": 12, "remarks": "memuaskan" },
    { "specificationId": 5, "specification": "Pengalaman Terdahulu",   "weightage": 10, "score": 4, "total": 10, "remarks": "berpengalaman dalam kerja ini" }
  ],
  "totalScore": 78,
  "passingMarks": 70,
  "result": "Passed"
}
```

**Screen → API Field Mapping:**

| # | UI Field | API Response/Request Field | Type | Notes |
|---|----------|---------------------------|------|-------|
| 1 | Reference ID | `referenceId` | string | Display only |
| 2 | Project Name | `projectName` | string | Display only |
| 3 | Tenderer Code | `tendererCode` | string | Display only |
| 4 | Criteria (name) | `criteria[].specification` | string | Display only |
| 5 | Weightage | `criteria[].weightage` | int | Display only (%) |
| 6 | Score (0-5) | `criteria[].score` | int | Editable dropdown (0-5) |
| 7 | Total | `criteria[].total` | int | Computed: `score × weightage / 5` |
| 8 | Remarks | `criteria[].remarks` | string? | Editable text input |
| 9 | Total Score | `totalScore` | int | Sum of all `total` values |
| 10 | Passing Marks | `passingMarks` | int | From evaluation criteria config |
| 11 | Result | `result` | string | Auto: "Passed" if `totalScore >= passingMarks` |

**Backend DTO:** `TenderTechnicalEvalPopupDto` / `TechnicalCriterionDto` → `DB/Entity/TenderEvaluationDto.cs`

#### API 2.4.2 – Save Technical Score

**Method:** POST  
**URL:** `/api/TenderManagement/SaveTechnicalScore`  
**Authentication:** Required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "scores": [
    { "specificationId": 1, "score": 4, "remarks": "manpower yang mecukupi" },
    { "specificationId": 2, "score": 4, "remarks": "spec jentera terbaru" },
    { "specificationId": 3, "score": 3, "remarks": "peralatan outdated" },
    { "specificationId": 4, "score": 4, "remarks": "memuaskan" },
    { "specificationId": 5, "score": 4, "remarks": "berpengalaman" }
  ]
}
```

**Validation Rules:**
- `score` must be between 0 and 5
- All criteria must be scored before saving
- Server auto-computes `total` = `score × weightage / 5`, `totalScore`, `result`, and `ranking`

**Success Response (200):** `"Technical scores saved successfully"`

**Backend DTO:** `SaveTechnicalScoreDto` → `DB/Entity/TenderEvaluationDto.cs`

#### API 2.4.3 – Save Tender Recommendation

**Method:** POST  
**URL:** `/api/TenderManagement/SaveTenderRecommendation`  
**Authentication:** Required

**Request Body:**
```json
{
  "tenderId": 45,
  "recommendedVendorId": 102,
  "reason": "Vendor 2 meets all technical requirements with competitive pricing."
}
```

**Success Response (200):** `"Recommendation saved successfully"`

**Backend DTO:** `TenderRecommendationPageDto` → `DB/Entity/TenderEvaluationDto.cs`

---

### Screen 2.5 – Vendor Offer Summary Popup

**Screen File:** `Screenshot 2026-04-16 115322.png` (right-side browser window)  
**Description:** Popup shown when clicking the Vendor Offer Price link in the Recommendation table. Shows material-level pricing breakdown by PKT/Blok and supports negotiation rounds.

**UI Elements:**
- **Vendor Offer Summary:** Table (Bil, PKT / Blok, Description (Short Text), Unit, Quantity, Unit Price (RM/SEN) — editable, Sub-Total (RM/SEN)). Total Amount (RM), FPMSB Estimation Price (RM).
- **Negotiation Summary (1):** Timestamped section with same table structure. Submitted on date/time shown.

**Screen → API Field Mapping:**

| # | UI Field | Likely API Source | Type | Match Status |
|---|----------|-------------------|------|--------------|
| 1 | Bil | *(row index)* | int | N/A |
| 2 | PKT / Blok | `jobScope[].wbqNo` | string | FROM TenderApplication |
| 3 | Description (Short Text) | `jobScope[].shortTask` | string | FROM TenderApplication |
| 4 | Unit | `jobScope[].unit` | string | FROM TenderApplication |
| 5 | Quantity | `jobScope[].quantity` | int | FROM TenderApplication |
| 6 | Unit Price (RM/SEN) | **PARTIAL** | decimal | Vendor's bid price — needs item-level mapping |
| 7 | Sub-Total (RM/SEN) | **COMPUTED** | decimal | `quantity × unitPrice` |
| 8 | Total Amount (RM) | **COMPUTED** | decimal | Sum of all sub-totals |
| 9 | FPMSB Estimation Price (RM) | `jobScope[].subTotal` SUM | decimal | FROM TenderApplication |
| 10 | Negotiation Summary | **NO API** | object | MISSING — no negotiation round history |

> **CRITICAL GAP:** No API endpoint returns the per-material vendor pricing breakdown. The existing `BidderSubmissionItem` model tracks `BiddingAssetId` + `BidPrice`, but there's no mapping to `TenderJobScope` material items. See [Missing APIs](#missing-apis).

---

## MODULE 3 – TENDER AWARD (ADMIN PORTAL)

### Screen 3.1 – Tender Award Listing

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_53_52.png`  
**Description:** List of tenders in the Award phase with filter options.

**UI Elements:** Filters: Application Level (dropdown), Tender Category (dropdown), Job Category (dropdown), Status (dropdown). Search button, New button. Table: No., Reference ID (link), Project Name, Application Level, Job Category, Tender Category, Status, Created Date / Time.

#### API 3.1.1 – Get Tender Award List

**Method:** GET  
**URL:** `/api/TenderManagement/GetTenderAwardList`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `referenceId` | string | No | Filter by reference ID |
| `projectName` | string | No | Filter by project name |

**Response:**
```json
[
  {
    "tenderId": 45,
    "referenceId": "2026/B020",
    "projectName": "Supply, Delivery, and Distribution of Compound...",
    "applicationLevel": "Projek",
    "jobCategory": "Kontrak Kejuruteraan",
    "tenderCategory": "Terbuka",
    "awardStatus": "Awarded",
    "createdDateTime": "2026-01-29T08:11:54"
  }
]
```

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | No. | *(row index)* | int | |
| 2 | Reference ID | `referenceId` | string | Clickable link |
| 3 | Project Name | `projectName` | string | |
| 4 | Application Level | `applicationLevel` | string | |
| 5 | Job Category | `jobCategory` | string | |
| 6 | Tender Category | `tenderCategory` | string | |
| 7 | Status | `awardStatus` | string | Awarded / Pending Acknowledgement |
| 8 | Created Date / Time | `createdDateTime` | DateTime | Format: `dd/MM/yyyy HH:mm:ss` |

**Data Contract Issues:**
- Screen has 4 filter dropdowns (Application Level, Tender Category, Job Category, Status) but the API only accepts `referenceId` and `projectName` — dropdown filtering is NOT supported server-side
- Screen shows "Pending Acknowledgement" as a status which maps to the vendor not yet responding to the award

**Backend DTO:** `TenderAwardListDto` → `DB/Entity/TenderAwardDto.cs`

---

### Screen 3.2 – Tender Award Details (Tender Award Tab)

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_54_12.png`  
**Description:** Two-tab page: "TENDER AWARD" and "VENDOR PERFORMANCE". The Tender Award tab shows reference info, minutes of meeting, vendor appointment details, and insurance details.

**UI Elements:**
- **Header:** Reference ID, Project Name (display)
- **Minutes of Meeting:** Add button, Table (Date, Meeting Outcome, Attachment link, Edit/Delete icons)
- **Vendor Appointment Details:** Vendor dropdown, Project Value (RM), Yearly Expenses (RM), Project Start Date, Project End Date, Agreement dropdown, Agreement Date Signed, PO Number
- **Insurance Details (read-only, auto-computed):**
  - Public Liability: Formula, Value (RM), Period
  - Contractor at Risk: Formula, Value (RM), Period
  - Workman Compensation: Formula, Value (RM), Period
  - LAD: Formula, Value (RM)
- **Buttons:** Back, Save, SST Document

#### API 3.2.1 – Get Tender Award Page

**Method:** GET  
**URL:** `/api/TenderManagement/GetTenderAwardPage?tenderId={id}`  
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T43534621",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN MENEBANG DAN MEMANGKAS POKOK BAHAYA...",
  "minutesOfMeetings": [
    {
      "id": 1,
      "tenderId": 45,
      "meetingDate": "2025-07-20T00:00:00",
      "meetingOutcome": "Board agree to award tender to Syarikat Maju Sdn. Bhd.",
      "attachmentPath": "/VendorUploads/Minutes_of_Meeting-20072027.pdf",
      "attachmentFileName": "Minutes_of_Meeting-20072027.pdf"
    }
  ],
  "vendorAppointment": {
    "awardedVendorId": null,
    "awardedVendorName": null,
    "projectValue": null,
    "yearlyExpenses": null,
    "projectStartDate": null,
    "projectEndDate": null,
    "agreement": null,
    "agreementDateSigned": null,
    "poNumber": null
  },
  "insurance": {
    "publicLiabilityFormula": "10% of total contract sum or a minimum of RM2,000,000.00",
    "publicLiabilityValue": 2000000.00,
    "publicLiabilityPeriodStart": "2026-01-01T00:00:00",
    "publicLiabilityPeriodEnd": "2027-01-01T00:00:00",
    "contractorAtRiskFormula": "Full Replacement Value of the Works + 10% for professional fees",
    "contractorAtRiskValue": 2000000.00,
    "contractorAtRiskPeriodStart": "2026-02-01T00:00:00",
    "contractorAtRiskPeriodEnd": "2026-07-31T00:00:00",
    "worksmanCompensationFormula": "As per Statutory Requirements (Common Law)",
    "worksmanCompensationValue": 2000000.00,
    "worksmanCompensationPeriodStart": "2026-01-01T00:00:00",
    "worksmanCompensationPeriodEnd": "2027-01-01T00:00:00",
    "ladFormula": "RM500.00 per day of delay up to a maximum of 10% of contract value",
    "ladValue": 500.00
  },
  "vendorOptions": [
    { "vendorId": 102, "vendorName": "Syarikat Maju Sdn. Bhd." }
  ]
}
```

**Screen → API Field Mapping (Header):**

| # | UI Field | API Response Field | Type |
|---|----------|--------------------|------|
| 1 | Reference ID | `referenceId` | string |
| 2 | Project Name | `projectName` | string |

**Screen → API Field Mapping (Minutes of Meeting):**

| # | UI Field | API Response Field | Type |
|---|----------|--------------------|------|
| 1 | Date | `minutesOfMeetings[].meetingDate` | DateTime |
| 2 | Meeting Outcome | `minutesOfMeetings[].meetingOutcome` | string |
| 3 | Attachment | `minutesOfMeetings[].attachmentPath` | string (URL) |
| 4 | Edit icon | *(uses minutesOfMeetings[].id)* | action |
| 5 | Delete icon | *(uses minutesOfMeetings[].id)* | action |

**Screen → API Field Mapping (Vendor Appointment Details):**

| # | UI Field | API Response/Request Field | Type | Notes |
|---|----------|---------------------------|------|-------|
| 1 | Vendor | `vendorAppointment.awardedVendorId` | int? | Dropdown from `vendorOptions` |
| 2 | Project Value (RM) | `vendorAppointment.projectValue` | decimal? | |
| 3 | Yearly Expenses (RM) | `vendorAppointment.yearlyExpenses` | decimal? | |
| 4 | Project Start Date | `vendorAppointment.projectStartDate` | DateTime? | |
| 5 | Project End Date | `vendorAppointment.projectEndDate` | DateTime? | |
| 6 | Agreement | `vendorAppointment.agreement` | string? | Dropdown (Yes/No) |
| 7 | Agreement Date Signed | `vendorAppointment.agreementDateSigned` | DateTime? | |
| 8 | PO Number | `vendorAppointment.poNumber` | string? | |

**Screen → API Field Mapping (Insurance Details — Read-Only):**

| # | UI Field | API Response Field | Type |
|---|----------|--------------------|------|
| 1 | Public Liability — Formula | `insurance.publicLiabilityFormula` | string |
| 2 | Public Liability — Value (RM) | `insurance.publicLiabilityValue` | decimal |
| 3 | Public Liability — Period | `insurance.publicLiabilityPeriodStart` + `End` | DateTime |
| 4 | Contractor at Risk — Formula | `insurance.contractorAtRiskFormula` | string |
| 5 | Contractor at Risk — Value (RM) | `insurance.contractorAtRiskValue` | decimal |
| 6 | Contractor at Risk — Period | `insurance.contractorAtRiskPeriodStart` + `End` | DateTime |
| 7 | Workman Compensation — Formula | `insurance.worksmanCompensationFormula` | string |
| 8 | Workman Compensation — Value (RM) | `insurance.worksmanCompensationValue` | decimal |
| 9 | Workman Compensation — Period | `insurance.worksmanCompensationPeriodStart` + `End` | DateTime |
| 10 | LAD — Formula | `insurance.ladFormula` | string |
| 11 | LAD — Value (RM) | `insurance.ladValue` | decimal |

**Backend DTOs:**
- `TenderAwardPageDto` → `DB/Entity/TenderAwardDto.cs`
- `TenderAwardMinutesDto` → `DB/Entity/TenderAwardDto.cs`
- `TenderAwardVendorAppointmentDto` → `DB/Entity/TenderAwardDto.cs`
- `TenderAwardInsuranceDto` → `DB/Entity/TenderAwardDto.cs`
- `TenderAwardVendorOptionDto` → `DB/Entity/TenderAwardDto.cs`

#### API 3.2.2 – Save Tender Award

**Method:** POST  
**URL:** `/api/TenderManagement/SaveTenderAward`  
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorAppointment": {
    "awardedVendorId": 102,
    "projectValue": 500000.00,
    "yearlyExpenses": 150000.00,
    "projectStartDate": "2026-02-01",
    "projectEndDate": "2027-01-31",
    "agreement": "Yes",
    "agreementDateSigned": "2026-01-15",
    "poNumber": "PO-2026-001"
  }
}
```

**Success Response (200):** `"Tender award saved successfully"`

**Backend DTO:** `SaveTenderAwardDto` → `DB/Entity/TenderAwardDto.cs`

---

### Screen 3.3 – Minutes of Meeting Popup

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_56_16.png`  
**Description:** Modal popup for adding/editing Minutes of Meeting. Triggered by the "Add" button or Edit icon in the Minutes of Meeting section.

**UI Elements:** Date (date picker), Meeting Outcome (text area), Attachment (Browse + filename display). Buttons: Close, Save.

#### API 3.3.1 – Save Tender Award Minutes

**Method:** POST  
**URL:** `/api/TenderManagement/SaveTenderAwardMinutes`  
**Authentication:** Not required

**Request Body:**
```json
{
  "id": 0,
  "tenderId": 45,
  "meetingDate": "2025-07-20",
  "meetingOutcome": "Board agree to award tender to Syarikat Maju Sdn. Bhd.",
  "attachmentPath": "/VendorUploads/MoM.pdf",
  "attachmentFileName": "MoM.pdf"
}
```

**Screen → API Field Mapping:**

| # | UI Field | API Request Field | Type | Notes |
|---|----------|-------------------|------|-------|
| 1 | Date | `meetingDate` | DateTime | Date picker |
| 2 | Meeting Outcome | `meetingOutcome` | string? | Text area |
| 3 | Attachment | `attachmentPath` / `attachmentFileName` | string? | File upload → path |
| 4 | *(context)* | `tenderId` | int | From parent page context |
| 5 | *(context)* | `id` | int | 0 = new; existing ID = update |

**Success Response (200):** The saved `TenderAwardMinutesDto` object.

**Backend DTO:** `SaveTenderAwardMinutesDto` → `DB/Entity/TenderAwardDto.cs`

#### API 3.3.2 – Delete Tender Award Minutes

**Method:** DELETE  
**URL:** `/api/TenderManagement/DeleteTenderAwardMinutes?minutesId={id}`  
**Authentication:** Not required

**Success Response (200):** `"Minutes of meeting deleted successfully"`

---

### Screen 3.4 – Vendor Performance Tab

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_56_44.png`  
**Description:** "VENDOR PERFORMANCE" tab of the Tender Award page. Rate vendor performance after project completion across 5 weighted categories, capture stakeholder feedback, and record reviewer details.

**UI Elements:**
- **Header:** Vendor Name, Reference ID, Project Name, Award Date, Review Month / Year (display)
- **Performance Table:** Category, Indicator, Weightage (%), Rating (1-5) dropdown, Score (%). Categories: Quality (30%), Schedule (25%), Cost (20%), Service (15%), Risk/Safety (10%). Total Score row.
- **Stakeholder Feedback:** Table (Description, Feedback dropdown — 1-Strongly Disagree to 5-Strongly Agree). 5 fixed questions.
- **Reviewed By:** PIC Name, Department, Designation, Mobile No., Created Date / Time (all display only — auto-populated from logged-in user)
- **Buttons:** Back, Save

#### API 3.4.1 – Get Vendor Performance Page

**Method:** GET  
**URL:** `/api/TenderManagement/GetVendorPerformancePage?tenderId={id}`  
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "vendorName": "Syarikat Maju Sdn. Bhd.",
  "referenceId": "T43534621",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN MENEBANG DAN MEMANGKAS POKOK BAHAYA...",
  "awardDate": "2025-11-12T00:00:00",
  "reviewMonthYear": "12/2025",
  "performanceScores": [
    { "category": "Quality",     "indicator": "Work met all technical specs and standards.",       "weightage": 30, "rating": 4, "score": 24 },
    { "category": "Schedule",    "indicator": "Milestones and final delivery were on time.",        "weightage": 25, "rating": 4, "score": 20 },
    { "category": "Cost",        "indicator": "Stayed within budget; no additional fees.",          "weightage": 20, "rating": 3, "score": 12 },
    { "category": "Service",     "indicator": "Proactive communication and problem-solving.",       "weightage": 15, "rating": 4, "score": 12 },
    { "category": "Risk/Safety", "indicator": "Complied with all legal/safety protocols.",          "weightage": 10, "rating": 5, "score": 10 }
  ],
  "totalScore": 78,
  "stakeholderFeedbacks": [
    { "questionOrder": 1, "description": "The vendor demonstrated a high level of expertise in their field?",          "feedbackScore": 4, "feedbackLabel": "4 - Agree" },
    { "questionOrder": 2, "description": "The vendor was responsive to emails/calls (within 24 hours)?",               "feedbackScore": 2, "feedbackLabel": "2 - Disagree" },
    { "questionOrder": 3, "description": "The vendor was adaptable when project requirements changed?",                "feedbackScore": 4, "feedbackLabel": "4 - Agree" },
    { "questionOrder": 4, "description": "Proactive communication and problem-solving.",                               "feedbackScore": 5, "feedbackLabel": "5 - Strongly Agree" },
    { "questionOrder": 5, "description": "The vendor's documentation (reports, invoices, manuals) was clear and accurate?", "feedbackScore": 4, "feedbackLabel": "4 - Agree" }
  ],
  "reviewer": {
    "picName": "Ahmad Azri",
    "department": "Finance",
    "designation": "Ketua Unit Perolehan",
    "mobileNo": "012-223 6672",
    "createdDateTime": "2021-04-12T10:21:23"
  }
}
```

**Screen → API Field Mapping (Header):**

| # | UI Field | API Response Field | Type |
|---|----------|--------------------|------|
| 1 | Vendor Name | `vendorName` | string |
| 2 | Reference ID | `referenceId` | string |
| 3 | Project Name | `projectName` | string |
| 4 | Award Date | `awardDate` | DateTime |
| 5 | Review Month / Year | `reviewMonthYear` | string |

**Screen → API Field Mapping (Performance Table):**

| # | UI Field | API Response/Request Field | Type | Notes |
|---|----------|---------------------------|------|-------|
| 1 | Category | `performanceScores[].category` | string | Display only |
| 2 | Indicator | `performanceScores[].indicator` | string | Display only |
| 3 | Weightage (%) | `performanceScores[].weightage` | int | Display only |
| 4 | Rating (1-5) | `performanceScores[].rating` | int | Editable dropdown |
| 5 | Score (%) | `performanceScores[].score` | int | Computed: `weightage × rating / 5` |
| 6 | Total Score | `totalScore` | int | Sum of all scores |

**Screen → API Field Mapping (Stakeholder Feedback):**

| # | UI Field | API Response/Request Field | Type | Notes |
|---|----------|---------------------------|------|-------|
| 1 | Description | `stakeholderFeedbacks[].description` | string | Display only |
| 2 | Feedback | `stakeholderFeedbacks[].feedbackScore` | int | Dropdown: 1-5 scale |

**Screen → API Field Mapping (Reviewed By):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | PIC Name | `reviewer.picName` | string | Auto from logged-in user |
| 2 | Department | `reviewer.department` | string | Auto |
| 3 | Designation | `reviewer.designation` | string | Auto |
| 4 | Mobile No. | `reviewer.mobileNo` | string | Auto |
| 5 | Created Date / Time | `reviewer.createdDateTime` | DateTime | Auto |

**Backend DTOs:**
- `VendorPerformancePageDto` → `DB/Entity/VendorPerformanceDto.cs`
- `VendorPerformanceCriteriaRowDto` → `DB/Entity/VendorPerformanceDto.cs`
- `VendorPerformanceFeedbackRowDto` → `DB/Entity/VendorPerformanceDto.cs`
- `VendorPerformanceReviewerDto` → `DB/Entity/VendorPerformanceDto.cs`

#### API 3.4.2 – Save Vendor Performance

**Method:** POST  
**URL:** `/api/TenderManagement/SaveVendorPerformance`  
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "reviewMonthYear": "12/2025",
  "scores": [
    { "category": "Quality",     "rating": 4 },
    { "category": "Schedule",    "rating": 4 },
    { "category": "Cost",        "rating": 3 },
    { "category": "Service",     "rating": 4 },
    { "category": "Risk/Safety", "rating": 5 }
  ],
  "feedbacks": [
    { "questionOrder": 1, "feedbackScore": 4 },
    { "questionOrder": 2, "feedbackScore": 2 },
    { "questionOrder": 3, "feedbackScore": 4 },
    { "questionOrder": 4, "feedbackScore": 5 },
    { "questionOrder": 5, "feedbackScore": 4 }
  ]
}
```

**Validation Rules:**
- Rating must be 1-5 for each category
- All 5 performance categories must be rated
- All 5 stakeholder feedback questions must be answered
- Server computes `score = weightage × rating / 5` and `totalScore`

**Success Response (200):** `"Vendor performance saved successfully"`

**Backend DTO:** `SaveVendorPerformanceDto` → `DB/Entity/VendorPerformanceDto.cs`

---

## MODULE 4 – BIDDING (VENDOR PORTAL)

### Screen 4.1 – Bidding Listing (Vendor)

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_57_13.png`  
**Description:** Vendor's list of tenders available for bidding or previously submitted bids.

**UI Elements:** Table: Reference, Bidding Title (clickable link), Advertisement Date, Closing Date / Time, Status.

#### API 4.1.1 – Get Active Bidding List

**Method:** GET  
**URL:** `/api/Bidding/GetActiveBiddingList?vendorId={vendorId}`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `vendorId` | int | Yes | Vendor ID (from JWT or session) |

**Response:**
```json
[
  {
    "tenderId": 45,
    "referenceId": "2026/B020",
    "biddingTitle": "Pembelian Kenderaan Terpakai Syarikat Hak Milik Felda Plantation Management Sdn. Bhd.",
    "tenderCategory": "Terbuka",
    "startingDate": "2026-01-20T00:00:00",
    "closingDate": "2026-02-23T00:00:00",
    "closingTime": "11:59 AM",
    "status": "Awarded"
  }
]
```

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Reference | `referenceId` | string | |
| 2 | Bidding Title | `biddingTitle` | string | Clickable → opens Bidding Details |
| 3 | Advertisement Date | `startingDate` | DateTime | Format: `dd/MM/yyyy` |
| 4 | Closing Date / Time | `closingDate` + `closingTime` | DateTime + string | Combined display |
| 5 | Status | `status` | string | Open / Closed / Awarded |

**Backend DTO:** `BiddingListDto` → `DB/Entity/BiddingDto.cs`

---

### Screen 4.2 – Bidding Details (Vendor)

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_57_27.png`  
**Description:** Full tender detail for bidding. Two tabs: "BIDDING DETAILS" (enter bid prices) and "AWARD DETAILS". Shows tender information and asset table with editable bid price.

**UI Elements — Bidding Details Tab:**
- **Info Fields:** Application Level, Bidding Title, Job Category, Tender Category, Deposit Amount (RM), Remarks, Starting Date, Closing Date / Time (all display only)
- **Asset Details Table:** No., Project / State, Asset Details, Asset Ref. No., Starting Price (RM), Year Purchased, Bid Price (RM) — editable input
- **Buttons:** Back, Submit Bidding

#### API 4.2.1 – Get Bidding Detail

**Method:** GET  
**URL:** `/api/Bidding/GetBiddingDetail?tenderId={tenderId}&vendorId={vendorId}`  
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `tenderId` | int | Yes | Tender ID |
| `vendorId` | int | Yes | Vendor ID |

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "2026/B020",
  "applicationLevel": "Ibu Pejabat (HQ)",
  "biddingTitle": "Pembelian Kenderaan Terpakai Syarikat Hak Milik Felda Plantation Management Sdn. Bhd.",
  "jobCategory": "Bidaan Aset",
  "tenderCategory": "Terbuka",
  "depositAmount": 500.00,
  "remarks": "100% dibayar dalam tempoh 60 hari dari tarikh penerimaan barang",
  "startingDate": "2026-03-19T00:00:00",
  "closingDate": "2026-03-29T00:00:00",
  "closingTime": "12:00PM",
  "alreadySubmitted": false,
  "assets": [
    {
      "id": 1,
      "sequenceNo": 1,
      "projectState": "Trolak Utara",
      "assetDetails": "Toyota Hilux",
      "assetRefNo": "WVH 8935",
      "startingPrice": 6000.00,
      "yearPurchased": 2011,
      "bidPrice": 0.00
    },
    {
      "id": 2,
      "sequenceNo": 2,
      "projectState": "Tersang 3",
      "assetDetails": "Toyota Hilux",
      "assetRefNo": "WVK 7301",
      "startingPrice": 6000.00,
      "yearPurchased": 2011,
      "bidPrice": 0.00
    },
    {
      "id": 3,
      "sequenceNo": 3,
      "projectState": "Kerteh 5",
      "assetDetails": "Ford Ranger",
      "assetRefNo": "WMY 7373",
      "startingPrice": 12500.00,
      "yearPurchased": 2013,
      "bidPrice": 0.00
    }
  ]
}
```

**Screen → API Field Mapping (Info Fields):**

| # | UI Field | API Response Field | Type |
|---|----------|--------------------|------|
| 1 | Application Level | `applicationLevel` | string |
| 2 | Bidding Title | `biddingTitle` | string |
| 3 | Job Category | `jobCategory` | string |
| 4 | Tender Category | `tenderCategory` | string |
| 5 | Deposit Amount (RM) | `depositAmount` | decimal? |
| 6 | Remarks | `remarks` | string? |
| 7 | Starting Date | `startingDate` | DateTime |
| 8 | Closing Date / Time | `closingDate` + `closingTime` | DateTime + string |

**Screen → API Field Mapping (Asset Details Table):**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | No. | `assets[].sequenceNo` | int | |
| 2 | Project / State | `assets[].projectState` | string | |
| 3 | Asset Details | `assets[].assetDetails` | string | |
| 4 | Asset Ref. No. | `assets[].assetRefNo` | string | |
| 5 | Starting Price (RM) | `assets[].startingPrice` | decimal | Display only |
| 6 | Year Purchased | `assets[].yearPurchased` | int? | Display only |
| 7 | Bid Price (RM) | `assets[].bidPrice` | decimal | **Editable input** |

**Backend DTOs:**
- `BiddingDetailDto` → `DB/Entity/BiddingDto.cs`
- `BiddingAssetDto` → `DB/Entity/BiddingDto.cs`

#### API 4.2.2 – Submit Bidding

**Method:** POST  
**URL:** `/api/Bidding/SubmitBidding`  
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "bidItems": [
    { "biddingAssetId": 1, "bidPrice": 7000.00 },
    { "biddingAssetId": 2, "bidPrice": 6500.00 },
    { "biddingAssetId": 3, "bidPrice": 13000.00 }
  ]
}
```

**Screen → API Field Mapping (Submit Action):**

| # | UI Field | API Request Field | Type | Notes |
|---|----------|-------------------|------|-------|
| 1 | *(context)* | `tenderId` | int | From page context |
| 2 | *(context)* | `vendorId` | int | From session/JWT |
| 3 | Bid Price (per asset) | `bidItems[].bidPrice` | decimal | From editable input |
| 4 | *(asset ID)* | `bidItems[].biddingAssetId` | int | From `assets[].id` |

**Validation Rules:**
- Tender must still be open (before closing date/time)
- `bidPrice` must be > 0 for each asset
- Vendor must not have already submitted (unless negotiation round)

**Success Response (200):** `"Bidding submitted successfully"`

**Backend DTOs:**
- `SubmitBiddingDto` → `DB/Entity/BiddingDto.cs`
- `BidItemDto` → `DB/Entity/BiddingDto.cs`

---

### Screen 4.3 – Bidding Award Details (Vendor)

**Screen File:** `screencapture-7aeo8h-axshare-2026-04-16-11_57_42.png`  
**Description:** "AWARD DETAILS" tab. Shows award notification and allows vendor to acknowledge (Accept/Reject) with stamp duty upload.

**UI Elements:** Award notification text paragraphs. Fields: Bidder Name, Reference ID, Bidder Title (display only). Award Date (display), Acknowledgement dropdown (- Select Acknowledgement - / Accept / Reject), Stamp Duty LHDN (Browse + filename), Date / Time (auto). Buttons: Back, Download SST Document, Submit.

#### API 4.3.1 – Get Bidding Award Detail

**Method:** GET  
**URL:** `/api/Bidding/GetBiddingAwardDetail?tenderId={tenderId}&vendorId={vendorId}`  
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T43534621",
  "bidderName": "Syarikat Maju Sdn. Bhd.",
  "biddingTitle": "Pembelian Kenderaan Terpakai Syarikat Hak Milik Felda Plantation Management Sdn. Bhd.",
  "isAwarded": true,
  "awardDate": "2025-11-12T00:00:00",
  "acknowledgement": null,
  "stampDutyPath": null,
  "stampDutyFileName": null,
  "acknowledgementDateTime": null,
  "alreadyAcknowledged": false,
  "acknowledgementOptions": ["Accept", "Reject"]
}
```

**Screen → API Field Mapping:**

| # | UI Field | API Response Field | Type | Notes |
|---|----------|--------------------|------|-------|
| 1 | Bidder Name | `bidderName` | string | Display only |
| 2 | Reference ID | `referenceId` | string | Display only |
| 3 | Bidder Title | `biddingTitle` | string | Display only |
| 4 | Award Date | `awardDate` | DateTime | Display only |
| 5 | Acknowledgement | `acknowledgement` | string? | Dropdown from `acknowledgementOptions` |
| 6 | Stamp Duty LHDN | `stampDutyPath` / `stampDutyFileName` | string? | File upload |
| 7 | Date / Time | `acknowledgementDateTime` | DateTime? | Auto-set on submit |

**Backend DTO:** `BiddingAwardDetailDto` → `DB/Entity/BiddingDto.cs`

#### API 4.3.2 – Submit Bidder Acknowledgement

**Method:** POST  
**URL:** `/api/Bidding/SubmitBidderAcknowledgement`  
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "acknowledgement": "Accept",
  "stampDutyPath": "/VendorUploads/stamp_duty.pdf",
  "stampDutyFileName": "stamp_duty.pdf"
}
```

**Validation Rules:**
- `acknowledgement` must be `"Accept"` or `"Reject"`
- If `"Accept"`, stamp duty file is required
- Cannot acknowledge if `alreadyAcknowledged` is `true`

**Success Response (200):** `"Acknowledgement submitted successfully"`

**Backend DTO:** `SubmitBidderAcknowledgementDto` → `DB/Entity/BiddingDto.cs`

---

## SCREEN-WISE MAPPING SUMMARY TABLE

| # | Screen | Module | API Endpoint(s) | Method | Coverage |
|---|--------|--------|-----------------|--------|----------|
| 1 | Tender Opening Listing | Opening | `/api/TenderManagement/GetTenderOpeningList` | GET | FULL |
| 2 | Tender Opening Appointment | Opening | `/api/TenderManagement/GetTenderOpeningDetail` | GET | PARTIAL (missing committeeRole) |
| 3 | Tender Opening Summary | Opening | `/api/TenderManagement/GetTenderOpeningPage` | GET | FULL |
| 4 | Tender Opening Verify | Opening | `/api/Bidding/VerifyTenderOpening` | POST | FULL |
| 5 | Tender Opening Progress | Opening | `/api/Bidding/GetTenderOpeningProgress` | GET | **CRITICAL GAP** (6 fields missing) |
| 6 | Vendor Details Tab | Opening | `/api/Vendor/GetVendorDetilsById` + 3 more | GET×4 | PARTIAL (no composite endpoint) |
| 7 | Submitted Documents Tab | Opening | **NO API** | — | **NO COVERAGE** |
| 8 | Document Validation Popup | Opening | **NO API** | — | **NO COVERAGE** |
| 9 | Price Summary Tab | Opening | **NO API** | — | **NO COVERAGE** |
| 10 | Tender Evaluation Listing | Evaluation | `GetTenderOpeningList` (reused) | GET | FULL (via reuse) |
| 11 | Tender Evaluation Appointment | Evaluation | `GetTenderOpeningDetail` (reused) | GET | PARTIAL |
| 12 | Tender Evaluation Details | Evaluation | `/api/TenderManagement/GetTenderEvaluationPage` | GET | FULL |
| 13 | Technical Scoring Popup | Evaluation | `GetTechnicalEvaluationPopup` + `SaveTechnicalScore` | GET+POST | FULL |
| 14 | Vendor Offer Summary Popup | Evaluation | **NO API** | — | **NO COVERAGE** |
| 15 | Save Recommendation | Evaluation | `/api/TenderManagement/SaveTenderRecommendation` | POST | FULL |
| 16 | Tender Award Listing | Award | `/api/TenderManagement/GetTenderAwardList` | GET | PARTIAL (filters not supported) |
| 17 | Tender Award Details | Award | `/api/TenderManagement/GetTenderAwardPage` | GET | FULL |
| 18 | Minutes of Meeting Popup | Award | `SaveTenderAwardMinutes` / `DeleteTenderAwardMinutes` | POST/DELETE | FULL |
| 19 | Save Award | Award | `/api/TenderManagement/SaveTenderAward` | POST | FULL |
| 20 | Vendor Performance | Award | `GetVendorPerformancePage` + `SaveVendorPerformance` | GET+POST | FULL |
| 21 | Bidding Listing | Bidding | `/api/Bidding/GetActiveBiddingList` | GET | FULL |
| 22 | Bidding Details | Bidding | `/api/Bidding/GetBiddingDetail` | GET | FULL |
| 23 | Submit Bidding | Bidding | `/api/Bidding/SubmitBidding` | POST | FULL |
| 24 | Bidding Award Details | Bidding | `GetBiddingAwardDetail` + `SubmitBidderAcknowledgement` | GET+POST | FULL |

---

## CRITICAL GAPS

### GAP 1: Tender Opening Progress Tab — 6 Missing Fields

**Screen:** Screen 1.4 (Tender Opening Progress)  
**Severity:** CRITICAL  
**Impact:** Frontend cannot display the Progress tab as designed

**Current `TenderOpeningBidProgressDto` fields (4):**
```csharp
public int Bil { get; set; }
public string? VendorName { get; set; }
public decimal? OfferedPrice { get; set; }
public string OpeningStatus { get; set; } = "Pending";
```

**Missing fields required by UI (6):**

| Missing Field | Source | Type | Required Action |
|---------------|--------|------|-----------------|
| `companyRegNo` | `Vendor.RocNumber` | string | Add to DTO + repository query |
| `type` | `Vendor.CompanyEntityType.Name` | string | Add to DTO + join CompanyEntityType |
| `state` | `Vendor.State.Name` | string | Add to DTO + join State lookup |
| `district` | `Vendor.City` (or new field) | string | Add to DTO; check if District entity exists |
| `vendorStatus` | `Vendor.Status` | string | Add to DTO |
| `submissionDateTime` | `TenderVendorSubmission.CreatedDateTime` | DateTime | Add to DTO + include BaseEntity fields |
| `receiptNo` | **NOT IN MODEL** | string | Need new field on `TenderVendorSubmission` |

**Recommended Fix — Expanded DTO:**
```csharp
public class TenderOpeningBidProgressDto
{
    public int Bil { get; set; }
    public string? CompanyRegNo { get; set; }        // NEW
    public string? VendorName { get; set; }
    public string? Type { get; set; }                // NEW (CompanyEntityType)
    public string? State { get; set; }               // NEW
    public string? District { get; set; }            // NEW
    public string? VendorStatus { get; set; }        // NEW
    public DateTime? SubmissionDateTime { get; set; } // NEW
    public string? ReceiptNo { get; set; }           // NEW (requires DB migration)
    public string OpeningStatus { get; set; } = "Pending";
}
```

**Database Migration Required:** Add `ReceiptNo` column to `TenderVendorSubmission` table.

---

### GAP 2: Submitted Documents & Document Validation — Entire Feature Missing

**Screens:** Screen 1.6 (Submitted Documents Tab) + Screen 1.7 (Document Validation Popup)  
**Severity:** CRITICAL  
**Impact:** Document validation workflow cannot function

**Required New Entities:**
```csharp
// New DB Model
public class TenderVendorDocument : BaseEntity
{
    public int Id { get; set; }
    public int TenderId { get; set; }
    public int VendorId { get; set; }
    public int TenderRequiredDocumentId { get; set; }
    public string? AttachmentPath { get; set; }
    public string? AttachmentFileName { get; set; }
    public string SubmissionStatus { get; set; } = "Not Applicable"; // Submitted / Not Applicable
    public string FirstValidationStatus { get; set; } = "Pending";   // Pending / Passed / Failed
    public int? FirstValidatorId { get; set; }
    public string? FirstValidationRemarks { get; set; }
    public DateTime? FirstValidationDateTime { get; set; }
    public string SecondValidationStatus { get; set; } = "Pending";  // Pending / Passed / Failed
    public int? SecondValidatorId { get; set; }
    public string? SecondValidationRemarks { get; set; }
    public DateTime? SecondValidationDateTime { get; set; }
    // Navigation properties
    public TenderApplication? Tender { get; set; }
    public Vendor? Vendor { get; set; }
    public TenderRequiredDocument? RequiredDocument { get; set; }
    public User? FirstValidator { get; set; }
    public User? SecondValidator { get; set; }
}
```

**Required New DTOs:**
```csharp
public class TenderVendorDocumentDto
{
    public int Id { get; set; }
    public string? DocumentName { get; set; }
    public string? AttachmentPath { get; set; }
    public string? AttachmentFileName { get; set; }
    public string SubmissionStatus { get; set; }
    public string FirstValidationStatus { get; set; }
    public string? FirstValidatorName { get; set; }
    public string SecondValidationStatus { get; set; }
    public string? SecondValidatorName { get; set; }
}

public class SaveDocumentValidationDto
{
    public int DocumentId { get; set; }
    public string FirstValidationResult { get; set; }   // "Passed" / "Failed"
    public string? FirstValidationRemarks { get; set; }
    public string SecondValidationResult { get; set; }   // "Passed" / "Failed"
    public string? SecondValidationRemarks { get; set; }
}
```

**Required New API Endpoints:**

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/TenderManagement/GetVendorSubmittedDocuments?tenderId={id}&vendorId={id}` | Get documents for a vendor in a tender |
| POST | `/api/TenderManagement/SaveDocumentValidation` | Save 1st/2nd validation results |

---

### GAP 3: Vendor Offer / Price Summary — No Per-Material Pricing API

**Screens:** Screen 1.8 (Price Summary Tab) + Screen 2.5 (Vendor Offer Popup)  
**Severity:** CRITICAL  
**Impact:** Cannot show vendor's per-material pricing or negotiation history

**Issue:** The current bidding model uses `BiddingAsset` / `BidderSubmissionItem` for asset-based bidding (vehicles). The screens show material/service-based pricing (Material Code, Material Group, Quantity, Unit Price) which maps to `TenderJobScope` items — but there's no link between vendor submissions and job scope items.

**Required New Entity:**
```csharp
public class TenderVendorPriceItem : BaseEntity
{
    public int Id { get; set; }
    public int TenderVendorSubmissionId { get; set; }
    public int TenderJobScopeId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    public int NegotiationRound { get; set; } = 0; // 0 = original, 1+ = negotiation
    public TenderVendorSubmission? Submission { get; set; }
    public TenderJobScope? JobScope { get; set; }
}
```

**Required New Endpoints:**

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/TenderManagement/GetVendorOfferSummary?tenderId={id}&vendorId={id}` | Get per-material pricing + negotiation history |
| POST | `/api/Bidding/SubmitNegotiationPricing` | Submit negotiation round pricing |

---

### GAP 4: Tender Award List — Filter Parameters Not Supported

**Screen:** Screen 3.1 (Tender Award Listing)  
**Severity:** MODERATE  
**Impact:** 4 dropdown filters shown on UI cannot filter server-side

**Current API:** `GetTenderAwardList` accepts only `referenceId` and `projectName`.  
**Screen needs:** Application Level, Tender Category, Job Category, Status dropdown filters.

**Required Fix:** Add filter parameters to the API:
```
GET /api/TenderManagement/GetTenderAwardList?applicationLevelId={id}&tenderCategoryId={id}&jobCategoryId={id}&statusId={id}
```

---

## IMPROVEMENTS

### IMP 1: Composite Vendor Details Endpoint for Tender Opening

**Current:** Frontend must make 4 separate API calls to display the Vendor Details tab:
1. `GET /api/Vendor/GetVendorDetilsById`
2. `GET /api/Vendor/Financial`
3. `GET /api/Vendor/Categories`
4. `GET /api/Vendor/Experiences`

**Recommended:** Create a single composite endpoint:
```
GET /api/TenderManagement/GetTenderOpeningVendorDetails?tenderId={id}&vendorId={id}
```

Returns `TenderOpeningVendorDetailsDto` combining profile + financial + categories + experiences in one response.

---

### IMP 2: Dedicated Tender Evaluation Listing Endpoint

**Current:** Evaluation listing reuses `GetTenderOpeningList` — client must filter by evaluation status.

**Recommended:** Create `GET /api/TenderManagement/GetTenderEvaluationList` with evaluation-specific statuses and date range.

---

### IMP 3: Status Value Inconsistency

**Issue:** The Progress tab UI shows "Pending" (yellow) and "Completed" (green), but the API returns "Pending" / "Passed" / "Failed".

**Recommended:** Standardize status values. Either:
- API returns "Pending" / "Completed" (match UI)
- Frontend maps "Passed" → "Completed" display text

---

### IMP 4: Download Report Endpoints

**Screens:** Technical Evaluation and Financial Evaluation sections both have "Download Report" buttons, but no corresponding API endpoints exist for report generation/download.

**Recommended Endpoints:**

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/TenderManagement/DownloadTechnicalEvaluationReport?tenderId={id}` | PDF/Excel technical report |
| GET | `/api/TenderManagement/DownloadFinancialEvaluationReport?tenderId={id}` | PDF/Excel financial report |

---

### IMP 5: SST Document Download Endpoint

**Screens:** Both Tender Award (Screen 3.2) and Bidding Award Details (Screen 4.3) have "SST Document" / "Download SST Document" buttons but no corresponding endpoint.

**Recommended:**
```
GET /api/TenderManagement/DownloadSSTDocument?tenderId={id}
```

---

## UNUSED APIs

The following Phase 3 API endpoints exist in the backend but are NOT directly used by any Phase 3 screen:

| # | Method | URL | Notes |
|---|--------|-----|-------|
| 1 | GET | `/api/Bidding/GetBiddingAssets?tenderId={id}` | Admin asset management — no screen in Phase 3 |
| 2 | POST | `/api/Bidding/SaveBiddingAsset` | Admin asset CRUD — no dedicated screen |
| 3 | DELETE | `/api/Bidding/DeleteBiddingAsset?assetId={id}` | Admin asset delete — no dedicated screen |

**Note:** These admin bidding asset management endpoints may be used in a separate admin screen not included in Phase 3 wireframes, or may be needed for future phases.

---

## MISSING APIs

The following APIs are required by Phase 3 screens but DO NOT exist in the backend:

| # | Priority | Screen | Required Endpoint | Purpose |
|---|----------|--------|-------------------|---------|
| 1 | P0 | Screen 1.6 | `GET /api/TenderManagement/GetVendorSubmittedDocuments?tenderId={id}&vendorId={id}` | Fetch submitted documents with validation status |
| 2 | P0 | Screen 1.7 | `POST /api/TenderManagement/SaveDocumentValidation` | Save 1st/2nd validation results |
| 3 | P0 | Screen 1.8 | `GET /api/TenderManagement/GetVendorOfferSummary?tenderId={id}&vendorId={id}` | Per-material vendor pricing + negotiation history |
| 4 | P1 | Screen 1.8 | `POST /api/Bidding/SubmitNegotiationPricing` | Submit negotiation round pricing |
| 5 | P1 | Screen 1.5 | `GET /api/TenderManagement/GetTenderOpeningVendorDetails?tenderId={id}&vendorId={id}` | Composite vendor details for opening context |
| 6 | P2 | Screen 2.3 | `GET /api/TenderManagement/DownloadTechnicalEvaluationReport?tenderId={id}` | PDF report download |
| 7 | P2 | Screen 2.3 | `GET /api/TenderManagement/DownloadFinancialEvaluationReport?tenderId={id}` | PDF report download |
| 8 | P2 | Screen 3.2/4.3 | `GET /api/TenderManagement/DownloadSSTDocument?tenderId={id}` | SST document download |
| 9 | P2 | Screen 2.1 | `GET /api/TenderManagement/GetTenderEvaluationList` | Dedicated evaluation listing |

**Database Migrations Required:**

| # | Table | Change | Reason |
|---|-------|--------|--------|
| 1 | `TenderVendorSubmission` | Add `ReceiptNo` (string) | Progress tab shows Receipt No. |
| 2 | NEW: `TenderVendorDocument` | Create table | Submitted documents + validation workflow |
| 3 | NEW: `TenderVendorPriceItem` | Create table | Per-material vendor pricing + negotiation |

---

## VALIDATION & EDGE CASES

### Tender Opening

| # | Scenario | Current Handling | Recommendation |
|---|----------|-----------------|----------------|
| 1 | Verify button clicked when no bids exist | No validation | Return error: "No bids to verify" |
| 2 | Committee member tries to verify twice | Unknown | Check `TenderOpeningVerification` exists → return "Already verified" |
| 3 | Opening attempted after evaluation started | No check | Add status guard: Opening status must be "In Progress" |
| 4 | Document validation — 2nd validator acts before 1st | No check | Enforce sequence: 1st validation must be complete before 2nd |

### Tender Evaluation

| # | Scenario | Current Handling | Recommendation |
|---|----------|-----------------|----------------|
| 1 | Score saved with total weightage != 100% | Server computes | Validate: SUM(weightage) must = 100 |
| 2 | Recommendation saved without completing all technical scores | No validation | Check all vendors scored before allowing recommendation |
| 3 | Financial evaluation with missing vendor financial data | May crash (null ref) | Handle null → show "Financial data not available" |
| 4 | Vendor's registration expired during evaluation | Not checked | Add warning flag in evaluation rows |

### Tender Award

| # | Scenario | Current Handling | Recommendation |
|---|----------|-----------------|----------------|
| 1 | Award saved without selecting vendor | No validation | `awardedVendorId` required validation |
| 2 | Minutes of Meeting — delete last entry | Allowed | Add confirmation dialog; no backend restriction needed |
| 3 | Insurance values when projectValue is null | Returns 0 | Show "N/A" when project value not set |
| 4 | Performance saved without all ratings | No validation | All 5 categories + 5 feedbacks required |

### Bidding (Vendor)

| # | Scenario | Current Handling | Recommendation |
|---|----------|-----------------|----------------|
| 1 | Submit bidding after closing date/time | Unknown | Server-side check: reject if past closing |
| 2 | Bid price lower than starting price | No validation | Add warning (not blocker — some assets allow below-start bids) |
| 3 | Acknowledge award with "Reject" but upload stamp duty | Allowed | Ignore stamp duty file if "Reject" |
| 4 | Double submission on same tender | `alreadySubmitted` flag | Enforce on POST: return error if already submitted |

---

## RECOMMENDED API CONTRACT (DTO STRUCTURE)

### New DTO: TenderOpeningBidProgressDto (Expanded)

```csharp
// File: DB/Entity/BiddingDto.cs
public class TenderOpeningBidProgressDto
{
    public int Bil { get; set; }
    public string? CompanyRegNo { get; set; }
    public string? VendorName { get; set; }
    public string? Type { get; set; }                 // CompanyEntityType name
    public string? State { get; set; }
    public string? District { get; set; }
    public string? VendorStatus { get; set; }
    public DateTime? SubmissionDateTime { get; set; }
    public string? ReceiptNo { get; set; }
    public string OpeningStatus { get; set; } = "Pending";
}
```

### New DTO: TenderVendorDocumentDto

```csharp
// File: DB/Entity/TenderVendorDocumentDto.cs (NEW FILE)
public class TenderVendorDocumentListDto
{
    public int TenderId { get; set; }
    public int VendorId { get; set; }
    public string? VendorName { get; set; }
    public List<TenderVendorDocumentRowDto> Documents { get; set; } = new();
}

public class TenderVendorDocumentRowDto
{
    public int Id { get; set; }
    public string? DocumentName { get; set; }
    public string? AttachmentPath { get; set; }
    public string? AttachmentFileName { get; set; }
    public string SubmissionStatus { get; set; } = "Not Applicable";
    public string FirstValidationStatus { get; set; } = "Pending";
    public string? FirstValidatorName { get; set; }
    public string? FirstValidationRemarks { get; set; }
    public string SecondValidationStatus { get; set; } = "Pending";
    public string? SecondValidatorName { get; set; }
    public string? SecondValidationRemarks { get; set; }
}

public class SaveDocumentValidationDto
{
    public int DocumentId { get; set; }
    public string? FirstValidationResult { get; set; }
    public string? FirstValidationRemarks { get; set; }
    public string? SecondValidationResult { get; set; }
    public string? SecondValidationRemarks { get; set; }
}
```

### New DTO: VendorOfferSummaryDto

```csharp
// File: DB/Entity/VendorOfferSummaryDto.cs (NEW FILE)
public class VendorOfferSummaryDto
{
    public int TenderId { get; set; }
    public int VendorId { get; set; }
    public string? VendorName { get; set; }
    public List<VendorOfferItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public decimal FpmsEstimationPrice { get; set; }
    public List<NegotiationRoundDto> NegotiationRounds { get; set; } = new();
}

public class VendorOfferItemDto
{
    public int Bil { get; set; }
    public string? PktBlok { get; set; }           // WBQ No.
    public string? Description { get; set; }        // Short Text
    public string? Unit { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}

public class NegotiationRoundDto
{
    public int RoundNumber { get; set; }
    public DateTime SubmittedOn { get; set; }
    public List<VendorOfferItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public decimal FpmsEstimationPrice { get; set; }
}
```

### New DTO: TenderOpeningVendorDetailsDto (Composite)

```csharp
// File: DB/Entity/TenderOpeningDto.cs (add to existing)
public class TenderOpeningVendorDetailsDto
{
    public int TenderId { get; set; }
    public int VendorId { get; set; }
    // Company Info
    public string? CompanyType { get; set; }
    public string? CompanyName { get; set; }
    public string? RegistrationNo { get; set; }
    public string? VendorCode { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public string? Address { get; set; }
    public string? Postcode { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? OfficePhoneNo { get; set; }
    public string? FaxNo { get; set; }
    public string? EmailAddress { get; set; }
    public string? Website { get; set; }
    public string? IndustryType { get; set; }
    public string? BusinessCoverageArea { get; set; }
    public string? PicName { get; set; }
    public string? PicMobileNo { get; set; }
    public string? PicEmailAddress { get; set; }
    // Financial
    public decimal PaidUpCapital { get; set; }
    public decimal AuthorizedCapital { get; set; }
    public decimal WorkingCapital { get; set; }
    public decimal LiquidCapital { get; set; }
    public decimal AssetBalance { get; set; }
    // Categories
    public List<VendorCategoryDisplayDto> FpmsbCodes { get; set; } = new();
    public List<VendorCategoryDisplayDto> MofCodes { get; set; } = new();
    public List<VendorCategoryDisplayDto> CidbCodes { get; set; } = new();
    // Experiences
    public List<VendorExperienceDisplayDto> Experiences { get; set; } = new();
}
```

---

## PHASE 3 API SUMMARY TABLE

| # | Module | Screen | Method | URL | Auth | Status |
|---|--------|--------|--------|-----|------|--------|
| 1 | Opening | Listing | GET | /api/TenderManagement/GetTenderOpeningList | No | EXISTS |
| 2 | Opening | Appointment | GET | /api/TenderManagement/GetTenderOpeningDetail | No | EXISTS |
| 3 | Opening | Summary | GET | /api/TenderManagement/GetTenderOpeningPage | No | EXISTS |
| 4 | Opening | Verify | POST | /api/Bidding/VerifyTenderOpening | No | EXISTS |
| 5 | Opening | Progress | GET | /api/Bidding/GetTenderOpeningProgress | No | EXISTS (needs expansion) |
| 6 | Opening | Vendor Details | GET | /api/TenderManagement/GetTenderOpeningVendorDetails | No | **MISSING** |
| 7 | Opening | Submitted Docs | GET | /api/TenderManagement/GetVendorSubmittedDocuments | No | **MISSING** |
| 8 | Opening | Doc Validation | POST | /api/TenderManagement/SaveDocumentValidation | Yes | **MISSING** |
| 9 | Opening | Price Summary | GET | /api/TenderManagement/GetVendorOfferSummary | No | **MISSING** |
| 10 | Evaluation | Listing | GET | /api/TenderManagement/GetTenderOpeningList (reused) | No | EXISTS |
| 11 | Evaluation | Appointment | GET | /api/TenderManagement/GetTenderOpeningDetail (reused) | No | EXISTS |
| 12 | Evaluation | Full Page | GET | /api/TenderManagement/GetTenderEvaluationPage | No | EXISTS |
| 13 | Evaluation | Tech Popup | GET | /api/TenderManagement/GetTechnicalEvaluationPopup | No | EXISTS |
| 14 | Evaluation | Save Scores | POST | /api/TenderManagement/SaveTechnicalScore | Yes | EXISTS |
| 15 | Evaluation | Recommendation | POST | /api/TenderManagement/SaveTenderRecommendation | Yes | EXISTS |
| 16 | Evaluation | Vendor Offer | GET | /api/TenderManagement/GetVendorOfferSummary | No | **MISSING** |
| 17 | Evaluation | Tech Report | GET | /api/TenderManagement/DownloadTechnicalEvaluationReport | No | **MISSING** |
| 18 | Evaluation | Fin Report | GET | /api/TenderManagement/DownloadFinancialEvaluationReport | No | **MISSING** |
| 19 | Award | Listing | GET | /api/TenderManagement/GetTenderAwardList | No | EXISTS (needs filter expansion) |
| 20 | Award | Award Page | GET | /api/TenderManagement/GetTenderAwardPage | No | EXISTS |
| 21 | Award | Save Award | POST | /api/TenderManagement/SaveTenderAward | No | EXISTS |
| 22 | Award | Save Minutes | POST | /api/TenderManagement/SaveTenderAwardMinutes | No | EXISTS |
| 23 | Award | Delete Minutes | DELETE | /api/TenderManagement/DeleteTenderAwardMinutes | No | EXISTS |
| 24 | Award | Performance | GET | /api/TenderManagement/GetVendorPerformancePage | No | EXISTS |
| 25 | Award | Save Perf. | POST | /api/TenderManagement/SaveVendorPerformance | No | EXISTS |
| 26 | Award | SST Document | GET | /api/TenderManagement/DownloadSSTDocument | No | **MISSING** |
| 27 | Bidding | Listing | GET | /api/Bidding/GetActiveBiddingList | No | EXISTS |
| 28 | Bidding | Details | GET | /api/Bidding/GetBiddingDetail | No | EXISTS |
| 29 | Bidding | Submit Bid | POST | /api/Bidding/SubmitBidding | No | EXISTS |
| 30 | Bidding | Award Detail | GET | /api/Bidding/GetBiddingAwardDetail | No | EXISTS |
| 31 | Bidding | Acknowledge | POST | /api/Bidding/SubmitBidderAcknowledgement | No | EXISTS |
| 32 | Bidding | Negotiation | POST | /api/Bidding/SubmitNegotiationPricing | No | **MISSING** |

**Summary:** 22 endpoints EXIST, 10 endpoints MISSING (4 critical, 3 important, 3 nice-to-have)

---

## NAVIGATION FLOWS

### Admin Portal — Tender Lifecycle (Phase 3)

```
Tender Opening
├── Listing (GET GetTenderOpeningList)
│   └── Click Reference ID
│       ├── Appointment Page (GET GetTenderOpeningDetail)
│       │   └── Click Proceed
│       │       ├── Summary Tab (GET GetTenderOpeningPage)
│       │       │   ├── Click Vendor Name → Vendor Details (GET GetVendorDetilsById × 4)
│       │       │   │   ├── Vendor Details Tab (profile, financial, categories, experiences)
│       │       │   │   ├── Submitted Documents Tab (GET GetVendorSubmittedDocuments) [MISSING]
│       │       │   │   │   └── Click Validate → Validation Popup (POST SaveDocumentValidation) [MISSING]
│       │       │   │   └── Summary Tab (GET GetVendorOfferSummary) [MISSING]
│       │       │   ├── Click Print → Print view
│       │       │   └── Click Verify → (POST VerifyTenderOpening)
│       │       └── Progress Tab (GET GetTenderOpeningProgress)
│       └── ...
│
Tender Evaluation
├── Listing (GET GetTenderOpeningList — reused, filter by evaluation status)
│   └── Click Reference ID
│       ├── Appointment Page (GET GetTenderOpeningDetail — reused)
│       │   └── Click Proceed
│       │       └── Evaluation Details Page (GET GetTenderEvaluationPage)
│       │           ├── Technical Evaluation
│       │           │   ├── Click Tenderer Code → Scoring Popup (GET GetTechnicalEvaluationPopup)
│       │           │   │   └── Click Save → (POST SaveTechnicalScore)
│       │           │   └── Click Download Report → [MISSING]
│       │           ├── Financial Evaluation
│       │           │   └── Click Download Report → [MISSING]
│       │           └── Recommendation
│       │               ├── Click Vendor Offer Price → Vendor Offer Popup (GET GetVendorOfferSummary) [MISSING]
│       │               └── Click Save → (POST SaveTenderRecommendation)
│       └── ...
│
Tender Award
├── Listing (GET GetTenderAwardList)
│   └── Click Reference ID
│       ├── Tender Award Tab (GET GetTenderAwardPage)
│       │   ├── Minutes of Meeting
│       │   │   ├── Click Add → Popup (POST SaveTenderAwardMinutes)
│       │   │   ├── Click Edit → Popup (POST SaveTenderAwardMinutes with ID)
│       │   │   └── Click Delete → (DELETE DeleteTenderAwardMinutes)
│       │   ├── Click Save → (POST SaveTenderAward)
│       │   └── Click SST Document → [MISSING]
│       └── Vendor Performance Tab (GET GetVendorPerformancePage)
│           └── Click Save → (POST SaveVendorPerformance)
```

### Vendor Portal — Bidding Flow (Phase 3)

```
Bidding
├── Listing (GET GetActiveBiddingList)
│   └── Click Bidding Title
│       ├── Bidding Details Tab (GET GetBiddingDetail)
│       │   ├── Enter Bid Prices → per-asset input
│       │   └── Click Submit Bidding → (POST SubmitBidding)
│       └── Award Details Tab (GET GetBiddingAwardDetail)
│           ├── Select Acknowledgement (Accept/Reject)
│           ├── Upload Stamp Duty (if Accept)
│           ├── Click Download SST Document → [MISSING]
│           └── Click Submit → (POST SubmitBidderAcknowledgement)
```

---

*Document generated on 2026-04-16 from analysis of 20 Phase 3 wireframe screens and complete backend codebase (controllers, DTOs, models, repositories, services).*
