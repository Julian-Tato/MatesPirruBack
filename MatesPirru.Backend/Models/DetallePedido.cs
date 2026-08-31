using System.ComponentModel.DataAnnotations.Schema;

namespace MatesPirru.Backend.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Propiedades de navegación para que Entity Framework nos traiga los datos completos
        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }

        // Evitamos una referencia circular compleja ocultando el objeto Pedido entero por ahora
        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }
    }
}
