using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CMN_Users_PER_Personeller",
                schema: "dbo",
                table: "CMN_Users");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                comment: "User TcKimlikNo - ZORUNLU (Personel veya TV User)",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_Personeller_CMN_Users_TcKimlikNo",
                schema: "dbo",
                table: "PER_Personeller",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "CMN_Users",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PER_Personeller_CMN_Users_TcKimlikNo",
                schema: "dbo",
                table: "PER_Personeller");

            migrationBuilder.DropIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections");

            migrationBuilder.AlterColumn<string>(
                name: "TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldComment: "User TcKimlikNo - ZORUNLU (Personel veya TV User)");

            migrationBuilder.CreateIndex(
                name: "IX_CMN_HubConnections_TcKimlikNo",
                schema: "dbo",
                table: "CMN_HubConnections",
                column: "TcKimlikNo",
                unique: true,
                filter: "[SilindiMi] = 0 AND [TcKimlikNo] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CMN_Users_PER_Personeller",
                schema: "dbo",
                table: "CMN_Users",
                column: "TcKimlikNo",
                principalSchema: "dbo",
                principalTable: "PER_Personeller",
                principalColumn: "TcKimlikNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
