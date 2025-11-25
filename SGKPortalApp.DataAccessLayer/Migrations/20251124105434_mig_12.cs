using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_Tv_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_Banko_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.AddColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "SIR_Tvler",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                comment: "TV için oluşturulan User'ın TcKimlikNo'su (FK)");

            migrationBuilder.AddColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "TC Kimlik Numarası - Primary Key & Foreign Key to Personel or TV User ID",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "TC Kimlik Numarası - Primary Key & Foreign Key to Personel");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                schema: "dbo",
                table: "CMN_Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Personel",
                comment: "Kullanıcı tipi: Personel veya TvUser");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_Tvler_TcKimlikNo",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "TcKimlikNo",
                unique: true,
                filter: "[TcKimlikNo] IS NOT NULL AND [SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "HubConnectionId",
                unique: true,
                filter: "[SilindiMi] = 0 AND [HubConnectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "HubConnectionId",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_Users_UserType",
                schema: "dbo",
                table: "CMN_Users",
                column: "UserType",
                filter: "[SilindiMi] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0 AND [TcKimlikNo] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubBankoConnections_CMN_HubConnections",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "HubConnectionId",
                principalSchema: "dbo",
                principalTable: "CMN_HubConnections",
                principalColumn: "HubConnectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubTvConnections_CMN_HubConnections",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                column: "HubConnectionId",
                principalSchema: "dbo",
                principalTable: "CMN_HubConnections",
                principalColumn: "HubConnectionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_Users_SIR_Tvler",
                schema: "dbo",
                table: "SIR_Tvler",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "CMN_Users",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubBankoConnections_CMN_HubConnections",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubTvConnections_CMN_HubConnections",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_CMN_Users_SIR_Tvler",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_Tvler_TcKimlikNo",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubTvConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_HubConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_CMN_Users_UserType",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.DropColumn(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "SIR_Tvler");

            migrationBuilder.DropColumn(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections");

            migrationBuilder.DropColumn(
                name: "HubConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropColumn(
                name: "UserType",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "KayitTarihi",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "KayitTarihi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                comment: "TC Kimlik Numarası - Primary Key & Foreign Key to Personel",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "TC Kimlik Numarası - Primary Key & Foreign Key to Personel or TV User ID");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubTvConnections_Tv_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubTvConnections",
                columns: new[] { "TvId", "ConnectionId", "ConnectionStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_Banko_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                columns: new[] { "BankoId", "ConnectionId", "ConnectionStatus" });

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
