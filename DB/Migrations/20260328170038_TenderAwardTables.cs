using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class TenderAwardTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenderAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    AwardedVendorId = table.Column<int>(type: "int", nullable: true),
                    ProjectValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YearlyExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjectStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Agreement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreementDateSigned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderAwards_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderAwards_Vendors_AwardedVendorId",
                        column: x => x.AwardedVendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenderAwardMinutesOfMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingOutcome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenderAwardId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderAwardMinutesOfMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderAwardMinutesOfMeetings_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderAwardMinutesOfMeetings_TenderAwards_TenderAwardId",
                        column: x => x.TenderAwardId,
                        principalTable: "TenderAwards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenderAwardMinutesOfMeetings_TenderAwardId",
                table: "TenderAwardMinutesOfMeetings",
                column: "TenderAwardId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderAwardMinutesOfMeetings_TenderId",
                table: "TenderAwardMinutesOfMeetings",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderAwards_AwardedVendorId",
                table: "TenderAwards",
                column: "AwardedVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderAwards_TenderId",
                table: "TenderAwards",
                column: "TenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenderAwardMinutesOfMeetings");

            migrationBuilder.DropTable(
                name: "TenderAwards");
        }
    }
}
