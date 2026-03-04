using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference8881industry1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndustryType",
                table: "Vendors");

            migrationBuilder.AddColumn<int>(
                name: "IndustryTypeId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IndustryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_IndustryTypeId",
                table: "Vendors",
                column: "IndustryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_IndustryTypes_IndustryTypeId",
                table: "Vendors",
                column: "IndustryTypeId",
                principalTable: "IndustryTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_IndustryTypes_IndustryTypeId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "IndustryTypes");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_IndustryTypeId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "IndustryTypeId",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "IndustryType",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
