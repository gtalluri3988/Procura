using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class designation1user11234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassingMarks",
                table: "TenderEvaluationCriteria",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenderId",
                table: "TenderEvaluationCriteria",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EvaluationCriteria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobCategoryId = table.Column<int>(type: "int", nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeightagePercent = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluationCriteria_JobCategories_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderEvaluationCriteria_TenderId",
                table: "TenderEvaluationCriteria",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_JobCategoryId",
                table: "EvaluationCriteria",
                column: "JobCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderEvaluationCriteria_TenderApplications_TenderId",
                table: "TenderEvaluationCriteria",
                column: "TenderId",
                principalTable: "TenderApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderEvaluationCriteria_TenderApplications_TenderId",
                table: "TenderEvaluationCriteria");

            migrationBuilder.DropTable(
                name: "EvaluationCriteria");

            migrationBuilder.DropIndex(
                name: "IX_TenderEvaluationCriteria_TenderId",
                table: "TenderEvaluationCriteria");

            migrationBuilder.DropColumn(
                name: "PassingMarks",
                table: "TenderEvaluationCriteria");

            migrationBuilder.DropColumn(
                name: "TenderId",
                table: "TenderEvaluationCriteria");
        }
    }
}
