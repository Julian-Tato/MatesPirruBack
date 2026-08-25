namespace MatesPirru.Backend.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int IdCategoria { get; set; }
        public bool Activo { get; set; } = true;
    }
}
