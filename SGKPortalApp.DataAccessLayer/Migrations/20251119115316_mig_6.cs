using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Kiosk_SIR_KioskMenuTanim",
                schema: "dbo",
                table: "SIR_KioskTanim");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskTanim_KioskMenuId",
                schema: "dbo",
                table: "SIR_KioskTanim");

            migrationBuilder.DropColumn(
                name: "KioskMenuId",
                schema: "dbo",
                table: "SIR_KioskTanim");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KioskMenuId",
                schema: "dbo",
                table: "SIR_KioskTanim",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskTanim_KioskMenuId",
                schema: "dbo",
                table: "SIR_KioskTanim",
                column: "KioskMenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Kiosk_SIR_KioskMenuTanim",
                schema: "dbo",
                table: "SIR_KioskTanim",
                column: "KioskMenuId",
                principalSchema: "dbo",
                principalTable: "SIR_KioskMenuTanim",
                principalColumn: "KioskMenuId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
