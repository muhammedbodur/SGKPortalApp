using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubBankoConnections_SIR_Bankolar_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropTable(
                name: "SIR_HubConnections",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CMN_HubConnections",
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
                    table.PrimaryKey("PK_CMN_HubConnections", x => x.HubConnectionId);
                    table.ForeignKey(
                        name: "FK_CMN_HubConnections_CMN_Users",
                        column: x => x.TcKimlikNo,
                        principalSchema: "dbo",
                        principalTable: "CMN_Users",
                        principalColumn: "TcKimlikNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_Banko_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                columns: new[] { "BankoId", "ConnectionId", "ConnectionStatus" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubBankoConnections_SIR_Bankolar",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "BankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubBankoConnections_SIR_Bankolar",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropTable(
                name: "CMN_HubConnections",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_Banko_ConnId_Status",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.AlterColumn<bool>(
                name: "SilindiMi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EklenmeTarihi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DuzenlenmeTarihi",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

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
                    DuzenlenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DuzenleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EkleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IslemZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SilenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "BankoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SIR_HubBankoConnections_ConnectionStatus",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "ConnectionStatus");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SIR_HubBankoConnections_SIR_Bankolar_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "BankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
