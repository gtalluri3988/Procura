using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class master1234566778900 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityOrDivision",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "VendorCategories");

            migrationBuilder.AddColumn<int>(
                name: "CodeSystemId",
                table: "VendorCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MasterCategoryId",
                table: "VendorCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_MasterCategoryId",
                table: "VendorCategories",
                column: "MasterCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_CodeHierarchies_MasterCategoryId",
                table: "VendorCategories",
                column: "MasterCategoryId",
                principalTable: "CodeHierarchies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_CodeHierarchies_MasterCategoryId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_MasterCategoryId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "CodeSystemId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "MasterCategoryId",
                table: "VendorCategories");

            migrationBuilder.AddColumn<string>(
                name: "ActivityOrDivision",
                table: "VendorCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryCode",
                table: "VendorCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "VendorCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
