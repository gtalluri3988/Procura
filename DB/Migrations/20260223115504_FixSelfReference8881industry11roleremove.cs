using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference8881industry11roleremove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Roles_RoleId1",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_RoleId1",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                table: "Vendors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleId1",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_RoleId1",
                table: "Vendors",
                column: "RoleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Roles_RoleId1",
                table: "Vendors",
                column: "RoleId1",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
