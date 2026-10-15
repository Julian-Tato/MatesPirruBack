using MatesPirru.Backend.Data;
using MatesPirru.Backend.DTOs;
using MatesPirru.Backend.Models;
using MatesPirru.Backend.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Necesario para IFormFile
using System.IO; // Necesario para flujos de lectura de archivos
using Firebase.Storage; // Necesario para conectar con Firebase Storage

namespace MatesPirru.Backend.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ObtenerTodosAsync(bool? activo = true)
        {
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .AsQueryable();

            if (activo.HasValue)
            {
                query = query.Where(p => p.Activo == activo.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Producto> CrearProductoAsync(Producto nuevoProducto, IFormFile? imagen)
        {
            if (nuevoProducto.Precio <= 0)
            {
                throw new ArgumentException("El precio del mate debe ser mayor a cero.");
            }

            // Si se envió una imagen, la subimos a Firebase Storage
            if (imagen != null && imagen.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);

                using (var stream = imagen.OpenReadStream())
                {
                    // Conectamos usando tu bucket oficial de Firebase
                    var firebaseStorage = new FirebaseStorage("matespirru-b3300.firebasestorage.app");

                    // Subimos la imagen a una carpeta "productos" y obtenemos su URL pública
                    var urlDescarga = await firebaseStorage
                        .Child("productos")
                        .Child(nombreArchivo)
                        .PutAsync(stream);

                    // Guardamos únicamente la URL liviana en la base de datos
                    var nuevaImagen = new ImagenProducto
                    {
                        Url = urlDescarga
                    };

                    if (nuevoProducto.Imagenes == null) nuevoProducto.Imagenes = new List<ImagenProducto>();
                    nuevoProducto.Imagenes.Add(nuevaImagen);
                }
            }

            _context.Productos.Add(nuevoProducto);
            await _context.SaveChangesAsync();

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
        public async Task<Producto> ActualizarProductoAsync(int id, Producto productoModificado)
        {
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

            if (productoModificado.Imagenes != null && productoModificado.Imagenes.Any())
            {
                _context.RemoveRange(productoExistente.Imagenes);
                productoExistente.Imagenes = productoModificado.Imagenes;
            }

            await _context.SaveChangesAsync();
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
            var query = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagenes)
                .AsQueryable();

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

            int totalItems = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalItems / (double)parametros.Limite);

            var items = await query
                .Skip((parametros.Pagina - 1) * parametros.Limite)
                .Take(parametros.Limite)
                .ToListAsync();

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