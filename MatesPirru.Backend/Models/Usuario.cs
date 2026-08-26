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
        public string IdGoogle { get; set; } = string.Empty;
        public string NombreApellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

    }
}
