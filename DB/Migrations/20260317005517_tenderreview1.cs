using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderreview1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderReviews_TenderApplications_TenderApplicationId",
                table: "TenderReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderReviews",
                table: "TenderReviews");

            migrationBuilder.RenameTable(
                name: "TenderReviews",
                newName: "TenderReview");

            migrationBuilder.RenameIndex(
                name: "IX_TenderReviews_TenderApplicationId",
                table: "TenderReview",
                newName: "IX_TenderReview_TenderApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderReview",
                table: "TenderReview",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderReview_TenderApplications_TenderApplicationId",
                table: "TenderReview",
                column: "TenderApplicationId",
                principalTable: "TenderApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderReview_TenderApplications_TenderApplicationId",
                table: "TenderReview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenderReview",
                table: "TenderReview");

            migrationBuilder.RenameTable(
                name: "TenderReview",
                newName: "TenderReviews");

            migrationBuilder.RenameIndex(
                name: "IX_TenderReview_TenderApplicationId",
                table: "TenderReviews",
                newName: "IX_TenderReviews_TenderApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenderReviews",
                table: "TenderReviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderReviews_TenderApplications_TenderApplicationId",
                table: "TenderReviews",
                column: "TenderApplicationId",
                principalTable: "TenderApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
