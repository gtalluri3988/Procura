# PROCURA – Complete API Documentation for Frontend Developers

**Version:** 1.0  
**Date:** 2026-04-03  
**Base URL:** `https://localhost:7225` (configurable per environment)  
**Framework:** ASP.NET Core 7.0 Web API  
**Client:** FELDA Plantation Management Sdn. Bhd. (FPMSB)

---

## TABLE OF CONTENTS

1. [Common Standards](#1-common-standards)
2. [Authentication Flow](#2-authentication-flow)
3. [Dropdown / Select List API](#3-dropdown--select-list-api)
4. [Module 1 – Authentication](#module-1--authentication)
5. [Module 2 – Vendor Registration (Vendor Portal)](#module-2--vendor-registration-vendor-portal)
6. [Module 3 – Vendor Dashboard (Vendor Portal)](#module-3--vendor-dashboard-vendor-portal)
7. [Module 4 – Tender Submission / Bidding (Vendor Portal)](#module-4--tender-submission--bidding-vendor-portal)
8. [Module 5 – Tender Management (Admin Portal)](#module-5--tender-management-admin-portal)
9. [Module 6 – Tender Opening (Admin Portal)](#module-6--tender-opening-admin-portal)
10. [Module 7 – Tender Evaluation (Admin Portal)](#module-7--tender-evaluation-admin-portal)
11. [Module 8 – Tender Award (Admin Portal)](#module-8--tender-award-admin-portal)
12. [Module 9 – Master Data (Admin Portal)](#module-9--master-data-admin-portal)
13. [Module 10 – User Management (Admin Portal)](#module-10--user-management-admin-portal)
14. [Module 11 – Vendor Management (Admin Portal)](#module-11--vendor-management-admin-portal)
15. [Module 12 – News Management (Admin Portal)](#module-12--news-management-admin-portal)
16. [API Summary Table](#api-summary-table)
17. [APIs Not Mapped to Screens](#apis-not-mapped-to-screens)

---

## 1. Common Standards

### 1.1 Base URL & Route Pattern
```
Base URL : https://localhost:7225
Pattern  : /api/{Controller}/{Action}
```

### 1.2 Standard Response Wrapper
All API responses are wrapped in `CSAResponseModel<T>`:
```json
{
  "error": false,
  "errors": [],
  "data": { ... }
}
```
On error:
```json
{
  "error": true,
  "errors": ["Validation failed: Field X is required"],
  "data": null
}
```

### 1.3 Authentication Header
All protected endpoints require a JWT Bearer token:
```
Authorization: Bearer <jwt_token>
```

### 1.4 Content-Type
```
Content-Type: application/json
```
For file uploads:
```
Content-Type: multipart/form-data
```

### 1.5 Common HTTP Error Responses

| Status | Meaning | When |
|--------|---------|------|
| 400 | Bad Request | Invalid input / validation failed |
| 401 | Unauthorized | Missing or invalid JWT token |
| 403 | Forbidden | Role not permitted |
| 404 | Not Found | Resource not found |
| 500 | Server Error | Unexpected server-side exception |

### 1.6 JSON Enum Serialization
All enums are serialized as **strings**, not numbers (e.g., `"Approved"` not `1`).

### 1.7 Pagination
The current API does not implement server-side pagination. All list endpoints return full results. Use search/filter parameters to reduce data volume.

---

## 2. Authentication Flow

```
1. Vendor or Staff submits credentials
2. POST /api/Auth/VendorAuthenticate  OR  POST /api/Auth/Authenticate
3. Server validates and returns JWT token (60-min expiry)
4. Frontend stores token (localStorage / sessionStorage)
5. All subsequent API calls include: Authorization: Bearer <token>
6. Token expires → user must re-authenticate
```

**JWT Token Claims:**

| Claim | Value |
|-------|-------|
| `userid` | Internal user ID |
| `roleid` | User's role ID |
| `Name` | Username |
| `FirstName` | First name |
| `LastName` | Last name |
| `CName` | Community/organization name |
| `Role` | Role name string |

**Token Lifetime:** 60 minutes (configured in `appsettings.json → JwtSettings:ExpiryMinutes`)

---

## 3. Dropdown / Select List API

Used by almost every screen for populating dropdowns.

### GET /api/SelectList/GetSelectListAsync

**Authentication:** Not required
**Description:** Returns dropdown items for one or more list types in a single call.

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `inputTypes` | string | Yes | Comma-separated list of dropdown type names |

**Single type — Request:**
```
GET /api/SelectList/GetSelectListAsync?inputTypes=VendorType
```
**Single type — Response:**
```json
[
  { "id": 1, "name": "Sendirian Berhad (Sdn. Bhd.)", "value": "1" },
  { "id": 2, "name": "Berhad (Bhd.)",                "value": "2" }
]
```

**Multiple types — Request:**
```
GET /api/SelectList/GetSelectListAsync?inputTypes=VendorType,State,RegistrationStatus
```
**Multiple types — Response:**
```json
{
  "VendorType": [
    { "id": 1, "name": "Sendirian Berhad (Sdn. Bhd.)" }
  ],
  "State": [
    { "id": 1, "name": "Johor" },
    { "id": 2, "name": "Kedah" }
  ],
  "RegistrationStatus": [
    { "id": 1, "name": "Active" },
    { "id": 2, "name": "Pending Approval" }
  ]
}
```

**Known Dropdown Types Used Across Screens:**

| Type Name | Used In |
|-----------|---------|
| `VendorType` | Vendor listing filter |
| `State` | Vendor registration, user management |
| `RegistrationStatus` | Vendor management filter |
| `ApplicationLevel` | Tender management |
| `TenderCategory` | Tender management |
| `JobCategory` | Tender management |
| `Status` | Various listing screens |
| `SiteLevel` | User management |
| `SiteOffice` | User management |
| `UserRole` | User management |
| `Country` | Vendor profile |
| `BankKey` | Vendor financial |
| `TaxType` | Vendor financial |
| `AccountType` | Vendor financial |
| `Acknowledgement` | Bidding award |

---

## MODULE 1 – AUTHENTICATION

### Screen 1.1 – Login (Vendor + Staff)

**Description:** Public-facing login modal with two tabs: "Vendor" (Registration No. + Password) and "FPMSB Staff" (Staff ID + Password).

**UI Elements:** Tab (Vendor / FPMSB Staff), Username field, Password field, Login button, Forgot Password link.

---

#### API 1.1.1 – Staff Login

**Endpoint Name:** Authenticate
**Method:** POST
**URL:** `/api/Auth/Authenticate`
**Authentication:** Not required

**Request Body:**
```json
{
  "username": "12345",
  "password": "P@ssw0rd123",
  "roleId": 1
}
```

**Field Descriptions:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `username` | string | Yes | Staff ID |
| `password` | string | Yes | Account password |
| `roleId` | int | Yes | Role ID for the login context |

**Validation Rules:**
- `username` and `password` must not be empty
- Account must be active (not deactivated)

**Success Response (200):**
```json
{
  "error": false,
  "errors": [],
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "redirectTo": "/dashboard",
    "nextStep": null,
    "firstTimeLogin": false,
    "isRegistrationComplete": false
  }
}
```

**Error Responses:**

| Status | Body | Reason |
|--------|------|--------|
| 400 | `{ "message": "Login failed" }` | Wrong credentials |
| 401 | `{ "message": "Login failed" }` | Account deactivated |

**Frontend Notes:**
- Store `data.token` in localStorage/sessionStorage
- Redirect to `data.redirectTo` after login
- If `data.firstTimeLogin = true` → redirect to password change screen

---

#### API 1.1.2 – Vendor Login

**Endpoint Name:** VendorAuthenticate
**Method:** POST
**URL:** `/api/Auth/VendorAuthenticate`
**Authentication:** Not required

**Request Body:**
```json
{
  "username": "784764-K",
  "password": "P@ssw0rd123",
  "roleId": 5
}
```

**Field Descriptions:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `username` | string | Yes | Company Registration No. (ROC number) |
| `password` | string | Yes | Account password |
| `roleId` | int | Yes | Vendor role ID |

**Success Response (200):**
```json
{
  "error": false,
  "errors": [],
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "nextStep": "Financial",
    "firstTimeLogin": false,
    "isRegistrationComplete": true
  }
}
```

**Response Field Descriptions:**

| Field | Type | Description |
|-------|------|-------------|
| `token` | string | JWT Bearer token |
| `nextStep` | string | Next incomplete registration step. Values: Profile / Members / Financial / Categories / Experiences / Declaration / Payment. Null if complete. |
| `firstTimeLogin` | bool | True if vendor must change password |
| `isRegistrationComplete` | bool | True if all registration steps are done |

**Frontend Notes:**
- If `isRegistrationComplete = false` → redirect to `nextStep` registration screen
- If `firstTimeLogin = true` → redirect to update-password screen
- If complete → redirect to Vendor Dashboard

---

## MODULE 2 – VENDOR REGISTRATION (VENDOR PORTAL)

Vendor registration is a **multi-step wizard** that must be completed in sequence:
**Profile → Members → Financial → Categories → Experiences → Declaration → Payment**

---

### Screen 2.1 – New Vendor Registration

#### API 2.1.1 – Register New Vendor

**Method:** POST
**URL:** `/api/Vendor/RegisterVendor`
**Authentication:** Not required

**Request Body:**
```json
{
  "rocNumber": "784764-K",
  "companyName": "Teguh Maju Sdn. Bhd.",
  "email": "admin@teguh.com",
  "password": "P@ssw0rd123",
  "confirmPassword": "P@ssw0rd123"
}
```

**Success Response (200):**
```json
{
  "id": 101,
  "currentStep": "Profile",
  "rocNumber": "784764-K",
  "nextStep": "Profile",
  "profileResponse": { }
}
```

**Error Responses:**

| Status | Reason |
|--------|--------|
| 400 | ROC number already registered / SSM lookup failed |

---

### Screen 2.2 – Vendor Profile

**Description:** Company profile details. Can be pre-populated from SSM registry lookup.

**UI Elements:** Company Type (dropdown), Company Name, Registration No., Vendor ID (display), Date of Incorporation, Address, Postcode, State, City, Country, Office Phone, Fax, Email, Website, Type of Industry, Business Coverage Area, Company PIC Name/Mobile/Email, Attachment (Form 24 for Non-ROB/ROC vendors).

#### API 2.2.1 – Save Vendor Profile

**Method:** POST
**URL:** `/api/Vendor/Profile?vendorId={vendorId}`
**Authentication:** Not required

**Request Body:**
```json
{
  "companyTypeId": 1,
  "companyName": "Teguh Maju Sdn. Bhd.",
  "registrationNo": "784764-K",
  "dateOfIncorporation": "1997-01-01",
  "address": "28 Jalan Impian Teratai",
  "postcode": "54000",
  "stateId": 14,
  "city": "Kuala Lumpur",
  "countryId": 1,
  "officePhoneNo": "012-6576474",
  "faxNo": "012-6576488",
  "emailAddress": "email@email.com",
  "website": "www.teguh.com",
  "industryTypeId": 3,
  "businessCoverageArea": "Nationwide",
  "picName": "Ahmad bin Ali",
  "picMobileNo": "012-3456789",
  "picEmailAddress": "ahmad@teguh.com",
  "form24Attachment": null
}
```

**Success Response (200):**
```json
{ "nextStep": "Members" }
```

#### API 2.2.2 – SSM Company Lookup

**Method:** POST
**URL:** `/api/Vendor/SearchCompanyEntity`
**Authentication:** Not required
**Description:** Queries SSM (Malaysia Company Registry) by registration number to auto-fill vendor profile.

**Request Body:**
```json
{
  "regNo": "784764-K",
  "entityType": "company"
}
```

**Success Response (200):** Raw SSM API JSON with company details.

**Error Responses:**

| Status | Reason |
|--------|--------|
| 400 | `regNo` is missing |
| 502 | Cannot reach SSM API |

---

### Screen 2.3 – Vendor Members

**Description:** Manage shareholders, Board of Directors, and Management Structure.

**UI Elements:** Shareholders table (Full Name, Nationality, NRIC/Passport, Bumiputera status, Shares %, FPMSB status), Board of Directors table, Management Structure table, shareholding statistics, declaration text.

#### API 2.3.1 – Save Vendor Members

**Method:** POST
**URL:** `/api/Vendor/Members?vendorId={vendorId}`
**Authentication:** Not required

**Request Body:**
```json
{
  "shareholders": [
    {
      "fullName": "Ahmad bin Ali",
      "nationality": "Malaysian",
      "nicPassport": "800101-14-5678",
      "bumiputeraStatus": "Bumiputera",
      "sharesPercent": 50,
      "shareAmount": 1500000,
      "fpmsbStatus": "None"
    }
  ],
  "boardOfDirectors": [
    {
      "fullName": "Dato Ahmad",
      "nationality": "Malaysian",
      "nicPassport": "700101-14-1234",
      "designation": "CEO",
      "bumiputeraStatus": "Bumiputera"
    }
  ],
  "managementStructure": [
    {
      "fullName": "Suraya binti Hassan",
      "staffId": "12345",
      "nicPassport": "820201-14-9876",
      "designation": "Manager",
      "companyName": "Teguh Maju Sdn. Bhd.",
      "facultyMember": false,
      "shareholder": false,
      "sharePercent": 0
    }
  ],
  "noOfBumiputeras": 2,
  "noOfNonBumiputeras": 1,
  "bumiputeraPercent": "66.67",
  "nonBumiputeraPercent": "33.33",
  "declaration": "I hereby declare the information provided is true and accurate."
}
```

**Success Response (200):**
```json
{ "nextStep": "Financial" }
```

---

### Screen 2.4 – Vendor Financial

**Description:** Financial details: capital, tax info, bank details, and credit facilities.

**UI Elements:** Paid-Up/Authorized/Working/Liquid Capital, Asset Balance, Bumiputera/Non-Bumiputera distribution, Credit Facilities (Rolling Capital, Overdraft), Credit by Suppliers table (Add New), Tax Details (Tax Type, SST No., TIN No., MSIC Code), Bank Details (Bank Name, Bank Key, Account No., Branch, etc.), Latest Bank Statement attachment.

#### API 2.4.1 – Save Vendor Financial

**Method:** POST
**URL:** `/api/Vendor/Financial?vendorId={vendorId}`
**Authentication:** Not required

**Request Body:**
```json
{
  "paidUpCapital": 1500000.00,
  "authorizedCapital": 1500000.00,
  "workingCapital": 1500000.00,
  "liquidCapital": 1500000.00,
  "assetBalance": 1500000.00,
  "bumiputeraCapitalRM": 1500000.00,
  "bumiputeraCapitalPercent": 50,
  "nonBumiputeraCapitalRM": 1500000.00,
  "nonBumiputeraCapitalPercent": 50,
  "rollingCapital": 1500000.00,
  "totalOverdraft": 1500000.00,
  "othersCapital": 3000000.00,
  "creditFacilitiesBySuppliers": [
    { "supplierName": "Supplier 1", "creditValue": 500000.00 }
  ],
  "taxTypeId": 1,
  "sstNo": "12345",
  "sstRegistrationDate": "2020-01-01",
  "tinNo": "1122334455",
  "msicCode": "01500",
  "mainBankName": "AMBANK",
  "bankKey": "AMBANK",
  "accountHolderName": "Teguh Maju Sdn. Bhd.",
  "accountTypeId": 1,
  "accountNo": "1234567890",
  "bankBranch": "Kuala Lumpur",
  "bankBranchAddress": "Jalan Ampang, KL",
  "balanceLastThreeMonths": 200000.00,
  "fixedDeposit": 100000.00,
  "bankStatementAttachment": null
}
```

**Success Response (200):**
```json
{ "nextStep": "Categories" }
```

---

### Screen 2.5 – Vendor Category Code

**Description:** Vendor applies for trade/license categories under three code systems: FPMSB, MOF (Ministry of Finance), and CIDB (Construction Industry Development Board).

**UI Elements:** Category code change settings display, FPMSB Code section (Certificate upload, Date range, hierarchical selection: Category → Sub-Category → Activities), MOF Code section, CIDB Code section. Each section has Add/Edit/Delete per code entry.

#### API 2.5.1 – Save Vendor Categories

**Method:** POST
**URL:** `/api/Vendor/Categories?vendorId={vendorId}`
**Authentication:** Not required

**Request Body:**
```json
{
  "fpmsb": [
    {
      "certificateFile": null,
      "startDate": "2024-01-01",
      "endDate": "2026-12-31",
      "categories": [
        { "categoryId": 2, "subCategoryId": 5, "activitiesId": 10 }
      ]
    }
  ],
  "mof": [
    {
      "certificateFile": null,
      "startDate": "2024-01-01",
      "endDate": "2026-12-31",
      "categories": [
        { "categoryId": 1, "subCategoryId": 2, "subCategoryDivisionId": 3 }
      ]
    }
  ],
  "cidb": [
    {
      "certificateFile": null,
      "startDate": "2024-01-01",
      "endDate": "2026-12-31",
      "categories": [
        { "categoryId": 1, "code": "CE", "descriptionId": 5 }
      ]
    }
  ]
}
```

**Success Response (200):**
```json
{ "nextStep": "Experiences" }
```

#### API 2.5.2 – Get Category Code Settings

**Method:** GET
**URL:** `/api/MasterData/GetCategoryCodeSetting`
**Authentication:** Not required

**Response:**
```json
{ "monthSetting": 6, "yearSetting": 3 }
```

#### API 2.5.3 – Get All Categories (Hierarchy)

**Method:** GET
**URL:** `/api/MasterData/GetAllCategories`
**Authentication:** Not required

**Response:**
```json
[
  {
    "codeSystemId": 1,
    "codeSystemName": "FPMSB",
    "categories": [
      {
        "id": 2,
        "code": "02",
        "description": "PERTANIAN",
        "subCategories": [
          {
            "id": 10,
            "code": "02-01",
            "description": "PERTANIAN TANAMAN",
            "activities": [
              { "id": 100, "code": "02-01-01", "description": "Penanaman BTS" }
            ]
          }
        ]
      }
    ]
  }
]
```

---

### Screen 2.6 – Vendor Experiences

**Description:** List of past projects. Add New via popup modal.

**UI Elements:** Table (No., Project Name, Organization, Project Value RM, Status, Completion Year, Attachment). Popup: Project Name, Organization, Project Value, Status dropdown, Completion Year, Attachment upload.

#### API 2.6.1 – Save Vendor Experiences

**Method:** POST
**URL:** `/api/Vendor/Experiences?vendorId={vendorId}`
**Authentication:** Not required

**Request Body:**
```json
[
  {
    "projectName": "Project A",
    "organization": "Company ABC",
    "projectValue": 250000.00,
    "status": "Completed",
    "completionYear": 2024,
    "attachment": null
  }
]
```

**Success Response (200):**
```json
{ "nextStep": "Declaration" }
```

---

### Screen 2.7 – Declaration & Payment

#### API 2.7.1 – Save Vendor Declaration

**Method:** POST
**URL:** `/api/Vendor/Declaration?vendorId={vendorId}`
**Authentication:** Not required

**Request Body:**
```json
{
  "declarationText": "I hereby declare that all information provided is true and accurate.",
  "agreedToTerms": true,
  "signature": null
}
```

**Success Response (200):**
```json
{ "nextStep": "Payment" }
```

#### API 2.7.2 – Get Payment Details

**Method:** POST
**URL:** `/api/Vendor/Payment?vendorId={vendorId}`
**Authentication:** Not required

**Response:**
```json
{
  "vendorId": 101,
  "registrationFee": 150.00,
  "currency": "MYR",
  "paymentStatus": "Pending"
}
```

---

## MODULE 3 – VENDOR DASHBOARD (VENDOR PORTAL)

### Screen 3.1 – Vendor Dashboard

**Description:** Main landing page for authenticated vendor. Shows registration status, statistics, and active tender advertisements.

**UI Elements:** Registration Status banner (Draft/Pending Approval/Active/Expired/Blacklisted), Stats cards (Tender Submitted, Tender Awarded), Certificate/Transaction/Profile/Registration cards, Tender Advertisement table (Reference, Project Name, Type, Advertisement Date, Closing Date/Time). Top tabs: Dashboard / Profile / Members / Financial / Category Code / Experiences.

#### API 3.1.1 – Get Vendor Dashboard

**Method:** GET
**URL:** `/api/Vendor/GetVendorDashboard`
**Authentication:** Required

**Response:**
```json
{
  "registrationStatus": "Pending Approval",
  "tenderSubmitted": 17,
  "tenderAwarded": 3,
  "profileLastUpdate": "27/01/2026 10:36:12",
  "registrationValidUntil": "27/01/2029",
  "tenderAdvertisements": [
    {
      "reference": "2026/B020",
      "projectName": "Supply, Delivery, and Distribution of Compound...",
      "type": "Tender",
      "advertisementDate": "2026-01-20",
      "closingDateTime": "2026-02-23T11:59:00"
    }
  ]
}
```

---

## MODULE 4 – TENDER SUBMISSION / BIDDING (VENDOR PORTAL)

### Screen 4.1 – Public Portal Home

**Description:** Public-facing home page with three tabs: News, Advertisement, Award Result. No login required.

**UI Elements:** Tab (News / Advertisement / Award Result), news/tender/award listing tables, Login button, Contact info, footer.

#### API 4.1.1 – Get News

**Method:** GET
**URL:** `/api/Announcement/GetAllAnnouncements?type=News`
**Authentication:** Not required

#### API 4.1.2 – Get Advertisements

**Method:** GET
**URL:** `/api/Announcement/GetAllAnnouncements?type=Advertisement`
**Authentication:** Not required

#### API 4.1.3 – Get Award Results

**Method:** GET
**URL:** `/api/Announcement/GetAllAnnouncements?type=AwardResult`
**Authentication:** Not required

**Response (all types):**
```json
[
  {
    "id": 1,
    "title": "Supply and Delivery of Compound...",
    "details": "Full content...",
    "status": "Active",
    "type": "News",
    "createdBy": "Admin",
    "createdDate": "2024-03-02T00:00:00"
  }
]
```

---

### Screen 4.2 – Submission: Tender/Quotation Details

**Description:** Vendor views tender details before proceeding with submission (public view of an advertised tender).

**UI Elements:** Project Name, Reference No., Type, Category, Advertisement Date, Closing Date/Time, Submission method, Document Fee, Tender Document link, Requirements table (Code, Description, Terms: Compulsory/Optional), Site Visit table (Date/Time, Venue, Terms). Buttons: Back, Purchase Tender Document, Proceed with Submission.

#### API 4.2.1 – Get Tender Advertisement Page

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderAdvertisementPage?tenderApplicationId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderApplicationId": 45,
  "referenceId": "2026/B020",
  "projectName": "Supply, Delivery, and Distribution of Compound...",
  "type": "Tender",
  "category": "Supply",
  "advertisementDate": "2026-06-22",
  "closingDate": "2026-07-05",
  "closingTime": "12:00PM",
  "submissionMethod": "Online via FPMSB PROCURA",
  "documentFee": 100.00,
  "tenderDocumentUrl": "/VendorUploads/tender_document.pdf",
  "requirements": [
    { "code": "110301", "description": "Alat Ganti/Aksesori Kenderaan", "terms": "Compulsory" }
  ],
  "siteVisits": [
    { "date": "2021-06-27", "time": "09:30AM", "venue": "Location ABC", "terms": "Compulsory" }
  ]
}
```

---

### Screen 4.3 – Submission Wizard Step 1: Details

**Description:** Submission wizard step 1. Shows full tender details including Tender ID, Category Code, site visit info.

*(Uses the same API 4.2.1 — `GetTenderAdvertisementPage`)*

---

### Screen 4.4 – Submission Wizard Step 2: Document Required

**Description:** Vendor uploads required and optional submission documents.

**UI Elements:** Section A (Compulsory Documents) and Section B (Optional Documents) — each with table showing No., Document Name, Submission Type (Upload in system / Physical Delivery), Attachment/Upload button. Buttons: Back, Proceed to Summary.

**Frontend Notes:** Document list comes from `GetTenderAdvertisementPage` response. For uploading files, use multipart/form-data POST to the relevant file upload endpoint.

---

### Screen 4.5 – Submission Wizard Step 3: Summary

**Description:** Price summary table for the bid. For negotiations, shows two tables (original + negotiation round — new price must be ≤ previous submission).

**UI Elements:** Table (Bil, PKT/Blok, Description Short Text, Unit, Quantity, Unit Price RM/SEN, Sub-Total), Total Amount. Negotiation Summary section if applicable. Buttons: Back, Proceed to Submission.

*(Data from `GetBiddingDetail` response)*

---

### Screen 4.6 – Submission Wizard Step 4: Final Submission

**Description:** Final step — vendor enters receipt number, uploads receipt, agrees to terms, and submits.

**UI Elements:** Ref No., Title, Category Code, Closing Date/Time, Tender Deposit (display only), Receipt No. (input), Upload Receipt (file), Acknowledgement checkbox, Buttons: Back, Submit.

#### API 4.6.1 – Submit Bidding

**Method:** POST
**URL:** `/api/Bidding/SubmitBidding`
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "receiptNo": "RCT-2026-00123",
  "receiptFile": null,
  "agreedToTerms": true,
  "bidItems": [
    { "assetId": 10, "unitPrice": 50.00, "subTotal": 50000.00 },
    { "assetId": 11, "unitPrice": 45.00, "subTotal": 22500.00 }
  ],
  "totalAmount": 72500.00
}
```

**Validation Rules:**
- `agreedToTerms` must be `true`
- `receiptNo` required if deposit is required
- Tender must still be open (before closing date/time)

**Success Response (200):** `"Bidding submitted successfully"`

**Error Responses:**

| Status | Reason |
|--------|--------|
| 400 | Missing fields / tender already closed |
| 404 | Tender or vendor not found |

---

### Screen 4.7 – Bidding Listing (Vendor)

**Description:** Vendor's list of tenders available for or previously submitted bids.

**UI Elements:** Table (Reference, Bidding Title, Advertisement Date, Closing Date/Time, Status). Click row → Bidding Details.

#### API 4.7.1 – Get Active Bidding List

**Method:** GET
**URL:** `/api/Bidding/GetActiveBiddingList?vendorId={vendorId}`
**Authentication:** Not required

**Response:**
```json
[
  {
    "tenderId": 45,
    "reference": "2026/B020",
    "biddingTitle": "Pembelian Kenderaan Terpakai...",
    "advertisementDate": "2026-01-20",
    "closingDateTime": "2026-02-23T11:59:00",
    "status": "Awarded"
  }
]
```

---

### Screen 4.8 – Bidding Details (Vendor)

**Description:** Full tender detail for bidding. Two tabs: "Bidding Details" (enter bid prices) and "Award Details" (award notification).

**UI Elements — Bidding Details Tab:**
- Application Level, Bidding Title, Job Category, Tender Category (display only)
- Deposit Amount (RM), Remarks, Starting Date, Closing Date/Time (display only)
- Asset Details table: No., Project/State, Asset Details, Asset Ref No., Starting Price (RM), Year Purchased, Bid Price (RM) — editable input
- Buttons: Back, Submit Bidding

#### API 4.8.1 – Get Bidding Detail

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
  "applicationLevel": "Ibu Pejabat (HQ)",
  "biddingTitle": "Pembelian Kenderaan Terpakai...",
  "jobCategory": "Bidaan Aset",
  "tenderCategory": "Terbuka",
  "depositAmount": 500.00,
  "remarks": "100% dibayar dalam tempoh 60 hari...",
  "startingDate": "2026-03-19",
  "closingDateTime": "2026-03-29T12:00:00",
  "assets": [
    {
      "assetId": 1,
      "projectState": "Trolak Utara",
      "assetDetails": "Toyota Hilux",
      "assetRefNo": "WVH 8935",
      "startingPrice": 6000.00,
      "yearPurchased": 2011,
      "vendorBidPrice": null
    }
  ]
}
```

---

### Screen 4.9 – Bidding Award Details (Vendor)

**Description:** "Award Details" tab. Vendor acknowledges award (Accept/Reject) and uploads Stamp Duty receipt.

**UI Elements:** Award notification text, Bidder Name/Reference ID/Title (display), Award Date (display), Acknowledgement dropdown (Accept/Reject), Stamp Duty LHDN upload, Date/Time (auto). Buttons: Back, Download SST Document, Submit.

#### API 4.9.1 – Get Bidding Award Detail

**Method:** GET
**URL:** `/api/Bidding/GetBiddingAwardDetail?tenderId={tenderId}&vendorId={vendorId}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "bidderName": "Syarikat Maju Sdn. Bhd.",
  "referenceId": "T43534621",
  "bidderTitle": "Pembelian Kenderaan Terpakai...",
  "awardDate": "2025-11-12",
  "acknowledgement": null,
  "stampDutyFile": null,
  "submittedDateTime": null
}
```

#### API 4.9.2 – Submit Bidder Acknowledgement

**Method:** POST
**URL:** `/api/Bidding/SubmitBidderAcknowledgement`
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "acknowledgement": "Accept",
  "stampDutyFile": null,
  "stampDutyFileName": "stamp_duty.pdf"
}
```

**Validation Rules:**
- `acknowledgement` must be `"Accept"` or `"Reject"`
- If `"Accept"`, `stampDutyFile` is required

**Success Response (200):** `"Acknowledgement submitted successfully"`

---

## MODULE 5 – TENDER MANAGEMENT (ADMIN PORTAL)

### Screen 5.1 – Tender Management Listing

**Description:** Admin view of all tenders with search filters.

**UI Elements:** Filters (Application Level, Tender Category, Job Category, Status dropdowns), Search button, New button, Table (No., Reference ID link, Project Name, Application Level, Job Category, Tender Category, Status, Created Date/Time).

#### API 5.1.1 – Get All Tender Applications

**Method:** GET
**URL:** `/api/TenderManagement/GetAllTenderApplications`
**Authentication:** Not required

**Response:**
```json
[
  {
    "id": 45,
    "referenceId": "2026/B020",
    "projectName": "Supply, Delivery, and Distribution of Compound...",
    "applicationLevel": "Projek",
    "jobCategory": "Kontrak Kejuruteraan",
    "tenderCategory": "Terbuka",
    "status": "New",
    "createdDateTime": "2026-01-29T08:11:54"
  }
]
```

#### API 5.1.2 – Search Tender Applications

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderApplicationBySearch`
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `applicationLevelId` | int | No | Filter by application level |
| `tenderCategoryId` | int | No | Filter by tender category |
| `jobCategoryId` | int | No | Filter by job category |
| `statusId` | int | No | Filter by status |

---

### Screen 5.2 – Tender Application Form (Create / Edit)

**Description:** Full tender form with two tabs: "Tender Application" and "Tender Issuance". Includes Job Scope, Category Code, Site Visit, Required Documents, and approval workflow.

**UI Elements — Tender Application Tab:**
Application Level, Project Name, Job Category, Tender Category, Job Start/End Date, Deposit Required, Deposit Amount, Remarks, Status.
Job Scope table (W/BQ No., Material Code, Material Group, Service Code, Short Task, Budget, Unit, Quantity, Price/Unit, Sub-Total).
Category Code section. Site Visit section. Required Documents section.
Created By / Reviewed By (1st, 2nd) / Approved By (1st, 2nd, 3rd) workflow sections.
Buttons: Back, Save, Approved, Reject / Proceed to Approved / Proceed Tender Issuance.

#### API 5.2.1 – Get Tender Application by ID

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderApplicationById?id={id}`
**Authentication:** Not required

**Response:**
```json
{
  "id": 45,
  "referenceId": "2026/B020",
  "applicationLevelId": 1,
  "applicationLevel": "Projek",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "jobCategoryId": 2,
  "tenderCategoryId": 1,
  "jobStartDate": "2026-07-03",
  "jobEndDate": "2027-07-03",
  "depositRequired": true,
  "depositAmount": 500.00,
  "remarks": "Approved",
  "status": "Approved",
  "jobScope": [
    {
      "id": 1,
      "wbqNo": "C88314800001",
      "materialCode": "242603",
      "materialGroupDescription": "Peranisan",
      "serviceCode": "ATMA09",
      "shortTask": "Manual BTS",
      "budget": 80000.00,
      "unit": "MT",
      "quantity": 1000,
      "pricePerUnit": 80.00,
      "subTotal": 80000.00
    }
  ],
  "categoryCodes": [
    { "categoryCode": "SPAD60", "category": "ALIKURUSAN TANAH", "subCategory": "PENGURUSAN SUMBER", "activities": "Menguruskut BTS" }
  ],
  "siteVisit": {
    "required": true,
    "visitDate": "2025-11-16",
    "venue": "Tapak Projek",
    "attendance": "Mandatory",
    "remarks": "Bring ID",
    "siteVisitFormUrl": "/VendorUploads/site_visit_form.pdf"
  },
  "requiredDocuments": [
    { "id": 1, "documentName": "Work Statement Jan - Mac 2025", "requirement": "Mandatory", "submissionType": "Upload in System" }
  ],
  "createdBy": {
    "picName": "Ahmad Azri",
    "department": "Finance",
    "designation": "Ketua Unit Perolehan",
    "mobileNo": "012-223 6672",
    "createdDateTime": "2026-01-29T08:11:54"
  },
  "reviewedBy": [
    { "reviewNo": 1, "picName": "Ahmad Subaan", "department": "Finance", "status": "Approved", "remarks": "OK", "reviewDateTime": "2026-01-29T08:18:11" }
  ],
  "approvedBy": [
    { "approvalNo": 1, "picName": "Mohd Rizal", "status": "Approved", "remarks": "ok to proceed", "reviewDateTime": "2026-01-29T08:18:11" }
  ]
}
```

#### API 5.2.2 – Save New Tender Application

**Method:** POST
**URL:** `/api/TenderManagement/SaveTenderApplication`
**Authentication:** Required

**Request Body:** Same structure as the response above, omit `id`, `referenceId`, `createdBy`, `reviewedBy`, `approvedBy` fields.

**Success Response (200):** The created `TenderApplicationDto` object.

#### API 5.2.3 – Update Tender Application

**Method:** POST
**URL:** `/api/TenderManagement/UpdateTenderApplication`
**Authentication:** Not required

**Request Body:** Same as Save, include `"id": 45`.

#### API 5.2.4 – Delete Tender Application

**Method:** POST
**URL:** `/api/TenderManagement/DeleteTenderApplication?tendrId={id}`
**Authentication:** Not required

**Success Response (200):** `"Tender application removed successfully"`

#### API 5.2.5 – Get Reviewers / Approvers

**Method:** GET
**URL:** `/api/TenderManagement/GetReviewersorApprovers?ApplicationLevelId={id}&DesignationId={id}&StateId={id}`
**Authentication:** Not required
**Description:** Returns eligible staff who can be assigned as reviewers/approvers for a tender.

---

### Screen 5.3 – Tender Issuance / Advertisement Page

**Description:** Second tab of Tender Management. Configure advertisement period, upload tender document, assign committees, set evaluation criteria, and review approval status.

**UI Elements:** Advertisement (Start-End Date, End Time, Document upload), Tender Opening Committee (date range + staff table with Add New), Tender Evaluation Committee (date range + staff table), Evaluation Criteria (Job Category, Passing Marks, Specification/Weightage table), Approved By section. Buttons: Back, Advertise, Reject.

#### API 5.3.1 – Save Tender Advertisement Page

**Method:** POST
**URL:** `/api/TenderManagement/SaveTenderAdvertisementPage`
**Authentication:** Required

**Request Body:**
```json
{
  "tenderApplicationId": 45,
  "advertisementStartDate": "2025-10-21",
  "advertisementEndDate": "2025-10-30",
  "advertisementEndTime": "12:00PM",
  "tenderDocumentUrl": "/VendorUploads/tender_document.pdf",
  "openingCommittee": {
    "openingStartDate": "2025-10-21",
    "openingEndDate": "2025-10-30",
    "members": [
      { "staffId": "012345", "name": "Ahmad", "department": "Procurement", "designation": "Kerani", "mobileNo": "012-667 7111" }
    ]
  },
  "evaluationCommittee": {
    "evaluationStartDate": "2025-10-21",
    "evaluationEndDate": "2025-10-30",
    "members": [
      { "staffId": "012345", "name": "Ahmad", "department": "Procurement", "designation": "Eksekutif", "mobileNo": "012-667 7111" }
    ]
  },
  "evaluationCriteria": {
    "jobCategoryId": 2,
    "passingMarks": 70,
    "criteria": [
      { "specification": "Ketersediaan Pekerja",    "weightage": 30 },
      { "specification": "Ketersediaan Jentera",    "weightage": 25 },
      { "specification": "Ketersediaan Peralatan",  "weightage": 20 },
      { "specification": "Ketersediaan Racun",      "weightage": 15 },
      { "specification": "Pengalaman Terdahulu",    "weightage": 10 }
    ]
  }
}
```

**Validation Rules:**
- Total weightage of all criteria must equal exactly 100%
- At least one Opening Committee member required
- At least one Evaluation Committee member required
- Advertisement end date must be after start date

**Success Response (200):** `"Tender advertisement page saved successfully"`

#### API 5.3.2 – Search Committee Users

**Method:** GET
**URL:** `/api/TenderManagement/SearchCommitteeUsers?name={name}&committeeType={type}`
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | string | Yes | Partial name to search |
| `committeeType` | string | Yes | `"opening"` or `"evaluation"` |

**Response:**
```json
[
  {
    "staffId": "012345",
    "name": "Ahmad bin Ali",
    "department": "Procurement",
    "designation": "Eksekutif",
    "mobileNo": "012-667 7111"
  }
]
```

---

## MODULE 6 – TENDER OPENING (ADMIN PORTAL)

### Screen 6.1 – Tender Opening Listing

**Description:** List of tenders in the Opening phase. Filter by Status.

**UI Elements:** Status filter, Search button, Table (No., Reference ID link, Project Name, Start Date, End Date, Status).

#### API 6.1.1 – Get Tender Opening List

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
    "tenderId": 45,
    "referenceId": "2026/B020",
    "projectName": "Supply, Delivery...",
    "startDate": "2026-01-29T08:11:54",
    "endDate": "2026-01-29T08:11:54",
    "status": "New"
  }
]
```

---

### Screen 6.2 – Tender Opening Appointment

**Description:** Committee member's appointment letter for a tender. Shows appointment info and Proceed button to start the opening process.

**UI Elements:** Reference ID, Project Name, appointment message text, Start Date, End Date, instruction text. Buttons: Back, Proceed.

#### API 6.2.1 – Get Tender Opening Detail

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderOpeningDetail?tenderId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T1823004",
  "projectName": "Supply, Delivery...",
  "openingStartDate": "2026-02-03",
  "openingEndDate": "2026-02-10",
  "committeeRole": "Member"
}
```

---

### Screen 6.3 – Tender Opening Summary / Progress

**Description:** Two-tab view: Summary (bids received) and Progress (verification status per committee member).

**UI Elements — Summary Tab:**
Reference ID, Project Name (display), Closing Date/Time, Type, Category Code, Estimation Cost, Validity.
Vendor table: Bil, Vendor Name, Bumi/Non-Bumi Status, Registration Expiry, Offered Price (RM).
Buttons: Print, Verify.

#### API 6.3.1 – Get Tender Opening Page (Summary)

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderOpeningPage?tenderId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T1234567",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "closingDate": "2024-12-03",
  "closingTime": "12:00 PM",
  "type": "SERVICE",
  "categoryCode": "010101 AND 010304",
  "estimationCost": 500000.00,
  "validity": "90 DAYS",
  "bids": [
    {
      "bil": 1,
      "vendorId": 101,
      "vendorName": "Vendor 1",
      "bumiStatus": "Bumiputera",
      "registrationExpiry": "2023-03-09",
      "offeredPrice": 500000.00
    }
  ]
}
```

#### API 6.3.2 – Verify Tender Opening

**Method:** POST
**URL:** `/api/Bidding/VerifyTenderOpening`
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "verificationRemarks": "All bids received and verified."
}
```

**Success Response (200):** `"Tender opening verified successfully"`

#### API 6.3.3 – Get Tender Opening Progress

**Method:** GET
**URL:** `/api/Bidding/GetTenderOpeningProgress?tenderId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "totalMembers": 3,
  "verifiedCount": 2,
  "members": [
    { "userId": 10, "name": "Ahmad", "status": "Verified", "verifiedAt": "2026-02-05T09:30:00" },
    { "userId": 11, "name": "Suraya", "status": "Pending", "verifiedAt": null }
  ]
}
```

---

## MODULE 7 – TENDER EVALUATION (ADMIN PORTAL)

### Screen 7.1 – Tender Evaluation Listing

**Description:** List of tenders in the Evaluation phase. Same layout as Tender Opening Listing.

**UI Elements:** Status filter, Search button, Table (No., Reference ID link, Project Name, Start Date, End Date, Status).

**Frontend Note:** No separate evaluation listing endpoint exists. Use `GET /api/TenderManagement/GetTenderOpeningList` and filter by evaluation-phase status on the client side.

---

### Screen 7.2 – Tender Evaluation Appointment

**Description:** Evaluation committee member's appointment letter for a tender.

**UI Elements:** Reference ID, Project Name, appointment text, Start/End Dates, instruction text. Buttons: Back, Proceed.

*(Uses the same `GetTenderOpeningDetail` API as Screen 6.2)*

---

### Screen 7.3 – Tender Evaluation Page

**Description:** Main evaluation page with Technical and Financial evaluation sections. Clicking a vendor code opens a scoring popup.

**UI Elements:**
Technical Evaluation section: table (Tenderer Code link, Tender Opening Status: Pending/Passed/Failed), Download Report button.
Financial Evaluation section: table (Tenderer Code, Capital Liquidation RM), Download Report button.
Technical Scoring Popup: Reference ID, Project Name, Tenderer Code (display), criteria table (No., Criteria, Weightage, Score 0–5 dropdown, Total, Remarks), Total Score, Passing Marks, Result. Buttons: Back, Save.

#### API 7.3.1 – Get Tender Evaluation Page

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderEvaluationPage?tenderId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "S11621009",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "technicalEvaluation": [
    { "vendorId": 101, "tendererCode": "1/3", "openingStatus": "Pending" },
    { "vendorId": 102, "tendererCode": "2/3", "openingStatus": "Passed" },
    { "vendorId": 103, "tendererCode": "3/3", "openingStatus": "Failed" }
  ],
  "financialEvaluation": [
    { "vendorId": 101, "tendererCode": "1/3", "capitalLiquidation": 299132.37 },
    { "vendorId": 102, "tendererCode": "2/3", "capitalLiquidation": 110344.81 },
    { "vendorId": 103, "tendererCode": "3/3", "capitalLiquidation": 22518.64 }
  ]
}
```

#### API 7.3.2 – Get Technical Evaluation Popup

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
  "passingMarks": 70,
  "criteria": [
    { "id": 1, "criteriaName": "Ketersediaan Pekerja",    "weightage": 30, "score": 4, "total": 24, "remarks": "manpower yang mecukupi" },
    { "id": 2, "criteriaName": "Ketersediaan Jentera",    "weightage": 25, "score": 4, "total": 20, "remarks": "spec jentera terbaru" },
    { "id": 3, "criteriaName": "Ketersediaan Peralatan",  "weightage": 20, "score": 3, "total": 12, "remarks": "peralatan outdated" },
    { "id": 4, "criteriaName": "Ketersediaan Racun",      "weightage": 15, "score": 4, "total": 12, "remarks": "memuaskan" },
    { "id": 5, "criteriaName": "Pengalaman Terdahulu",    "weightage": 10, "score": 4, "total": 10, "remarks": "berpengalaman" }
  ],
  "totalScore": 78,
  "result": "Passed"
}
```

**Field Descriptions:**

| Field | Description |
|-------|-------------|
| `score` | Rating selected (0–5) |
| `total` | `score × weightage / 5` (computed by server) |
| `totalScore` | Sum of all `total` values |
| `result` | `"Passed"` if `totalScore >= passingMarks`, else `"Failed"` |

#### API 7.3.3 – Save Technical Score

**Method:** POST
**URL:** `/api/TenderManagement/SaveTechnicalScore`
**Authentication:** Required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 101,
  "scores": [
    { "criteriaId": 1, "score": 4, "remarks": "manpower yang mecukupi" },
    { "criteriaId": 2, "score": 4, "remarks": "spec jentera terbaru" },
    { "criteriaId": 3, "score": 3, "remarks": "peralatan outdated" },
    { "criteriaId": 4, "score": 4, "remarks": "memuaskan" },
    { "criteriaId": 5, "score": 4, "remarks": "berpengalaman" }
  ]
}
```

**Validation Rules:**
- `score` must be between 0 and 5
- All criteria must be scored before saving

**Success Response (200):** `"Technical scores saved successfully"`

#### API 7.3.4 – Save Tender Recommendation

**Method:** POST
**URL:** `/api/TenderManagement/SaveTenderRecommendation`
**Authentication:** Required

**Request Body:**
```json
{
  "tenderId": 45,
  "recommendedVendorId": 102,
  "recommendationRemarks": "Vendor 2 meets all technical requirements with competitive pricing."
}
```

**Success Response (200):** `"Recommendation saved successfully"`

---

## MODULE 8 – TENDER AWARD (ADMIN PORTAL)

### Screen 8.1 – Tender Award Listing

**Description:** List of tenders in the Award phase.

**UI Elements:** Filters (Application Level, Tender Category, Job Category, Status), Search button, New button, Table (No., Reference ID link, Project Name, Application Level, Job Category, Tender Category, Status, Created Date/Time).

#### API 8.1.1 – Get Tender Award List

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
    "projectName": "Supply, Delivery...",
    "applicationLevel": "Projek",
    "jobCategory": "Kontrak Kejuruteraan",
    "tenderCategory": "Terbuka",
    "status": "Awarded",
    "createdDateTime": "2026-01-29T08:11:54"
  }
]
```

---

### Screen 8.2 – Tender Award Details

**Description:** Two-tab page: "Tender Award" (appointment details + minutes) and "Vendor Performance".

**UI Elements — Tender Award Tab:**
Reference ID, Project Name (display).
Minutes of Meeting section: table (Date, Meeting Outcome, Attachment link), Add button.
Vendor Appointment Details: Vendor dropdown, Project Value (RM), Yearly Expenses (RM), Project Start/End Date, Agreement dropdown, Agreement Date Signed, PO Number.
Insurance Details: Public Liability (Formula display, Value RM display, Period display), Contractor at Risk, Workman Compensation, LAD.
Buttons: Back, Save, SST Document.

#### API 8.2.1 – Get Tender Award Page

**Method:** GET
**URL:** `/api/TenderManagement/GetTenderAwardPage?tenderId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "referenceId": "T43534621",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "minutesOfMeeting": [
    {
      "id": 1,
      "date": "2025-07-20",
      "meetingOutcome": "Board agree to award tender to Syarikat Maju Sdn. Bhd.",
      "attachmentUrl": "/VendorUploads/Minutes_of_Meeting-20072025.pdf"
    }
  ],
  "vendorAppointment": {
    "vendorId": null,
    "projectValue": null,
    "yearlyExpenses": null,
    "projectStartDate": null,
    "projectEndDate": null,
    "agreement": null,
    "agreementDateSigned": null,
    "poNumber": null
  },
  "insurance": {
    "publicLiability": {
      "formula": "10% of total contract sum or minimum RM2,000,000.00",
      "value": 2000000.00,
      "period": "01/01/2026 to 01/01/2027"
    },
    "contractorAtRisk": {
      "formula": "Full Replacement Value of Works + 10% professional fees",
      "value": 2000000.00,
      "period": "01/02/2026 to 31/07/2026"
    },
    "workmanCompensation": {
      "formula": "As per Statutory Requirements (Common Law)",
      "value": 2000000.00,
      "period": "01/01/2026 to 01/01/2027"
    },
    "lad": {
      "formula": "RM500.00 per day of delay up to maximum 10% of contract value",
      "value": 500.00
    }
  }
}
```

#### API 8.2.2 – Save Tender Award

**Method:** POST
**URL:** `/api/TenderManagement/SaveTenderAward`
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 102,
  "projectValue": 500000.00,
  "yearlyExpenses": 150000.00,
  "projectStartDate": "2026-02-01",
  "projectEndDate": "2027-01-31",
  "agreement": "Standard",
  "agreementDateSigned": "2026-01-15",
  "poNumber": "PO-2026-001"
}
```

**Success Response (200):** `"Tender award saved successfully"`

#### API 8.2.3 – Save Minutes of Meeting

**Method:** POST
**URL:** `/api/TenderManagement/SaveTenderAwardMinutes`
**Authentication:** Not required

**Request Body:**
```json
{
  "id": null,
  "tenderId": 45,
  "date": "2025-07-20",
  "meetingOutcome": "Board agree to award tender to Syarikat Maju Sdn. Bhd.",
  "attachmentFileName": "Minutes_of_Meeting.pdf"
}
```

**Note:** `id` = null for new entry; provide the existing `id` to update.

**Success Response (200):** The saved minutes object.

#### API 8.2.4 – Delete Minutes of Meeting

**Method:** DELETE
**URL:** `/api/TenderManagement/DeleteTenderAwardMinutes?minutesId={id}`
**Authentication:** Not required

**Success Response (200):** `"Minutes of meeting deleted successfully"`

---

### Screen 8.3 – Vendor Performance

**Description:** "Vendor Performance" tab of the Tender Award page. Rate vendor performance after project completion.

**UI Elements:** Vendor Name, Reference ID, Project Name, Award Date, Review Month/Year (display). Performance table: Category, Indicator, Weightage (%), Rating (1–5) dropdown, Score (%). 5 categories: Quality (30%), Schedule (25%), Cost (20%), Service (15%), Risk/Safety (10%). Total Score. Stakeholder Feedback section (questions with Likert scale). Reviewed By section. Buttons: Back, Save.

#### API 8.3.1 – Get Vendor Performance Page

**Method:** GET
**URL:** `/api/TenderManagement/GetVendorPerformancePage?tenderId={id}`
**Authentication:** Not required

**Response:**
```json
{
  "tenderId": 45,
  "vendorName": "Syarikat Maju Sdn. Bhd.",
  "referenceId": "T43534621",
  "projectName": "SEBUT HARGA BAGI PERKHIDMATAN...",
  "awardDate": "2025-11-12",
  "reviewMonthYear": "12/2025",
  "performanceCategories": [
    { "category": "Quality",     "indicator": "Work met all technical specs and standards.", "weightage": 30, "rating": 4, "score": 24 },
    { "category": "Schedule",    "indicator": "Milestones and final delivery were on time.",  "weightage": 25, "rating": 4, "score": 20 },
    { "category": "Cost",        "indicator": "Stayed within budget; no additional fees.",    "weightage": 20, "rating": 3, "score": 12 },
    { "category": "Service",     "indicator": "Proactive communication and problem-solving.", "weightage": 15, "rating": 4, "score": 12 },
    { "category": "Risk/Safety", "indicator": "Complied with all legal/safety protocols.",   "weightage": 10, "rating": 5, "score": 10 }
  ],
  "totalScore": 78,
  "stakeholderFeedback": [
    { "questionId": 1, "question": "The vendor demonstrated a high level of expertise?", "answer": "4 - Agree" },
    { "questionId": 2, "question": "The vendor was responsive to emails/calls (within 24 hours)?", "answer": "2 - Disagree" }
  ],
  "reviewedBy": {
    "picName": "Ahmad Azri",
    "department": "Finance",
    "designation": "Ketua Unit Perolehan",
    "mobileNo": "012-223 6672",
    "createdDateTime": "2021-04-12T10:21:23"
  }
}
```

#### API 8.3.2 – Save Vendor Performance

**Method:** POST
**URL:** `/api/TenderManagement/SaveVendorPerformance`
**Authentication:** Not required

**Request Body:**
```json
{
  "tenderId": 45,
  "vendorId": 102,
  "reviewMonthYear": "12/2025",
  "performanceRatings": [
    { "category": "Quality",     "rating": 4 },
    { "category": "Schedule",    "rating": 4 },
    { "category": "Cost",        "rating": 3 },
    { "category": "Service",     "rating": 4 },
    { "category": "Risk/Safety", "rating": 5 }
  ],
  "stakeholderFeedback": [
    { "questionId": 1, "answer": "4 - Agree" },
    { "questionId": 2, "answer": "2 - Disagree" }
  ]
}
```

**Validation Rules:**
- Rating must be 1–5 for each category
- All 5 performance categories must be rated
- All stakeholder feedback questions must be answered

**Success Response (200):** `"Vendor performance saved successfully"`

---

## MODULE 9 – MASTER DATA (ADMIN PORTAL)

### Screen 9.1 – Master Data: Category Code

**Description:** Manages FPMSB, MOF, and CIDB code hierarchies plus category code change settings.

**UI Elements:** Settings fields (months/year limits), FPMSB Code section (Category → Sub-Category → Activities with Add/Edit/Delete), MOF Code section (Category → Sub-Category → Sub-Category Division), CIDB Code section (Category → Code → Description). Save button.

#### API 9.1.1 – Get All Categories

**Method:** GET
**URL:** `/api/MasterData/GetAllCategories`
**Authentication:** Not required

*(Response: hierarchical code system list — see API 2.5.3)*

#### API 9.1.2 – Save Hierarchy

**Method:** POST
**URL:** `/api/MasterData/SaveHierarchy?monthSetting={n}&YearSetting={n}`
**Authentication:** Not required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `monthSetting` | int | Yes | Months before vendor can add/edit category code |
| `YearSetting` | int | Yes | Max times per year vendor can add/edit |

**Request Body:** Array of `CategoryDto` objects with the updated hierarchy.

**Success Response (200):** `"Saved Successfully"`

---

### Screen 9.2 – Master Data: Tender Management

**Description:** Configure min. capital requirement, negotiation limit, required documents per job category, and evaluation criteria.

**UI Elements:** Min. Capital Required (%), Negotiation Limit (Times), Tender Document table (Job Category, Document Name, Requirement), Evaluation Criteria table (Job Category, Specification, Weightage %). Save button.

#### API 9.2.1 – Get Tender Management Settings

**Method:** GET
**URL:** `/api/MasterData/GetTenderManagement`
**Authentication:** Not required

**Response:**
```json
{
  "minCapitalRequired": 50,
  "negotiationLimit": 2,
  "tenderDocuments": [
    { "id": 1, "jobCategoryId": 2, "jobCategory": "Kontrak Kejuruteraan", "documentName": "Bank Statement Jan - Mac 2025", "requirement": "Mandatory" }
  ],
  "evaluationCriteria": [
    { "id": 1, "jobCategoryId": 3, "jobCategory": "Tapak Semaian", "specification": "Ketersediaan Pekerja", "weightage": 30 }
  ]
}
```

#### API 9.2.2 – Save Tender Management Settings

**Method:** POST
**URL:** `/api/MasterData/SaveTenderManagement`
**Authentication:** Not required

**Request Body:**
```json
{
  "minCapitalRequired": 50,
  "negotiationLimit": 2,
  "tenderDocuments": [
    { "id": 0, "jobCategoryId": 2, "documentName": "Bank Statement Jan - Mac 2025", "requirement": "Mandatory" }
  ],
  "evaluationCriteria": [
    { "id": 0, "jobCategoryId": 3, "specification": "Ketersediaan Pekerja", "weightage": 30 }
  ]
}
```

**Success Response (200):** `"Saved Successfully"`

---

### Screen 9.3 – Master Data: Material Budget

**Description:** Manage material budget records. Add New or upload via Excel.

**UI Elements:** Add New and Upload buttons, Table (Job Category, Service Code, Short Text, Material Group, Material Group Description, Unit, PO Act. Asign., GL Account, Keterangan GL, Rujukan WBS, Rujukan Cost Centre, Rujukan IO, Amount RM) with Edit/Delete icons.

#### API 9.3.1 – Get Material Budget List

**Method:** GET
**URL:** `/api/MasterData/GetMaterialBudgetList`
**Authentication:** Not required

**Response:**
```json
[
  {
    "id": 1,
    "jobCategoryId": 1,
    "jobCategory": "Kontrak Pertanian",
    "serviceCode": "KPTN001",
    "shortText": null,
    "materialGroup": "DM004A",
    "materialGroupDescription": "Pakaian Seragam",
    "unit": "PCS",
    "poActAsign": "Y",
    "glAccount": "70200040",
    "keteranganGL": "Pakaian Seragam",
    "rujukanWBS": null,
    "rujukanCostCentre": "880201010",
    "rujukanIO": null,
    "amount": 150000.00
  }
]
```

#### API 9.3.2 – Save Material Budget

**Method:** POST
**URL:** `/api/MasterData/SaveMaterialBudget`
**Authentication:** Not required

**Request Body:**
```json
{
  "jobCategoryId": 1,
  "serviceCode": "KPTN001",
  "shortText": "Pakaian Kerja",
  "materialGroup": "DM004A",
  "materialGroupDescription": "Pakaian Seragam",
  "unit": "PCS",
  "poActAsign": "Y",
  "glAccount": "70200040",
  "keteranganGL": "Pakaian Seragam",
  "rujukanWBS": null,
  "rujukanCostCentre": "880201010",
  "rujukanIO": null,
  "amount": 150000.00
}
```

**Success Response (200):** `"Saved Successfully"`

#### API 9.3.3 – Update Material Budget

**Method:** POST
**URL:** `/api/MasterData/UpdateMaterialBudget`
**Authentication:** Not required

**Request Body:** Same as Save, with `"id": 1`.

#### API 9.3.4 – Delete Material Budget

**Method:** DELETE
**URL:** `/api/MasterData/DeleteMaterialBudge/{id}`
**Authentication:** Not required

#### API 9.3.5 – Upload Material Budget Files

**Method:** POST
**URL:** `/api/MasterData/UploadMaterialBudgetFiles?uploadedBy={userId}`
**Authentication:** Not required
**Content-Type:** `multipart/form-data`

**Form Fields:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `files` | file[] | Yes | One or more Excel/CSV files |
| `uploadedBy` (query) | int | Yes | User ID performing the upload |

**Success Response (200):** `"Files uploaded success"`

---

### Screen 9.4 – Master Data: Vendor Management

**Description:** Configure vendor registration fees and settings.

**UI Elements:** Registration Fee (RM), Registration Validity (Years), Renewal Fee (RM), Late Renewal Fee (RM), Category Code Change Fee (RM), Certificate Background Image upload, Deny blacklisted vendor for (months). Save button.

#### API 9.4.1 – Get Vendor Management Setting

**Method:** GET
**URL:** `/api/MasterData/GetVendorManagementSetting`
**Authentication:** Not required

**Response:**
```json
{
  "registrationFee": 150.00,
  "registrationValidityYears": 3,
  "renewalFee": 150.00,
  "lateRenewalFee": 200.00,
  "categoryCodeChangeFee": 70.00,
  "certificateBackgroundImageUrl": "/VendorUploads/certificate_bg.jpg",
  "denyBlacklistedVendorMonths": 6
}
```

#### API 9.4.2 – Save / Update Vendor Management Setting

**Method:** POST
**URL:** `/api/MasterData/SaveorUpdateVendorManagementSetting`
**Authentication:** Not required

**Request Body:**
```json
{
  "registrationFee": 150.00,
  "registrationValidityYears": 3,
  "renewalFee": 150.00,
  "lateRenewalFee": 200.00,
  "categoryCodeChangeFee": 70.00,
  "certificateBackgroundImageFile": null,
  "denyBlacklistedVendorMonths": 6
}
```

**Success Response (200):** `"Files uploaded success"`

---

## MODULE 10 – USER MANAGEMENT (ADMIN PORTAL)

### Screen 10.1 – User Listing (Staff)

**Description:** All FPMSB staff users. Click Staff ID to view/edit.

#### API 10.1.1 – Get All Users (No Filter)

**Method:** GET
**URL:** `/api/User/GetAllUsers`
**Authentication:** Required

**Response:**
```json
[
  {
    "id": 10,
    "fullName": "Abdul Rahman",
    "staffId": "12345",
    "emailAddress": "rahman@fpmsb.my",
    "mobileNo": "012-6576474",
    "siteLevel": "Ibu Pejabat (HQ)",
    "siteOffice": "Kuala Lumpur",
    "designation": "Pegawai Perolehan",
    "roleId": 2,
    "role": "Business User",
    "isOpeningCommittee": true,
    "isEvaluationCommittee": true,
    "isNegotiationCommittee": true,
    "status": "Active"
  }
]
```

#### API 10.1.2 – Search Users (With Filter)

**Method:** POST
**URL:** `/api/User/GetAllUsers`
**Authentication:** Required

**Request Body:**
```json
{
  "siteLevelId": 1,
  "siteOfficeId": 3,
  "status": "Active"
}
```

---

### Screen 10.2 – Add / Edit User

**Description:** Create or update a staff user.

**UI Elements:** Full Name (required), Staff ID (required), Email Address (required), Mobile No., Site Level dropdown, Site Office dropdown, Designation, User Role dropdown, Opening Committee (Yes/No), Evaluation Committee (Yes/No), Negotiation Committee (Yes/No). Buttons: Back, Save, Reset Password.

#### API 10.2.1 – Create User

**Method:** POST
**URL:** `/api/User/CreateUser`
**Authentication:** Required

**Request Body:**
```json
{
  "fullName": "Abdul Rahman",
  "staffId": "12345",
  "emailAddress": "rahman@fpmsb.my",
  "mobileNo": "012-6576474",
  "siteLevelId": 1,
  "siteOfficeId": 3,
  "designation": "Pegawai Perolehan",
  "roleId": 2,
  "isOpeningCommittee": true,
  "isEvaluationCommittee": true,
  "isNegotiationCommittee": true
}
```

**Validation Rules:**
- `fullName`, `staffId`, `emailAddress` are required
- `emailAddress` must be a valid email format
- `staffId` must be unique in the system

**Success Response (200):**
```json
{ "message": "User created successfully" }
```

**Error Responses:**

| Status | Reason |
|--------|--------|
| 400 | Duplicate staff ID or email / missing required fields |

#### API 10.2.2 – Get User by ID

**Method:** GET
**URL:** `/api/User/GetUserById?userId={id}`
**Authentication:** Required

#### API 10.2.3 – Update User

**Method:** POST
**URL:** `/api/User/UpdateUserByAdmin`
**Authentication:** Required

**Request Body:** Same as Create, include `"id": 10`.

**Success Response (204):** No content.

---

### Screen 10.3 – User Role & Permission

**Description:** Two-step flow: Role management (add/edit/delete roles) then Permission assignment per role per menu.

**UI Elements:** Tab bar (User Listing / User Role & Permission / Bidder User Listing), Role breadcrumb step, Role input + Add button, Roles table (System Admin, Business Admin, Approver) with Edit/Delete. Save button.

#### API 10.3.1 – Get All Roles

**Method:** GET
**URL:** `/api/Admin/GetAllRoles`
**Authentication:** Required

**Response:**
```json
[
  { "id": 1, "roleName": "System Admin" },
  { "id": 2, "roleName": "Business Admin" },
  { "id": 3, "roleName": "Approver" }
]
```

#### API 10.3.2 – Save Role

**Method:** POST
**URL:** `/api/Admin/SaveRole`
**Authentication:** Required

**Request Body:**
```json
{ "roleName": "New Role Name" }
```

#### API 10.3.3 – Update Role

**Method:** POST
**URL:** `/api/Admin/UpdateRole?roleId={id}`
**Authentication:** Required

**Request Body:**
```json
{ "roleName": "Updated Role Name" }
```

#### API 10.3.4 – Get All Menu Role Permissions

**Method:** GET
**URL:** `/api/Admin/GetAllMenuRolePermission`
**Authentication:** Required

#### API 10.3.5 – Save Menu Role Permission

**Method:** POST
**URL:** `/api/Admin/SaveMenuRolePermission`
**Authentication:** Required

**Request Body:**
```json
{
  "roleId": 2,
  "menuId": 5,
  "canView": true,
  "canCreate": true,
  "canEdit": true,
  "canDelete": false
}
```

#### API 10.3.6 – Update Menu Role Permission

**Method:** POST
**URL:** `/api/Admin/UpdateRoleMenuPermission?id={id}`
**Authentication:** Required

**Request Body:** Same as Save.

#### API 10.3.7 – Get All Menus

**Method:** GET
**URL:** `/api/Admin/GetAllMenu`
**Authentication:** Required

#### API 10.3.8 – Get Menus by Role

**Method:** GET
**URL:** `/api/Admin/GetAllMenusByRole?roleId={id}`
**Authentication:** Required
**Description:** Returns menu structure for a given role — used to render the sidebar navigation.

---

### Screen 10.4 – Bidder User Listing

**Description:** List of vendor/bidder accounts. Filter by Site Level, Site Office, Status.

**UI Elements:** Filters (Site Level, Status, Site Office), Search button, New button, Table (No., Staff ID link, Full Name, Email, Mobile No., Site Level, Site Office, Status, Registration Date/Time).

#### API 10.4.1 – Get Bidding Users List (No Filter)

**Method:** GET
**URL:** `/api/User/GetBiddingUsersList`
**Authentication:** Required

#### API 10.4.2 – Get Bidding Users List (With Filter)

**Method:** POST
**URL:** `/api/User/GetBiddingUsersList`
**Authentication:** Required

**Request Body:**
```json
{
  "siteLevelId": 1,
  "siteOfficeId": 3,
  "status": "Active"
}
```

---

## MODULE 11 – VENDOR MANAGEMENT (ADMIN PORTAL)

### Screen 11.1 – Vendor Management Dashboard

**Description:** Shows vendor registration counts by status. Two tabs: "Vendor Approval" (vendor code requests + category code approvals) and "Vendor Listing".

**UI Elements:** Status cards (Draft, Pending Approval, Active, Expired, Blacklisted with counts), Vendor Approval tab (Request Vendor Code table + Category Code Approval table with Review action), Vendor Listing tab (filter + table).

#### API 11.1.1 – Get Vendor Dashboard (Admin View)

**Method:** GET
**URL:** `/api/Vendor/GetVendorDashboard`
**Authentication:** Required

**Response:**
```json
{
  "draft": 17,
  "pendingApproval": 3,
  "active": 472,
  "expired": 5,
  "blacklisted": 1,
  "vendorCodeRequests": [
    {
      "registrationNo": "123456-G",
      "vendorName": "Vendor 1 Sdn Bhd",
      "registrationDateTime": "2025-12-01T20:01:01",
      "vendorCodeStatus": "Pending Request",
      "vendorCode": null,
      "requestDateTime": null
    }
  ],
  "categoryCodeApprovals": [
    {
      "vendorName": "Vendor 1 Sdn Bhd",
      "registrationNo": "123456-G",
      "details": "Update Code",
      "requestDateTime": "2025-12-01T20:01:01",
      "status": "Pending Approval",
      "approvedDateTime": null
    }
  ]
}
```

---

### Screen 11.2 – Vendor Listing

**Description:** All registered vendors with filter options.

**UI Elements:** Filters (Vendor Type, Registration Status, State), Search button, Table (No., Registration No. link, Vendor Name, Type, State, Status).

#### API 11.2.1 – Get Vendors (Filtered)

**Method:** GET
**URL:** `/api/Vendor/GetVendors`
**Authentication:** Required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `vendorTypeId` | int | No | Filter by vendor type |
| `registrationStatusId` | int | No | Filter by registration status |
| `stateId` | int | No | Filter by state |

**Response:**
```json
[
  {
    "vendorId": 101,
    "registrationNo": "12345",
    "vendorName": "Teguh Maju Sdn. Bhd.",
    "type": "Sendirian Berhad",
    "state": "Johor",
    "status": "Active"
  }
]
```

---

### Screen 11.3 – Vendor Code Request Details

**Description:** Admin reviews vendor details before initiating SAP vendor code creation.

**UI Elements:** Vendor Code Request Details (Vendor Name, Registration No., Address, Postcode, State, Phone, Fax, Email), Tax Details (Tax Type, Industry Type, SST No., TIN No.), Financial Details (Bank Key, Account No., Recon Account No., Payment Method dropdown, Payment Term, GR Based Invoice Verification). Buttons: Back, Request Vendor Code.

#### API 11.3.1 – Get Vendor Details by ID

**Method:** GET
**URL:** `/api/Vendor/GetVendorDetilsById?vendorId={id}`
**Authentication:** Required

**Response:**
```json
{
  "vendorId": 101,
  "vendorName": "Teguh Maju Sdn. Bhd.",
  "registrationNo": "12345",
  "address": "28 Jalan Impian Teratai",
  "postcode": "54000",
  "state": "Kuala Lumpur",
  "phoneNo": "012-6576474",
  "faxNo": "012-6576488",
  "emailAddress": "email@email.com",
  "taxType": "01 - NRIC",
  "industryType": "01500 Mixed Farming",
  "sstNo": "12345",
  "tinNo": "1122334455",
  "bankKey": "AMBANK",
  "accountNo": "1234567890",
  "reconAccountNo": "24001000",
  "paymentMethodId": null,
  "paymentTerm": "Z030 - Due in 30 Days",
  "grBasedInvoiceVerification": null
}
```

#### API 11.3.2 – Request Vendor Code from SAP

**Method:** POST
**URL:** `/api/Vendor/RequestVendorCodeFromSAP`
**Authentication:** Not required

**Request Body:**
```json
{
  "vendorId": 101,
  "paymentMethod": "T",
  "accountNo": "1234567890",
  "grInvoiceVerification": "X"
}
```

**Success Response (200):** SAP response with generated vendor code.

---

## MODULE 12 – NEWS MANAGEMENT (ADMIN PORTAL)

### Screen 12.1 – News Listing

**Description:** Admin list of news/announcements with Status filter.

**UI Elements:** Status filter (dropdown), Search button, New button, Table (No., News Title link, Status, Created By, Created Date/Time).

#### API 12.1.1 – Get All Announcements

**Method:** GET
**URL:** `/api/Announcement/GetAllAnnouncements?type=News`
**Authentication:** Required

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `type` | AnnouncementType | Yes | `News`, `Advertisement`, or `AwardResult` |

**Response:**
```json
[
  {
    "id": 1,
    "title": "News 1",
    "details": "Full news content...",
    "status": "Active",
    "type": "News",
    "createdBy": "Admin",
    "createdDate": "2024-03-02T00:00:00"
  }
]
```

---

### Screen 12.2 – News Details (Add / Edit)

**Description:** Form to create or edit a news item.

**UI Elements:** News Title (required), Details (text area), Status dropdown (Active/Inactive). Buttons: Back, Post, Delete.

#### API 12.2.1 – Create Announcement

**Method:** POST
**URL:** `/api/Announcement/CreateAnnouncement`
**Authentication:** Required

**Request Body:**
```json
{
  "title": "News Title Here",
  "details": "Full news details content...",
  "status": "Active",
  "type": "News"
}
```

**Validation Rules:**
- `title` is required
- `type` must be valid `AnnouncementType` value (`News`, `Advertisement`, `AwardResult`)

**Success Response (201):** Created announcement ID.

#### API 12.2.2 – Update Announcement

**Method:** POST
**URL:** `/api/Announcement/UpdateAnnouncement`
**Authentication:** Required

**Request Body:**
```json
{
  "id": 1,
  "title": "Updated News Title",
  "details": "Updated content...",
  "status": "Inactive",
  "type": "News"
}
```

**Success Response (204):** No content.

#### API 12.2.3 – Delete Announcement

**Method:** DELETE
**URL:** `/api/Announcement/DeleteAnnouncement?id={id}`
**Authentication:** Required

**Success Response (204):** No content.

#### API 12.2.4 – Get Announcement by ID

**Method:** GET
**URL:** `/api/Announcement/GetAnnouncementById?id={id}`
**Authentication:** Required

---

## API SUMMARY TABLE

| # | Module | Screen | Method | URL | Auth |
|---|--------|--------|--------|-----|------|
| 1 | Auth | Staff Login | POST | /api/Auth/Authenticate | No |
| 2 | Auth | Vendor Login | POST | /api/Auth/VendorAuthenticate | No |
| 3 | Vendor Reg. | Register | POST | /api/Vendor/RegisterVendor | No |
| 4 | Vendor Reg. | Profile | POST | /api/Vendor/Profile | No |
| 5 | Vendor Reg. | Members | POST | /api/Vendor/Members | No |
| 6 | Vendor Reg. | Financial | POST | /api/Vendor/Financial | No |
| 7 | Vendor Reg. | Categories | POST | /api/Vendor/Categories | No |
| 8 | Vendor Reg. | Experiences | POST | /api/Vendor/Experiences | No |
| 9 | Vendor Reg. | Declaration | POST | /api/Vendor/Declaration | No |
| 10 | Vendor Reg. | Payment | POST | /api/Vendor/Payment | No |
| 11 | Vendor Reg. | SSM Lookup | POST | /api/Vendor/SearchCompanyEntity | No |
| 12 | Vendor Reg. | Save Step Generic | POST | /api/Vendor/SaveStep | No |
| 13 | Vendor Dashboard | Dashboard | GET | /api/Vendor/GetVendorDashboard | Yes |
| 14 | Dropdown | All Screens | GET | /api/SelectList/GetSelectListAsync | No |
| 15 | Public Portal | News | GET | /api/Announcement/GetAllAnnouncements?type=News | No |
| 16 | Public Portal | Advertisement | GET | /api/Announcement/GetAllAnnouncements?type=Advertisement | No |
| 17 | Public Portal | Award Result | GET | /api/Announcement/GetAllAnnouncements?type=AwardResult | No |
| 18 | Bidding | Active Tender List | GET | /api/Bidding/GetActiveBiddingList | No |
| 19 | Bidding | Bidding Detail | GET | /api/Bidding/GetBiddingDetail | No |
| 20 | Bidding | Submit Bid | POST | /api/Bidding/SubmitBidding | No |
| 21 | Bidding | Award Detail | GET | /api/Bidding/GetBiddingAwardDetail | No |
| 22 | Bidding | Acknowledge Award | POST | /api/Bidding/SubmitBidderAcknowledgement | No |
| 23 | Tender Mgmt | Listing | GET | /api/TenderManagement/GetAllTenderApplications | No |
| 24 | Tender Mgmt | Search | GET | /api/TenderManagement/GetTenderApplicationBySearch | No |
| 25 | Tender Mgmt | Get by ID | GET | /api/TenderManagement/GetTenderApplicationById | No |
| 26 | Tender Mgmt | Create | POST | /api/TenderManagement/SaveTenderApplication | Yes |
| 27 | Tender Mgmt | Update | POST | /api/TenderManagement/UpdateTenderApplication | No |
| 28 | Tender Mgmt | Delete | POST | /api/TenderManagement/DeleteTenderApplication | No |
| 29 | Tender Mgmt | Get Reviewers | GET | /api/TenderManagement/GetReviewersorApprovers | No |
| 30 | Tender Issuance | Save Advertisement | POST | /api/TenderManagement/SaveTenderAdvertisementPage | Yes |
| 31 | Tender Issuance | Get Advertisement | GET | /api/TenderManagement/GetTenderAdvertisementPage | No |
| 32 | Tender Issuance | Search Committee | GET | /api/TenderManagement/SearchCommitteeUsers | No |
| 33 | Tender Opening | Listing | GET | /api/TenderManagement/GetTenderOpeningList | No |
| 34 | Tender Opening | Appointment Detail | GET | /api/TenderManagement/GetTenderOpeningDetail | No |
| 35 | Tender Opening | Summary Page | GET | /api/TenderManagement/GetTenderOpeningPage | No |
| 36 | Tender Opening | Verify | POST | /api/Bidding/VerifyTenderOpening | No |
| 37 | Tender Opening | Progress | GET | /api/Bidding/GetTenderOpeningProgress | No |
| 38 | Tender Evaluation | Evaluation Page | GET | /api/TenderManagement/GetTenderEvaluationPage | No |
| 39 | Tender Evaluation | Tech. Popup | GET | /api/TenderManagement/GetTechnicalEvaluationPopup | No |
| 40 | Tender Evaluation | Save Scores | POST | /api/TenderManagement/SaveTechnicalScore | Yes |
| 41 | Tender Evaluation | Recommendation | POST | /api/TenderManagement/SaveTenderRecommendation | Yes |
| 42 | Tender Award | Listing | GET | /api/TenderManagement/GetTenderAwardList | No |
| 43 | Tender Award | Award Page | GET | /api/TenderManagement/GetTenderAwardPage | No |
| 44 | Tender Award | Save Award | POST | /api/TenderManagement/SaveTenderAward | No |
| 45 | Tender Award | Save Minutes | POST | /api/TenderManagement/SaveTenderAwardMinutes | No |
| 46 | Tender Award | Delete Minutes | DELETE | /api/TenderManagement/DeleteTenderAwardMinutes | No |
| 47 | Vendor Performance | Get Page | GET | /api/TenderManagement/GetVendorPerformancePage | No |
| 48 | Vendor Performance | Save | POST | /api/TenderManagement/SaveVendorPerformance | No |
| 49 | Master Data | Get Categories | GET | /api/MasterData/GetAllCategories | No |
| 50 | Master Data | Save Hierarchy | POST | /api/MasterData/SaveHierarchy | No |
| 51 | Master Data | Get Tender Mgmt | GET | /api/MasterData/GetTenderManagement | No |
| 52 | Master Data | Save Tender Mgmt | POST | /api/MasterData/SaveTenderManagement | No |
| 53 | Master Data | Get Material Budget | GET | /api/MasterData/GetMaterialBudgetList | No |
| 54 | Master Data | Save Material Budget | POST | /api/MasterData/SaveMaterialBudget | No |
| 55 | Master Data | Update Material Budget | POST | /api/MasterData/UpdateMaterialBudget | No |
| 56 | Master Data | Delete Material Budget | DELETE | /api/MasterData/DeleteMaterialBudge/{id} | No |
| 57 | Master Data | Upload Budget Files | POST | /api/MasterData/UploadMaterialBudgetFiles | No |
| 58 | Master Data | Get Vendor Mgmt Setting | GET | /api/MasterData/GetVendorManagementSetting | No |
| 59 | Master Data | Save Vendor Mgmt Setting | POST | /api/MasterData/SaveorUpdateVendorManagementSetting | No |
| 60 | Master Data | Category Code Setting | GET | /api/MasterData/GetCategoryCodeSetting | No |
| 61 | User Mgmt | Get Users | GET | /api/User/GetAllUsers | Yes |
| 62 | User Mgmt | Search Users | POST | /api/User/GetAllUsers | Yes |
| 63 | User Mgmt | Create User | POST | /api/User/CreateUser | Yes |
| 64 | User Mgmt | Update User | POST | /api/User/UpdateUserByAdmin | Yes |
| 65 | User Mgmt | Get User by ID | GET | /api/User/GetUserById | Yes |
| 66 | User Mgmt | Get Bidder Users | GET | /api/User/GetBiddingUsersList | Yes |
| 67 | User Mgmt | Search Bidder Users | POST | /api/User/GetBiddingUsersList | Yes |
| 68 | User Mgmt | Get All Roles | GET | /api/Admin/GetAllRoles | Yes |
| 69 | User Mgmt | Save Role | POST | /api/Admin/SaveRole | Yes |
| 70 | User Mgmt | Update Role | POST | /api/Admin/UpdateRole | Yes |
| 71 | User Mgmt | Get Menu Permissions | GET | /api/Admin/GetAllMenuRolePermission | Yes |
| 72 | User Mgmt | Save Menu Permission | POST | /api/Admin/SaveMenuRolePermission | Yes |
| 73 | User Mgmt | Update Menu Permission | POST | /api/Admin/UpdateRoleMenuPermission | Yes |
| 74 | User Mgmt | Get All Menus | GET | /api/Admin/GetAllMenu | Yes |
| 75 | User Mgmt | Get Menus by Role | GET | /api/Admin/GetAllMenusByRole | Yes |
| 76 | Vendor Mgmt | Get Vendors Filtered | GET | /api/Vendor/GetVendors | Yes |
| 77 | Vendor Mgmt | Get Vendor List | GET | /api/Vendor/GetVendorList | Yes |
| 78 | Vendor Mgmt | Get Vendor Details | GET | /api/Vendor/GetVendorDetilsById | Yes |
| 79 | Vendor Mgmt | Request Vendor Code | POST | /api/Vendor/RequestVendorCodeFromSAP | No |
| 80 | News Mgmt | Get Announcements | GET | /api/Announcement/GetAllAnnouncements | Yes |
| 81 | News Mgmt | Get by ID | GET | /api/Announcement/GetAnnouncementById | Yes |
| 82 | News Mgmt | Create | POST | /api/Announcement/CreateAnnouncement | Yes |
| 83 | News Mgmt | Update | POST | /api/Announcement/UpdateAnnouncement | Yes |
| 84 | News Mgmt | Delete | DELETE | /api/Announcement/DeleteAnnouncement | Yes |
| 85 | Bidding Admin | Get Assets | GET | /api/Bidding/GetBiddingAssets | No |
| 86 | Bidding Admin | Save Asset | POST | /api/Bidding/SaveBiddingAsset | No |
| 87 | Bidding Admin | Delete Asset | DELETE | /api/Bidding/DeleteBiddingAsset | No |

---

## APIs NOT MAPPED TO SCREENS

The following APIs exist in the codebase but are not directly visible in the wireframes. They may be used internally, for future screens, or as helper endpoints:

| # | Method | URL | Notes |
|---|--------|-----|-------|
| 1 | POST | /api/Vendor/SaveStepRaw | Generic step-save with raw JSON payload — alternative to typed step endpoints |
| 2 | GET | /api/Vendor/GetCompanyTypes | Company type dropdown (used during registration) |
| 3 | GET | /api/Vendor/GetCompanyEntitiesByTypeId | Sub-dropdown based on company type |
| 4 | GET | /api/Vendor/SAPSimulator | SAP simulator test endpoint (dev/test only) |
| 5 | GET | /api/Vendor/GetIndustryTypeList | Industry type dropdown |
| 6 | POST | /api/Vendor/SaveQuestionAnswers | Save vendor questionnaire responses |
| 7 | GET | /api/Vendor/GetQuestionAnswers | Retrieve saved questionnaire answers |
| 8 | GET | /api/MasterData/GetMaterialBudgetUploads | List of uploaded material budget files |
| 9 | GET | /api/MasterData/GetCategoryListByMasterCodeId | Sub-category list by code master ID |
| 10 | GET | /api/Admin/GetRoleById | Get specific role details |
| 11 | GET | /api/Admin/GetSubMenuListByMenuId | Sub-menu items for a parent menu |
| 12 | GET | /api/Admin/GetAllPasswordPolicyDetails | Password policy configuration |
| 13 | POST | /api/Admin/UpdatePasswordPolicy | Update password policy |
| 14 | GET | /api/User/GetCurrentUser | Returns current authenticated user info from JWT claims |
| 15 | POST | /api/User/UpdateUser/{id} | Update user by ID (route-based alternative to UpdateUserByAdmin) |
| 16 | GET | /api/Announcement/GetAnnouncementsByType | Duplicate of GetAllAnnouncements |
| 17 | POST | /api/Announcement/AddAnnouncement | Duplicate of CreateAnnouncement |

---

## FRONTEND IMPLEMENTATION NOTES

### 1. Required Headers for All Authenticated Requests
```javascript
{
  'Authorization': `Bearer ${localStorage.getItem('token')}`,
  'Content-Type': 'application/json'
}
```

### 2. Handling Token Expiry
JWT tokens expire after **60 minutes**. Recommended approach:
1. Intercept all 401 responses globally (via Axios interceptor or similar)
2. Clear the stored token from localStorage/sessionStorage
3. Redirect to the login screen
4. Display: *"Session expired. Please log in again."*

### 3. Standard Error Handling Pattern
```javascript
const response = await fetch('/api/Auth/Authenticate', { ... });
const json = await response.json();

if (json.error) {
  // Display json.errors[] to the user
  showErrors(json.errors);
  return;
}

// Use json.data for the actual payload
const data = json.data;
```

### 4. Dropdown Loading — Batch Strategy
Use a single API call for all dropdowns on a screen:
```javascript
// One call instead of multiple:
GET /api/SelectList/GetSelectListAsync?inputTypes=ApplicationLevel,TenderCategory,JobCategory,Status
```

### 5. File Uploads
- Maximum file size: **100 MB**
- Use `multipart/form-data` — do NOT manually set `Content-Type`, let the browser set it with the boundary
- Uploaded files are served from: `/VendorUploads/{filename}`
- Static file URL example: `https://localhost:7225/VendorUploads/tender_document.pdf`

### 6. Key Enum Values

```
AnnouncementType       : News | Advertisement | AwardResult
VendorRegistrationStep : Profile | Members | Financial | Categories | Experiences | Declaration | Payment
CommitteeType          : opening | evaluation
```

### 7. Pagination
No server-side pagination is currently implemented. All list endpoints return full results. Implement client-side pagination/virtual scrolling where needed.

### 8. CORS
The API uses an AllowAll CORS policy in both development and production. No special frontend configuration is required.

### 9. Navigation Flow (Admin Portal)
```
Login → Dashboard
Dashboard → Tender Management → Tender Issuance
                              → Tender Opening → Verify → Progress
                              → Tender Evaluation → Score Popup → Save
                              → Tender Award → Save Award → Vendor Performance
```

### 10. Navigation Flow (Vendor Portal)
```
Public Home (News/Advertisement/Award) → Login
Login → Vendor Dashboard
      → Profile / Members / Financial / Category Code / Experiences (registration steps)
      → Bidding → Bidding Details (enter bid prices) → Submit
                → Award Details → Acknowledge
```

---

*Document generated on 2026-04-03 from analysis of wireframes (46 screens) and API codebase (27 controllers, 87 active endpoints).*
