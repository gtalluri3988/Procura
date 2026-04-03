using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class TenderEvaluationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenderRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    RecommendedVendorId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderRecommendations_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderRecommendations_Vendors_RecommendedVendorId",
                        column: x => x.RecommendedVendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenderTechnicalEvaluationResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    PassingMarks = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ranking = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderTechnicalEvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderTechnicalEvaluationResults_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderTechnicalEvaluationResults_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderTechnicalEvaluationScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderTechnicalEvaluationScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderTechnicalEvaluationScores_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderTechnicalEvaluationScores_TenderEvaluationSpecifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalTable: "TenderEvaluationSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TenderTechnicalEvaluationScores_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderVendorSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    OfferedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TenderOpeningStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderVendorSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderVendorSubmissions_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderVendorSubmissions_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderRecommendations_RecommendedVendorId",
                table: "TenderRecommendations",
                column: "RecommendedVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderRecommendations_TenderId",
                table: "TenderRecommendations",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTechnicalEvaluationResults_TenderId",
                table: "TenderTechnicalEvaluationResults",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTechnicalEvaluationResults_VendorId",
                table: "TenderTechnicalEvaluationResults",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTechnicalEvaluationScores_SpecificationId",
                table: "TenderTechnicalEvaluationScores",
                column: "SpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTechnicalEvaluationScores_TenderId",
                table: "TenderTechnicalEvaluationScores",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderTechnicalEvaluationScores_VendorId",
                table: "TenderTechnicalEvaluationScores",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderVendorSubmissions_TenderId",
                table: "TenderVendorSubmissions",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderVendorSubmissions_VendorId",
                table: "TenderVendorSubmissions",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderRecommendations");

            migrationBuilder.DropTable(
                name: "TenderTechnicalEvaluationResults");

            migrationBuilder.DropTable(
                name: "TenderTechnicalEvaluationScores");

            migrationBuilder.DropTable(
                name: "TenderVendorSubmissions");
        }
    }
}
