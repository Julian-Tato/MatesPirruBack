namespace MatesPirru.Backend.Models
{
    public enum RolUsuario
    {
        Cliente,
        Admin
    }
    public class Usuario
    {
        public int Id { get; set; }
        public int IdGoogle { get; set; }
        public string NombreApellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Telefono { get; set; }
        public RolUsuario Rol { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

    }
}
