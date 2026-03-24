using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderreview123456789012345611123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "TenderApprovals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewedByUserId",
                table: "TenderReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "TenderApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenderReviews_ReviewedByUserId",
                table: "TenderReviews",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderApprovals_ApprovedByUserId",
                table: "TenderApprovals",
                column: "ApprovedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApprovals_Users_ApprovedByUserId",
                table: "TenderApprovals",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderReviews_Users_ReviewedByUserId",
                table: "TenderReviews",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
