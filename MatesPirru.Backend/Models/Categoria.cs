namespace MatesPirru.Backend.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
    }
}
