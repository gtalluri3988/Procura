using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tendertables11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenderCategoryCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    CodeMasterId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    ActivityId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderCategoryCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderCategoryCodes_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderCategoryCodes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderCategoryCodes_CodeMasters_CodeMasterId",
                        column: x => x.CodeMasterId,
                        principalTable: "CodeMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderCategoryCodes_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderCategoryCodes_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderSiteVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    SiteVisitRequired = table.Column<bool>(type: "bit", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Attendance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderSiteVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderSiteVisits_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_ActivityId",
                table: "TenderCategoryCodes",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_CategoryId",
                table: "TenderCategoryCodes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_CodeMasterId",
                table: "TenderCategoryCodes",
                column: "CodeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_SubCategoryId",
                table: "TenderCategoryCodes",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderSiteVisits_TenderId",
                table: "TenderSiteVisits",
                column: "TenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderCategoryCodes");

            migrationBuilder.DropTable(
                name: "TenderSiteVisits");
        }
    }
}
