using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class kycrelatedchanges123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "VendorBanks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "VendorBanks",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "VendorBanks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranchAddress",
                table: "VendorBanks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FixedDeposit",
                table: "VendorBanks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorCreditFacilities_VendorId",
                table: "VendorCreditFacilities",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorCreditFacilities_Vendors_VendorId",
                table: "VendorCreditFacilities",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCreditFacilities_Vendors_VendorId",
                table: "VendorCreditFacilities");

            migrationBuilder.DropIndex(
                name: "IX_VendorCreditFacilities_VendorId",
                table: "VendorCreditFacilities");

            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "VendorBanks");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "VendorBanks");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "VendorBanks");

            migrationBuilder.DropColumn(
                name: "BankBranchAddress",
                table: "VendorBanks");

            migrationBuilder.DropColumn(
                name: "FixedDeposit",
                table: "VendorBanks");
        }
    }
}
