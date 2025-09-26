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
            migrationBuilder.CreateTable(
                name: "CMN_DatabaseLoglar",
                columns: table => new
                {
                    DatabaseLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BeforeData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AfterData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_DatabaseLoglar", x => x.DatabaseLogId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_Iller",
                columns: table => new
                {
                    IlId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_Iller", x => x.IlId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_LoginLogoutLoglar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SessionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_LoginLogoutLoglar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CMN_Modullar",
                columns: table => new
                {
                    ModulId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_Modullar", x => x.ModulId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_Userlar",
                columns: table => new
                {
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasarisizGirisSayisi = table.Column<int>(type: "int", nullable: false),
                    HesapKilitTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_Userlar", x => x.TcKimlikNo);
                });

            migrationBuilder.CreateTable(
                name: "PER_AtanmaNedenlerilar",
                columns: table => new
                {
                    AtanmaNedeniId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtanmaNedeni = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_AtanmaNedenlerilar", x => x.AtanmaNedeniId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Departmanlar",
                columns: table => new
                {
                    DepartmanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmanAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DepartmanAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Departmanlar", x => x.DepartmanId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Sendikalar",
                columns: table => new
                {
                    SendikaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendikaAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Sendikalar", x => x.SendikaId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Servisler",
                columns: table => new
                {
                    ServisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServisAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ServisAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Servisler", x => x.ServisId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Unvanlar",
                columns: table => new
                {
                    UnvanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnvanAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnvanAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Unvanlar", x => x.UnvanId);
                });

            migrationBuilder.CreateTable(
                name: "PER_Yetkiler",
                columns: table => new
                {
                    YetkiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YetkiTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YetkiAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UstYetkiId = table.Column<int>(type: "int", nullable: true),
                    ControllerAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Yetkiler", x => x.YetkiId);
                    table.ForeignKey(
                        name: "FK_PER_Yetkiler_PER_Yetkiler_UstYetkiId",
                        column: x => x.UstYetkiId,
                        principalTable: "PER_Yetkiler",
                        principalColumn: "YetkiId");
                });

            migrationBuilder.CreateTable(
                name: "SIR_BankoIslemlar",
                columns: table => new
                {
                    BankoIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankoGrup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankoUstIslemId = table.Column<int>(type: "int", nullable: false),
                    BankoIslemAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BankoIslemSira = table.Column<int>(type: "int", nullable: false),
                    BankoIslemAktiflik = table.Column<int>(type: "int", nullable: false),
                    DiffLang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_BankoIslemlar", x => x.BankoIslemId);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Kanallar",
                columns: table => new
                {
                    KanalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_Kanallar", x => x.KanalId);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskGruplar",
                columns: table => new
                {
                    KioskGrupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskGrupAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KioskGruplar", x => x.KioskGrupId);
                });

            migrationBuilder.CreateTable(
                name: "CMN_Ilceler",
                columns: table => new
                {
                    IlceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlId = table.Column<int>(type: "int", nullable: false),
                    IlceAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_Ilceler", x => x.IlceId);
                    table.ForeignKey(
                        name: "FK_CMN_Ilceler_CMN_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "CMN_Iller",
                        principalColumn: "IlId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMN_ModulAltlar",
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
                    table.PrimaryKey("PK_CMN_ModulAltlar", x => x.ModulAltId);
                    table.ForeignKey(
                        name: "FK_CMN_ModulAltlar_CMN_Modullar_ModulId",
                        column: x => x.ModulId,
                        principalTable: "CMN_Modullar",
                        principalColumn: "ModulId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CMN_ModulControllerlar",
                columns: table => new
                {
                    ModulControllerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulControllerAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModulId = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_ModulControllerlar", x => x.ModulControllerId);
                    table.ForeignKey(
                        name: "FK_CMN_ModulControllerlar_CMN_Modullar_ModulId",
                        column: x => x.ModulId,
                        principalTable: "CMN_Modullar",
                        principalColumn: "ModulId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMN_HizmetBinalari",
                columns: table => new
                {
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DepartmanId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_HizmetBinalari", x => x.HizmetBinasiId);
                    table.ForeignKey(
                        name: "FK_CMN_HizmetBinalari_PER_Departmanlar_DepartmanId",
                        column: x => x.DepartmanId,
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalAltlar",
                columns: table => new
                {
                    KanalAltId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalAltAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    KanalId = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KanalAltlar", x => x.KanalAltId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltlar_SIR_Kanallar_KanalId",
                        column: x => x.KanalId,
                        principalTable: "SIR_Kanallar",
                        principalColumn: "KanalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMN_ModulControllerIslemlar",
                columns: table => new
                {
                    ModulControllerIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModulControllerIslemAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModulControllerId = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMN_ModulControllerIslemlar", x => x.ModulControllerIslemId);
                    table.ForeignKey(
                        name: "FK_CMN_ModulControllerIslemlar_CMN_ModulControllerlar_ModulControllerId",
                        column: x => x.ModulControllerId,
                        principalTable: "CMN_ModulControllerlar",
                        principalColumn: "ModulControllerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PER_Personeller",
                columns: table => new
                {
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    SicilNo = table.Column<int>(type: "int", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                    SendikaId = table.Column<int>(type: "int", nullable: false),
                    EsininIsIlId = table.Column<int>(type: "int", nullable: false),
                    EsininIsIlceId = table.Column<int>(type: "int", nullable: false),
                    Gorev = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Uzmanlik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PersonelTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dahili = table.Column<int>(type: "int", nullable: false),
                    CepTelefonu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CepTelefonu2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EvTelefonu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Semt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cinsiyet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedeniDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KanGrubu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UlasimServis1 = table.Column<int>(type: "int", nullable: false),
                    UlasimServis2 = table.Column<int>(type: "int", nullable: false),
                    Tabldot = table.Column<int>(type: "int", nullable: false),
                    PersonelAktiflikDurum = table.Column<int>(type: "int", nullable: false),
                    EmekliSicilNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OgrenimDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BitirdigiOkul = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BitirdigiBolum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OgrenimSuresi = table.Column<int>(type: "int", nullable: false),
                    Bransi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SehitYakinligi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsininAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EsininIsDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsininUnvani = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EsininIsAdresi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EsininIsSemt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HizmetBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EgitimBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImzaYetkileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CezaBilgileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EngelBilgileri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resim = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PassWord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SessionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_Personeller", x => x.TcKimlikNo);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Ilceler_EsininIsIlceId",
                        column: x => x.EsininIsIlceId,
                        principalTable: "CMN_Ilceler",
                        principalColumn: "IlceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "CMN_Ilceler",
                        principalColumn: "IlceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Iller_EsininIsIlId",
                        column: x => x.EsininIsIlId,
                        principalTable: "CMN_Iller",
                        principalColumn: "IlId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_CMN_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "CMN_Iller",
                        principalColumn: "IlId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_AtanmaNedenlerilar_AtanmaNedeniId",
                        column: x => x.AtanmaNedeniId,
                        principalTable: "PER_AtanmaNedenlerilar",
                        principalColumn: "AtanmaNedeniId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Departmanlar_DepartmanId",
                        column: x => x.DepartmanId,
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Sendikalar_SendikaId",
                        column: x => x.SendikaId,
                        principalTable: "PER_Sendikalar",
                        principalColumn: "SendikaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Servisler_ServisId",
                        column: x => x.ServisId,
                        principalTable: "PER_Servisler",
                        principalColumn: "ServisId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_Personeller_PER_Unvanlar_UnvanId",
                        column: x => x.UnvanId,
                        principalTable: "PER_Unvanlar",
                        principalColumn: "UnvanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Bankolar",
                columns: table => new
                {
                    BankoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    BankoNo = table.Column<int>(type: "int", nullable: false),
                    BankoTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KatTipi = table.Column<int>(type: "int", nullable: false),
                    BankoAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_Bankolar", x => x.BankoId);
                    table.ForeignKey(
                        name: "FK_SIR_Bankolar_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalIslemlar",
                columns: table => new
                {
                    KanalIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    BaslangicNumara = table.Column<int>(type: "int", nullable: false),
                    BitisNumara = table.Column<int>(type: "int", nullable: false),
                    KanalIslemAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KanalIslemlar", x => x.KanalIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalIslemlar_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIR_KanalIslemlar_SIR_Kanallar_KanalId",
                        column: x => x.KanalId,
                        principalTable: "SIR_Kanallar",
                        principalColumn: "KanalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Tvler",
                columns: table => new
                {
                    TvId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KatTipi = table.Column<int>(type: "int", nullable: false),
                    Aktiflik = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_Tvler", x => x.TvId);
                    table.ForeignKey(
                        name: "FK_SIR_Tvler_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KioskIslemGruplar",
                columns: table => new
                {
                    KioskIslemGrupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KioskGrupId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KioskIslemGrupSira = table.Column<int>(type: "int", nullable: false),
                    KioskIslemGrupAktiflik = table.Column<int>(type: "int", nullable: false),
                    KanalAltId = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KioskIslemGruplar", x => x.KioskIslemGrupId);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplar_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplar_SIR_KanalAltlar_KanalAltId",
                        column: x => x.KanalAltId,
                        principalTable: "SIR_KanalAltlar",
                        principalColumn: "KanalAltId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIR_KioskIslemGruplar_SIR_KioskGruplar_KioskGrupId",
                        column: x => x.KioskGrupId,
                        principalTable: "SIR_KioskGruplar",
                        principalColumn: "KioskGrupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelCocuklari",
                columns: table => new
                {
                    PersonelCocukId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    CocukAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CocukDogumTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    OgrenimDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_PersonelCocuklari", x => x.PersonelCocukId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelCocuklari_PER_Personeller_PersonelTcKimlikNo",
                        column: x => x.PersonelTcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelDepartmanlar",
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
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_PersonelDepartmanlar", x => x.PersonelDepartmanId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelDepartmanlar_PER_Departmanlar_DepartmanId",
                        column: x => x.DepartmanId,
                        principalTable: "PER_Departmanlar",
                        principalColumn: "DepartmanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PER_PersonelDepartmanlar_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PER_PersonelYetkileri",
                columns: table => new
                {
                    PersonelYetkiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    YetkiId = table.Column<int>(type: "int", nullable: false),
                    ModulControllerIslemId = table.Column<int>(type: "int", nullable: false),
                    YetkiTipleri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PER_PersonelYetkileri", x => x.PersonelYetkiId);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_CMN_ModulControllerIslemlar_ModulControllerIslemId",
                        column: x => x.ModulControllerIslemId,
                        principalTable: "CMN_ModulControllerIslemlar",
                        principalColumn: "ModulControllerIslemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PER_PersonelYetkileri_PER_Yetkiler_YetkiId",
                        column: x => x.YetkiId,
                        principalTable: "PER_Yetkiler",
                        principalColumn: "YetkiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIR_HubConnectionlar",
                columns: table => new
                {
                    HubConnectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ConnectionStatus = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_HubConnectionlar", x => x.HubConnectionId);
                    table.ForeignKey(
                        name: "FK_SIR_HubConnectionlar_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_BankoKullanicilari",
                columns: table => new
                {
                    BankoKullaniciId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankoId = table.Column<int>(type: "int", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_BankoKullanicilari", x => x.BankoKullaniciId);
                    table.ForeignKey(
                        name: "FK_SIR_BankoKullanicilari_PER_Personeller_PersonelTcKimlikNo",
                        column: x => x.PersonelTcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo");
                    table.ForeignKey(
                        name: "FK_SIR_BankoKullanicilari_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_BankoKullanicilari_SIR_Bankolar_BankoId",
                        column: x => x.BankoId,
                        principalTable: "SIR_Bankolar",
                        principalColumn: "BankoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_HubTvConnectionlar",
                columns: table => new
                {
                    HubTvConnectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvId = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ConnectionStatus = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_HubTvConnectionlar", x => x.HubTvConnectionId);
                    table.ForeignKey(
                        name: "FK_SIR_HubTvConnectionlar_SIR_Tvler_TvId",
                        column: x => x.TvId,
                        principalTable: "SIR_Tvler",
                        principalColumn: "TvId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_TvBankolari",
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
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_TvBankolari", x => x.TvBankoId);
                    table.ForeignKey(
                        name: "FK_SIR_TvBankolari_SIR_Bankolar_BankoId",
                        column: x => x.BankoId,
                        principalTable: "SIR_Bankolar",
                        principalColumn: "BankoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_TvBankolari_SIR_Tvler_TvId",
                        column: x => x.TvId,
                        principalTable: "SIR_Tvler",
                        principalColumn: "TvId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalAltIslemlar",
                columns: table => new
                {
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KanalAltId = table.Column<int>(type: "int", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    KanalIslemId = table.Column<int>(type: "int", nullable: false),
                    KioskIslemGrupId = table.Column<int>(type: "int", nullable: true),
                    KanalAltIslemAktiflik = table.Column<int>(type: "int", nullable: false),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KanalAltIslemlar", x => x.KanalAltIslemId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemlar_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemlar_SIR_KanalAltlar_KanalAltId",
                        column: x => x.KanalAltId,
                        principalTable: "SIR_KanalAltlar",
                        principalColumn: "KanalAltId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemlar_SIR_KanalIslemlar_KanalIslemId",
                        column: x => x.KanalIslemId,
                        principalTable: "SIR_KanalIslemlar",
                        principalColumn: "KanalIslemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalAltIslemlar_SIR_KioskIslemGruplar_KioskIslemGrupId",
                        column: x => x.KioskIslemGrupId,
                        principalTable: "SIR_KioskIslemGruplar",
                        principalColumn: "KioskIslemGrupId");
                });

            migrationBuilder.CreateTable(
                name: "SIR_KanalPersonellar",
                columns: table => new
                {
                    KanalPersonelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    Uzmanlik = table.Column<int>(type: "int", nullable: false),
                    KanalAltIslemPersonelAktiflik = table.Column<int>(type: "int", nullable: false),
                    PersonelTcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_KanalPersonellar", x => x.KanalPersonelId);
                    table.ForeignKey(
                        name: "FK_SIR_KanalPersonellar_PER_Personeller_PersonelTcKimlikNo",
                        column: x => x.PersonelTcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo");
                    table.ForeignKey(
                        name: "FK_SIR_KanalPersonellar_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_KanalPersonellar_SIR_KanalAltIslemlar_KanalAltIslemId",
                        column: x => x.KanalAltIslemId,
                        principalTable: "SIR_KanalAltIslemlar",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SIR_Siralar",
                columns: table => new
                {
                    SiraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    KanalAltIslemId = table.Column<int>(type: "int", nullable: false),
                    KanalAltAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HizmetBinasiId = table.Column<int>(type: "int", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    SiraAlisZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemBaslamaZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IslemBitisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BeklemeDurum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIR_Siralar", x => x.SiraId);
                    table.ForeignKey(
                        name: "FK_SIR_Siralar_CMN_HizmetBinalari_HizmetBinasiId",
                        column: x => x.HizmetBinasiId,
                        principalTable: "CMN_HizmetBinalari",
                        principalColumn: "HizmetBinasiId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SIR_Siralar_PER_Personeller_TcKimlikNo",
                        column: x => x.TcKimlikNo,
                        principalTable: "PER_Personeller",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SIR_Siralar_SIR_KanalAltIslemlar_KanalAltIslemId",
                        column: x => x.KanalAltIslemId,
                        principalTable: "SIR_KanalAltIslemlar",
                        principalColumn: "KanalAltIslemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HizmetBinalari_DepartmanId",
                table: "CMN_HizmetBinalari",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Ilceler_IlId_IlceAdi",
                table: "CMN_Ilceler",
                columns: new[] { "IlId", "IlceAdi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Iller_IlAdi",
                table: "CMN_Iller",
                column: "IlAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_ModulAltlar_ModulId",
                table: "CMN_ModulAltlar",
                column: "ModulId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_ModulControllerIslemlar_ModulControllerId",
                table: "CMN_ModulControllerIslemlar",
                column: "ModulControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_ModulControllerlar_ModulId",
                table: "CMN_ModulControllerlar",
                column: "ModulId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Userlar_Email",
                table: "CMN_Userlar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Userlar_KullaniciAdi",
                table: "CMN_Userlar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Userlar_TcKimlikNo",
                table: "CMN_Userlar",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Departmanlar_DepartmanAdi",
                table: "PER_Departmanlar",
                column: "DepartmanAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelCocuklari_PersonelTcKimlikNo",
                table: "PER_PersonelCocuklari",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelDepartmanlar_DepartmanId",
                table: "PER_PersonelDepartmanlar",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelDepartmanlar_TcKimlikNo",
                table: "PER_PersonelDepartmanlar",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_AtanmaNedeniId",
                table: "PER_Personeller",
                column: "AtanmaNedeniId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_DepartmanId_PersonelAktiflikDurum",
                table: "PER_Personeller",
                columns: new[] { "DepartmanId", "PersonelAktiflikDurum" });

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_Email",
                table: "PER_Personeller",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_EsininIsIlceId",
                table: "PER_Personeller",
                column: "EsininIsIlceId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_EsininIsIlId",
                table: "PER_Personeller",
                column: "EsininIsIlId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_HizmetBinasiId_PersonelAktiflikDurum",
                table: "PER_Personeller",
                columns: new[] { "HizmetBinasiId", "PersonelAktiflikDurum" });

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_IlceId",
                table: "PER_Personeller",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_IlId",
                table: "PER_Personeller",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_PersonelAktiflikDurum",
                table: "PER_Personeller",
                column: "PersonelAktiflikDurum");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_SendikaId",
                table: "PER_Personeller",
                column: "SendikaId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_ServisId",
                table: "PER_Personeller",
                column: "ServisId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_SicilNo",
                table: "PER_Personeller",
                column: "SicilNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_TcKimlikNo",
                table: "PER_Personeller",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_UnvanId",
                table: "PER_Personeller",
                column: "UnvanId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_ModulControllerIslemId",
                table: "PER_PersonelYetkileri",
                column: "ModulControllerIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_TcKimlikNo",
                table: "PER_PersonelYetkileri",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_YetkiId",
                table: "PER_PersonelYetkileri",
                column: "YetkiId");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Sendikalar_SendikaAdi",
                table: "PER_Sendikalar",
                column: "SendikaAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Servisler_ServisAdi",
                table: "PER_Servisler",
                column: "ServisAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Unvanlar_UnvanAdi",
                table: "PER_Unvanlar",
                column: "UnvanAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_ControllerAdi",
                table: "PER_Yetkiler",
                column: "ControllerAdi");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_UstYetkiId_YetkiAdi",
                table: "PER_Yetkiler",
                columns: new[] { "UstYetkiId", "YetkiAdi" },
                unique: true,
                filter: "[UstYetkiId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_YetkiAdi",
                table: "PER_Yetkiler",
                column: "YetkiAdi");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_BankoId",
                table: "SIR_BankoKullanicilari",
                column: "BankoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_PersonelTcKimlikNo",
                table: "SIR_BankoKullanicilari",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_TcKimlikNo",
                table: "SIR_BankoKullanicilari",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_HizmetBinasiId_BankoNo",
                table: "SIR_Bankolar",
                columns: new[] { "HizmetBinasiId", "BankoNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubConnectionlar_TcKimlikNo",
                table: "SIR_HubConnectionlar",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubConnectionlar_TcKimlikNo_ConnectionId_ConnectionStatus",
                table: "SIR_HubConnectionlar",
                columns: new[] { "TcKimlikNo", "ConnectionId", "ConnectionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnectionlar_TvId",
                table: "SIR_HubTvConnectionlar",
                column: "TvId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnectionlar_TvId_ConnectionId_ConnectionStatus",
                table: "SIR_HubTvConnectionlar",
                columns: new[] { "TvId", "ConnectionId", "ConnectionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemlar_HizmetBinasiId",
                table: "SIR_KanalAltIslemlar",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemlar_KanalAltId",
                table: "SIR_KanalAltIslemlar",
                column: "KanalAltId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemlar_KanalIslemId",
                table: "SIR_KanalAltIslemlar",
                column: "KanalIslemId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemlar_KioskIslemGrupId",
                table: "SIR_KanalAltIslemlar",
                column: "KioskIslemGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltlar_KanalId",
                table: "SIR_KanalAltlar",
                column: "KanalId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalIslemlar_HizmetBinasiId",
                table: "SIR_KanalIslemlar",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalIslemlar_KanalId",
                table: "SIR_KanalIslemlar",
                column: "KanalId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonellar_KanalAltIslemId_TcKimlikNo",
                table: "SIR_KanalPersonellar",
                columns: new[] { "KanalAltIslemId", "TcKimlikNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonellar_PersonelTcKimlikNo",
                table: "SIR_KanalPersonellar",
                column: "PersonelTcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonellar_TcKimlikNo",
                table: "SIR_KanalPersonellar",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplar_HizmetBinasiId",
                table: "SIR_KioskIslemGruplar",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplar_KanalAltId",
                table: "SIR_KioskIslemGruplar",
                column: "KanalAltId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplar_KioskGrupId",
                table: "SIR_KioskIslemGruplar",
                column: "KioskGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_BeklemeDurum",
                table: "SIR_Siralar",
                column: "BeklemeDurum");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_HizmetBinasiId",
                table: "SIR_Siralar",
                column: "HizmetBinasiId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_KanalAltIslemId_BeklemeDurum",
                table: "SIR_Siralar",
                columns: new[] { "KanalAltIslemId", "BeklemeDurum" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_SiraAlisZamani",
                table: "SIR_Siralar",
                column: "SiraAlisZamani");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_SiraNo_HizmetBinasiId_SiraAlisZamani",
                table: "SIR_Siralar",
                columns: new[] { "SiraNo", "HizmetBinasiId", "SiraAlisZamani" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_TcKimlikNo",
                table: "SIR_Siralar",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_TvBankolari_BankoId",
                table: "SIR_TvBankolari",
                column: "BankoId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_TvBankolari_TvId_BankoId",
                table: "SIR_TvBankolari",
                columns: new[] { "TvId", "BankoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_HizmetBinasiId",
                table: "SIR_Tvler",
                column: "HizmetBinasiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CMN_DatabaseLoglar");

            migrationBuilder.DropTable(
                name: "CMN_LoginLogoutLoglar");

            migrationBuilder.DropTable(
                name: "CMN_ModulAltlar");

            migrationBuilder.DropTable(
                name: "CMN_Userlar");

            migrationBuilder.DropTable(
                name: "PER_PersonelCocuklari");

            migrationBuilder.DropTable(
                name: "PER_PersonelDepartmanlar");

            migrationBuilder.DropTable(
                name: "PER_PersonelYetkileri");

            migrationBuilder.DropTable(
                name: "SIR_BankoIslemlar");

            migrationBuilder.DropTable(
                name: "SIR_BankoKullanicilari");

            migrationBuilder.DropTable(
                name: "SIR_HubConnectionlar");

            migrationBuilder.DropTable(
                name: "SIR_HubTvConnectionlar");

            migrationBuilder.DropTable(
                name: "SIR_KanalPersonellar");

            migrationBuilder.DropTable(
                name: "SIR_Siralar");

            migrationBuilder.DropTable(
                name: "SIR_TvBankolari");

            migrationBuilder.DropTable(
                name: "CMN_ModulControllerIslemlar");

            migrationBuilder.DropTable(
                name: "PER_Yetkiler");

            migrationBuilder.DropTable(
                name: "PER_Personeller");

            migrationBuilder.DropTable(
                name: "SIR_KanalAltIslemlar");

            migrationBuilder.DropTable(
                name: "SIR_Bankolar");

            migrationBuilder.DropTable(
                name: "SIR_Tvler");

            migrationBuilder.DropTable(
                name: "CMN_ModulControllerlar");

            migrationBuilder.DropTable(
                name: "CMN_Ilceler");

            migrationBuilder.DropTable(
                name: "PER_AtanmaNedenlerilar");

            migrationBuilder.DropTable(
                name: "PER_Sendikalar");

            migrationBuilder.DropTable(
                name: "PER_Servisler");

            migrationBuilder.DropTable(
                name: "PER_Unvanlar");

            migrationBuilder.DropTable(
                name: "SIR_KanalIslemlar");

            migrationBuilder.DropTable(
                name: "SIR_KioskIslemGruplar");

            migrationBuilder.DropTable(
                name: "CMN_Modullar");

            migrationBuilder.DropTable(
                name: "CMN_Iller");

            migrationBuilder.DropTable(
                name: "CMN_HizmetBinalari");

            migrationBuilder.DropTable(
                name: "SIR_KanalAltlar");

            migrationBuilder.DropTable(
                name: "SIR_KioskGruplar");

            migrationBuilder.DropTable(
                name: "PER_Departmanlar");

            migrationBuilder.DropTable(
                name: "SIR_Kanallar");
        }
    }
}
