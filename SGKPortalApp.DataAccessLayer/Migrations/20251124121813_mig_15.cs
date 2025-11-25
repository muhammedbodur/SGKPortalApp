using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AktifBankoId",
                schema: "dbo",
                table: "CMN_Users",
                type: "int",
                nullable: true,
                comment: "Aktif banko ID (banko modundaysa)");

            migrationBuilder.AddColumn<bool>(
                name: "BankoModuAktif",
                schema: "dbo",
                table: "CMN_Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Kullanıcı banko modunda mı? (true ise sadece sıra çağırma kullanılabilir)");

            migrationBuilder.AddColumn<DateTime>(
                name: "BankoModuBaslangic",
                schema: "dbo",
                table: "CMN_Users",
                type: "datetime2",
                nullable: true,
                comment: "Banko moduna geçiş zamanı");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AktifBankoId",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropColumn(
                name: "BankoModuAktif",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropColumn(
                name: "BankoModuBaslangic",
                schema: "dbo",
                table: "CMN_Users");
        }
    }
}
