using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PER_PersonelCezalari",
                schema: "dbo",
                columns: table => new
                {
                    PersonelCezaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CezaSebebi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AltBendi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CezaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PER_PersonelCezalari", x => x.PersonelCezaId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelCezalari_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelEgitimleri",
                schema: "dbo",
                columns: table => new
                {
                    PersonelEgitimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EgitimAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EgitimBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EgitimBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PER_PersonelEgitimleri", x => x.PersonelEgitimId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelEgitimleri_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelEngelleri",
                schema: "dbo",
                columns: table => new
                {
                    PersonelEngelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EngelDerecesi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EngelNedeni1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EngelNedeni2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EngelNedeni3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PER_PersonelEngelleri", x => x.PersonelEngelId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelEngelleri_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelHizmetleri",
                schema: "dbo",
                columns: table => new
                {
                    PersonelHizmetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DepartmanId = table.Column<int>(type: "int", nullable: false),
                    ServisId = table.Column<int>(type: "int", nullable: false),
                    GorevBaslamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GorevAyrilmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sebep = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PER_PersonelHizmetleri", x => x.PersonelHizmetId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelHizmetleri_PER_Departmanlar",
                        column: x => x.DepartmanId,
                        principalSchema: "dbo",
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_PersonelHizmetleri_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PER_PersonelHizmetleri_PER_Servisler",
                        column: x => x.ServisId,
                        principalSchema: "dbo",
                        principalTable: "PER_Servisler",
                        principalColumn: "ServisId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelImzaYetkileri",
                schema: "dbo",
                columns: table => new
                {
                    PersonelImzaYetkisiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DepartmanId = table.Column<int>(type: "int", nullable: false),
                    ServisId = table.Column<int>(type: "int", nullable: false),
                    GorevDegisimSebebi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImzaYetkisiBaslamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImzaYetkisiBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PER_PersonelImzaYetkileri", x => x.PersonelImzaYetkisiId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelImzaYetkileri_PER_Departmanlar",
                        column: x => x.DepartmanId,
                        principalSchema: "dbo",
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_PersonelImzaYetkileri_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PER_PersonelImzaYetkileri_PER_Servisler",
                        column: x => x.ServisId,
                        principalSchema: "dbo",
                        principalTable: "PER_Servisler",
                        principalColumn: "ServisId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelCezalari_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelCezalari",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelEgitimleri_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelEgitimleri",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelEngelleri_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelEngelleri",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelHizmetleri_DepartmanId",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelHizmetleri_ServisId",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "ServisId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelHizmetleri_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelImzaYetkileri_DepartmanId",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelImzaYetkileri_ServisId",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "ServisId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelImzaYetkileri_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "TcKimlikNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PER_PersonelCezalari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_PersonelEgitimleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_PersonelEngelleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_PersonelHizmetleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_PersonelImzaYetkileri",
                schema: "dbo");
        }
    }
}
