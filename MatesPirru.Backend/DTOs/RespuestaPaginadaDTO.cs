namespace MatesPirru.Backend.DTOs
{
    public class RespuestaPaginada<T>
    {
        public List<T> Items { get; set; } = new List<T>(); // Los mates de esta página
        public int PaginaActual { get; set; } // En qué página estamos (ej: 1)
        public int TotalPaginas { get; set; } // Cuántas páginas hay en total (ej: 5)
        public int TotalItems { get; set; } // Cuántos mates hay en total en la base de datos (ej: 50)
    }
}

