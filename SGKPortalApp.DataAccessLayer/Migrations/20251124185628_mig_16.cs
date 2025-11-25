using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_Tc_ConnId_Status",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.AlterColumn<int>(
                name: "TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "int",
                nullable: false,
                comment: "TV ID - Birden fazla kullanıcı aynı TV'yi izleyebilir",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "HubConnection ID - ZORUNLU (TV User veya Personel)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "int",
                nullable: false,
                comment: "HubConnection ID - ZORUNLU (Personel olmak zorunda)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "int",
                nullable: false,
                comment: "Banko ID - UNIQUE (Bir bankoya sadece 1 personel)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "BankoModuAktif",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "bit",
                nullable: false,
                defaultValue: true,
                comment: "Banko modu aktif mi?");

            migrationBuilder.AddColumn<DateTime>(
                name: "BankoModuBaslangic",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Banko moduna giriş zamanı");

            migrationBuilder.AddColumn<DateTime>(
                name: "BankoModuBitis",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: true,
                comment: "Banko modundan çıkış zamanı (nullable)");

            migrationBuilder.AddColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                comment: "Personel TcKimlikNo - UNIQUE (Bir personel aynı anda sadece 1 bankoda)");

            migrationBuilder.AddColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "CMN_Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "User TcKimlikNo - ZORUNLU (Personel veya TV User) - Bir kullanıcının birden fazla bağlantısı olabilir",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "User TcKimlikNo - ZORUNLU (Personel veya TV User)");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "SignalR ConnectionId - Unique",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConnectedAt",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Bağlantı kurulma zamanı");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionType",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "MainLayout",
                comment: "Bağlantı Tipi: MainLayout, TvDisplay, BankoMode, Monitoring");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Son aktivite zamanı");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "HubConnectionId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "TvId",
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "BankoId",
                unique: true,
                filter: "[SilindiMi] = 0 AND [BankoModuAktif] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0 AND [BankoModuAktif] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_HubConnectionId",
                schema: "dbo",
                table: "CMN_Users",
                column: "HubConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_ConnectionId",
                schema: "dbo",
                table: "CMN_HubConnections",
                column: "ConnectionId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_Status",
                schema: "dbo",
                table: "CMN_HubConnections",
                column: "ConnectionStatus",
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_Tc_Type_Status",
                schema: "dbo",
                table: "CMN_HubConnections",
                columns: new[] { "TcKimlikNo", "ConnectionType", "ConnectionStatus" },
                filter: "[SilindiMi] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_Users_CMN_HubConnections_HubConnectionId",
                schema: "dbo",
                table: "CMN_Users",
                column: "HubConnectionId",
                principalSchema: "dbo",
                principalTable: "CMN_HubConnections",
                principalColumn: "HubConnectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CMN_Users_CMN_HubConnections_HubConnectionId",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Users_HubConnectionId",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_ConnectionId",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_Status",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_Tc_Type_Status",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropColumn(
                name: "BankoModuAktif",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "BankoModuBaslangic",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "BankoModuBitis",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "HubConnectionId",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropColumn(
                name: "ConnectedAt",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropColumn(
                name: "ConnectionType",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.AlterColumn<int>(
                name: "TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TV ID - Birden fazla kullanıcı aynı TV'yi izleyebilir");

            migrationBuilder.AlterColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "HubConnection ID - ZORUNLU (TV User veya Personel)");

            migrationBuilder.AlterColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "HubConnection ID - ZORUNLU (Personel olmak zorunda)");

            migrationBuilder.AlterColumn<int>(
                name: "BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Banko ID - UNIQUE (Bir bankoya sadece 1 personel)");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "User TcKimlikNo - ZORUNLU (Personel veya TV User)",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "User TcKimlikNo - ZORUNLU (Personel veya TV User) - Bir kullanıcının birden fazla bağlantısı olabilir");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "SignalR ConnectionId - Unique");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "HubConnectionId",
                unique: true,
                filter: "[SilindiMi] = 0 AND [HubConnectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_TvId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "TvId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "BankoId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_Tc_ConnId_Status",
                schema: "dbo",
                table: "CMN_HubConnections",
                columns: new[] { "TcKimlikNo", "ConnectionId", "ConnectionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");
        }
    }
}
