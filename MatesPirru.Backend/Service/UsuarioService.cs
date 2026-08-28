using MatesPirru.Backend.Data;
using MatesPirru.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MatesPirru.Backend.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        // 1. TRAER TODOS (Solo los activos)
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _context.Usuarios.Where(u => u.Activo == true).ToListAsync();
        }

        // 2. TRAER POR ID
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        // 3. CREAR USUARIO
        public async Task<Usuario> CrearUsuarioAsync(Usuario nuevoUsuario)
        {
            // Pequeña validación: no podemos tener un usuario sin email
            if (string.IsNullOrWhiteSpace(nuevoUsuario.Email))
                throw new ArgumentException("El email es obligatorio para registrar un usuario.");

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();
            return nuevoUsuario;
        }

        // 4. ACTUALIZAR (Parche Inteligente)
        public async Task<Usuario> ActualizarUsuarioAsync(int id, Usuario usuarioModificado)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
                throw new Exception("El usuario que intentás modificar no existe.");

            // Textos: Si mandan algo distinto a vacío, lo guardamos
            if (!string.IsNullOrEmpty(usuarioModificado.NombreApellido))
                usuarioExistente.NombreApellido = usuarioModificado.NombreApellido;

            if (!string.IsNullOrEmpty(usuarioModificado.Email))
                usuarioExistente.Email = usuarioModificado.Email;

            if (!string.IsNullOrEmpty(usuarioModificado.Telefono))
                usuarioExistente.Telefono = usuarioModificado.Telefono;

            if (!string.IsNullOrEmpty(usuarioModificado.Direccion))
                usuarioExistente.Direccion = usuarioModificado.Direccion;

            if (!string.IsNullOrEmpty(usuarioModificado.IdGoogle))
                usuarioExistente.IdGoogle = usuarioModificado.IdGoogle;

            // Fechas: Chequeamos que la fecha no sea la que viene por defecto (0001-01-01)
            if (usuarioModificado.FechaNacimiento != default)
                usuarioExistente.FechaNacimiento = usuarioModificado.FechaNacimiento;

            // Enums y Booleanos: Se pisan directamente
            usuarioExistente.Rol = usuarioModificado.Rol;
            usuarioExistente.Activo = usuarioModificado.Activo;

            await _context.SaveChangesAsync();

            return usuarioExistente;
        }

        // 5. ELIMINAR (Borrado Lógico)
        public async Task<bool> EliminarUsuarioAsync(int id)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null) return false;

            // Lo apagamos en lugar de borrarlo físicamente
            usuarioExistente.Activo = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}