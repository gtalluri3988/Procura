using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class includefilenames112222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostCentreReference",
                table: "MaterialBudgets");

            migrationBuilder.RenameColumn(
                name: "WBSReference",
                table: "MaterialBudgets",
                newName: "RujukanValue");

            migrationBuilder.RenameColumn(
                name: "IOReference",
                table: "MaterialBudgets",
                newName: "RujukanType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RujukanValue",
                table: "MaterialBudgets",
                newName: "WBSReference");

            migrationBuilder.RenameColumn(
                name: "RujukanType",
                table: "MaterialBudgets",
                newName: "IOReference");

            migrationBuilder.AddColumn<string>(
                name: "CostCentreReference",
                table: "MaterialBudgets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
