using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tendertables1112 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenderSiteVisits_TenderId",
                table: "TenderSiteVisits");

            migrationBuilder.DropIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes");

            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "JobCategory",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitAttendance",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitDate",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitForm",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitRemarks",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitRequired",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitTime",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "SiteVisitVenue",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "TenderCategory",
                table: "TenderApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "MinCapitalPercent",
                table: "TenderApplications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinCapitalAmount",
                table: "TenderApplications",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedPrices",
                table: "TenderApplications",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositAmount",
                table: "TenderApplications",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationLevel",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "JobCategoryId",
                table: "TenderApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenderCategoryId",
                table: "TenderApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenderSiteVisits_TenderId",
                table: "TenderSiteVisits",
                column: "TenderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes",
                column: "TenderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenderApplications_JobCategoryId",
                table: "TenderApplications",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderApplications_TenderCategoryId",
                table: "TenderApplications",
                column: "TenderCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApplications_JobCategories_JobCategoryId",
                table: "TenderApplications",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApplications_TenderCategories_TenderCategoryId",
                table: "TenderApplications",
                column: "TenderCategoryId",
                principalTable: "TenderCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderApplications_JobCategories_JobCategoryId",
                table: "TenderApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderApplications_TenderCategories_TenderCategoryId",
                table: "TenderApplications");

            migrationBuilder.DropIndex(
                name: "IX_TenderSiteVisits_TenderId",
                table: "TenderSiteVisits");

            migrationBuilder.DropIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes");

            migrationBuilder.DropIndex(
                name: "IX_TenderApplications_JobCategoryId",
                table: "TenderApplications");

            migrationBuilder.DropIndex(
                name: "IX_TenderApplications_TenderCategoryId",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "JobCategoryId",
                table: "TenderApplications");

            migrationBuilder.DropColumn(
                name: "TenderCategoryId",
                table: "TenderApplications");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MinCapitalPercent",
                table: "TenderApplications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinCapitalAmount",
                table: "TenderApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedPrices",
                table: "TenderApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DepositAmount",
                table: "TenderApplications",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationLevel",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryCode",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobCategory",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteVisitAttendance",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SiteVisitDate",
                table: "TenderApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteVisitForm",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteVisitRemarks",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SiteVisitRequired",
                table: "TenderApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SiteVisitTime",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteVisitVenue",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenderCategory",
                table: "TenderApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TenderSiteVisits_TenderId",
                table: "TenderSiteVisits",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderCategoryCodes_TenderId",
                table: "TenderCategoryCodes",
                column: "TenderId");
        }
    }
}
