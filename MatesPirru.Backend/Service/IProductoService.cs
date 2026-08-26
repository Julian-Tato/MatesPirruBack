using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Service
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync();
        Task<Producto> CrearProductoAsync(Producto nuevoProducto);
    }
}
