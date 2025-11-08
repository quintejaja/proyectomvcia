using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_IA.Migrations
{
    /// <inheritdoc />
    public partial class NuevaEstructuraCitas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Usuarios_UsuarioId",
                table: "Citas");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 4);

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Citas",
                newName: "IdCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Citas_UsuarioId",
                table: "Citas",
                newName: "IX_Citas_IdCliente");

            migrationBuilder.AlterColumn<int>(
                name: "TecnicoId",
                table: "Citas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 2,
                column: "TipoRol",
                value: "Cliente");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 3,
                column: "TipoRol",
                value: "Tecnico");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Usuarios_IdCliente",
                table: "Citas",
                column: "IdCliente",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Usuarios_IdCliente",
                table: "Citas");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "Citas",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Citas_IdCliente",
                table: "Citas",
                newName: "IX_Citas_UsuarioId");

            migrationBuilder.AlterColumn<int>(
                name: "TecnicoId",
                table: "Citas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 2,
                column: "TipoRol",
                value: "Usuario");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "IdRol",
                keyValue: 3,
                column: "TipoRol",
                value: "Cliente");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "TipoRol" },
                values: new object[] { 4, "Tecnico" });

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Usuarios_UsuarioId",
                table: "Citas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
