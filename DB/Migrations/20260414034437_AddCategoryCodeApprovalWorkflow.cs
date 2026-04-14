using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryCodeApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryCodeApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCodeApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovals_Users_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovals_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryCodeApprovalItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryCodeApprovalId = table.Column<int>(type: "int", nullable: false),
                    CodeMasterId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    ActivityId = table.Column<int>(type: "int", nullable: true),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificateEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCodeApprovalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovalItems_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovalItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovalItems_CategoryCodeApprovals_CategoryCodeApprovalId",
                        column: x => x.CategoryCodeApprovalId,
                        principalTable: "CategoryCodeApprovals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovalItems_CodeMasters_CodeMasterId",
                        column: x => x.CodeMasterId,
                        principalTable: "CodeMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryCodeApprovalItems_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovalItems_ActivityId",
                table: "CategoryCodeApprovalItems",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovalItems_CategoryCodeApprovalId",
                table: "CategoryCodeApprovalItems",
                column: "CategoryCodeApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovalItems_CategoryId",
                table: "CategoryCodeApprovalItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovalItems_CodeMasterId",
                table: "CategoryCodeApprovalItems",
                column: "CodeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovalItems_SubCategoryId",
                table: "CategoryCodeApprovalItems",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovals_ReviewedBy",
                table: "CategoryCodeApprovals",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCodeApprovals_VendorId",
                table: "CategoryCodeApprovals",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCodeApprovalItems");

            migrationBuilder.DropTable(
                name: "CategoryCodeApprovals");
        }
    }
}
