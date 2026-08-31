using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Services
{
    public interface IUsuarioService
    {
        // Trae todos los usuarios activos
        Task<List<Usuario>> ObtenerTodosAsync();

        // Busca un usuario puntual por su ID
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);

        // Crea un usuario nuevo
        Task<Usuario> CrearUsuarioAsync(Usuario nuevoUsuario);

        // Modifica los datos de un usuario
        Task<Usuario> ActualizarUsuarioAsync(int id, Usuario usuarioModificado);

        // Apaga un usuario (Borrado Lógico)
        Task<bool> EliminarUsuarioAsync(int id);

        // Devuelve el usuario si acierta, o "null" si le erró a la clave/email
        Task<Usuario?> ValidarCredencialesAsync(string email, string password);

        
    }
}
