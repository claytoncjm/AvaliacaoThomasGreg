using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCadCliente.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClienteLogoToBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Logotipo",
                table: "Clientes",
                newName: "LogotipoBytes");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Logradouros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogotipoContentType",
                table: "Clientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogotipoContentType",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "LogotipoBytes",
                table: "Clientes",
                newName: "Logotipo");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Logradouros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
