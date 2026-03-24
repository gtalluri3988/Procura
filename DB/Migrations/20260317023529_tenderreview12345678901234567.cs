using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderreview12345678901234567 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenderApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderApplicationId = table.Column<int>(type: "int", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: false),
                    TenderApplicationStatusId = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_TenderApprovals_TenderApplicationStatus_TenderApplicationStatusId",
                        column: x => x.TenderApplicationStatusId,
                        principalTable: "TenderApplicationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderApprovals_TenderApplications_TenderApplicationId",
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
                    ReviewedByUserId = table.Column<int>(type: "int", nullable: false),
                    TenderApplicationStatusId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                        name: "FK_TenderReviews_TenderApplicationStatus_TenderApplicationStatusId",
                        column: x => x.TenderApplicationStatusId,
                        principalTable: "TenderApplicationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_TenderApprovals_TenderApplicationStatusId",
                table: "TenderApprovals",
                column: "TenderApplicationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderReviews_TenderApplicationId",
                table: "TenderReviews",
                column: "TenderApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderReviews_TenderApplicationStatusId",
                table: "TenderReviews",
                column: "TenderApplicationStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderApprovals");

            migrationBuilder.DropTable(
                name: "TenderReviews");
        }
    }
}
