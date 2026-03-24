using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderapplication12234567880099901 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenderCreatedBy",
                table: "TenderApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenderApplications_TenderCreatedBy",
                table: "TenderApplications",
                column: "TenderCreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApplications_Users_TenderCreatedBy",
                table: "TenderApplications",
                column: "TenderCreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderApplications_Users_TenderCreatedBy",
                table: "TenderApplications");

            migrationBuilder.DropIndex(
                name: "IX_TenderApplications_TenderCreatedBy",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "TenderCreatedBy",
                table: "TenderApplications");
        }
    }
}
