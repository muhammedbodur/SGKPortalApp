using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGKPortalApp.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mig_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelHizmetleri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelHizmetleri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "ServisId",
                principalSchema: "dbo",
                principalTable: "PER_Servisler",
                principalColumn: "ServisId");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "ServisId",
                principalSchema: "dbo",
                principalTable: "PER_Servisler",
                principalColumn: "ServisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelHizmetleri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelHizmetleri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri");

            migrationBuilder.DropForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri");

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelHizmetleri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelHizmetleri",
                column: "ServisId",
                principalSchema: "dbo",
                principalTable: "PER_Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Departmanlar",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "DepartmanId",
                principalSchema: "dbo",
                principalTable: "PER_Departmanlar",
                principalColumn: "DepartmanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PER_PersonelImzaYetkileri_PER_Servisler",
                schema: "dbo",
                table: "PER_PersonelImzaYetkileri",
                column: "ServisId",
                principalSchema: "dbo",
                principalTable: "PER_Servisler",
                principalColumn: "ServisId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
