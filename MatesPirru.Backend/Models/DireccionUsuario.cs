namespace MatesPirru.Backend.Models
{
    public class DireccionUsuario
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Calle { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
    }
}
