using MatesPirru.Backend.DTOs;
using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Service
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync(bool? activo = true);
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto> ActualizarProductoAsync(int id, Producto productoModificado);
        Task<bool> EliminarProductoAsync(int id);
        Task<Producto> CrearProductoAsync(Producto nuevoProducto, IFormFile? imagen);
        Task<RespuestaPaginada<Producto>> ObtenerPaginadosAsync(ProductoQueryParameters parametros);
    }
}
