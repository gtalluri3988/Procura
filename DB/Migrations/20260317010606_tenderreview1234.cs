using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class tenderreview1234 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "PicName",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "TenderApprovals");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "TenderApprovals");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "TenderApprovals");

            migrationBuilder.DropColumn(
                name: "PicName",
                table: "TenderApprovals");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ReviewedBy",
                table: "TenderReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByUserId",
                table: "TenderReviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenderApplicationStatusId",
                table: "TenderReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TenderApprovals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ReviewedBy",
                table: "TenderApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByUserId",
                table: "TenderApprovals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenderApplicationStatusId",
                table: "TenderApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenderReviews_ReviewedByUserId",
                table: "TenderReviews",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderReviews_TenderApplicationStatusId",
                table: "TenderReviews",
                column: "TenderApplicationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderApprovals_ReviewedByUserId",
                table: "TenderApprovals",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenderApprovals_TenderApplicationStatusId",
                table: "TenderApprovals",
                column: "TenderApplicationStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApprovals_TenderApplicationStatus_TenderApplicationStatusId",
                table: "TenderApprovals",
                column: "TenderApplicationStatusId",
                principalTable: "TenderApplicationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderApprovals_Users_ReviewedByUserId",
                table: "TenderApprovals",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenderReviews_TenderApplicationStatus_TenderApplicationStatusId",
                table: "TenderReviews",
                column: "TenderApplicationStatusId",
                principalTable: "TenderApplicationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenderReviews_Users_ReviewedByUserId",
                table: "TenderReviews",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenderApprovals_TenderApplicationStatus_TenderApplicationStatusId",
                table: "TenderApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderApprovals_Users_ReviewedByUserId",
                table: "TenderApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderReviews_TenderApplicationStatus_TenderApplicationStatusId",
                table: "TenderReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_TenderReviews_Users_ReviewedByUserId",
                table: "TenderReviews");

            migrationBuilder.DropIndex(
                name: "IX_TenderReviews_ReviewedByUserId",
                table: "TenderReviews");

            migrationBuilder.DropIndex(
                name: "IX_TenderReviews_TenderApplicationStatusId",
                table: "TenderReviews");

            migrationBuilder.DropIndex(
                name: "IX_TenderApprovals_ReviewedByUserId",
                table: "TenderApprovals");

            migrationBuilder.DropIndex(
                name: "IX_TenderApprovals_TenderApplicationStatusId",
                table: "TenderApprovals");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "TenderApplicationStatusId",
                table: "TenderReviews");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "TenderApprovals");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "TenderApprovals");

            migrationBuilder.DropColumn(
                name: "TenderApplicationStatusId",
                table: "TenderApprovals");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PicName",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TenderReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "TenderApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "TenderApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "TenderApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "TenderApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PicName",
                table: "TenderApprovals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
