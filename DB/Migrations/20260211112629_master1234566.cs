using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class master1234566 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "PaymentRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategorySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditCategoryCodeAfterMonths = table.Column<int>(type: "int", nullable: false),
                    EditCategoryCodeLimit = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RocNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfIncorporation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficePhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndustryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessCoverageArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PicMobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PicEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Form24AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorBanks_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityOrDivision = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCategories_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorCreditFacilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCreditFacilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCreditFacilities_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorDeclarations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    EsqQuestionnaireAccepted = table.Column<bool>(type: "bit", nullable: false),
                    ConfidentialityAgreementAccepted = table.Column<bool>(type: "bit", nullable: false),
                    PoTermsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    CodeOfConductAccepted = table.Column<bool>(type: "bit", nullable: false),
                    PdpAAccepted = table.Column<bool>(type: "bit", nullable: false),
                    EnvironmentalPolicyAccepted = table.Column<bool>(type: "bit", nullable: false),
                    NoGiftPolicyAccepted = table.Column<bool>(type: "bit", nullable: false),
                    IntegrityDeclarationAccepted = table.Column<bool>(type: "bit", nullable: false),
                    FinalDeclarationAccepted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorDeclarations_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorExperiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletionYear = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorExperiences_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorFinancials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    PaidUpCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AuthorizedCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkingCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LiquidCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssetBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BumiputeraEquityAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BumiputeraEquityPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonBumiputeraEquityAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonBumiputeraEquityPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RollingCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOverdraft = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OthersCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorFinancials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorFinancials_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorManpowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    NoOfBumiputera = table.Column<int>(type: "int", nullable: false),
                    NoOfNonBumiputera = table.Column<int>(type: "int", nullable: false),
                    BumiputeraPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonBumiputeraPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorManpowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorManpowers_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    MemberType = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBumiputera = table.Column<bool>(type: "bit", nullable: false),
                    SharePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShareAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsFpmSbStaff = table.Column<bool>(type: "bit", nullable: false),
                    IsFamilyOfFpmSbStaff = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorMembers_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorTaxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    TaxType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SstNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SstRegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TinNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MsicCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorTaxes_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorBanks_VendorId",
                table: "VendorBanks",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_VendorId",
                table: "VendorCategories",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCreditFacilities_VendorId",
                table: "VendorCreditFacilities",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorDeclarations_VendorId",
                table: "VendorDeclarations",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorExperiences_VendorId",
                table: "VendorExperiences",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorFinancials_VendorId",
                table: "VendorFinancials",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorManpowers_VendorId",
                table: "VendorManpowers",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorMembers_VendorId",
                table: "VendorMembers",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxes_VendorId",
                table: "VendorTaxes",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequest_Vendors_VendorId",
                table: "PaymentRequest",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequest_Vendors_VendorId",
                table: "PaymentRequest");

            migrationBuilder.DropTable(
                name: "CategorySettings");

            migrationBuilder.DropTable(
                name: "VendorBanks");

            migrationBuilder.DropTable(
                name: "VendorCategories");

            migrationBuilder.DropTable(
                name: "VendorCreditFacilities");

            migrationBuilder.DropTable(
                name: "VendorDeclarations");

            migrationBuilder.DropTable(
                name: "VendorExperiences");

            migrationBuilder.DropTable(
                name: "VendorFinancials");

            migrationBuilder.DropTable(
                name: "VendorManpowers");

            migrationBuilder.DropTable(
                name: "VendorMembers");

            migrationBuilder.DropTable(
                name: "VendorTaxes");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "PaymentRequest");
        }
    }
}
