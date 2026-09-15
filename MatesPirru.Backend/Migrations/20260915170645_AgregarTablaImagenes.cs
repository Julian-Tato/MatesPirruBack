using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatesPirru.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImagenesProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenesProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagenesProducto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesProducto_ProductoId",
                table: "ImagenesProducto",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagenesProducto");
        }
    }
}
