namespace MatesPirru.Backend.DTOs
{
    public class GoogleLoginDTO
    {
        // Acá vamos a recibir el ticket que nos manda el frontend
        public string TokenId { get; set; } = string.Empty;
    }
}
