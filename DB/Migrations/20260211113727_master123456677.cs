using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class master123456677 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VendorManpowers_VendorId",
                table: "VendorManpowers");

            migrationBuilder.DropIndex(
                name: "IX_VendorFinancials_VendorId",
                table: "VendorFinancials");

            migrationBuilder.DropIndex(
                name: "IX_VendorDeclarations_VendorId",
                table: "VendorDeclarations");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest");

            migrationBuilder.CreateIndex(
                name: "IX_VendorManpowers_VendorId",
                table: "VendorManpowers",
                column: "VendorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorFinancials_VendorId",
                table: "VendorFinancials",
                column: "VendorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorDeclarations_VendorId",
                table: "VendorDeclarations",
                column: "VendorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest",
                column: "VendorId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VendorManpowers_VendorId",
                table: "VendorManpowers");

            migrationBuilder.DropIndex(
                name: "IX_VendorFinancials_VendorId",
                table: "VendorFinancials");

            migrationBuilder.DropIndex(
                name: "IX_VendorDeclarations_VendorId",
                table: "VendorDeclarations");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest");

            migrationBuilder.CreateIndex(
                name: "IX_VendorManpowers_VendorId",
                table: "VendorManpowers",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorFinancials_VendorId",
                table: "VendorFinancials",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorDeclarations_VendorId",
                table: "VendorDeclarations",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest",
                column: "VendorId");
        }
    }
}
