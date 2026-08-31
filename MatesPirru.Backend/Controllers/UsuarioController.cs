using Google.Apis.Auth;
using MatesPirru.Backend.DTOs;
using MatesPirru.Backend.Models;
using MatesPirru.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace MatesPirru.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _config;

        // El recepcionista recibe al cocinero de usuarios
        public UsuariosController(IUsuarioService usuarioService, IConfiguration config)
        {
            _usuarioService = usuarioService;
            _config = config;
        }

        // --- Método privado para fabricar la pulsera VIP ---
        private string GenerarTokenJWT(Usuario usuario)
        {
            // 1. Agarramos la llave secreta del appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Metemos los datos del usuario en la pulsera (Claims)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol == RolUsuario.Admin ? "Admin" : "Cliente")
            };

            // 3. Armamos el token con una validez de 2 horas
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
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
        [Authorize(Roles = "Admin")]
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

        // POST: api/usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginData)
        {
            // 1. Le pasamos el email y clave al servicio
            var usuario = await _usuarioService.ValidarCredencialesAsync(loginData.Email, loginData.Password);

            // 2. Si nos devuelve nulo, significa que algo estaba mal
            if (usuario == null)
            {
                // Devolvemos un código 401 (No Autorizado)
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
            }

            // 3. Si acierta, por ahora le damos la bienvenida. 
            var tokenString = GenerarTokenJWT(usuario);

            return Ok(new
            {
                mensaje = "Login exitoso",
                token = tokenString,
                usuarioLogueado = usuario.NombreApellido,
                rol = usuario.Rol
            });
        }

        // POST: api/usuarios/google-login
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO googleData)
        {
            try
            {
                // 1. Validamos el ticket criptográfico con los servidores de Google
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _config["Jwt:GoogleClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleData.TokenId, settings);

                // 2. Buscamos si ya tenemos a este usuario registrado
                var usuario = await _usuarioService.ObtenerPorEmailAsync(payload.Email);

                if (usuario == null)
                {
                    // 3. Si es la primera vez que entra, lo registramos silenciosamente
                    var nuevoUsuario = new Usuario
                    {
                        Email = payload.Email,
                        NombreApellido = payload.Name,
                        IdGoogle = payload.Subject, // Subject es el ID único interno de Google
                        Rol = RolUsuario.Cliente, // Siempre entra como cliente
                        Activo = true,
                        Password = "" // No usa contraseña de nuestro sistema
                    };

                    usuario = await _usuarioService.CrearUsuarioAsync(nuevoUsuario);
                }
                else if (string.IsNullOrEmpty(usuario.IdGoogle))
                {
                    // Si ya tenía cuenta con contraseña, pero ahora entró con Google, le vinculamos la cuenta
                    usuario.IdGoogle = payload.Subject;
                    await _usuarioService.ActualizarUsuarioAsync(usuario.Id, usuario);
                }

                // 4. Le fabricamos nuestra pulsera VIP (Token JWT de Mates Pirru)
                var tokenString = GenerarTokenJWT(usuario);

                return Ok(new
                {
                    mensaje = "Login con Google exitoso",
                    token = tokenString,
                    usuarioLogueado = usuario.NombreApellido,
                    rol = usuario.Rol
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { mensaje = "El token de Google es inválido o está vencido." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al procesar el login con Google.", error = ex.Message });
            }
        }

        // POST: api/usuarios/registro
        [HttpPost("registro")]
        public async Task<IActionResult> RegistrarCliente([FromBody] RegistroDTO registroData)
        {
            try
            {
                // 1. Verificamos que el email no esté ocupado
                var usuarioExistente = await _usuarioService.ObtenerPorEmailAsync(registroData.Email);
                if (usuarioExistente != null)
                {
                    return BadRequest(new { mensaje = "El email ya está registrado." });
                }

                // 2. Armamos el usuario forzando los campos sensibles
                var nuevoUsuario = new Usuario
                {
                    NombreApellido = registroData.NombreApellido,
                    Email = registroData.Email,
                    Password = registroData.Password, // El Service se va a encargar de encriptarla con BCrypt
                    Telefono = registroData.Telefono,
                    Direccion = registroData.Direccion,

                    // ¡ACÁ ESTÁ EL CANDADO! Siempre será Cliente (0)
                    Rol = RolUsuario.Cliente,
                    Activo = true,
                    IdGoogle = string.Empty
                };

                // 3. Guardamos el usuario
                var usuarioCreado = await _usuarioService.CrearUsuarioAsync(nuevoUsuario);

                return Ok(new
                {
                    mensaje = "Usuario registrado con éxito",
                    nombre = usuarioCreado.NombreApellido,
                    email = usuarioCreado.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("mi-perfil")]
        public async Task<IActionResult> ObtenerMiPerfil()
        {
            // 1. Extraemos el ID numérico que guardamos adentro del Token JWT
            var idClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int usuarioId))
                return Unauthorized(new { mensaje = "Token inválido." });

            // 2. Buscamos al usuario en la base de datos
            var usuario = await _usuarioService.ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado." });

            // 3. Devolvemos su ficha, pero dejamos afuera datos sensibles como el Hash de la clave
            return Ok(new
            {
                usuario.NombreApellido,
                usuario.Email,
                usuario.Telefono,
                usuario.Direccion,
                usuario.Rol
            });
        }

        // PUT: api/usuarios/mi-perfil
        [Authorize]
        [HttpPut("mi-perfil")]
        public async Task<IActionResult> ActualizarMiPerfil([FromBody] ActualizarPerfilDTO perfilData)
        {
            try
            {
                // 1. Volvemos a leer el ID de forma segura desde el Token
                var idClaim = User.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int usuarioId))
                    return Unauthorized(new { mensaje = "Token inválido." });

                // 2. Traemos al usuario completo
                var usuarioExistente = await _usuarioService.ObtenerPorIdAsync(usuarioId);
                if (usuarioExistente == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                // 3. Le pisamos solo los datos permitidos con lo que nos mandó en el DTO
                if (!string.IsNullOrEmpty(perfilData.NombreApellido))
                    usuarioExistente.NombreApellido = perfilData.NombreApellido;

                if (!string.IsNullOrEmpty(perfilData.Telefono))
                    usuarioExistente.Telefono = perfilData.Telefono;

                if (!string.IsNullOrEmpty(perfilData.Direccion))
                    usuarioExistente.Direccion = perfilData.Direccion;

                // 4. Mandamos a guardar los cambios usando tu Servicio actual
                await _usuarioService.ActualizarUsuarioAsync(usuarioId, usuarioExistente);

                return Ok(new { mensaje = "Perfil actualizado con éxito." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}