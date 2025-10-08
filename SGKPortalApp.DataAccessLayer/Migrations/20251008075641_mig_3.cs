using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_Tvler",
                newName: "TvAktiflik");

            migrationBuilder.RenameColumn(
                name: "Aciklama",
                schema: "dbo",
                table: "SIR_Tvler",
                newName: "TvAciklama");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TvAktiflik",
                schema: "dbo",
                table: "SIR_Tvler",
                newName: "Aktiflik");

            migrationBuilder.RenameColumn(
                name: "TvAciklama",
                schema: "dbo",
                table: "SIR_Tvler",
                newName: "Aciklama");
        }
    }
}
