using System.Text.Json.Serialization;

namespace MatesPirru.Backend.Models

{
    public class ImagenProducto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;

        // Llave foránea para relacionarla con su respectivo mate
        public int ProductoId { get; set; }

        [JsonIgnore] // Evita referencias circulares al serializar en JSON hacia el front
        public Producto? Producto { get; set; }
    }
}
