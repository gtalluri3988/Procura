using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class BiddingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BiddingAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    ProjectState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetRefNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearPurchased = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiddingAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiddingAssets_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenderOpeningVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderId = table.Column<int>(type: "int", nullable: false),
                    VerifiedByUserId = table.Column<int>(type: "int", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderOpeningVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderOpeningVerifications_TenderApplications_TenderId",
                        column: x => x.TenderId,
                        principalTable: "TenderApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenderOpeningVerifications_Users_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "BidderSubmissionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenderVendorSubmissionId = table.Column<int>(type: "int", nullable: false),
                    BiddingAssetId = table.Column<int>(type: "int", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidderSubmissionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BidderSubmissionItems_BiddingAssets_BiddingAssetId",
                        column: x => x.BiddingAssetId,
                        principalTable: "BiddingAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BidderSubmissionItems_TenderVendorSubmissions_TenderVendorSubmissionId",
                        column: x => x.TenderVendorSubmissionId,
                        principalTable: "TenderVendorSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BidderSubmissionItems_BiddingAssetId",
                table: "BidderSubmissionItems",
                column: "BiddingAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BidderSubmissionItems_TenderVendorSubmissionId",
                table: "BidderSubmissionItems",
                column: "TenderVendorSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_BiddingAssets_TenderId",
                table: "BiddingAssets",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderOpeningVerifications_TenderId",
                table: "TenderOpeningVerifications",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderOpeningVerifications_VerifiedByUserId",
                table: "TenderOpeningVerifications",
                column: "VerifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidderSubmissionItems");

            migrationBuilder.DropTable(
                name: "TenderOpeningVerifications");

            migrationBuilder.DropTable(
                name: "BiddingAssets");
        }
    }
}
