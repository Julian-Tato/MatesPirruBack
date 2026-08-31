using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatesPirru.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablasPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCupon",
                table: "Pedidos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Estado",
                table: "Pedidos",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "CostoEnvio",
                table: "Pedidos",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DireccionEnvio",
                table: "Pedidos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdUsuario",
                table: "Pedidos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_IdPedido",
                table: "DetallesPedido",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_IdProducto",
                table: "DetallesPedido",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Pedidos_IdPedido",
                table: "DetallesPedido",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Productos_IdProducto",
                table: "DetallesPedido",
                column: "IdProducto",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Usuarios_IdUsuario",
                table: "Pedidos",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Pedidos_IdPedido",
                table: "DetallesPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Productos_IdProducto",
                table: "DetallesPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Usuarios_IdUsuario",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_IdUsuario",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_IdPedido",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_IdProducto",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "CostoEnvio",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "DireccionEnvio",
                table: "Pedidos");

            migrationBuilder.AlterColumn<int>(
                name: "IdCupon",
                table: "Pedidos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Pedidos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
