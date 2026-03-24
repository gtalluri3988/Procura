using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class material122456788998901457890 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CodeMasterId",
                table: "VendorCategoryCertificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategoryCertificates_CodeMasterId",
                table: "VendorCategoryCertificates",
                column: "CodeMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategoryCertificates_CodeMasters_CodeMasterId",
                table: "VendorCategoryCertificates",
                column: "CodeMasterId",
                principalTable: "CodeMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategoryCertificates_CodeMasters_CodeMasterId",
                table: "VendorCategoryCertificates");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategoryCertificates_CodeMasterId",
                table: "VendorCategoryCertificates");

            migrationBuilder.DropColumn(
                name: "CodeMasterId",
                table: "VendorCategoryCertificates");
        }
    }
}
