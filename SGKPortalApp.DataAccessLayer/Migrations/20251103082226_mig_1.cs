using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "CMN_DatabaseLogs",
                schema: "dbo",
                columns: table => new
                {
                    DatabaseLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseAction = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BeforeData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AfterData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_DatabaseLogs", x => x.DatabaseLogId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_Iller",
                schema: "dbo",
                columns: table => new
                {
                    IlId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_CMN_Iller", x => x.IlId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_LoginLogoutLogs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SessionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_LoginLogoutLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PER_AtanmaNedenleri",
                schema: "dbo",
                columns: table => new
                {
                    AtanmaNedeniId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtanmaNedeni = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_PER_AtanmaNedenleri", x => x.AtanmaNedeniId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Departmanlar",
                schema: "dbo",
                columns: table => new
                {
                    DepartmanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmanAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Departman adı"),
                    DepartmanAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Departman aktiflik durumu (0: Pasif, 1: Aktif)"),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()", comment: "Kayıt oluşturulma tarihi"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()", comment: "Son güncelleme tarihi"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Soft delete flag"),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Departmanlar", x => x.DepartmanId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Moduller",
                schema: "dbo",
                columns: table => new
                {
                    ModulId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_PER_Moduller", x => x.ModulId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Sendikalar",
                schema: "dbo",
                columns: table => new
                {
                    SendikaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendikaAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Sendika adı"),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PER_Sendikalar", x => x.SendikaId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Servisler",
                schema: "dbo",
                columns: table => new
                {
                    ServisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServisAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Servis adı"),
                    ServisAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Servis aktiflik durumu"),
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
                    table.PrimaryKey("PK_PER_Servisler", x => x.ServisId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Unvanlar",
                schema: "dbo",
                columns: table => new
                {
                    UnvanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnvanAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Unvan adı"),
                    UnvanAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Unvan aktiflik durumu"),
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
                    table.PrimaryKey("PK_PER_Unvanlar", x => x.UnvanId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Yetkiler",
                schema: "dbo",
                columns: table => new
                {
                    YetkiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YetkiTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YetkiAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UstYetkiId = table.Column<int>(type: "int", nullable: true),
                    ControllerAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_PER_Yetkiler", x => x.YetkiId);
                    table.ForeignKey(
                        name: "FK_PER_Yetkiler_PER_Yetkiler_UstYetkiId",
                        column: x => x.UstYetkiId,
                        principalSchema: "dbo",
                        principalTable: "PER_Yetkiler",
                        principalColumn: "YetkiId");
                });

            migrationBuilder.CreateTable(
                name: "SIR_BankoIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    BankoIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankoGrup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankoUstIslemId = table.Column<int>(type: "int", nullable: false),
                    BankoIslemAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BankoIslemSira = table.Column<int>(type: "int", nullable: false),
                    BankoIslemAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DiffLang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_SIR_BankoIslemleri", x => x.BankoIslemId);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Kanallar",
                schema: "dbo",
                columns: table => new
                {
                    KanalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Kanal adı"),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SIR_Kanallar", x => x.KanalId);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskGruplari",
                schema: "dbo",
                columns: table => new
                {
                    KioskGrupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskGrupAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SIR_KioskGruplari", x => x.KioskGrupId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_Ilceler",
                schema: "dbo",
                columns: table => new
                {
                    IlceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlId = table.Column<int>(type: "int", nullable: false),
                    IlceAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_CMN_Ilceler", x => x.IlceId);
                    table.ForeignKey(
                        name: "FK_CMN_Ilceler_CMN_Iller",
                        column: x => x.IlId,
                        principalSchema: "dbo",
                        principalTable: "CMN_Iller",
                        principalColumn: "IlId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMN_HizmetBinalari",
                schema: "dbo",
                columns: table => new
                {
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmanId = table.Column<int>(type: "int", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HizmetBinasiAktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CMN_HizmetBinalari", x => x.HizmetBinasiId);
                    table.ForeignKey(
                        name: "FK_CMN_HizmetBinalari_PER_Departmanlar",
                        column: x => x.DepartmanId,
                        principalSchema: "dbo",
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModulAlt",
                columns: table => new
                {
                    ModulAltId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulAltAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModulId = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulAlt", x => x.ModulAltId);
                    table.ForeignKey(
                        name: "FK_ModulAlt_PER_Moduller_ModulId",
                        column: x => x.ModulId,
                        principalSchema: "dbo",
                        principalTable: "PER_Moduller",
                        principalColumn: "ModulId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_ModulControllers",
                schema: "dbo",
                columns: table => new
                {
                    ModulControllerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulControllerAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModulId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PER_ModulControllers", x => x.ModulControllerId);
                    table.ForeignKey(
                        name: "FK_PER_ModulControllers_PER_Moduller",
                        column: x => x.ModulId,
                        principalSchema: "dbo",
                        principalTable: "PER_Moduller",
                        principalColumn: "ModulId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanallarAlt",
                schema: "dbo",
                columns: table => new
                {
                    KanalAltId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalAltAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    KanalId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SIR_KanallarAlt", x => x.KanalAltId);
                    table.ForeignKey(
                        name: "FK_SIR_KanallarAlt_SIR_Kanallar",
                        column: x => x.KanalId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Kanallar",
                        principalColumn: "KanalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PER_Personeller",
                schema: "dbo",
                columns: table => new
                {
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false, comment: "TC Kimlik Numarası - Primary Key"),
                    SicilNo = table.Column<int>(type: "int", nullable: false, comment: "Personel sicil numarası"),
                    AdSoyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Ad Soyad"),
                    NickName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonelKayitNo = table.Column<int>(type: "int", nullable: false),
                    KartNo = table.Column<int>(type: "int", nullable: false),
                    KartNoAktiflikTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KartNoDuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KartNoGonderimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KartGonderimIslemBasari = table.Column<int>(type: "int", nullable: false),
                    DepartmanId = table.Column<int>(type: "int", nullable: false),
                    ServisId = table.Column<int>(type: "int", nullable: false),
                    UnvanId = table.Column<int>(type: "int", nullable: false),
                    AtanmaNedeniId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    IlId = table.Column<int>(type: "int", nullable: false),
                    IlceId = table.Column<int>(type: "int", nullable: false),
                    SendikaId = table.Column<int>(type: "int", nullable: true),
                    EsininIsIlId = table.Column<int>(type: "int", nullable: true),
                    EsininIsIlceId = table.Column<int>(type: "int", nullable: true),
                    Gorev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Uzmanlik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PersonelTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "E-posta adresi"),
                    Dahili = table.Column<int>(type: "int", nullable: false),
                    CepTelefonu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Cep telefonu numarası"),
                    CepTelefonu2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EvTelefonu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Adres bilgisi"),
                    Semt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cinsiyet = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MedeniDurumu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KanGrubu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EvDurumu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UlasimServis1 = table.Column<int>(type: "int", nullable: false),
                    UlasimServis2 = table.Column<int>(type: "int", nullable: false),
                    Tabldot = table.Column<int>(type: "int", nullable: false),
                    PersonelAktiflikDurum = table.Column<int>(type: "int", nullable: false),
                    EmekliSicilNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OgrenimDurumu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BitirdigiOkul = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BitirdigiBolum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OgrenimSuresi = table.Column<int>(type: "int", nullable: false),
                    Bransi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SehitYakinligi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EsininAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EsininIsDurumu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EsininUnvani = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EsininIsAdresi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EsininIsSemt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HizmetBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EgitimBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImzaYetkileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CezaBilgileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngelBilgileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_PER_Personeller", x => x.TcKimlikNo);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_HizmetBinalari",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Ilceler_EsininIsIlceId",
                        column: x => x.EsininIsIlceId,
                        principalSchema: "dbo",
                        principalTable: "CMN_Ilceler",
                        principalColumn: "IlceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalSchema: "dbo",
                        principalTable: "CMN_Ilceler",
                        principalColumn: "IlceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Iller_EsininIsIlId",
                        column: x => x.EsininIsIlId,
                        principalSchema: "dbo",
                        principalTable: "CMN_Iller",
                        principalColumn: "IlId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Iller_IlId",
                        column: x => x.IlId,
                        principalSchema: "dbo",
                        principalTable: "CMN_Iller",
                        principalColumn: "IlId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_AtanmaNedenleri",
                        column: x => x.AtanmaNedeniId,
                        principalSchema: "dbo",
                        principalTable: "PER_AtanmaNedenleri",
                        principalColumn: "AtanmaNedeniId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Departmanlar",
                        column: x => x.DepartmanId,
                        principalSchema: "dbo",
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Sendikalar",
                        column: x => x.SendikaId,
                        principalSchema: "dbo",
                        principalTable: "PER_Sendikalar",
                        principalColumn: "SendikaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Servisler",
                        column: x => x.ServisId,
                        principalSchema: "dbo",
                        principalTable: "PER_Servisler",
                        principalColumn: "ServisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Unvanlar",
                        column: x => x.UnvanId,
                        principalSchema: "dbo",
                        principalTable: "PER_Unvanlar",
                        principalColumn: "UnvanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Bankolar",
                schema: "dbo",
                columns: table => new
                {
                    BankoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    BankoNo = table.Column<int>(type: "int", nullable: false, comment: "Banko numarası"),
                    BankoTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Banko tipi (Normal/Oncelikli/vb)"),
                    KatTipi = table.Column<int>(type: "int", nullable: false, comment: "Kat bilgisi"),
                    BankoAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "Banko aktiflik durumu"),
                    HizmetBinasiId1 = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_SIR_Bankolar", x => x.BankoId);
                    table.ForeignKey(
                        name: "FK_SIR_Bankolar_CMN_HizmetBinalari",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_Bankolar_CMN_HizmetBinalari_HizmetBinasiId1",
                        column: x => x.HizmetBinasiId1,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId");
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    KanalIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalId = table.Column<int>(type: "int", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    BaslangicNumara = table.Column<int>(type: "int", nullable: false),
                    BitisNumara = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SIR_KanalIslemleri", x => x.KanalIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalIslemleri_CMN_HizmetBinalari",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalIslemleri_SIR_Kanallar",
                        column: x => x.KanalId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Kanallar",
                        principalColumn: "KanalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Tvler",
                schema: "dbo",
                columns: table => new
                {
                    TvId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "TV ekran adı"),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KatTipi = table.Column<int>(type: "int", nullable: false, comment: "Kat bilgisi"),
                    TvAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1, comment: "TV aktiflik durumu"),
                    TvAciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HizmetBinasiId1 = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_SIR_Tvler", x => x.TvId);
                    table.ForeignKey(
                        name: "FK_SIR_Tvler_CMN_HizmetBinalari",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_Tvler_CMN_HizmetBinalari_HizmetBinasiId1",
                        column: x => x.HizmetBinasiId1,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId");
                });

            migrationBuilder.CreateTable(
                name: "PER_ModulControllerIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    ModulControllerIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulControllerIslemAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModulControllerId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PER_ModulControllerIslemleri", x => x.ModulControllerIslemId);
                    table.ForeignKey(
                        name: "FK_PER_ModulControllerIslemleri_PER_ModulControllers",
                        column: x => x.ModulControllerId,
                        principalSchema: "dbo",
                        principalTable: "PER_ModulControllers",
                        principalColumn: "ModulControllerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskIslemGruplari",
                schema: "dbo",
                columns: table => new
                {
                    KioskIslemGrupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskGrupId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KioskIslemGrupSira = table.Column<int>(type: "int", nullable: false),
                    KioskIslemGrupAktiflik = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    KanalAltId = table.Column<int>(type: "int", nullable: false),
                    KioskGrupId1 = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "CMN_Users",
                schema: "dbo",
                columns: table => new
                {
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false, comment: "TC Kimlik Numarası - Primary Key & Foreign Key to Personel"),
                    PassWord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Şifre (hash'lenmiş)"),
                    SessionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Aktif oturum ID"),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Kullanıcı aktif mi?"),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Son giriş tarihi"),
                    BasarisizGirisSayisi = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "Başarısız giriş denemesi sayısı"),
                    HesapKilitTarihi = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Hesap kilitlenme tarihi"),
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
                    table.PrimaryKey("PK_CMN_Users", x => x.TcKimlikNo);
                    table.ForeignKey(
                        name: "FK_CMN_Users_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "PER_PersonelCocuklari",
                schema: "dbo",
                columns: table => new
                {
                    PersonelCocukId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CocukAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CocukDogumTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    OgrenimDurumu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_PER_PersonelCocuklari", x => x.PersonelCocukId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelCocuklari_PER_Personeller",
                        column: x => x.PersonelTcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelDepartmanlar",
                schema: "dbo",
                columns: table => new
                {
                    PersonelDepartmanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DepartmanId = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PER_PersonelDepartmanlar", x => x.PersonelDepartmanId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelDepartmanlar_PER_Departmanlar",
                        column: x => x.DepartmanId,
                        principalSchema: "dbo",
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_PersonelDepartmanlar_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "DepartmanId");
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
                        principalColumn: "ServisId");
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
                        principalColumn: "DepartmanId");
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
                        principalColumn: "ServisId");
                });

            migrationBuilder.CreateTable(
                name: "SIR_BankoKullanicilari",
                schema: "dbo",
                columns: table => new
                {
                    BankoKullaniciId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankoId = table.Column<int>(type: "int", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_BankoKullanicilari", x => x.BankoKullaniciId);
                    table.ForeignKey(
                        name: "FK_SIR_BankoKullanicilari_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_BankoKullanicilari_PER_Personeller_PersonelTcKimlikNo",
                        column: x => x.PersonelTcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo");
                    table.ForeignKey(
                        name: "FK_SIR_BankoKullanicilari_SIR_Bankolar",
                        column: x => x.BankoId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Bankolar",
                        principalColumn: "BankoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_HubTvConnections",
                schema: "dbo",
                columns: table => new
                {
                    HubTvConnectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvId = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConnectionStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_SIR_HubTvConnections", x => x.HubTvConnectionId);
                    table.ForeignKey(
                        name: "FK_SIR_HubTvConnections_SIR_Tvler",
                        column: x => x.TvId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Tvler",
                        principalColumn: "TvId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_TvBankolari",
                schema: "dbo",
                columns: table => new
                {
                    TvBankoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvId = table.Column<int>(type: "int", nullable: false),
                    BankoId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SIR_TvBankolari", x => x.TvBankoId);
                    table.ForeignKey(
                        name: "FK_SIR_TvBankolari_SIR_Bankolar",
                        column: x => x.BankoId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Bankolar",
                        principalColumn: "BankoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_TvBankolari_SIR_Tvler",
                        column: x => x.TvId,
                        principalSchema: "dbo",
                        principalTable: "SIR_Tvler",
                        principalColumn: "TvId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelYetkileri",
                schema: "dbo",
                columns: table => new
                {
                    PersonelYetkiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    YetkiId = table.Column<int>(type: "int", nullable: false),
                    ModulControllerIslemId = table.Column<int>(type: "int", nullable: false),
                    YetkiTipleri = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    YetkiId1 = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PER_PersonelYetkileri", x => x.PersonelYetkiId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_ModulControllerIslemleri_ModulControllerIslemId",
                        column: x => x.ModulControllerIslemId,
                        principalSchema: "dbo",
                        principalTable: "PER_ModulControllerIslemleri",
                        principalColumn: "ModulControllerIslemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_Personeller_PersonelTcKimlikNo",
                        column: x => x.PersonelTcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo");
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_Yetkiler",
                        column: x => x.YetkiId,
                        principalSchema: "dbo",
                        principalTable: "PER_Yetkiler",
                        principalColumn: "YetkiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_Yetkiler_YetkiId1",
                        column: x => x.YetkiId1,
                        principalSchema: "dbo",
                        principalTable: "PER_Yetkiler",
                        principalColumn: "YetkiId");
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalAltIslemleri",
                schema: "dbo",
                columns: table => new
                {
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalAltId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KanalIslemId = table.Column<int>(type: "int", nullable: false),
                    KioskIslemGrupId = table.Column<int>(type: "int", nullable: true),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SIR_KanalAltIslemleri", x => x.KanalAltIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemleri_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemleri_SIR_KanalIslemleri",
                        column: x => x.KanalIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalIslemleri",
                        principalColumn: "KanalIslemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemleri_SIR_KanallarAlt",
                        column: x => x.KanalAltId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanallarAlt",
                        principalColumn: "KanalAltId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemleri_SIR_KioskIslemGruplari",
                        column: x => x.KioskIslemGrupId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KioskIslemGruplari",
                        principalColumn: "KioskIslemGrupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_HubConnections",
                schema: "dbo",
                columns: table => new
                {
                    HubConnectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConnectionStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_SIR_HubConnections", x => x.HubConnectionId);
                    table.ForeignKey(
                        name: "FK_SIR_HubConnections_CMN_Users",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "CMN_Users",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalPersonelleri",
                schema: "dbo",
                columns: table => new
                {
                    KanalPersonelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    Uzmanlik = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true),
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
                    table.PrimaryKey("PK_SIR_KanalPersonelleri", x => x.KanalPersonelId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalPersonelleri_PER_Personeller",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalPersonelleri_PER_Personeller_PersonelTcKimlikNo",
                        column: x => x.PersonelTcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo");
                    table.ForeignKey(
                        name: "FK_SIR_KanalPersonelleri_SIR_KanalAltIslemleri",
                        column: x => x.KanalAltIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalAltIslemleri",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Siralar",
                schema: "dbo",
                columns: table => new
                {
                    SiraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiraNo = table.Column<int>(type: "int", nullable: false, comment: "Sıra numarası"),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    KanalAltAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    SiraAlisZamani = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Sıra alış zamanı"),
                    IslemBaslamaZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IslemBitisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BeklemeDurum = table.Column<int>(type: "int", nullable: false, comment: "Bekleme durumu"),
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
                    table.PrimaryKey("PK_SIR_Siralar", x => x.SiraId);
                    table.ForeignKey(
                        name: "FK_SIR_Siralar_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalSchema: "dbo",
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIR_Siralar_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SIR_Siralar_SIR_KanalAltIslemleri",
                        column: x => x.KanalAltIslemId,
                        principalSchema: "dbo",
                        principalTable: "SIR_KanalAltIslemleri",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CMN_DatabaseLogs_EklenmeTarihi",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                column: "EklenmeTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_DatabaseLogs_TableName",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                column: "TableName");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HizmetBinalari_DepartmanId",
                schema: "dbo",
                table: "CMN_HizmetBinalari",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HizmetBinalari_HizmetBinasiAdi",
                schema: "dbo",
                table: "CMN_HizmetBinalari",
                column: "HizmetBinasiAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Ilceler_Il_IlceAdi",
                schema: "dbo",
                table: "CMN_Ilceler",
                columns: new[] { "IlId", "IlceAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Iller_IlAdi",
                schema: "dbo",
                table: "CMN_Iller",
                column: "IlAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_LoginLogoutLogs_EklenmeTarihi",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs",
                column: "EklenmeTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_LoginLogoutLogs_TcKimlikNo",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_SessionID",
                schema: "dbo",
                table: "CMN_Users",
                column: "SessionID",
                filter: "[SessionID] IS NOT NULL AND [SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_TcKimlikNo",
                schema: "dbo",
                table: "CMN_Users",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ModulAlt_ModulId",
                table: "ModulAlt",
                column: "ModulId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_AtanmaNedenleri_AtanmaNedeni",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                column: "AtanmaNedeni",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Departmanlar_Aktiflik_SilindiMi",
                schema: "dbo",
                table: "PER_Departmanlar",
                columns: new[] { "DepartmanAktiflik", "SilindiMi" });

            migrationBuilder.CreateIndex(
                name: "IX_PER_Departmanlar_DepartmanAdi",
                schema: "dbo",
                table: "PER_Departmanlar",
                column: "DepartmanAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Departmanlar_DepartmanAktiflik",
                schema: "dbo",
                table: "PER_Departmanlar",
                column: "DepartmanAktiflik");

            migrationBuilder.CreateIndex(
                name: "IX_PER_ModulControllerIslemleri_Controller_Islem",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                columns: new[] { "ModulControllerId", "ModulControllerIslemAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_ModulControllers_Modul_Controller",
                schema: "dbo",
                table: "PER_ModulControllers",
                columns: new[] { "ModulId", "ModulControllerAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Moduller_ModulAdi",
                schema: "dbo",
                table: "PER_Moduller",
                column: "ModulAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelCezalari_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelCezalari",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelCocuklari_TcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelCocuklari",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelDepartmanlar_DepartmanId",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelDepartmanlar_Tc_Departman",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar",
                columns: new[] { "TcKimlikNo", "DepartmanId" });

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

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_AktiflikDurum",
                schema: "dbo",
                table: "PER_Personeller",
                column: "PersonelAktiflikDurum");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_AtanmaNedeniId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "AtanmaNedeniId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_Departman_Aktiflik",
                schema: "dbo",
                table: "PER_Personeller",
                columns: new[] { "DepartmanId", "PersonelAktiflikDurum" });

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_Email",
                schema: "dbo",
                table: "PER_Personeller",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL AND [SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_EsininIsIlceId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "EsininIsIlceId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_EsininIsIlId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "EsininIsIlId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_HizmetBinasi_Aktiflik",
                schema: "dbo",
                table: "PER_Personeller",
                columns: new[] { "HizmetBinasiId", "PersonelAktiflikDurum" });

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_IlceId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_IlId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_SendikaId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "SendikaId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_ServisId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "ServisId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_SicilNo",
                schema: "dbo",
                table: "PER_Personeller",
                column: "SicilNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_TcKimlikNo",
                schema: "dbo",
                table: "PER_Personeller",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_UnvanId",
                schema: "dbo",
                table: "PER_Personeller",
                column: "UnvanId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_ModulControllerIslemId",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "ModulControllerIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_Tc_Yetki",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                columns: new[] { "TcKimlikNo", "YetkiId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_YetkiId",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "YetkiId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "YetkiId1");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Sendikalar_SendikaAdi",
                schema: "dbo",
                table: "PER_Sendikalar",
                column: "SendikaAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Servisler_ServisAdi",
                schema: "dbo",
                table: "PER_Servisler",
                column: "ServisAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Servisler_ServisAktiflik",
                schema: "dbo",
                table: "PER_Servisler",
                column: "ServisAktiflik");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Unvanlar_UnvanAdi",
                schema: "dbo",
                table: "PER_Unvanlar",
                column: "UnvanAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Unvanlar_UnvanAktiflik",
                schema: "dbo",
                table: "PER_Unvanlar",
                column: "UnvanAktiflik");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_ControllerAdi",
                schema: "dbo",
                table: "PER_Yetkiler",
                column: "ControllerAdi");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_UstYetki_YetkiAdi",
                schema: "dbo",
                table: "PER_Yetkiler",
                columns: new[] { "UstYetkiId", "YetkiAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_YetkiAdi",
                schema: "dbo",
                table: "PER_Yetkiler",
                column: "YetkiAdi");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoIslemleri_BankoIslemAdi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                column: "BankoIslemAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_BankoId",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "BankoId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_PersonelTcKimlikNo",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_TcKimlikNo",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_BankoAktiflik",
                schema: "dbo",
                table: "SIR_Bankolar",
                column: "BankoAktiflik");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar",
                columns: new[] { "HizmetBinasiId", "BankoNo" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Bankolar",
                column: "HizmetBinasiId1");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubConnections_Tc_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubConnections",
                columns: new[] { "TcKimlikNo", "ConnectionId", "ConnectionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_Tv_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                columns: new[] { "TvId", "ConnectionId", "ConnectionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "TvId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemleri_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemleri_KanalAlt_KanalIslem",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                columns: new[] { "KanalAltId", "KanalIslemId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemleri_KanalIslemId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KanalIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemleri_KioskIslemGrupId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KioskIslemGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalIslemleri_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalIslemleri_Kanal_HizmetBinasi",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                columns: new[] { "KanalId", "HizmetBinasiId" },
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Kanallar_KanalAdi",
                schema: "dbo",
                table: "SIR_Kanallar",
                column: "KanalAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanallarAlt_Kanal_KanalAltAdi",
                schema: "dbo",
                table: "SIR_KanallarAlt",
                columns: new[] { "KanalId", "KanalAltAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonelleri_KanalAltIslem_Tc",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                columns: new[] { "KanalAltIslemId", "TcKimlikNo" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonelleri_PersonelTcKimlikNo",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonelleri_TcKimlikNo",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                column: "TcKimlikNo");

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

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_BeklemeDurum",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "BeklemeDurum");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_KanalAltIslem_BeklemeDurum",
                schema: "dbo",
                table: "SIR_Siralar",
                columns: new[] { "KanalAltIslemId", "BeklemeDurum" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_SiraAlisZamani",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "SiraAlisZamani");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_SiraNo_HizmetBinasi_Zaman",
                schema: "dbo",
                table: "SIR_Siralar",
                columns: new[] { "SiraNo", "HizmetBinasiId", "SiraAlisZamani" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_TcKimlikNo",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_TvBankolari_BankoId",
                schema: "dbo",
                table: "SIR_TvBankolari",
                column: "BankoId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_TvBankolari_Tv_Banko",
                schema: "dbo",
                table: "SIR_TvBankolari",
                columns: new[] { "TvId", "BankoId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_Aktiflik",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "TvAktiflik");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "HizmetBinasiId1");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_TvAdi",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "TvAdi",
                unique: true,
                filter: "[SilindiMi] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CMN_DatabaseLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CMN_LoginLogoutLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ModulAlt");

            migrationBuilder.DropTable(
                name: "PER_PersonelCezalari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_PersonelCocuklari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_PersonelDepartmanlar",
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

            migrationBuilder.DropTable(
                name: "PER_PersonelYetkileri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_BankoIslemleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_BankoKullanicilari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_HubConnections",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_HubTvConnections",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KanalPersonelleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_Siralar",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_TvBankolari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_ModulControllerIslemleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Yetkiler",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CMN_Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KanalAltIslemleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_Bankolar",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_Tvler",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_ModulControllers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Personeller",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KanalIslemleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskIslemGruplari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Moduller",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CMN_Ilceler",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_AtanmaNedenleri",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Sendikalar",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Servisler",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Unvanlar",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CMN_HizmetBinalari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KanallarAlt",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_KioskGruplari",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CMN_Iller",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PER_Departmanlar",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SIR_Kanallar",
                schema: "dbo");
        }
    }
}
