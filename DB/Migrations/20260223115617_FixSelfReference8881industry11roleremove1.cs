using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference8881industry11roleremove1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_RoleId",
                table: "Vendors",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Roles_RoleId",
                table: "Vendors",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Roles_RoleId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_RoleId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Vendors");
        }
    }
}
