using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKioskMenuAtamaUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskMenuAtama_Kiosk_Aktif",
                table: "SIR_KioskMenuAtama");

            migrationBuilder.DropColumn(
                name: "IslemAdi",
                table: "SIR_KioskMenuIslem");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuAtama_Kiosk_Menu",
                table: "SIR_KioskMenuAtama",
                columns: new[] { "KioskId", "KioskMenuId" },
                unique: true,
                filter: "[Aktiflik] = 1 AND [SilindiMi] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskMenuAtama_Kiosk_Menu",
                table: "SIR_KioskMenuAtama");

            migrationBuilder.AddColumn<string>(
                name: "IslemAdi",
                table: "SIR_KioskMenuIslem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuAtama_Kiosk_Aktif",
                table: "SIR_KioskMenuAtama",
                columns: new[] { "KioskId", "Aktiflik" },
                unique: true,
                filter: "[Aktiflik] = 1 AND [SilindiMi] = 0");
        }
    }
}
