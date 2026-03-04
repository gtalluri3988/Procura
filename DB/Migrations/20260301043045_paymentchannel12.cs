using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class paymentchannel12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "VendorPaymentStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VendorPaymentStatus_PaymentTypeId",
                table: "VendorPaymentStatus",
                column: "PaymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorPaymentStatus_PaymentType_PaymentTypeId",
                table: "VendorPaymentStatus",
                column: "PaymentTypeId",
                principalTable: "PaymentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorPaymentStatus_PaymentType_PaymentTypeId",
                table: "VendorPaymentStatus");

            migrationBuilder.DropIndex(
                name: "IX_VendorPaymentStatus_PaymentTypeId",
                table: "VendorPaymentStatus");

            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "VendorPaymentStatus");
        }
    }
}
