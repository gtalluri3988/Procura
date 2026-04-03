using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class designation1user1123456788 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenderAdvertisementSettingId",
                table: "TenderOpeningCommittee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenderAdvertisementSettingId",
                table: "TenderEvaluationCommittee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluationEndDate",
                table: "TenderAdvertisementSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluationStartDate",
                table: "TenderAdvertisementSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JobCategory",
                table: "TenderAdvertisementSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpeningEndDate",
                table: "TenderAdvertisementSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OpeningStartDate",
                table: "TenderAdvertisementSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PassingMarks",
                table: "TenderAdvertisementSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenderOpeningCommittee_TenderAdvertisementSettingId",
                table: "TenderOpeningCommittee",
                column: "TenderAdvertisementSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderEvaluationCommittee_TenderAdvertisementSettingId",
                table: "TenderEvaluationCommittee",
                column: "TenderAdvertisementSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderEvaluationCommittee_TenderAdvertisementSettings_TenderAdvertisementSettingId",
                table: "TenderEvaluationCommittee",
                column: "TenderAdvertisementSettingId",
                principalTable: "TenderAdvertisementSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderOpeningCommittee_TenderAdvertisementSettings_TenderAdvertisementSettingId",
                table: "TenderOpeningCommittee",
                column: "TenderAdvertisementSettingId",
                principalTable: "TenderAdvertisementSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderEvaluationCommittee_TenderAdvertisementSettings_TenderAdvertisementSettingId",
                table: "TenderEvaluationCommittee");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderOpeningCommittee_TenderAdvertisementSettings_TenderAdvertisementSettingId",
                table: "TenderOpeningCommittee");

            migrationBuilder.DropIndex(
                name: "IX_TenderOpeningCommittee_TenderAdvertisementSettingId",
                table: "TenderOpeningCommittee");

            migrationBuilder.DropIndex(
                name: "IX_TenderEvaluationCommittee_TenderAdvertisementSettingId",
                table: "TenderEvaluationCommittee");

            migrationBuilder.DropColumn(
                name: "TenderAdvertisementSettingId",
                table: "TenderOpeningCommittee");

            migrationBuilder.DropColumn(
                name: "TenderAdvertisementSettingId",
                table: "TenderEvaluationCommittee");

            migrationBuilder.DropColumn(
                name: "EvaluationEndDate",
                table: "TenderAdvertisementSettings");

            migrationBuilder.DropColumn(
                name: "EvaluationStartDate",
                table: "TenderAdvertisementSettings");

            migrationBuilder.DropColumn(
                name: "JobCategory",
                table: "TenderAdvertisementSettings");

            migrationBuilder.DropColumn(
                name: "OpeningEndDate",
                table: "TenderAdvertisementSettings");

            migrationBuilder.DropColumn(
                name: "OpeningStartDate",
                table: "TenderAdvertisementSettings");

            migrationBuilder.DropColumn(
                name: "PassingMarks",
                table: "TenderAdvertisementSettings");
        }
    }
}
