using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class material122456788 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_CodeSystems_CodeSystemId",
                table: "VendorCategories");

            migrationBuilder.RenameColumn(
                name: "CodeSystemId",
                table: "VendorCategories",
                newName: "CodeMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_VendorCategories_CodeSystemId",
                table: "VendorCategories",
                newName: "IX_VendorCategories_CodeMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_CodeMasters_CodeMasterId",
                table: "VendorCategories",
                column: "CodeMasterId",
                principalTable: "CodeMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_CodeMasters_CodeMasterId",
                table: "VendorCategories");

            migrationBuilder.RenameColumn(
                name: "CodeMasterId",
                table: "VendorCategories",
                newName: "CodeSystemId");

            migrationBuilder.RenameIndex(
                name: "IX_VendorCategories_CodeMasterId",
                table: "VendorCategories",
                newName: "IX_VendorCategories_CodeSystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_CodeSystems_CodeSystemId",
                table: "VendorCategories",
                column: "CodeSystemId",
                principalTable: "CodeSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
