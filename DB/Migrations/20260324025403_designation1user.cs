using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class designation1user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "DesignationId",
                table: "Users",
                type: "int",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DesignationId",
                table: "Users",
                column: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Designations_DesignationId",
                table: "Users",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Designations_DesignationId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DesignationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Users",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
