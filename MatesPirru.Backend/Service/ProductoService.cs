using MatesPirru.Backend.Data;
using MatesPirru.Backend.Models;
using MatesPirru.Backend.Service;
using Microsoft.EntityFrameworkCore;

namespace MatesPirru.Backend.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _context;

        // El constructor recibe la base de datos
        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            // Devuelve todos los productos que estén activos
            return await _context.Productos.Where(p => p.Activo == true).ToListAsync();
        }

        public async Task<Producto> CrearProductoAsync(Producto nuevoProducto)
        {
            // Regla de negocio básica: No se puede crear con precio negativo
            if (nuevoProducto.Precio <= 0)
            {
                throw new ArgumentException("El precio del mate debe ser mayor a cero.");
            }

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync(); // Guarda los cambios en SQLite

            return nuevoProducto;
        }
    }
}
