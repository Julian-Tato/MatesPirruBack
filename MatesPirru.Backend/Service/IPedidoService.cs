using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Service
{
    public interface IPedidoService
    {
        Task<Pedido> CrearPedidoAsync(int idUsuario, CrearPedidoDTO pedidoData);
        Task<List<Pedido>> ObtenerPedidosPorUsuarioAsync(int idUsuario);
        Task<Pedido> ActualizarPedido(int id, Pedido pedido);
        Task<Pedido> ObtenerPedidoPorId(int id);
        Task<bool> EliminarPedido(int id);

    }
}