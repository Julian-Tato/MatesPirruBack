using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Service
{
    public interface IPedidoService
    {
        Task<Pedido> CrearPedidoAsync(int idUsuario, CrearPedidoDTO pedidoData);
        Task<List<Pedido>> ObtenerPedidosPorUsuarioAsync(int idUsuario);
    }
}