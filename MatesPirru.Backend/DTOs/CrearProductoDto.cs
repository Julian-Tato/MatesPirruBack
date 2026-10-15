namespace MatesPirru.Backend.DTOs
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}