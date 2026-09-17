namespace MatesPirru.Backend.Models
{
    public class RespuestaPaginada<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
    }
}
