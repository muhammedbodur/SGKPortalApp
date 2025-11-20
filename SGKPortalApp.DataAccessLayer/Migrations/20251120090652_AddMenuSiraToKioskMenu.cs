using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuSiraToKioskMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuSira",
                schema: "dbo",
                table: "SIR_KioskMenuTanim",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuSira",
                schema: "dbo",
                table: "SIR_KioskMenuTanim");
        }
    }
}
