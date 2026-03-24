using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderreview123456789012345611123457 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
