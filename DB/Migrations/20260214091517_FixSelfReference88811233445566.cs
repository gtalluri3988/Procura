using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference88811233445566 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CompanyEntityTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "companyCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "companyCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "companyCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "Vendors");

            migrationBuilder.AddColumn<int>(
                name: "CompanyEntityTypeId",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CompanyEntityTypeId",
                table: "Vendors",
                column: "CompanyEntityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_CompanyEntityTypes_CompanyEntityTypeId",
                table: "Vendors",
                column: "CompanyEntityTypeId",
                principalTable: "CompanyEntityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_CompanyEntityTypes_CompanyEntityTypeId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CompanyEntityTypeId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CompanyEntityTypeId",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "companyCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ROB / ROC Vendor" },
                    { 2, "Non-ROB / ROC Vendor" },
                    { 3, "Foreign Vendor" }
                });

            migrationBuilder.InsertData(
                table: "CompanyEntityTypes",
                columns: new[] { "Id", "CompanyCategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Sendirian Berhad" },
                    { 2, 1, "Berhad" },
                    { 3, 1, "Partnership" },
                    { 4, 1, "Sole Proprietorship" },
                    { 5, 2, "Cooperative Organization" },
                    { 6, 2, "Trading License (Sabah/Sarawak)" },
                    { 7, 2, "Legal Firm" },
                    { 8, 2, "Limited Liability Partnership" },
                    { 9, 3, "Business entity outside of Malaysia" }
                });
        }
    }
}
