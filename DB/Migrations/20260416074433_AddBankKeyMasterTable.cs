using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Migrations
{
    /// <inheritdoc />
    public partial class AddBankKeyMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankKeyCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankKeys", x => x.Id);
                });

            // Seed Bank Key master data
            migrationBuilder.Sql(@"
                INSERT INTO BankKeys (BankName, BankKeyCode) VALUES
                ('Affin Bank Bhd', 'AFFIN BANK'),
                ('AFFIN ISLAMIC BANK BERHAD', 'AFFIN'),
                ('Alliance Bank M Bhd', 'ABB'),
                ('ALLIANCE ISLAMIC BANK BERHAD', 'ABB'),
                ('AMBANK BERHAD', 'AMBANK'),
                ('AMISLAMIC BANK BERHAD', 'AMBANK'),
                ('BANK ISLAM BERHAD', 'BIMB'),
                ('BANK KERJASAMA RAKYAT MALAYSIA', 'BANK RAKYAT'),
                ('Bank Kerjasama Rakyat Malaysia Berhad', 'BANK RAKYAT'),
                ('BANK MUAMALAT MALAYSIA BERHAD', 'BMMB'),
                ('Bank Pertanian Malaysia Berhad', 'AGROBANK'),
                ('BANK SIMPANAN NASIONAL', 'BSN'),
                ('BANK SIMPANAN NASIONAL - akaun baru', 'BSN'),
                ('BANK SIMPANAN NASIONAL - ISLAMIC', 'BSN'),
                ('CIMB BANK BERHAD', 'CIMB'),
                ('CIMB ISLAMIC BANK BERHAD- AKAUN LAMA', 'CIMB'),
                ('Hong Leong Bank Bhd', 'HLB'),
                ('HONG LEONG ISLAMIC BANK BERHAD', 'HLB'),
                ('HSBC Bank M Bhd', 'HSBC'),
                ('Maybank Berhad', 'MBB'),
                ('Public Bank Bhd', 'PBB'),
                ('PUBLIC ISLAMIC BANK BERHAD', 'PBB'),
                ('RHB BANK BHD', 'RHB'),
                ('Standard Chartered Bank M Bhd', 'SCBC');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankKeys");
        }

        // Note: Down() drops the entire table, so seed data cleanup is automatic.
    }
}
