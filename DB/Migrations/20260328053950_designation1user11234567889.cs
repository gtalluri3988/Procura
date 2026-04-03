using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class designation1user11234567889 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobCategory",
                table: "TenderAdvertisementSettings");

            migrationBuilder.DropColumn(
                name: "PassingMarks",
                table: "TenderAdvertisementSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobCategory",
                table: "TenderAdvertisementSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassingMarks",
                table: "TenderAdvertisementSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
