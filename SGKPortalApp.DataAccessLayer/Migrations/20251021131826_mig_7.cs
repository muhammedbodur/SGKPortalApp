using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KanalAltIslemPersonelAktiflik",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                newName: "Aktiflik");

            migrationBuilder.RenameColumn(
                name: "KanalIslemAktiflik",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                newName: "Sira");

            migrationBuilder.RenameColumn(
                name: "KanalAltIslemAktiflik",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                newName: "Sira");

            migrationBuilder.AddColumn<int>(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "KanalIslemAdi",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropColumn(
                name: "KanalIslemAdi",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropColumn(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.RenameColumn(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                newName: "KanalAltIslemPersonelAktiflik");

            migrationBuilder.RenameColumn(
                name: "Sira",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                newName: "KanalIslemAktiflik");

            migrationBuilder.RenameColumn(
                name: "Sira",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                newName: "KanalAltIslemAktiflik");
        }
    }
}
