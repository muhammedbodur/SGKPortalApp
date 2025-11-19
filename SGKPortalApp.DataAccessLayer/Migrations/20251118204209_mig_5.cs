using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KioskIslemGruplari",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropTable(
                name: "SIR_KioskIslemGruplari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskGruplari",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalAltIslemleri_KioskIslemGrupId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropColumn(
                name: "KioskIslemGrupId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.CreateTable(
                name: "SIR_KioskMenuTanim",
                schema: "dbo",
                columns: table => new
                {
                    KioskMenuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_SIR_KioskMenuTanim", x => x.KioskMenuId);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskTanim",
                schema: "dbo",
                columns: table => new
                {
                    KioskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KioskAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KioskIp = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    KioskMenuId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_SIR_KioskTanim", x => x.KioskId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskTanim_CMN_HizmetBinalari",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_Kiosk_SIR_KioskMenuTanim",
                        column: x => x.KioskMenuId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskMenuTanim",
                        principalColumn: "KioskMenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    KioskIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskId = table.Column<int>(type: "int", nullable: false),
                    MenuSira = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_SIR_KioskIslemleri", x => x.KioskIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemleri_SIR_KioskTanim",
                        column: x => x.KioskId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskTanim",
                        principalColumn: "KioskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskKanalAltIslem",
                schema: "dbo",
                columns: table => new
                {
                    KioskKanalAltIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskIslemId = table.Column<int>(type: "int", nullable: false),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_SIR_KioskKanalAltIslem", x => x.KioskKanalAltIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskKanalAltIslem_SIR_KanalAltIslemleri",
                        column: x => x.KanalAltIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalAltIslemleri",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KioskKanalAltIslem_SIR_KioskIslemleri",
                        column: x => x.KioskIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskIslemleri",
                        principalColumn: "KioskIslemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemleri_KioskId",
                schema: "dbo",
                table: "SIR_KioskIslemleri",
                column: "KioskId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskKanalAltIslem_KanalAltIslemId",
                schema: "dbo",
                table: "SIR_KioskKanalAltIslem",
                column: "KanalAltIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskKanalAltIslem_KioskIslem_KanalAltIslem",
                schema: "dbo",
                table: "SIR_KioskKanalAltIslem",
                columns: new[] { "KioskIslemId", "KanalAltIslemId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuTanim_MenuAdi",
                schema: "dbo",
                table: "SIR_KioskMenuTanim",
                column: "MenuAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskTanim_HizmetBinasi_KioskAdi",
                schema: "dbo",
                table: "SIR_KioskTanim",
                columns: new[] { "HizmetBinasiId", "KioskAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskTanim_KioskMenuId",
                schema: "dbo",
                table: "SIR_KioskTanim",
                column: "KioskMenuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SIR_KioskKanalAltIslem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskIslemleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskTanim",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskMenuTanim",
                schema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "KioskIslemGrupId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SIR_KioskGruplari",
                schema: "dbo",
                columns: table => new
                {
                    KioskGrupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KioskGrupAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KioskGruplari", x => x.KioskGrupId);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskIslemGruplari",
                schema: "dbo",
                columns: table => new
                {
                    KioskIslemGrupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KanalAltId = table.Column<int>(type: "int", nullable: false),
                    KioskGrupId = table.Column<int>(type: "int", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KioskGrupId1 = table.Column<int>(type: "int", nullable: true),
                    KioskIslemGrupAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    KioskIslemGrupSira = table.Column<int>(type: "int", nullable: false),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KioskIslemGruplari", x => x.KioskIslemGrupId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplari_CMN_HizmetBinalari",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplari_SIR_KanallarAlt",
                        column: x => x.KanalAltId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanallarAlt",
                        principalColumn: "KanalAltId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplari_SIR_KioskGruplari",
                        column: x => x.KioskGrupId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskGruplari",
                        principalColumn: "KioskGrupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplari_SIR_KioskGruplari_KioskGrupId1",
                        column: x => x.KioskGrupId1,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskGruplari",
                        principalColumn: "KioskGrupId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemleri_KioskIslemGrupId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KioskIslemGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskGruplari_KioskGrupAdi",
                schema: "dbo",
                table: "SIR_KioskGruplari",
                column: "KioskGrupAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplari_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplari_KanalAltId",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "KanalAltId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplari_KioskGrup_KanalAlt",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                columns: new[] { "KioskGrupId", "KanalAltId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplari_KioskGrupId1",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "KioskGrupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KioskIslemGruplari",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KioskIslemGrupId",
                principalSchema: "dbo",
                principalTable: "SIR_KioskIslemGruplari",
                principalColumn: "KioskIslemGrupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
