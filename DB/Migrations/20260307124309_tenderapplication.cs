using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderapplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenderApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenderCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedJobStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepositRequired = table.Column<bool>(type: "bit", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedPrices = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinCapitalPercent = table.Column<int>(type: "int", nullable: false),
                    MinCapitalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteVisitRequired = table.Column<bool>(type: "bit", nullable: false),
                    SiteVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SiteVisitTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteVisitVenue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteVisitAttendance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteVisitRemarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteVisitForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenderApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    PicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderApprovals_TenderApplications_TenderApplicationId",
                        column: x => x.TenderApplicationId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderJobScopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderApplicationId = table.Column<int>(type: "int", nullable: false),
                    IOWBS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialGroupDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderJobScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderJobScopes_TenderApplications_TenderApplicationId",
                        column: x => x.TenderApplicationId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderRequiredDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Submission = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderRequiredDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderRequiredDocuments_TenderApplications_TenderApplicationId",
                        column: x => x.TenderApplicationId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderApplicationId = table.Column<int>(type: "int", nullable: false),
                    ReviewLevel = table.Column<int>(type: "int", nullable: false),
                    PicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderReviews_TenderApplications_TenderApplicationId",
                        column: x => x.TenderApplicationId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderApprovals_TenderApplicationId",
                table: "TenderApprovals",
                column: "TenderApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderJobScopes_TenderApplicationId",
                table: "TenderJobScopes",
                column: "TenderApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderRequiredDocuments_TenderApplicationId",
                table: "TenderRequiredDocuments",
                column: "TenderApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderReviews_TenderApplicationId",
                table: "TenderReviews",
                column: "TenderApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderApprovals");

            migrationBuilder.DropTable(
                name: "TenderJobScopes");

            migrationBuilder.DropTable(
                name: "TenderRequiredDocuments");

            migrationBuilder.DropTable(
                name: "TenderReviews");

            migrationBuilder.DropTable(
                name: "TenderApplications");
        }
    }
}
