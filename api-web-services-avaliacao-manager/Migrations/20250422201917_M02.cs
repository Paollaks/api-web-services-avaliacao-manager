using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_web_services_avaliacao_manager.Migrations
{
    /// <inheritdoc />
    public partial class M02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NomeDeUsuário",
                table: "Usuarios",
                newName: "NomeDeUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NomeDeUsuario",
                table: "Usuarios",
                newName: "NomeDeUsuário");
        }
    }
}
