using MatesPirru.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MatesPirru.Backend.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor necesario para que SQLite se conecte
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Mapeo de todas tus tablas (Modelos) a la base de datos
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<DireccionUsuario> DireccionesUsuario { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
    }
}