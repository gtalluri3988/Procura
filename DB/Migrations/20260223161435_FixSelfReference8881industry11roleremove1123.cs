using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference8881industry11roleremove1123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Vendors");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_StateId",
                table: "Vendors",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_State_StateId",
                table: "Vendors",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_State_StateId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_StateId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
