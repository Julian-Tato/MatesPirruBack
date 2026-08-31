using MatesPirru.Backend.DTOs;

namespace MatesPirru.Backend.Models
{
    public class CrearPedidoDTO
    {
        public string DireccionEnvio { get; set; } = string.Empty;

        // El frontend nos manda una lista de los productos que eligió
        public List<CrearDetallePedidoDTO> Detalles { get; set; } = new List<CrearDetallePedidoDTO>();
    }
}
