using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKioskMenuIslemToUseKanalAlt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskMenuIslem_SIR_KanalAltIslem",
                table: "SIR_KioskMenuIslem");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskMenuIslem_SIR_KanalAltIslemleri_KanalAltIslemId1",
                table: "SIR_KioskMenuIslem");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskMenuIslem_KanalAltIslemId1",
                table: "SIR_KioskMenuIslem");

            migrationBuilder.DropColumn(
                name: "KanalAltIslemId1",
                table: "SIR_KioskMenuIslem");

            migrationBuilder.RenameColumn(
                name: "KanalAltIslemId",
                table: "SIR_KioskMenuIslem",
                newName: "KanalAltId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KioskMenuIslem_KanalAltIslemId",
                table: "SIR_KioskMenuIslem",
                newName: "IX_SIR_KioskMenuIslem_KanalAltId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskMenuIslem_SIR_KanalAlt",
                table: "SIR_KioskMenuIslem",
                column: "KanalAltId",
                principalSchema: "dbo",
                principalTable: "SIR_KanallarAlt",
                principalColumn: "KanalAltId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskMenuIslem_SIR_KanalAlt",
                table: "SIR_KioskMenuIslem");

            migrationBuilder.RenameColumn(
                name: "KanalAltId",
                table: "SIR_KioskMenuIslem",
                newName: "KanalAltIslemId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KioskMenuIslem_KanalAltId",
                table: "SIR_KioskMenuIslem",
                newName: "IX_SIR_KioskMenuIslem_KanalAltIslemId");

            migrationBuilder.AddColumn<int>(
                name: "KanalAltIslemId1",
                table: "SIR_KioskMenuIslem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuIslem_KanalAltIslemId1",
                table: "SIR_KioskMenuIslem",
                column: "KanalAltIslemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskMenuIslem_SIR_KanalAltIslem",
                table: "SIR_KioskMenuIslem",
                column: "KanalAltIslemId",
                principalSchema: "dbo",
                principalTable: "SIR_KanalAltIslemleri",
                principalColumn: "KanalAltIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskMenuIslem_SIR_KanalAltIslemleri_KanalAltIslemId1",
                table: "SIR_KioskMenuIslem",
                column: "KanalAltIslemId1",
                principalSchema: "dbo",
                principalTable: "SIR_KanalAltIslemleri",
                principalColumn: "KanalAltIslemId");
        }
    }
}
