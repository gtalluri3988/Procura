using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class master12345667789 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "VendorFinancials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LatestBankStatementPath",
                table: "VendorFinancials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxId",
                table: "VendorFinancials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorFinancialId",
                table: "VendorCreditFacilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorDirector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBumiputera = table.Column<bool>(type: "bit", nullable: false),
                    FpmsbRelationshipStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorDirector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorDirector_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorShareholder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBumiputera = table.Column<bool>(type: "bit", nullable: false),
                    SharePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShareAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FpmsbRelationshipStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeldaSettlerStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorShareholder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorShareholder_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorStaffDeclaration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFamilyMember = table.Column<bool>(type: "bit", nullable: false),
                    IsShareholder = table.Column<bool>(type: "bit", nullable: false),
                    IsBoardOfDirector = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorStaffDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorStaffDeclaration_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorFinancials_BankId",
                table: "VendorFinancials",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorFinancials_TaxId",
                table: "VendorFinancials",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCreditFacilities_VendorFinancialId",
                table: "VendorCreditFacilities",
                column: "VendorFinancialId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorDirector_VendorId",
                table: "VendorDirector",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorShareholder_VendorId",
                table: "VendorShareholder",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorStaffDeclaration_VendorId",
                table: "VendorStaffDeclaration",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCreditFacilities_VendorFinancials_VendorFinancialId",
                table: "VendorCreditFacilities",
                column: "VendorFinancialId",
                principalTable: "VendorFinancials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorFinancials_VendorBanks_BankId",
                table: "VendorFinancials",
                column: "BankId",
                principalTable: "VendorBanks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorFinancials_VendorTaxes_TaxId",
                table: "VendorFinancials",
                column: "TaxId",
                principalTable: "VendorTaxes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCreditFacilities_VendorFinancials_VendorFinancialId",
                table: "VendorCreditFacilities");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorFinancials_VendorBanks_BankId",
                table: "VendorFinancials");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorFinancials_VendorTaxes_TaxId",
                table: "VendorFinancials");

            migrationBuilder.DropTable(
                name: "VendorDirector");

            migrationBuilder.DropTable(
                name: "VendorShareholder");

            migrationBuilder.DropTable(
                name: "VendorStaffDeclaration");

            migrationBuilder.DropIndex(
                name: "IX_VendorFinancials_BankId",
                table: "VendorFinancials");

            migrationBuilder.DropIndex(
                name: "IX_VendorFinancials_TaxId",
                table: "VendorFinancials");

            migrationBuilder.DropIndex(
                name: "IX_VendorCreditFacilities_VendorFinancialId",
                table: "VendorCreditFacilities");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "VendorFinancials");

            migrationBuilder.DropColumn(
                name: "LatestBankStatementPath",
                table: "VendorFinancials");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "VendorFinancials");

            migrationBuilder.DropColumn(
                name: "VendorFinancialId",
                table: "VendorCreditFacilities");
        }
    }
}
