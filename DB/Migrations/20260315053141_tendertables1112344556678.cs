using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tendertables1112344556678 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TenderApplications_ApplicationLevelId",
                table: "TenderApplications",
                column: "ApplicationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApplications_ApplicationLevels_ApplicationLevelId",
                table: "TenderApplications",
                column: "ApplicationLevelId",
                principalTable: "ApplicationLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderApplications_ApplicationLevels_ApplicationLevelId",
                table: "TenderApplications");

            migrationBuilder.DropIndex(
                name: "IX_TenderApplications_ApplicationLevelId",
                table: "TenderApplications");
        }
    }
}
