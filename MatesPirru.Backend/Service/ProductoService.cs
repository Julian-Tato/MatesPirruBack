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

        // 1. BUSCAR POR ID(Reemplaza al "SELECT * FROM Productos WHERE Id = X")
        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            // FindAsync va directo a buscar por la Clave Primaria (Id). Es súper rápido.
            return await _context.Productos.FindAsync(id);
        }

        // 2. ACTUALIZAR (Reemplaza al "UPDATE Productos SET Nombre = '...', Precio = '...' WHERE Id = X")
        public async Task<Producto> ActualizarProductoAsync(int id, Producto productoModificado)
        {
            // Primero, buscamos el mate original en la base de datos
            var productoExistente = await _context.Productos.FindAsync(id);

            // Si no existe, cortamos acá
            if (productoExistente == null)
                throw new Exception("El producto que intentás modificar no existe.");

            // LA MAGIA DE EF CORE (Tracking):
            // Como sacamos "productoExistente" de la base de datos, EF Core lo está "vigilando".
            // Si nosotros le cambiamos los valores acá, EF Core se da cuenta solito de qué columnas cambiaron.
            // Le decimos: "Si me mandaron un nombre que no esté vacío, actualizalo. Si no, dejá el que estaba."
            if (!string.IsNullOrEmpty(productoModificado.Nombre))
                productoExistente.Nombre = productoModificado.Nombre;

            if (!string.IsNullOrEmpty(productoModificado.Descripcion))
                productoExistente.Descripcion = productoModificado.Descripcion;

            if (!string.IsNullOrEmpty(productoModificado.Modelo))
                productoExistente.Modelo = productoModificado.Modelo;

            if (!string.IsNullOrEmpty(productoModificado.UrlImagen))
                productoExistente.UrlImagen = productoModificado.UrlImagen;

            if (!string.IsNullOrEmpty(productoModificado.Material))
                productoExistente.Material = productoModificado.Material;

            if (productoModificado.Precio > 0)
                productoExistente.Precio = productoModificado.Precio;
            // Solo actualiza el stock si nos mandaron algo (asumimos que puede llegar a 0)
            // Nota: para hacer esto perfecto en el futuro, se suelen usar atributos anulables (int?) en los modelos.
            if (productoModificado.Stock != 0)
                productoExistente.Stock = productoModificado.Stock;

            productoExistente.IdCategoria = productoModificado.IdCategoria;
            productoExistente.Activo = productoModificado.Activo;

            // Al hacer SaveChanges, EF Core arma el código "UPDATE..." solo con los campos que tocamos.
            await _context.SaveChangesAsync();

            return productoExistente;
        }

        // 3. ELIMINAR (Reemplaza al "DELETE FROM Productos WHERE Id = X")
        public async Task<bool> EliminarProductoAsync(int id)
        {
            // Buscamos si existe
            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null) return false; // No lo encontró

            // Le decimos a EF Core: "Marcá este objeto para ser destruido"
            _context.Productos.Remove(productoExistente);

            // Al guardar, EF Core ejecuta el DELETE en SQLite.
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
