using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class paymentchannel1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentChannelId",
                table: "PaymentRequest",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequest_PaymentChannelId",
                table: "PaymentRequest",
                column: "PaymentChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequest_PaymentChannel_PaymentChannelId",
                table: "PaymentRequest",
                column: "PaymentChannelId",
                principalTable: "PaymentChannel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequest_PaymentChannel_PaymentChannelId",
                table: "PaymentRequest");

            migrationBuilder.DropIndex(
                name: "IX_PaymentRequest_PaymentChannelId",
                table: "PaymentRequest");

            migrationBuilder.DropColumn(
                name: "PaymentChannelId",
                table: "PaymentRequest");
        }
    }
}
