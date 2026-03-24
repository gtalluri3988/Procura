using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class designation1user11234567 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderEvaluationSpecification_TenderEvaluationCriteria_CriteriaId",
                table: "TenderEvaluationSpecification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderEvaluationSpecification",
                table: "TenderEvaluationSpecification");

            migrationBuilder.RenameTable(
                name: "TenderEvaluationSpecification",
                newName: "TenderEvaluationSpecifications");

            migrationBuilder.RenameIndex(
                name: "IX_TenderEvaluationSpecification_CriteriaId",
                table: "TenderEvaluationSpecifications",
                newName: "IX_TenderEvaluationSpecifications_CriteriaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderEvaluationSpecifications",
                table: "TenderEvaluationSpecifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TenderIssuenceApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    ReviewLevel = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    TenderStatusId = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderIssuenceApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderIssuenceApprovals_TenderApplicationStatus_TenderStatusId",
                        column: x => x.TenderStatusId,
                        principalTable: "TenderApplicationStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenderIssuenceApprovals_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderIssuenceApprovals_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderIssuenceApprovals_ReviewerId",
                table: "TenderIssuenceApprovals",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderIssuenceApprovals_TenderId",
                table: "TenderIssuenceApprovals",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderIssuenceApprovals_TenderStatusId",
                table: "TenderIssuenceApprovals",
                column: "TenderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderEvaluationSpecifications_TenderEvaluationCriteria_CriteriaId",
                table: "TenderEvaluationSpecifications",
                column: "CriteriaId",
                principalTable: "TenderEvaluationCriteria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderEvaluationSpecifications_TenderEvaluationCriteria_CriteriaId",
                table: "TenderEvaluationSpecifications");

            migrationBuilder.DropTable(
                name: "TenderIssuenceApprovals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderEvaluationSpecifications",
                table: "TenderEvaluationSpecifications");

            migrationBuilder.RenameTable(
                name: "TenderEvaluationSpecifications",
                newName: "TenderEvaluationSpecification");

            migrationBuilder.RenameIndex(
                name: "IX_TenderEvaluationSpecifications_CriteriaId",
                table: "TenderEvaluationSpecification",
                newName: "IX_TenderEvaluationSpecification_CriteriaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderEvaluationSpecification",
                table: "TenderEvaluationSpecification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderEvaluationSpecification_TenderEvaluationCriteria_CriteriaId",
                table: "TenderEvaluationSpecification",
                column: "CriteriaId",
                principalTable: "TenderEvaluationCriteria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
