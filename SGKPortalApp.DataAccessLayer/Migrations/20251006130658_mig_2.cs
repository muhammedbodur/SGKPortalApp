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
            migrationBuilder.DropForeignKey(
                name: "FK_CMN_HizmetBinalari_PER_Departmanlar_DepartmanId",
                table: "CMN_HizmetBinalari");

            migrationBuilder.DropForeignKey(
                name: "FK_CMN_Ilceler_CMN_Iller_IlId",
                table: "CMN_Ilceler");

            migrationBuilder.DropForeignKey(
                name: "FK_CMN_ModulAltlar_CMN_Modullar_ModulId",
                table: "CMN_ModulAltlar");

            migrationBuilder.DropForeignKey(
                name: "FK_CMN_ModulControllerIslemlar_CMN_ModulControllerlar_ModulControllerId",
                table: "CMN_ModulControllerIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_CMN_ModulControllerlar_CMN_Modullar_ModulId",
                table: "CMN_ModulControllerlar");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelCocuklari_PER_Personeller_PersonelTcKimlikNo",
                table: "PER_PersonelCocuklari");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Departmanlar_DepartmanId",
                table: "PER_PersonelDepartmanlar");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Personeller_TcKimlikNo",
                table: "PER_PersonelDepartmanlar");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_CMN_HizmetBinalari_HizmetBinasiId",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_AtanmaNedenlerilar_AtanmaNedeniId",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Departmanlar_DepartmanId",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Sendikalar_SendikaId",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Servisler_ServisId",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Unvanlar_UnvanId",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_CMN_ModulControllerIslemlar_ModulControllerIslemId",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Personeller_TcKimlikNo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Yetkiler_YetkiId",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_BankoKullanicilari_PER_Personeller_TcKimlikNo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_BankoKullanicilari_SIR_Bankolar_BankoId",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Bankolar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_Bankolar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubConnectionlar_PER_Personeller_TcKimlikNo",
                table: "SIR_HubConnectionlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubTvConnectionlar_SIR_Tvler_TvId",
                table: "SIR_HubTvConnectionlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemlar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_KanalAltIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemlar_SIR_KanalAltlar_KanalAltId",
                table: "SIR_KanalAltIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemlar_SIR_KanalIslemlar_KanalIslemId",
                table: "SIR_KanalAltIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemlar_SIR_KioskIslemGruplar_KioskIslemGrupId",
                table: "SIR_KanalAltIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltlar_SIR_Kanallar_KanalId",
                table: "SIR_KanalAltlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalIslemlar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_KanalIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalIslemlar_SIR_Kanallar_KanalId",
                table: "SIR_KanalIslemlar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalPersonellar_PER_Personeller_PersonelTcKimlikNo",
                table: "SIR_KanalPersonellar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalPersonellar_PER_Personeller_TcKimlikNo",
                table: "SIR_KanalPersonellar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalPersonellar_SIR_KanalAltIslemlar_KanalAltIslemId",
                table: "SIR_KanalPersonellar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_KioskIslemGruplar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplar_SIR_KanalAltlar_KanalAltId",
                table: "SIR_KioskIslemGruplar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplar_SIR_KioskGruplar_KioskGrupId",
                table: "SIR_KioskIslemGruplar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_Siralar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_SIR_KanalAltIslemlar_KanalAltIslemId",
                table: "SIR_Siralar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Bankolar_BankoId",
                table: "SIR_TvBankolari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Tvler_TvId",
                table: "SIR_TvBankolari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Tvler_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_TvBankolari_TvId_BankoId",
                table: "SIR_TvBankolari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Siralar_SiraNo_HizmetBinasiId_SiraAlisZamani",
                table: "SIR_Siralar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_HizmetBinasiId_BankoNo",
                table: "SIR_Bankolar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_BankoKullanicilari_BankoId",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_BankoKullanicilari_TcKimlikNo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropIndex(
                name: "IX_PER_Yetkiler_UstYetkiId_YetkiAdi",
                table: "PER_Yetkiler");

            migrationBuilder.DropIndex(
                name: "IX_PER_Unvanlar_UnvanAdi",
                table: "PER_Unvanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Servisler_ServisAdi",
                table: "PER_Servisler");

            migrationBuilder.DropIndex(
                name: "IX_PER_Sendikalar_SendikaAdi",
                table: "PER_Sendikalar");

            migrationBuilder.DropIndex(
                name: "IX_PER_PersonelYetkileri_TcKimlikNo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropIndex(
                name: "IX_PER_Personeller_Email",
                table: "PER_Personeller");

            migrationBuilder.DropIndex(
                name: "IX_PER_PersonelDepartmanlar_TcKimlikNo",
                table: "PER_PersonelDepartmanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Departmanlar_DepartmanAdi",
                table: "PER_Departmanlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Iller_IlAdi",
                table: "CMN_Iller");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Ilceler_IlId_IlceAdi",
                table: "CMN_Ilceler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KioskIslemGruplar",
                table: "SIR_KioskIslemGruplar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskIslemGruplar_KioskGrupId",
                table: "SIR_KioskIslemGruplar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KioskGruplar",
                table: "SIR_KioskGruplar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalPersonellar",
                table: "SIR_KanalPersonellar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalPersonellar_KanalAltIslemId_TcKimlikNo",
                table: "SIR_KanalPersonellar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalIslemlar",
                table: "SIR_KanalIslemlar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalIslemlar_KanalId",
                table: "SIR_KanalIslemlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalAltlar",
                table: "SIR_KanalAltlar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalAltlar_KanalId",
                table: "SIR_KanalAltlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalAltIslemlar",
                table: "SIR_KanalAltIslemlar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalAltIslemlar_KanalAltId",
                table: "SIR_KanalAltIslemlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_HubTvConnectionlar",
                table: "SIR_HubTvConnectionlar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnectionlar_TvId",
                table: "SIR_HubTvConnectionlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_HubConnectionlar",
                table: "SIR_HubConnectionlar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubConnectionlar_TcKimlikNo",
                table: "SIR_HubConnectionlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_BankoIslemlar",
                table: "SIR_BankoIslemlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PER_AtanmaNedenlerilar",
                table: "PER_AtanmaNedenlerilar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_Userlar",
                table: "CMN_Userlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Userlar_Email",
                table: "CMN_Userlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Userlar_KullaniciAdi",
                table: "CMN_Userlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Userlar_TcKimlikNo",
                table: "CMN_Userlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_Modullar",
                table: "CMN_Modullar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_ModulControllerlar",
                table: "CMN_ModulControllerlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_ModulControllerlar_ModulId",
                table: "CMN_ModulControllerlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_ModulControllerIslemlar",
                table: "CMN_ModulControllerIslemlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_ModulControllerIslemlar_ModulControllerId",
                table: "CMN_ModulControllerIslemlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_ModulAltlar",
                table: "CMN_ModulAltlar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_LoginLogoutLoglar",
                table: "CMN_LoginLogoutLoglar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_DatabaseLoglar",
                table: "CMN_DatabaseLoglar");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_Tvler",
                newName: "SIR_Tvler",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_TvBankolari",
                newName: "SIR_TvBankolari",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_Siralar",
                newName: "SIR_Siralar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_Kanallar",
                newName: "SIR_Kanallar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_Bankolar",
                newName: "SIR_Bankolar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_BankoKullanicilari",
                newName: "SIR_BankoKullanicilari",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_Yetkiler",
                newName: "PER_Yetkiler",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_Unvanlar",
                newName: "PER_Unvanlar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_Servisler",
                newName: "PER_Servisler",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_Sendikalar",
                newName: "PER_Sendikalar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_PersonelYetkileri",
                newName: "PER_PersonelYetkileri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_Personeller",
                newName: "PER_Personeller",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_PersonelDepartmanlar",
                newName: "PER_PersonelDepartmanlar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_PersonelCocuklari",
                newName: "PER_PersonelCocuklari",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_Departmanlar",
                newName: "PER_Departmanlar",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_Iller",
                newName: "CMN_Iller",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_Ilceler",
                newName: "CMN_Ilceler",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_HizmetBinalari",
                newName: "CMN_HizmetBinalari",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_KioskIslemGruplar",
                newName: "SIR_KioskIslemGruplari",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_KioskGruplar",
                newName: "SIR_KioskGruplari",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_KanalPersonellar",
                newName: "SIR_KanalPersonelleri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_KanalIslemlar",
                newName: "SIR_KanalIslemleri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_KanalAltlar",
                newName: "SIR_KanallarAlt",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_KanalAltIslemlar",
                newName: "SIR_KanalAltIslemleri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_HubTvConnectionlar",
                newName: "SIR_HubTvConnections",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_HubConnectionlar",
                newName: "SIR_HubConnections",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SIR_BankoIslemlar",
                newName: "SIR_BankoIslemleri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PER_AtanmaNedenlerilar",
                newName: "PER_AtanmaNedenleri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_Userlar",
                newName: "CMN_Users",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_Modullar",
                newName: "PER_Moduller",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_ModulControllerlar",
                newName: "PER_ModulControllers",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_ModulControllerIslemlar",
                newName: "PER_ModulControllerIslemleri",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_ModulAltlar",
                newName: "ModulAlt");

            migrationBuilder.RenameTable(
                name: "CMN_LoginLogoutLoglar",
                newName: "CMN_LoginLogoutLogs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CMN_DatabaseLoglar",
                newName: "CMN_DatabaseLogs",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_Siralar_KanalAltIslemId_BeklemeDurum",
                schema: "dbo",
                table: "SIR_Siralar",
                newName: "IX_SIR_Siralar_KanalAltIslem_BeklemeDurum");

            migrationBuilder.RenameIndex(
                name: "IX_PER_Personeller_PersonelAktiflikDurum",
                schema: "dbo",
                table: "PER_Personeller",
                newName: "IX_PER_Personeller_AktiflikDurum");

            migrationBuilder.RenameIndex(
                name: "IX_PER_Personeller_HizmetBinasiId_PersonelAktiflikDurum",
                schema: "dbo",
                table: "PER_Personeller",
                newName: "IX_PER_Personeller_HizmetBinasi_Aktiflik");

            migrationBuilder.RenameIndex(
                name: "IX_PER_Personeller_DepartmanId_PersonelAktiflikDurum",
                schema: "dbo",
                table: "PER_Personeller",
                newName: "IX_PER_Personeller_Departman_Aktiflik");

            migrationBuilder.RenameIndex(
                name: "IX_PER_PersonelCocuklari_PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelCocuklari",
                newName: "IX_PER_PersonelCocuklari_TcKimlikNo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KioskIslemGruplar_KanalAltId",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                newName: "IX_SIR_KioskIslemGruplari_KanalAltId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KioskIslemGruplar_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                newName: "IX_SIR_KioskIslemGruplari_HizmetBinasiId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalPersonellar_TcKimlikNo",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                newName: "IX_SIR_KanalPersonelleri_TcKimlikNo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalPersonellar_PersonelTcKimlikNo",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                newName: "IX_SIR_KanalPersonelleri_PersonelTcKimlikNo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalIslemlar_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                newName: "IX_SIR_KanalIslemleri_HizmetBinasiId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalAltIslemlar_KioskIslemGrupId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                newName: "IX_SIR_KanalAltIslemleri_KioskIslemGrupId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalAltIslemlar_KanalIslemId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                newName: "IX_SIR_KanalAltIslemleri_KanalIslemId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalAltIslemlar_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                newName: "IX_SIR_KanalAltIslemleri_HizmetBinasiId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_HubTvConnectionlar_TvId_ConnectionId_ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                newName: "IX_SIR_HubTvConnections_Tv_ConnId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_HubConnectionlar_TcKimlikNo_ConnectionId_ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubConnections",
                newName: "IX_SIR_HubConnections_Tc_ConnId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_CMN_ModulAltlar_ModulId",
                table: "ModulAlt",
                newName: "IX_ModulAlt_ModulId");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_Tvler",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "KatTipi",
                schema: "dbo",
                table: "SIR_Tvler",
                type: "int",
                nullable: false,
                comment: "Kat bilgisi",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Aktiflik",
                schema: "dbo",
                table: "SIR_Tvler",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "TV aktiflik durumu",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Tvler",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TvAdi",
                schema: "dbo",
                table: "SIR_Tvler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "TV ekran adı");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_TvBankolari",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "SiraNo",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "int",
                nullable: false,
                comment: "Sıra numarası",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SiraAlisZamani",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "datetime2",
                nullable: false,
                comment: "Sıra alış zamanı",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "BeklemeDurum",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "int",
                nullable: false,
                comment: "Bekleme durumu",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_Siralar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_Kanallar",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "KanalAdi",
                schema: "dbo",
                table: "SIR_Kanallar",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "Kanal adı",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "KatTipi",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                comment: "Kat bilgisi",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BankoTipi",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Banko tipi (Normal/Oncelikli/vb)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                comment: "Banko numarası",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BankoAktiflik",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Banko aktiflik durumu",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Bankolar",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "YetkiTuru",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_Yetkiler",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnvanAktiflik",
                schema: "dbo",
                table: "PER_Unvanlar",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Unvan aktiflik durumu",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UnvanAdi",
                schema: "dbo",
                table: "PER_Unvanlar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Unvan adı",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Unvanlar",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Servisler",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ServisAktiflik",
                schema: "dbo",
                table: "PER_Servisler",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Servis aktiflik durumu",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ServisAdi",
                schema: "dbo",
                table: "PER_Servisler",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "Servis adı",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "SendikaAdi",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "Sendika adı",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_Sendikalar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "YetkiTipleri",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "nvarchar(11)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Personeller",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "SicilNo",
                schema: "dbo",
                table: "PER_Personeller",
                type: "int",
                nullable: false,
                comment: "Personel sicil numarası",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SehitYakinligi",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PersonelTipi",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OgrenimDurumu",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MedeniDurumu",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "KanGrubu",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EvDurumu",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EsininIsDurumu",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "E-posta adresi",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Cinsiyet",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CepTelefonu",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Cep telefonu numarası",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Adres",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Adres bilgisi",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdSoyad",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Ad Soyad",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "PER_Personeller",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "TC Kimlik Numarası - Primary Key",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_PersonelCocuklari",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "OgrenimDurumu",
                schema: "dbo",
                table: "PER_PersonelCocuklari",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CocukAdi",
                schema: "dbo",
                table: "PER_PersonelCocuklari",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Departmanlar",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Soft delete flag",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_Departmanlar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Kayıt oluşturulma tarihi",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_Departmanlar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Son güncelleme tarihi",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmanAktiflik",
                schema: "dbo",
                table: "PER_Departmanlar",
                type: "int",
                nullable: false,
                defaultValue: 1,
                comment: "Departman aktiflik durumu (0: Pasif, 1: Aktif)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmanAdi",
                schema: "dbo",
                table: "PER_Departmanlar",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "Departman adı",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "CMN_Iller",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "CMN_Iller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "CMN_Iller",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "CMN_Iller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "CMN_Iller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "CMN_Iller",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "CMN_Iller",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "CMN_Ilceler",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "CMN_HizmetBinalari",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Adres",
                schema: "dbo",
                table: "CMN_HizmetBinalari",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "KioskIslemGrupAktiflik",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "KioskGrupId1",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_KioskGruplari",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "KioskGrupAdi",
                schema: "dbo",
                table: "SIR_KioskGruplari",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_KanallarAlt",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "KanalAltAdi",
                schema: "dbo",
                table: "SIR_KanallarAlt",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_HubConnections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "BankoIslemAktiflik",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BankoIslemAdi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "BankoGrup",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AtanmaNedeni",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "CMN_Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "TC Kimlik Numarası - Primary Key",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "ModulAdi",
                schema: "dbo",
                table: "PER_Moduller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_Moduller",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_Moduller",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_Moduller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_Moduller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_Moduller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Moduller",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_Moduller",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModulControllerAdi",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModulControllerIslemAdi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "DatabaseAction",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BeforeData",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AfterData",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KioskIslemGruplari",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "KioskIslemGrupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KioskGruplari",
                schema: "dbo",
                table: "SIR_KioskGruplari",
                column: "KioskGrupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalPersonelleri",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                column: "KanalPersonelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalIslemleri",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                column: "KanalIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanallarAlt",
                schema: "dbo",
                table: "SIR_KanallarAlt",
                column: "KanalAltId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalAltIslemleri",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KanalAltIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_HubTvConnections",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "HubTvConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_HubConnections",
                schema: "dbo",
                table: "SIR_HubConnections",
                column: "HubConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_BankoIslemleri",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                column: "BankoIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PER_AtanmaNedenleri",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                column: "AtanmaNedeniId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_Users",
                schema: "dbo",
                table: "CMN_Users",
                column: "TcKimlikNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PER_Moduller",
                schema: "dbo",
                table: "PER_Moduller",
                column: "ModulId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PER_ModulControllers",
                schema: "dbo",
                table: "PER_ModulControllers",
                column: "ModulControllerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PER_ModulControllerIslemleri",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                column: "ModulControllerIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModulAlt",
                table: "ModulAlt",
                column: "ModulAltId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_LoginLogoutLogs",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_DatabaseLogs",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                column: "DatabaseLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_Aktiflik",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "Aktiflik");

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

            migrationBuilder.CreateIndex(
                name: "IX_SIR_TvBankolari_Tv_Banko",
                schema: "dbo",
                table: "SIR_TvBankolari",
                columns: new[] { "TvId", "BankoId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_SiraNo_HizmetBinasi_Zaman",
                schema: "dbo",
                table: "SIR_Siralar",
                columns: new[] { "SiraNo", "HizmetBinasiId", "SiraAlisZamani" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Kanallar_KanalAdi",
                schema: "dbo",
                table: "SIR_Kanallar",
                column: "KanalAdi",
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
                name: "IX_SIR_BankoKullanicilari_BankoId",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "BankoId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_TcKimlikNo",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_UstYetki_YetkiAdi",
                schema: "dbo",
                table: "PER_Yetkiler",
                columns: new[] { "UstYetkiId", "YetkiAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

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
                name: "IX_PER_Sendikalar_SendikaAdi",
                schema: "dbo",
                table: "PER_Sendikalar",
                column: "SendikaAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

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
                name: "IX_PER_PersonelYetkileri_YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "YetkiId1");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_Email",
                schema: "dbo",
                table: "PER_Personeller",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL AND [SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelDepartmanlar_Tc_Departman",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar",
                columns: new[] { "TcKimlikNo", "DepartmanId" });

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
                name: "IX_CMN_Iller_IlAdi",
                schema: "dbo",
                table: "CMN_Iller",
                column: "IlAdi",
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
                name: "IX_CMN_HizmetBinalari_HizmetBinasiAdi",
                schema: "dbo",
                table: "CMN_HizmetBinalari",
                column: "HizmetBinasiAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

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
                name: "IX_SIR_KioskGruplari_KioskGrupAdi",
                schema: "dbo",
                table: "SIR_KioskGruplari",
                column: "KioskGrupAdi",
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
                name: "IX_SIR_KanalIslemleri_Kanal_HizmetBinasi",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                columns: new[] { "KanalId", "HizmetBinasiId" },
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanallarAlt_Kanal_KanalAltAdi",
                schema: "dbo",
                table: "SIR_KanallarAlt",
                columns: new[] { "KanalId", "KanalAltAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemleri_KanalAlt_KanalIslem",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                columns: new[] { "KanalAltId", "KanalIslemId" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "TvId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoIslemleri_BankoIslemAdi",
                schema: "dbo",
                table: "SIR_BankoIslemleri",
                column: "BankoIslemAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_AtanmaNedenleri_AtanmaNedeni",
                schema: "dbo",
                table: "PER_AtanmaNedenleri",
                column: "AtanmaNedeni",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_Email",
                schema: "dbo",
                table: "CMN_Users",
                column: "Email",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_KullaniciAdi",
                schema: "dbo",
                table: "CMN_Users",
                column: "KullaniciAdi",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_TcKimlikNo",
                schema: "dbo",
                table: "CMN_Users",
                column: "TcKimlikNo",
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
                name: "IX_PER_ModulControllers_Modul_Controller",
                schema: "dbo",
                table: "PER_ModulControllers",
                columns: new[] { "ModulId", "ModulControllerAdi" },
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PER_ModulControllerIslemleri_Controller_Islem",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                columns: new[] { "ModulControllerId", "ModulControllerIslemAdi" },
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
                name: "IX_CMN_DatabaseLogs_EklenmeTarihi",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                column: "EklenmeTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_DatabaseLogs_TableName",
                schema: "dbo",
                table: "CMN_DatabaseLogs",
                column: "TableName");

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_HizmetBinalari_PER_Departmanlar",
                schema: "dbo",
                table: "CMN_HizmetBinalari",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_Ilceler_CMN_Iller",
                schema: "dbo",
                table: "CMN_Ilceler",
                column: "IlId",
                principalSchema: "dbo",
                principalTable: "CMN_Iller",
                principalColumn: "IlId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModulAlt_PER_Moduller_ModulId",
                table: "ModulAlt",
                column: "ModulId",
                principalSchema: "dbo",
                principalTable: "PER_Moduller",
                principalColumn: "ModulId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_ModulControllerIslemleri_PER_ModulControllers",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri",
                column: "ModulControllerId",
                principalSchema: "dbo",
                principalTable: "PER_ModulControllers",
                principalColumn: "ModulControllerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_ModulControllers_PER_Moduller",
                schema: "dbo",
                table: "PER_ModulControllers",
                column: "ModulId",
                principalSchema: "dbo",
                principalTable: "PER_Moduller",
                principalColumn: "ModulId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelCocuklari_PER_Personeller",
                schema: "dbo",
                table: "PER_PersonelCocuklari",
                column: "PersonelTcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Personeller",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_CMN_HizmetBinalari",
                schema: "dbo",
                table: "PER_Personeller",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_AtanmaNedenleri",
                schema: "dbo",
                table: "PER_Personeller",
                column: "AtanmaNedeniId",
                principalSchema: "dbo",
                principalTable: "PER_AtanmaNedenleri",
                principalColumn: "AtanmaNedeniId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Departmanlar",
                schema: "dbo",
                table: "PER_Personeller",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Sendikalar",
                schema: "dbo",
                table: "PER_Personeller",
                column: "SendikaId",
                principalSchema: "dbo",
                principalTable: "PER_Sendikalar",
                principalColumn: "SendikaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Servisler",
                schema: "dbo",
                table: "PER_Personeller",
                column: "ServisId",
                principalSchema: "dbo",
                principalTable: "PER_Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Unvanlar",
                schema: "dbo",
                table: "PER_Personeller",
                column: "UnvanId",
                principalSchema: "dbo",
                principalTable: "PER_Unvanlar",
                principalColumn: "UnvanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_ModulControllerIslemleri_ModulControllerIslemId",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "ModulControllerIslemId",
                principalSchema: "dbo",
                principalTable: "PER_ModulControllerIslemleri",
                principalColumn: "ModulControllerIslemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Personeller",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Personeller_PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "PersonelTcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Yetkiler",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "YetkiId",
                principalSchema: "dbo",
                principalTable: "PER_Yetkiler",
                principalColumn: "YetkiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Yetkiler_YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri",
                column: "YetkiId1",
                principalSchema: "dbo",
                principalTable: "PER_Yetkiler",
                principalColumn: "YetkiId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_BankoKullanicilari_PER_Personeller",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_BankoKullanicilari_SIR_Bankolar",
                schema: "dbo",
                table: "SIR_BankoKullanicilari",
                column: "BankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Bankolar_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_Bankolar",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Bankolar_CMN_HizmetBinalari_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Bankolar",
                column: "HizmetBinasiId1",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubConnections_PER_Personeller",
                schema: "dbo",
                table: "SIR_HubConnections",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubTvConnections_SIR_Tvler",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "TvId",
                principalSchema: "dbo",
                principalTable: "SIR_Tvler",
                principalColumn: "TvId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemleri_CMN_HizmetBinalari_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KanalIslemleri",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KanalIslemId",
                principalSchema: "dbo",
                principalTable: "SIR_KanalIslemleri",
                principalColumn: "KanalIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KanallarAlt",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KanalAltId",
                principalSchema: "dbo",
                principalTable: "SIR_KanallarAlt",
                principalColumn: "KanalAltId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KioskIslemGruplari",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri",
                column: "KioskIslemGrupId",
                principalSchema: "dbo",
                principalTable: "SIR_KioskIslemGruplari",
                principalColumn: "KioskIslemGrupId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalIslemleri_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalIslemleri_SIR_Kanallar",
                schema: "dbo",
                table: "SIR_KanalIslemleri",
                column: "KanalId",
                principalSchema: "dbo",
                principalTable: "SIR_Kanallar",
                principalColumn: "KanalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanallarAlt_SIR_Kanallar",
                schema: "dbo",
                table: "SIR_KanallarAlt",
                column: "KanalId",
                principalSchema: "dbo",
                principalTable: "SIR_Kanallar",
                principalColumn: "KanalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalPersonelleri_PER_Personeller",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalPersonelleri_PER_Personeller_PersonelTcKimlikNo",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                column: "PersonelTcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalPersonelleri_SIR_KanalAltIslemleri",
                schema: "dbo",
                table: "SIR_KanalPersonelleri",
                column: "KanalAltIslemId",
                principalSchema: "dbo",
                principalTable: "SIR_KanalAltIslemleri",
                principalColumn: "KanalAltIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplari_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplari_SIR_KanallarAlt",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "KanalAltId",
                principalSchema: "dbo",
                principalTable: "SIR_KanallarAlt",
                principalColumn: "KanalAltId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplari_SIR_KioskGruplari",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "KioskGrupId",
                principalSchema: "dbo",
                principalTable: "SIR_KioskGruplari",
                principalColumn: "KioskGrupId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplari_SIR_KioskGruplari_KioskGrupId1",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari",
                column: "KioskGrupId1",
                principalSchema: "dbo",
                principalTable: "SIR_KioskGruplari",
                principalColumn: "KioskGrupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_CMN_HizmetBinalari_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_SIR_KanalAltIslemleri",
                schema: "dbo",
                table: "SIR_Siralar",
                column: "KanalAltIslemId",
                principalSchema: "dbo",
                principalTable: "SIR_KanalAltIslemleri",
                principalColumn: "KanalAltIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Bankolar",
                schema: "dbo",
                table: "SIR_TvBankolari",
                column: "BankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Tvler",
                schema: "dbo",
                table: "SIR_TvBankolari",
                column: "TvId",
                principalSchema: "dbo",
                principalTable: "SIR_Tvler",
                principalColumn: "TvId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Tvler_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "HizmetBinasiId",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Tvler_CMN_HizmetBinalari_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "HizmetBinasiId1",
                principalSchema: "dbo",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CMN_HizmetBinalari_PER_Departmanlar",
                schema: "dbo",
                table: "CMN_HizmetBinalari");

            migrationBuilder.DropForeignKey(
                name: "FK_CMN_Ilceler_CMN_Iller",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropForeignKey(
                name: "FK_ModulAlt_PER_Moduller_ModulId",
                table: "ModulAlt");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_ModulControllerIslemleri_PER_ModulControllers",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_ModulControllers_PER_Moduller",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelCocuklari_PER_Personeller",
                schema: "dbo",
                table: "PER_PersonelCocuklari");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Personeller",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_CMN_HizmetBinalari",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_AtanmaNedenleri",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Departmanlar",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Sendikalar",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Servisler",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_PER_Unvanlar",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_ModulControllerIslemleri_ModulControllerIslemId",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Personeller",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Personeller_PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Yetkiler",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Yetkiler_YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_BankoKullanicilari_PER_Personeller",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_BankoKullanicilari_SIR_Bankolar",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Bankolar_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Bankolar_CMN_HizmetBinalari_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubConnections_PER_Personeller",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubTvConnections_SIR_Tvler",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemleri_CMN_HizmetBinalari_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KanalIslemleri",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KanallarAlt",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalAltIslemleri_SIR_KioskIslemGruplari",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalIslemleri_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalIslemleri_SIR_Kanallar",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanallarAlt_SIR_Kanallar",
                schema: "dbo",
                table: "SIR_KanallarAlt");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalPersonelleri_PER_Personeller",
                schema: "dbo",
                table: "SIR_KanalPersonelleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalPersonelleri_PER_Personeller_PersonelTcKimlikNo",
                schema: "dbo",
                table: "SIR_KanalPersonelleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KanalPersonelleri_SIR_KanalAltIslemleri",
                schema: "dbo",
                table: "SIR_KanalPersonelleri");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplari_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplari_SIR_KanallarAlt",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplari_SIR_KioskGruplari",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_KioskIslemGruplari_SIR_KioskGruplari_KioskGrupId1",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_CMN_HizmetBinalari_HizmetBinasiId",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Siralar_SIR_KanalAltIslemleri",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Bankolar",
                schema: "dbo",
                table: "SIR_TvBankolari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Tvler",
                schema: "dbo",
                table: "SIR_TvBankolari");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Tvler_CMN_HizmetBinalari",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_Tvler_CMN_HizmetBinalari_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Tvler_Aktiflik",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Tvler_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Tvler_TvAdi",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_TvBankolari_Tv_Banko",
                schema: "dbo",
                table: "SIR_TvBankolari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Siralar_SiraNo_HizmetBinasi_Zaman",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Kanallar_KanalAdi",
                schema: "dbo",
                table: "SIR_Kanallar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_BankoAktiflik",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_HizmetBinasi_BankoNo",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Bankolar_HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropIndex(
                name: "IX_SIR_BankoKullanicilari_BankoId",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_BankoKullanicilari_TcKimlikNo",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropIndex(
                name: "IX_PER_Yetkiler_UstYetki_YetkiAdi",
                schema: "dbo",
                table: "PER_Yetkiler");

            migrationBuilder.DropIndex(
                name: "IX_PER_Unvanlar_UnvanAdi",
                schema: "dbo",
                table: "PER_Unvanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Unvanlar_UnvanAktiflik",
                schema: "dbo",
                table: "PER_Unvanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Servisler_ServisAdi",
                schema: "dbo",
                table: "PER_Servisler");

            migrationBuilder.DropIndex(
                name: "IX_PER_Servisler_ServisAktiflik",
                schema: "dbo",
                table: "PER_Servisler");

            migrationBuilder.DropIndex(
                name: "IX_PER_Sendikalar_SendikaAdi",
                schema: "dbo",
                table: "PER_Sendikalar");

            migrationBuilder.DropIndex(
                name: "IX_PER_PersonelYetkileri_PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropIndex(
                name: "IX_PER_PersonelYetkileri_Tc_Yetki",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropIndex(
                name: "IX_PER_PersonelYetkileri_YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropIndex(
                name: "IX_PER_Personeller_Email",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropIndex(
                name: "IX_PER_PersonelDepartmanlar_Tc_Departman",
                schema: "dbo",
                table: "PER_PersonelDepartmanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Departmanlar_Aktiflik_SilindiMi",
                schema: "dbo",
                table: "PER_Departmanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Departmanlar_DepartmanAdi",
                schema: "dbo",
                table: "PER_Departmanlar");

            migrationBuilder.DropIndex(
                name: "IX_PER_Departmanlar_DepartmanAktiflik",
                schema: "dbo",
                table: "PER_Departmanlar");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Iller_IlAdi",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Ilceler_Il_IlceAdi",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HizmetBinalari_HizmetBinasiAdi",
                schema: "dbo",
                table: "CMN_HizmetBinalari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KioskIslemGruplari",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskIslemGruplari_KioskGrup_KanalAlt",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskIslemGruplari_KioskGrupId1",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KioskGruplari",
                schema: "dbo",
                table: "SIR_KioskGruplari");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KioskGruplari_KioskGrupAdi",
                schema: "dbo",
                table: "SIR_KioskGruplari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalPersonelleri",
                schema: "dbo",
                table: "SIR_KanalPersonelleri");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalPersonelleri_KanalAltIslem_Tc",
                schema: "dbo",
                table: "SIR_KanalPersonelleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanallarAlt",
                schema: "dbo",
                table: "SIR_KanallarAlt");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanallarAlt_Kanal_KanalAltAdi",
                schema: "dbo",
                table: "SIR_KanallarAlt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalIslemleri",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalIslemleri_Kanal_HizmetBinasi",
                schema: "dbo",
                table: "SIR_KanalIslemleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_KanalAltIslemleri",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropIndex(
                name: "IX_SIR_KanalAltIslemleri_KanalAlt_KanalIslem",
                schema: "dbo",
                table: "SIR_KanalAltIslemleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_HubTvConnections",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_HubConnections",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_BankoIslemleri",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropIndex(
                name: "IX_SIR_BankoIslemleri_BankoIslemAdi",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PER_Moduller",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropIndex(
                name: "IX_PER_Moduller_ModulAdi",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PER_ModulControllers",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropIndex(
                name: "IX_PER_ModulControllers_Modul_Controller",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PER_ModulControllerIslemleri",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropIndex(
                name: "IX_PER_ModulControllerIslemleri_Controller_Islem",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PER_AtanmaNedenleri",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.DropIndex(
                name: "IX_PER_AtanmaNedenleri_AtanmaNedeni",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModulAlt",
                table: "ModulAlt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_Users",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Users_Email",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Users_KullaniciAdi",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Users_TcKimlikNo",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_LoginLogoutLogs",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs");

            migrationBuilder.DropIndex(
                name: "IX_CMN_LoginLogoutLogs_EklenmeTarihi",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs");

            migrationBuilder.DropIndex(
                name: "IX_CMN_LoginLogoutLogs_TcKimlikNo",
                schema: "dbo",
                table: "CMN_LoginLogoutLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CMN_DatabaseLogs",
                schema: "dbo",
                table: "CMN_DatabaseLogs");

            migrationBuilder.DropIndex(
                name: "IX_CMN_DatabaseLogs_EklenmeTarihi",
                schema: "dbo",
                table: "CMN_DatabaseLogs");

            migrationBuilder.DropIndex(
                name: "IX_CMN_DatabaseLogs_TableName",
                schema: "dbo",
                table: "CMN_DatabaseLogs");

            migrationBuilder.DropColumn(
                name: "HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropColumn(
                name: "TvAdi",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropColumn(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_Siralar");

            migrationBuilder.DropColumn(
                name: "HizmetBinasiId1",
                schema: "dbo",
                table: "SIR_Bankolar");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_BankoKullanicilari");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_Yetkiler");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_Yetkiler");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_Yetkiler");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Yetkiler");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_Yetkiler");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_Sendikalar");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_Sendikalar");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_Sendikalar");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Sendikalar");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_Sendikalar");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "PersonelTcKimlikNo",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "YetkiId1",
                schema: "dbo",
                table: "PER_PersonelYetkileri");

            migrationBuilder.DropColumn(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "CMN_Iller");

            migrationBuilder.DropColumn(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "CMN_Ilceler");

            migrationBuilder.DropColumn(
                name: "Adres",
                schema: "dbo",
                table: "CMN_HizmetBinalari");

            migrationBuilder.DropColumn(
                name: "KioskGrupId1",
                schema: "dbo",
                table: "SIR_KioskIslemGruplari");

            migrationBuilder.DropColumn(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_HubConnections");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "SIR_BankoIslemleri");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_Moduller");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllers");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_ModulControllerIslemleri");

            migrationBuilder.DropColumn(
                name: "DuzenleyenKullanici",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.DropColumn(
                name: "EkleyenKullanici",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.DropColumn(
                name: "SilenKullanici",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.DropColumn(
                name: "SilindiMi",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.DropColumn(
                name: "SilinmeTarihi",
                schema: "dbo",
                table: "PER_AtanmaNedenleri");

            migrationBuilder.RenameTable(
                name: "SIR_Tvler",
                schema: "dbo",
                newName: "SIR_Tvler");

            migrationBuilder.RenameTable(
                name: "SIR_TvBankolari",
                schema: "dbo",
                newName: "SIR_TvBankolari");

            migrationBuilder.RenameTable(
                name: "SIR_Siralar",
                schema: "dbo",
                newName: "SIR_Siralar");

            migrationBuilder.RenameTable(
                name: "SIR_Kanallar",
                schema: "dbo",
                newName: "SIR_Kanallar");

            migrationBuilder.RenameTable(
                name: "SIR_Bankolar",
                schema: "dbo",
                newName: "SIR_Bankolar");

            migrationBuilder.RenameTable(
                name: "SIR_BankoKullanicilari",
                schema: "dbo",
                newName: "SIR_BankoKullanicilari");

            migrationBuilder.RenameTable(
                name: "PER_Yetkiler",
                schema: "dbo",
                newName: "PER_Yetkiler");

            migrationBuilder.RenameTable(
                name: "PER_Unvanlar",
                schema: "dbo",
                newName: "PER_Unvanlar");

            migrationBuilder.RenameTable(
                name: "PER_Servisler",
                schema: "dbo",
                newName: "PER_Servisler");

            migrationBuilder.RenameTable(
                name: "PER_Sendikalar",
                schema: "dbo",
                newName: "PER_Sendikalar");

            migrationBuilder.RenameTable(
                name: "PER_PersonelYetkileri",
                schema: "dbo",
                newName: "PER_PersonelYetkileri");

            migrationBuilder.RenameTable(
                name: "PER_Personeller",
                schema: "dbo",
                newName: "PER_Personeller");

            migrationBuilder.RenameTable(
                name: "PER_PersonelDepartmanlar",
                schema: "dbo",
                newName: "PER_PersonelDepartmanlar");

            migrationBuilder.RenameTable(
                name: "PER_PersonelCocuklari",
                schema: "dbo",
                newName: "PER_PersonelCocuklari");

            migrationBuilder.RenameTable(
                name: "PER_Departmanlar",
                schema: "dbo",
                newName: "PER_Departmanlar");

            migrationBuilder.RenameTable(
                name: "CMN_Iller",
                schema: "dbo",
                newName: "CMN_Iller");

            migrationBuilder.RenameTable(
                name: "CMN_Ilceler",
                schema: "dbo",
                newName: "CMN_Ilceler");

            migrationBuilder.RenameTable(
                name: "CMN_HizmetBinalari",
                schema: "dbo",
                newName: "CMN_HizmetBinalari");

            migrationBuilder.RenameTable(
                name: "SIR_KioskIslemGruplari",
                schema: "dbo",
                newName: "SIR_KioskIslemGruplar");

            migrationBuilder.RenameTable(
                name: "SIR_KioskGruplari",
                schema: "dbo",
                newName: "SIR_KioskGruplar");

            migrationBuilder.RenameTable(
                name: "SIR_KanalPersonelleri",
                schema: "dbo",
                newName: "SIR_KanalPersonellar");

            migrationBuilder.RenameTable(
                name: "SIR_KanallarAlt",
                schema: "dbo",
                newName: "SIR_KanalAltlar");

            migrationBuilder.RenameTable(
                name: "SIR_KanalIslemleri",
                schema: "dbo",
                newName: "SIR_KanalIslemlar");

            migrationBuilder.RenameTable(
                name: "SIR_KanalAltIslemleri",
                schema: "dbo",
                newName: "SIR_KanalAltIslemlar");

            migrationBuilder.RenameTable(
                name: "SIR_HubTvConnections",
                schema: "dbo",
                newName: "SIR_HubTvConnectionlar");

            migrationBuilder.RenameTable(
                name: "SIR_HubConnections",
                schema: "dbo",
                newName: "SIR_HubConnectionlar");

            migrationBuilder.RenameTable(
                name: "SIR_BankoIslemleri",
                schema: "dbo",
                newName: "SIR_BankoIslemlar");

            migrationBuilder.RenameTable(
                name: "PER_Moduller",
                schema: "dbo",
                newName: "CMN_Modullar");

            migrationBuilder.RenameTable(
                name: "PER_ModulControllers",
                schema: "dbo",
                newName: "CMN_ModulControllerlar");

            migrationBuilder.RenameTable(
                name: "PER_ModulControllerIslemleri",
                schema: "dbo",
                newName: "CMN_ModulControllerIslemlar");

            migrationBuilder.RenameTable(
                name: "PER_AtanmaNedenleri",
                schema: "dbo",
                newName: "PER_AtanmaNedenlerilar");

            migrationBuilder.RenameTable(
                name: "ModulAlt",
                newName: "CMN_ModulAltlar");

            migrationBuilder.RenameTable(
                name: "CMN_Users",
                schema: "dbo",
                newName: "CMN_Userlar");

            migrationBuilder.RenameTable(
                name: "CMN_LoginLogoutLogs",
                schema: "dbo",
                newName: "CMN_LoginLogoutLoglar");

            migrationBuilder.RenameTable(
                name: "CMN_DatabaseLogs",
                schema: "dbo",
                newName: "CMN_DatabaseLoglar");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_Siralar_KanalAltIslem_BeklemeDurum",
                table: "SIR_Siralar",
                newName: "IX_SIR_Siralar_KanalAltIslemId_BeklemeDurum");

            migrationBuilder.RenameIndex(
                name: "IX_PER_Personeller_HizmetBinasi_Aktiflik",
                table: "PER_Personeller",
                newName: "IX_PER_Personeller_HizmetBinasiId_PersonelAktiflikDurum");

            migrationBuilder.RenameIndex(
                name: "IX_PER_Personeller_Departman_Aktiflik",
                table: "PER_Personeller",
                newName: "IX_PER_Personeller_DepartmanId_PersonelAktiflikDurum");

            migrationBuilder.RenameIndex(
                name: "IX_PER_Personeller_AktiflikDurum",
                table: "PER_Personeller",
                newName: "IX_PER_Personeller_PersonelAktiflikDurum");

            migrationBuilder.RenameIndex(
                name: "IX_PER_PersonelCocuklari_TcKimlikNo",
                table: "PER_PersonelCocuklari",
                newName: "IX_PER_PersonelCocuklari_PersonelTcKimlikNo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KioskIslemGruplari_KanalAltId",
                table: "SIR_KioskIslemGruplar",
                newName: "IX_SIR_KioskIslemGruplar_KanalAltId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KioskIslemGruplari_HizmetBinasiId",
                table: "SIR_KioskIslemGruplar",
                newName: "IX_SIR_KioskIslemGruplar_HizmetBinasiId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalPersonelleri_TcKimlikNo",
                table: "SIR_KanalPersonellar",
                newName: "IX_SIR_KanalPersonellar_TcKimlikNo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalPersonelleri_PersonelTcKimlikNo",
                table: "SIR_KanalPersonellar",
                newName: "IX_SIR_KanalPersonellar_PersonelTcKimlikNo");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalIslemleri_HizmetBinasiId",
                table: "SIR_KanalIslemlar",
                newName: "IX_SIR_KanalIslemlar_HizmetBinasiId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalAltIslemleri_KioskIslemGrupId",
                table: "SIR_KanalAltIslemlar",
                newName: "IX_SIR_KanalAltIslemlar_KioskIslemGrupId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalAltIslemleri_KanalIslemId",
                table: "SIR_KanalAltIslemlar",
                newName: "IX_SIR_KanalAltIslemlar_KanalIslemId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_KanalAltIslemleri_HizmetBinasiId",
                table: "SIR_KanalAltIslemlar",
                newName: "IX_SIR_KanalAltIslemlar_HizmetBinasiId");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_HubTvConnections_Tv_ConnId_Status",
                table: "SIR_HubTvConnectionlar",
                newName: "IX_SIR_HubTvConnectionlar_TvId_ConnectionId_ConnectionStatus");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_HubConnections_Tc_ConnId_Status",
                table: "SIR_HubConnectionlar",
                newName: "IX_SIR_HubConnectionlar_TcKimlikNo_ConnectionId_ConnectionStatus");

            migrationBuilder.RenameIndex(
                name: "IX_ModulAlt_ModulId",
                table: "CMN_ModulAltlar",
                newName: "IX_CMN_ModulAltlar_ModulId");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_Tvler",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "KatTipi",
                table: "SIR_Tvler",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Kat bilgisi");

            migrationBuilder.AlterColumn<int>(
                name: "Aktiflik",
                table: "SIR_Tvler",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1,
                oldComment: "TV aktiflik durumu");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_TvBankolari",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SiraNo",
                table: "SIR_Siralar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sıra numarası");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SiraAlisZamani",
                table: "SIR_Siralar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Sıra alış zamanı");

            migrationBuilder.AlterColumn<int>(
                name: "BeklemeDurum",
                table: "SIR_Siralar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Bekleme durumu");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_Kanallar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "KanalAdi",
                table: "SIR_Kanallar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "Kanal adı");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_Bankolar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "KatTipi",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Kat bilgisi");

            migrationBuilder.AlterColumn<string>(
                name: "BankoTipi",
                table: "SIR_Bankolar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Banko tipi (Normal/Oncelikli/vb)");

            migrationBuilder.AlterColumn<int>(
                name: "BankoNo",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Banko numarası");

            migrationBuilder.AlterColumn<int>(
                name: "BankoAktiflik",
                table: "SIR_Bankolar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1,
                oldComment: "Banko aktiflik durumu");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "SIR_BankoKullanicilari",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "SIR_BankoKullanicilari",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "YetkiTuru",
                table: "PER_Yetkiler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "PER_Yetkiler",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "PER_Yetkiler",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "UnvanAktiflik",
                table: "PER_Unvanlar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1,
                oldComment: "Unvan aktiflik durumu");

            migrationBuilder.AlterColumn<string>(
                name: "UnvanAdi",
                table: "PER_Unvanlar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Unvan adı");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "PER_Unvanlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "PER_Servisler",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ServisAktiflik",
                table: "PER_Servisler",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1,
                oldComment: "Servis aktiflik durumu");

            migrationBuilder.AlterColumn<string>(
                name: "ServisAdi",
                table: "PER_Servisler",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "Servis adı");

            migrationBuilder.AlterColumn<string>(
                name: "SendikaAdi",
                table: "PER_Sendikalar",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "Sendika adı");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "PER_Sendikalar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "PER_Sendikalar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "YetkiTipleri",
                table: "PER_PersonelYetkileri",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "PER_PersonelYetkileri",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "PER_PersonelYetkileri",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "PER_Personeller",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SicilNo",
                table: "PER_Personeller",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Personel sicil numarası");

            migrationBuilder.AlterColumn<string>(
                name: "SehitYakinligi",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PersonelTipi",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "OgrenimDurumu",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MedeniDurumu",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "KanGrubu",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "EvDurumu",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EsininIsDurumu",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "PER_Personeller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "E-posta adresi");

            migrationBuilder.AlterColumn<string>(
                name: "Cinsiyet",
                table: "PER_Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CepTelefonu",
                table: "PER_Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Cep telefonu numarası");

            migrationBuilder.AlterColumn<string>(
                name: "Adres",
                table: "PER_Personeller",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Adres bilgisi");

            migrationBuilder.AlterColumn<string>(
                name: "AdSoyad",
                table: "PER_Personeller",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "Ad Soyad");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                table: "PER_Personeller",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "TC Kimlik Numarası - Primary Key");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "PER_PersonelDepartmanlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "PER_PersonelCocuklari",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "OgrenimDurumu",
                table: "PER_PersonelCocuklari",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CocukAdi",
                table: "PER_PersonelCocuklari",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "PER_Departmanlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false,
                oldComment: "Soft delete flag");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "PER_Departmanlar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "Kayıt oluşturulma tarihi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "PER_Departmanlar",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()",
                oldComment: "Son güncelleme tarihi");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmanAktiflik",
                table: "PER_Departmanlar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1,
                oldComment: "Departman aktiflik durumu (0: Pasif, 1: Aktif)");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmanAdi",
                table: "PER_Departmanlar",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "Departman adı");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "CMN_HizmetBinalari",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_KioskIslemGruplar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "KioskIslemGrupAktiflik",
                table: "SIR_KioskIslemGruplar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_KioskGruplar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "KioskGrupAdi",
                table: "SIR_KioskGruplar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_KanalPersonellar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_KanalAltlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "KanalAltAdi",
                table: "SIR_KanalAltlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_KanalIslemlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "SIR_KanalAltIslemlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionStatus",
                table: "SIR_HubTvConnectionlar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                table: "SIR_HubTvConnectionlar",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "SIR_HubConnectionlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "SIR_HubConnectionlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionStatus",
                table: "SIR_HubConnectionlar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                table: "SIR_HubConnectionlar",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "SIR_BankoIslemlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "SIR_BankoIslemlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "BankoIslemAktiflik",
                table: "SIR_BankoIslemlar",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "BankoIslemAdi",
                table: "SIR_BankoIslemlar",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "BankoGrup",
                table: "SIR_BankoIslemlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ModulAdi",
                table: "CMN_Modullar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "CMN_Modullar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "CMN_Modullar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ModulControllerAdi",
                table: "CMN_ModulControllerlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "CMN_ModulControllerlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "CMN_ModulControllerlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ModulControllerIslemAdi",
                table: "CMN_ModulControllerIslemlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "CMN_ModulControllerIslemlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "CMN_ModulControllerIslemlar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "PER_AtanmaNedenlerilar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                table: "PER_AtanmaNedenlerilar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "AtanmaNedeni",
                table: "PER_AtanmaNedenlerilar",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                table: "CMN_Userlar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                table: "CMN_Userlar",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "TC Kimlik Numarası - Primary Key");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                table: "CMN_LoginLogoutLoglar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "CMN_LoginLogoutLoglar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                table: "CMN_DatabaseLoglar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                table: "CMN_DatabaseLoglar",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "DatabaseAction",
                table: "CMN_DatabaseLoglar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "BeforeData",
                table: "CMN_DatabaseLoglar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "AfterData",
                table: "CMN_DatabaseLoglar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KioskIslemGruplar",
                table: "SIR_KioskIslemGruplar",
                column: "KioskIslemGrupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KioskGruplar",
                table: "SIR_KioskGruplar",
                column: "KioskGrupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalPersonellar",
                table: "SIR_KanalPersonellar",
                column: "KanalPersonelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalAltlar",
                table: "SIR_KanalAltlar",
                column: "KanalAltId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalIslemlar",
                table: "SIR_KanalIslemlar",
                column: "KanalIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_KanalAltIslemlar",
                table: "SIR_KanalAltIslemlar",
                column: "KanalAltIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_HubTvConnectionlar",
                table: "SIR_HubTvConnectionlar",
                column: "HubTvConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_HubConnectionlar",
                table: "SIR_HubConnectionlar",
                column: "HubConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_BankoIslemlar",
                table: "SIR_BankoIslemlar",
                column: "BankoIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_Modullar",
                table: "CMN_Modullar",
                column: "ModulId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_ModulControllerlar",
                table: "CMN_ModulControllerlar",
                column: "ModulControllerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_ModulControllerIslemlar",
                table: "CMN_ModulControllerIslemlar",
                column: "ModulControllerIslemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PER_AtanmaNedenlerilar",
                table: "PER_AtanmaNedenlerilar",
                column: "AtanmaNedeniId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_ModulAltlar",
                table: "CMN_ModulAltlar",
                column: "ModulAltId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_Userlar",
                table: "CMN_Userlar",
                column: "TcKimlikNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_LoginLogoutLoglar",
                table: "CMN_LoginLogoutLoglar",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CMN_DatabaseLoglar",
                table: "CMN_DatabaseLoglar",
                column: "DatabaseLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_TvBankolari_TvId_BankoId",
                table: "SIR_TvBankolari",
                columns: new[] { "TvId", "BankoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Siralar_SiraNo_HizmetBinasiId_SiraAlisZamani",
                table: "SIR_Siralar",
                columns: new[] { "SiraNo", "HizmetBinasiId", "SiraAlisZamani" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Bankolar_HizmetBinasiId_BankoNo",
                table: "SIR_Bankolar",
                columns: new[] { "HizmetBinasiId", "BankoNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_BankoId",
                table: "SIR_BankoKullanicilari",
                column: "BankoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_BankoKullanicilari_TcKimlikNo",
                table: "SIR_BankoKullanicilari",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Yetkiler_UstYetkiId_YetkiAdi",
                table: "PER_Yetkiler",
                columns: new[] { "UstYetkiId", "YetkiAdi" },
                unique: true,
                filter: "[UstYetkiId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Unvanlar_UnvanAdi",
                table: "PER_Unvanlar",
                column: "UnvanAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Servisler_ServisAdi",
                table: "PER_Servisler",
                column: "ServisAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_Sendikalar_SendikaAdi",
                table: "PER_Sendikalar",
                column: "SendikaAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelYetkileri_TcKimlikNo",
                table: "PER_PersonelYetkileri",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Personeller_Email",
                table: "PER_Personeller",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PER_PersonelDepartmanlar_TcKimlikNo",
                table: "PER_PersonelDepartmanlar",
                column: "TcKimlikNo");

            migrationBuilder.CreateIndex(
                name: "IX_PER_Departmanlar_DepartmanAdi",
                table: "PER_Departmanlar",
                column: "DepartmanAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Iller_IlAdi",
                table: "CMN_Iller",
                column: "IlAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Ilceler_IlId_IlceAdi",
                table: "CMN_Ilceler",
                columns: new[] { "IlId", "IlceAdi" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KioskIslemGruplar_KioskGrupId",
                table: "SIR_KioskIslemGruplar",
                column: "KioskGrupId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalPersonellar_KanalAltIslemId_TcKimlikNo",
                table: "SIR_KanalPersonellar",
                columns: new[] { "KanalAltIslemId", "TcKimlikNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltlar_KanalId",
                table: "SIR_KanalAltlar",
                column: "KanalId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalIslemlar_KanalId",
                table: "SIR_KanalIslemlar",
                column: "KanalId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_KanalAltIslemlar_KanalAltId",
                table: "SIR_KanalAltIslemlar",
                column: "KanalAltId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnectionlar_TvId",
                table: "SIR_HubTvConnectionlar",
                column: "TvId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubConnectionlar_TcKimlikNo",
                table: "SIR_HubConnectionlar",
                column: "TcKimlikNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_ModulControllerlar_ModulId",
                table: "CMN_ModulControllerlar",
                column: "ModulId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_ModulControllerIslemlar_ModulControllerId",
                table: "CMN_ModulControllerIslemlar",
                column: "ModulControllerId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_HizmetBinalari_PER_Departmanlar_DepartmanId",
                table: "CMN_HizmetBinalari",
                column: "DepartmanId",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_Ilceler_CMN_Iller_IlId",
                table: "CMN_Ilceler",
                column: "IlId",
                principalTable: "CMN_Iller",
                principalColumn: "IlId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_ModulAltlar_CMN_Modullar_ModulId",
                table: "CMN_ModulAltlar",
                column: "ModulId",
                principalTable: "CMN_Modullar",
                principalColumn: "ModulId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_ModulControllerIslemlar_CMN_ModulControllerlar_ModulControllerId",
                table: "CMN_ModulControllerIslemlar",
                column: "ModulControllerId",
                principalTable: "CMN_ModulControllerlar",
                principalColumn: "ModulControllerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_ModulControllerlar_CMN_Modullar_ModulId",
                table: "CMN_ModulControllerlar",
                column: "ModulId",
                principalTable: "CMN_Modullar",
                principalColumn: "ModulId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelCocuklari_PER_Personeller_PersonelTcKimlikNo",
                table: "PER_PersonelCocuklari",
                column: "PersonelTcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Departmanlar_DepartmanId",
                table: "PER_PersonelDepartmanlar",
                column: "DepartmanId",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelDepartmanlar_PER_Personeller_TcKimlikNo",
                table: "PER_PersonelDepartmanlar",
                column: "TcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_CMN_HizmetBinalari_HizmetBinasiId",
                table: "PER_Personeller",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_AtanmaNedenlerilar_AtanmaNedeniId",
                table: "PER_Personeller",
                column: "AtanmaNedeniId",
                principalTable: "PER_AtanmaNedenlerilar",
                principalColumn: "AtanmaNedeniId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Departmanlar_DepartmanId",
                table: "PER_Personeller",
                column: "DepartmanId",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Sendikalar_SendikaId",
                table: "PER_Personeller",
                column: "SendikaId",
                principalTable: "PER_Sendikalar",
                principalColumn: "SendikaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Servisler_ServisId",
                table: "PER_Personeller",
                column: "ServisId",
                principalTable: "PER_Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_PER_Unvanlar_UnvanId",
                table: "PER_Personeller",
                column: "UnvanId",
                principalTable: "PER_Unvanlar",
                principalColumn: "UnvanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_CMN_ModulControllerIslemlar_ModulControllerIslemId",
                table: "PER_PersonelYetkileri",
                column: "ModulControllerIslemId",
                principalTable: "CMN_ModulControllerIslemlar",
                principalColumn: "ModulControllerIslemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Personeller_TcKimlikNo",
                table: "PER_PersonelYetkileri",
                column: "TcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelYetkileri_PER_Yetkiler_YetkiId",
                table: "PER_PersonelYetkileri",
                column: "YetkiId",
                principalTable: "PER_Yetkiler",
                principalColumn: "YetkiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_BankoKullanicilari_PER_Personeller_TcKimlikNo",
                table: "SIR_BankoKullanicilari",
                column: "TcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_BankoKullanicilari_SIR_Bankolar_BankoId",
                table: "SIR_BankoKullanicilari",
                column: "BankoId",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Bankolar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_Bankolar",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubConnectionlar_PER_Personeller_TcKimlikNo",
                table: "SIR_HubConnectionlar",
                column: "TcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubTvConnectionlar_SIR_Tvler_TvId",
                table: "SIR_HubTvConnectionlar",
                column: "TvId",
                principalTable: "SIR_Tvler",
                principalColumn: "TvId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemlar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_KanalAltIslemlar",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemlar_SIR_KanalAltlar_KanalAltId",
                table: "SIR_KanalAltIslemlar",
                column: "KanalAltId",
                principalTable: "SIR_KanalAltlar",
                principalColumn: "KanalAltId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemlar_SIR_KanalIslemlar_KanalIslemId",
                table: "SIR_KanalAltIslemlar",
                column: "KanalIslemId",
                principalTable: "SIR_KanalIslemlar",
                principalColumn: "KanalIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltIslemlar_SIR_KioskIslemGruplar_KioskIslemGrupId",
                table: "SIR_KanalAltIslemlar",
                column: "KioskIslemGrupId",
                principalTable: "SIR_KioskIslemGruplar",
                principalColumn: "KioskIslemGrupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalAltlar_SIR_Kanallar_KanalId",
                table: "SIR_KanalAltlar",
                column: "KanalId",
                principalTable: "SIR_Kanallar",
                principalColumn: "KanalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalIslemlar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_KanalIslemlar",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalIslemlar_SIR_Kanallar_KanalId",
                table: "SIR_KanalIslemlar",
                column: "KanalId",
                principalTable: "SIR_Kanallar",
                principalColumn: "KanalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalPersonellar_PER_Personeller_PersonelTcKimlikNo",
                table: "SIR_KanalPersonellar",
                column: "PersonelTcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalPersonellar_PER_Personeller_TcKimlikNo",
                table: "SIR_KanalPersonellar",
                column: "TcKimlikNo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KanalPersonellar_SIR_KanalAltIslemlar_KanalAltIslemId",
                table: "SIR_KanalPersonellar",
                column: "KanalAltIslemId",
                principalTable: "SIR_KanalAltIslemlar",
                principalColumn: "KanalAltIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_KioskIslemGruplar",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplar_SIR_KanalAltlar_KanalAltId",
                table: "SIR_KioskIslemGruplar",
                column: "KanalAltId",
                principalTable: "SIR_KanalAltlar",
                principalColumn: "KanalAltId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_KioskIslemGruplar_SIR_KioskGruplar_KioskGrupId",
                table: "SIR_KioskIslemGruplar",
                column: "KioskGrupId",
                principalTable: "SIR_KioskGruplar",
                principalColumn: "KioskGrupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_Siralar",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Siralar_SIR_KanalAltIslemlar_KanalAltIslemId",
                table: "SIR_Siralar",
                column: "KanalAltIslemId",
                principalTable: "SIR_KanalAltIslemlar",
                principalColumn: "KanalAltIslemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Bankolar_BankoId",
                table: "SIR_TvBankolari",
                column: "BankoId",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_TvBankolari_SIR_Tvler_TvId",
                table: "SIR_TvBankolari",
                column: "TvId",
                principalTable: "SIR_Tvler",
                principalColumn: "TvId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_Tvler_CMN_HizmetBinalari_HizmetBinasiId",
                table: "SIR_Tvler",
                column: "HizmetBinasiId",
                principalTable: "CMN_HizmetBinalari",
                principalColumn: "HizmetBinasiId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
