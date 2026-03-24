using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderreview1234567890123456111234578 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddForeignKey(
                name: "FK_TenderApprovals_Users_ApprovedByUserId",
                table: "TenderApprovals",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderReviews_Users_ReviewedByUserId",
                table: "TenderReviews",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderApprovals_Users_ApprovedByUserId",
                table: "TenderApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderReviews_Users_ReviewedByUserId",
                table: "TenderReviews");

         
        }
    }
}
