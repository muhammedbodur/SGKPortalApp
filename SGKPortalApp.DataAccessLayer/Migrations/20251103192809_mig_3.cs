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
            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_Kat_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar",
                columns: new[] { "HizmetBinasiId", "KatTipi", "BankoNo" },
                unique: true,
                filter: "[SilindiMi] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_Kat_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar",
                columns: new[] { "HizmetBinasiId", "BankoNo" },
                unique: true,
                filter: "[SilindiMi] = 0");
        }
    }
}
