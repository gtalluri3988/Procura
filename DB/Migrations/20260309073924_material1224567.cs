using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class material1224567 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_CodeHierarchies_MasterCategoryId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_MasterCategoryId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "MasterCategoryId",
                table: "VendorCategories");

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "VendorCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "VendorCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CodeHierarchyId",
                table: "VendorCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "VendorCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_ActivityId",
                table: "VendorCategories",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_CategoryId",
                table: "VendorCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_CodeHierarchyId",
                table: "VendorCategories",
                column: "CodeHierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_CodeSystemId",
                table: "VendorCategories",
                column: "CodeSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategories_SubCategoryId",
                table: "VendorCategories",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_Activities_ActivityId",
                table: "VendorCategories",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_Categories_CategoryId",
                table: "VendorCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_CodeHierarchies_CodeHierarchyId",
                table: "VendorCategories",
                column: "CodeHierarchyId",
                principalTable: "CodeHierarchies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_CodeSystems_CodeSystemId",
                table: "VendorCategories",
                column: "CodeSystemId",
                principalTable: "CodeSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCategories_SubCategories_SubCategoryId",
                table: "VendorCategories",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_Activities_ActivityId",
                table: "VendorCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_Categories_CategoryId",
                table: "VendorCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_CodeHierarchies_CodeHierarchyId",
                table: "VendorCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_CodeSystems_CodeSystemId",
                table: "VendorCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorCategories_SubCategories_SubCategoryId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_ActivityId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_CategoryId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_CodeHierarchyId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_CodeSystemId",
                table: "VendorCategories");

            migrationBuilder.DropIndex(
                name: "IX_VendorCategories_SubCategoryId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "CodeHierarchyId",
                table: "VendorCategories");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "VendorCategories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryType",
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
    }
}
