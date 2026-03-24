using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tendertables11123445566789 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenderApplicationStatusId",
                table: "TenderApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenderApplications_TenderApplicationStatusId",
                table: "TenderApplications",
                column: "TenderApplicationStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApplications_TenderApplicationStatus_TenderApplicationStatusId",
                table: "TenderApplications",
                column: "TenderApplicationStatusId",
                principalTable: "TenderApplicationStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderApplications_TenderApplicationStatus_TenderApplicationStatusId",
                table: "TenderApplications");

            migrationBuilder.DropIndex(
                name: "IX_TenderApplications_TenderApplicationStatusId",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "TenderApplicationStatusId",
                table: "TenderApplications");
        }
    }
}
