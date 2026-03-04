using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class paymentchannel123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorPaymentStatus_PaymentType_PaymentTypeId",
                table: "VendorPaymentStatus");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTypeId",
                table: "VendorPaymentStatus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorPaymentStatus_PaymentType_PaymentTypeId",
                table: "VendorPaymentStatus",
                column: "PaymentTypeId",
                principalTable: "PaymentType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorPaymentStatus_PaymentType_PaymentTypeId",
                table: "VendorPaymentStatus");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTypeId",
                table: "VendorPaymentStatus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorPaymentStatus_PaymentType_PaymentTypeId",
                table: "VendorPaymentStatus",
                column: "PaymentTypeId",
                principalTable: "PaymentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
