using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBankoUniqueConstraint_AddKatTipi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eski unique index'i DROP et (varsa)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIR_Bankolar_HizmetBinasi_BankoNo' AND object_id = OBJECT_ID('dbo.SIR_Bankolar'))
                BEGIN
                    DROP INDEX [IX_SIR_Bankolar_HizmetBinasi_BankoNo] ON [dbo].[SIR_Bankolar];
                END
            ");

            // Yeni unique index oluştur (varsa oluşturma)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SIR_Bankolar_HizmetBinasi_Kat_BankoNo' AND object_id = OBJECT_ID('dbo.SIR_Bankolar'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_SIR_Bankolar_HizmetBinasi_Kat_BankoNo] ON [dbo].[SIR_Bankolar] ([HizmetBinasiId], [KatTipi], [BankoNo]) WHERE [SilindiMi] = 0;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Yeni index'i DROP et
            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_Kat_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar");

            // Eski index'i geri oluştur
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
