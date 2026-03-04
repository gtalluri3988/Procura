using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference888112334455 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyEntityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEntityTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyEntityTypes_companyCategories_CompanyCategoryId",
                        column: x => x.CompanyCategoryId,
                        principalTable: "companyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEntityTypes_CompanyCategoryId",
                table: "CompanyEntityTypes",
                column: "CompanyCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyEntityTypes");
        }
    }
}
