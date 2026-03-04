using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class FixSelfReference88811233445566778802 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorCreditFacilities_Vendors_VendorId",
                table: "VendorCreditFacilities");

            migrationBuilder.DropIndex(
                name: "IX_VendorCreditFacilities_VendorId",
                table: "VendorCreditFacilities");

            migrationBuilder.CreateTable(
                name: "VendorPaymentStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPaymentStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorPaymentStatus_PaymentRequest_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "PaymentRequest",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorPaymentStatus_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorPaymentStatus_PaymentId",
                table: "VendorPaymentStatus",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPaymentStatus_VendorId",
                table: "VendorPaymentStatus",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorPaymentStatus");

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
    }
}
