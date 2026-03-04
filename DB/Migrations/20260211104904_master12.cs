using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class master12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TenderEvaluationCriteria_JobCategoryId",
                table: "TenderEvaluationCriteria",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderDocuments_JobCategoryId",
                table: "TenderDocuments",
                column: "JobCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderDocuments_JobCategories_JobCategoryId",
                table: "TenderDocuments",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderEvaluationCriteria_JobCategories_JobCategoryId",
                table: "TenderEvaluationCriteria",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderDocuments_JobCategories_JobCategoryId",
                table: "TenderDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderEvaluationCriteria_JobCategories_JobCategoryId",
                table: "TenderEvaluationCriteria");

            migrationBuilder.DropIndex(
                name: "IX_TenderEvaluationCriteria_JobCategoryId",
                table: "TenderEvaluationCriteria");

            migrationBuilder.DropIndex(
                name: "IX_TenderDocuments_JobCategoryId",
                table: "TenderDocuments");
        }
    }
}
