namespace MatesPirru.Backend.DTOs
{
    public class ProductoQueryParameters
    {
        public int Pagina { get; set; } = 1;
        public int Limite { get; set; } = 10;
        public string? Buscar { get; set; } // Para filtrar por nombre o modelo
        public int? IdCategoria { get; set; } // Para el select de categorías
        public bool? Activo { get; set; } = true; // Por defecto, mostramos mates activo
    }
}
