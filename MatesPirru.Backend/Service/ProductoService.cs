using MatesPirru.Backend.Data;
using MatesPirru.Backend.DTOs;
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

        public async Task<List<Producto>> ObtenerTodosAsync(bool? activo = true)
        {
            // 1. Preparamos la consulta sumando la Categoría y las Imágenes de tu compañero
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .AsQueryable();

            // 2. Aplicamos NUESTRA lógica para el panel de administración
            if (activo.HasValue)
            {
                query = query.Where(p => p.Activo == activo.Value);
            }

            return await query.ToListAsync();
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

        // 1. BUSCAR POR ID
        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // 2. ACTUALIZAR
        // 2. ACTUALIZAR
        public async Task<Producto> ActualizarProductoAsync(int id, Producto productoModificado)
        {
            // Traemos el producto incluyendo las imágenes para poder modificarlas si cambian
            var productoExistente = await _context.Productos
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoExistente == null)
                throw new Exception("El producto que intentás modificar no existe.");

            if (!string.IsNullOrEmpty(productoModificado.Nombre))
                productoExistente.Nombre = productoModificado.Nombre;

            if (!string.IsNullOrEmpty(productoModificado.Descripcion))
                productoExistente.Descripcion = productoModificado.Descripcion;

            if (!string.IsNullOrEmpty(productoModificado.Modelo))
                productoExistente.Modelo = productoModificado.Modelo;

            if (!string.IsNullOrEmpty(productoModificado.Material))
                productoExistente.Material = productoModificado.Material;

            if (productoModificado.Precio > 0)
                productoExistente.Precio = productoModificado.Precio;

            if (productoModificado.Stock != 0)
                productoExistente.Stock = productoModificado.Stock;

            productoExistente.IdCategoria = productoModificado.IdCategoria;
            productoExistente.Activo = productoModificado.Activo;

            // Sincronización de imágenes si el JSON del PUT trae nuevas
            if (productoModificado.Imagenes != null && productoModificado.Imagenes.Any())
            {
                // Usamos RemoveRange directo del contexto sin especificar DbSet
                _context.RemoveRange(productoExistente.Imagenes);
                productoExistente.Imagenes = productoModificado.Imagenes;
            }

            await _context.SaveChangesAsync();

            // Retornamos el producto con las relaciones recargadas para el frontend
            return await ObtenerPorIdAsync(id);
        }

        // 3. ELIMINAR
        public async Task<bool> EliminarProductoAsync(int id)
        {
            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null) return false;

            productoExistente.Activo = false;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<RespuestaPaginada<Producto>> ObtenerPaginadosAsync(ProductoQueryParameters parametros)
        {
            // 1. Iniciamos la consulta (AsQueryable permite sumar filtros antes de tocar SQLite)
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .AsQueryable();

            // 2. Aplicamos filtros dinámicos si el usuario los envió
            if (parametros.Activo.HasValue)
            {
                query = query.Where(p => p.Activo == parametros.Activo.Value);
            }

            if (parametros.IdCategoria.HasValue)
            {
                query = query.Where(p => p.IdCategoria == parametros.IdCategoria.Value);
            }

            if (!string.IsNullOrWhiteSpace(parametros.Buscar))
            {
                var busqueda = parametros.Buscar.ToLower();
                query = query.Where(p => p.Nombre.ToLower().Contains(busqueda) ||
                                         p.Modelo.ToLower().Contains(busqueda));
            }

            // 3. Contamos cuántos mates cumplen esta condición en total
            int totalItems = await query.CountAsync();

            // 4. Calculamos cuántas páginas se generan
            int totalPaginas = (int)Math.Ceiling(totalItems / (double)parametros.Limite);

            // 5. Aplicamos la paginación (Salteamos los anteriores y agarramos los de la página actual)
            var items = await query
                .Skip((parametros.Pagina - 1) * parametros.Limite)
                .Take(parametros.Limite)
                .ToListAsync();

            // 6. Armamos y devolvemos el paquete final
            return new RespuestaPaginada<Producto>
            {
                Items = items,
                PaginaActual = parametros.Pagina,
                TotalPaginas = totalPaginas,
                TotalItems = totalItems
            };
        }
    }
}