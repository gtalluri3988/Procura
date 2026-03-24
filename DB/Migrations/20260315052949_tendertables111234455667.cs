using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tendertables111234455667 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationLevel",
                table: "TenderApplications");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationLevelId",
                table: "TenderApplications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationLevelId",
                table: "TenderApplications");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationLevel",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
