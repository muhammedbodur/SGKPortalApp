using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HubBankoConnections_SIR_Bankolar_BankoId",
                table: "HubBankoConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HubBankoConnections",
                table: "HubBankoConnections");

            migrationBuilder.RenameTable(
                name: "HubBankoConnections",
                newName: "SIR_HubBankoConnections",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_HubBankoConnections_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                newName: "IX_SIR_HubBankoConnections_BankoId");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SIR_HubBankoConnections",
                schema: "dbo",
                table: "SIR_HubBankoConnections",
                column: "HubBankoConnectionId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SIR_HubBankoConnections_SIR_Bankolar_BankoId",
                schema: "dbo",
                table: "SIR_HubBankoConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SIR_HubBankoConnections",
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

            migrationBuilder.RenameTable(
                name: "SIR_HubBankoConnections",
                schema: "dbo",
                newName: "HubBankoConnections");

            migrationBuilder.RenameIndex(
                name: "IX_SIR_HubBankoConnections_BankoId",
                table: "HubBankoConnections",
                newName: "IX_HubBankoConnections_BankoId");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                table: "HubBankoConnections",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HubBankoConnections",
                table: "HubBankoConnections",
                column: "HubBankoConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_HubBankoConnections_SIR_Bankolar_BankoId",
                table: "HubBankoConnections",
                column: "BankoId",
                principalSchema: "dbo",
                principalTable: "SIR_Bankolar",
                principalColumn: "BankoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
