using System.ComponentModel.DataAnnotations.Schema;

namespace MatesPirru.Backend.Models
{
    // Usamos un Enum para los estados en lugar de un String suelto, 
    // así evitamos errores de tipeo al guardar "Enviado" o "Pendiente".
    public enum EstadoPedido
    {
        PendientePago,
        Pagado,
        Enviado,
        Entregado,
        Cancelado
    }

    public class Pedido
    {
        public int Id { get; set; }

        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int? IdCupon { get; set; } // Opcional, por si no usó cupón
        public decimal MontoDescuento { get; set; }

        // ¡Agregados para e-commerce real!
        public decimal CostoEnvio { get; set; }
        public string DireccionEnvio { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public EstadoPedido Estado { get; set; } = EstadoPedido.PendientePago;

        // Propiedades de navegación
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        // Esta es la magia: Un pedido contiene una LISTA de detalles
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}