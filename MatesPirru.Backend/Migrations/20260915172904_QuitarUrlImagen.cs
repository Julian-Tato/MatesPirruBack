using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatesPirru.Backend.Migrations
{
    /// <inheritdoc />
    public partial class QuitarUrlImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "Productos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
