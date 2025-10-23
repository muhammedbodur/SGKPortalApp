using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KanalIslemAdi",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropColumn(
                name: "Sira",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KanalIslemAdi",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sira",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
