using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class master1234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorManagementSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RegistrationValidityYears = table.Column<int>(type: "int", nullable: false),
                    RenewalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateRenewalFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryCodeChangeFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CertificateBackgroundImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlacklistDenyDurationMonths = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorManagementSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorManagementSettings");
        }
    }
}
