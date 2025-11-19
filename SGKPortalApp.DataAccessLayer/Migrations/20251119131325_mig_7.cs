using System;
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
            migrationBuilder.DropTable(
                name: "SIR_KioskKanalAltIslem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskIslemleri",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "SIR_KioskMenuAtama",
                columns: table => new
                {
                    KioskMenuAtamaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskId = table.Column<int>(type: "int", nullable: false),
                    KioskMenuId = table.Column<int>(type: "int", nullable: false),
                    AtamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KioskMenuAtama", x => x.KioskMenuAtamaId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskMenuAtama_SIR_Kiosk",
                        column: x => x.KioskId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskTanim",
                        principalColumn: "KioskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KioskMenuAtama_SIR_KioskMenu",
                        column: x => x.KioskMenuId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskMenuTanim",
                        principalColumn: "KioskMenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskMenuIslem",
                columns: table => new
                {
                    KioskMenuIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskMenuId = table.Column<int>(type: "int", nullable: false),
                    IslemAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    MenuSira = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    KanalAltIslemId1 = table.Column<int>(type: "int", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KioskMenuIslem", x => x.KioskMenuIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskMenuIslem_SIR_KanalAltIslem",
                        column: x => x.KanalAltIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalAltIslemleri",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KioskMenuIslem_SIR_KanalAltIslemleri_KanalAltIslemId1",
                        column: x => x.KanalAltIslemId1,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalAltIslemleri",
                        principalColumn: "KanalAltIslemId");
                    table.ForeignKey(
                        name: "FK_SIR_KioskMenuIslem_SIR_KioskMenu",
                        column: x => x.KioskMenuId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskMenuTanim",
                        principalColumn: "KioskMenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuAtama_Kiosk_Aktif",
                table: "SIR_KioskMenuAtama",
                columns: new[] { "KioskId", "Aktiflik" },
                unique: true,
                filter: "[Aktiflik] = 1 AND [SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuAtama_KioskId",
                table: "SIR_KioskMenuAtama",
                column: "KioskId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuAtama_KioskMenuId",
                table: "SIR_KioskMenuAtama",
                column: "KioskMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuAtama_SilindiMi",
                table: "SIR_KioskMenuAtama",
                column: "SilindiMi",
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuIslem_KanalAltIslemId",
                table: "SIR_KioskMenuIslem",
                column: "KanalAltIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuIslem_KanalAltIslemId1",
                table: "SIR_KioskMenuIslem",
                column: "KanalAltIslemId1");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuIslem_KioskMenuId",
                table: "SIR_KioskMenuIslem",
                column: "KioskMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuIslem_Menu_Sira",
                table: "SIR_KioskMenuIslem",
                columns: new[] { "KioskMenuId", "MenuSira" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskMenuIslem_SilindiMi",
                table: "SIR_KioskMenuIslem",
                column: "SilindiMi",
                filter: "[SilindiMi] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SIR_KioskMenuAtama");

            migrationBuilder.DropTable(
                name: "SIR_KioskMenuIslem");

            migrationBuilder.CreateTable(
                name: "SIR_KioskIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    KioskIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuSira = table.Column<int>(type: "int", nullable: false),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    KioskIslemId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
        }
    }
}
