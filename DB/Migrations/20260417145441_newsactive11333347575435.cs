using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class newsactive11333347575435 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes",
                column: "TenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes",
                column: "TenderId",
                unique: true);
        }
    }
}
