using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference888112 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteLevel",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "SiteOffice",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "SiteLevelId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteLevelId",
                table: "Users",
                column: "SiteLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteOffice",
                table: "Users",
                column: "SiteOffice");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SiteLevel_SiteLevelId",
                table: "Users",
                column: "SiteLevelId",
                principalTable: "SiteLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_State_SiteOffice",
                table: "Users",
                column: "SiteOffice",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SiteLevel_SiteLevelId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_State_SiteOffice",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SiteLevelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SiteOffice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SiteLevelId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "SiteOffice",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SiteLevel",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
