using MatesPirru.Backend.Models;
using MatesPirru.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatesPirru.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        // El recepcionista recibe al cocinero de usuarios
        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // 1. GET: api/usuarios (Trae todos los activos)
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        // 2. GET POR ID: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "El usuario no existe." });

            return Ok(usuario);
        }

        // 3. POST: api/usuarios (Crea uno nuevo)
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario nuevoUsuario)
        {
            try
            {
                var usuarioCreado = await _usuarioService.CrearUsuarioAsync(nuevoUsuario);
                return Ok(usuarioCreado);
            }
            catch (ArgumentException ex)
            {
                // Si viene sin email, tiramos el error 400
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // 4. PUT: api/usuarios/5 (Actualiza)
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] Usuario usuarioModificado)
        {
            try
            {
                var usuarioActualizado = await _usuarioService.ActualizarUsuarioAsync(id, usuarioModificado);
                return Ok(usuarioActualizado);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        // 5. DELETE: api/usuarios/5 (Apaga el usuario lógicamente)
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var exito = await _usuarioService.EliminarUsuarioAsync(id);

            if (!exito)
                return NotFound(new { mensaje = "No se encontró el usuario para eliminar." });

            return NoContent();
        }
    }
}