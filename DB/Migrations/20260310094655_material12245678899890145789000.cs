using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class material12245678899890145789000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequest_Vendors_VendorId",
                table: "PaymentRequest");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest");

            migrationBuilder.AlterColumn<int>(
                name: "VendorId",
                table: "PaymentRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest",
                column: "VendorId",
                unique: true,
                filter: "[VendorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequest_Vendors_VendorId",
                table: "PaymentRequest",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequest_Vendors_VendorId",
                table: "PaymentRequest");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest");

            migrationBuilder.AlterColumn<int>(
                name: "VendorId",
                table: "PaymentRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_VendorId",
                table: "PaymentRequest",
                column: "VendorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequest_Vendors_VendorId",
                table: "PaymentRequest",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
