using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class designation1user112345 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specification",
                table: "TenderEvaluationCriteria");

            migrationBuilder.DropColumn(
                name: "WeightagePercent",
                table: "TenderEvaluationCriteria");

            migrationBuilder.CreateTable(
                name: "TenderEvaluationSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriteriaId = table.Column<int>(type: "int", nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weightage = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderEvaluationSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderEvaluationSpecification_TenderEvaluationCriteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "TenderEvaluationCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderEvaluationSpecification_CriteriaId",
                table: "TenderEvaluationSpecification",
                column: "CriteriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderEvaluationSpecification");

            migrationBuilder.AddColumn<string>(
                name: "Specification",
                table: "TenderEvaluationCriteria",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WeightagePercent",
                table: "TenderEvaluationCriteria",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
