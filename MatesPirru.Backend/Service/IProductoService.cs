using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Service
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync();
        Task<Producto> CrearProductoAsync(Producto nuevoProducto);
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto> ActualizarProductoAsync(int id, Producto productoModificado);
        Task<bool> EliminarProductoAsync(int id);
    }
}
