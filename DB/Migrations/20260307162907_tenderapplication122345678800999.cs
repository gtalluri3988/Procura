using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderapplication122345678800999 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TenderSettings");

            migrationBuilder.DropColumn(
                name: "MinCapitalRequiredPercent",
                table: "TenderSettings");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "TenderSettings",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TenderSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinCapitalRequired",
                table: "TenderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TenderSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "TenderSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryCode",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ActivityCode",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TenderSettings");

            migrationBuilder.DropColumn(
                name: "MinCapitalRequired",
                table: "TenderSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TenderSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "TenderSettings");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "TenderSettings",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TenderSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCapitalRequiredPercent",
                table: "TenderSettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "SubCategoryCode",
                table: "SubCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActivityCode",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
