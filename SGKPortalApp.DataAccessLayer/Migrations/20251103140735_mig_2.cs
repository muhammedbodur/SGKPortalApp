using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SIR_BankoIslemleri",
                schema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "HedefBankoId",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YonlendirenPersonelTc",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "YonlendirildiMi",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "YonlendirmeBankoId",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YonlendirmeNedeni",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YonlendirmeTipi",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "YonlendirmeZamani",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankoAciklama",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Banko açıklaması (opsiyonel)");

            migrationBuilder.AddColumn<int>(
                name: "BankoSira",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Gösterim sırası");

            migrationBuilder.CreateTable(
                name: "SIR_BankoHareketleri",
                schema: "dbo",
                columns: table => new
                {
                    BankoHareketId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankoId = table.Column<int>(type: "int", nullable: false),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false, comment: "İşlemi yapan personel TC"),
                    SiraId = table.Column<int>(type: "int", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false, comment: "Sıra numarası"),
                    KanalIslemId = table.Column<int>(type: "int", nullable: false),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    IslemBaslamaZamani = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "İşlem başlama zamanı"),
                    IslemBitisZamani = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "İşlem bitiş zamanı (NULL = hala işlemde)"),
                    IslemSuresiSaniye = table.Column<int>(type: "int", nullable: true, comment: "İşlem süresi (saniye)"),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_BankoHareketleri", x => x.BankoHareketId);
                    table.ForeignKey(
                        name: "FK_SIR_BankoHareketleri_PER_Personeller",
                        column: x => x.PersonelTcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_BankoHareketleri_SIR_Bankolar",
                        column: x => x.BankoId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Bankolar",
                        principalColumn: "BankoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_BankoHareketleri_SIR_KanalAltIslemleri",
                        column: x => x.KanalAltIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalAltIslemleri",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_BankoHareketleri_SIR_KanalIslemleri",
                        column: x => x.KanalIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalIslemleri",
                        principalColumn: "KanalIslemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_BankoHareketleri_SIR_Siralar",
                        column: x => x.SiraId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Siralar",
                        principalColumn: "SiraId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_HedefBankoId",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "HedefBankoId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_YonlendirenPersonelTc",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "YonlendirenPersonelTc");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_YonlendirmeBankoId",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "YonlendirmeBankoId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoHareketleri_BankoId",
                schema: "dbo",
                table: "SIR_BankoHareketleri",
                column: "BankoId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoHareketleri_IslemBaslamaZamani",
                schema: "dbo",
                table: "SIR_BankoHareketleri",
                column: "IslemBaslamaZamani");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoHareketleri_KanalAltIslemId",
                schema: "dbo",
                table: "SIR_BankoHareketleri",
                column: "KanalAltIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoHareketleri_KanalIslemId",
                schema: "dbo",
                table: "SIR_BankoHareketleri",
                column: "KanalIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoHareketleri_PersonelTc",
                schema: "dbo",
                table: "SIR_BankoHareketleri",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoHareketleri_SiraId",
                schema: "dbo",
                table: "SIR_BankoHareketleri",
                column: "SiraId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_PER_Personeller_YonlendirenPersonelTc",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "YonlendirenPersonelTc",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_SIR_Bankolar_HedefBankoId",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "HedefBankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_SIR_Bankolar_YonlendirmeBankoId",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "YonlendirmeBankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_PER_Personeller_YonlendirenPersonelTc",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_SIR_Bankolar_HedefBankoId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_SIR_Bankolar_YonlendirmeBankoId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropTable(
                name: "SIR_BankoHareketleri",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Siralar_HedefBankoId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Siralar_YonlendirenPersonelTc",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Siralar_YonlendirmeBankoId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "HedefBankoId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "YonlendirenPersonelTc",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "YonlendirildiMi",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "YonlendirmeBankoId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "YonlendirmeNedeni",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "YonlendirmeTipi",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "YonlendirmeZamani",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "BankoAciklama",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropColumn(
                name: "BankoSira",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.CreateTable(
                name: "SIR_BankoIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    BankoIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankoGrup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankoIslemAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BankoIslemAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BankoIslemSira = table.Column<int>(type: "int", nullable: false),
                    BankoUstIslemId = table.Column<int>(type: "int", nullable: false),
                    DiffLang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_BankoIslemleri", x => x.BankoIslemId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoIslemleri_BankoIslemAdi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                column: "BankoIslemAdi",
                unique: true,
                filter: "[SilindiMi] = 0");
        }
    }
}
