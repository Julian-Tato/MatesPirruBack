using MatesPirru.Backend.Models;
using MatesPirru.Backend.Service;
using MatesPirru.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatesPirru.Backend.Controllers
{
    // Esto le dice a internet cómo llegar acá: http://tu-pagina/api/productos
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        // El recepcionista recibe al cocinero (Inyección de dependencias)
        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: api/productos (Para ver todos los mates)
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _productoService.ObtenerTodosAsync();
            return Ok(productos); // Devuelve un código 200 (Éxito) con la lista
        }

        // POST: api/productos (Para crear un mate nuevo)
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] Producto nuevoProducto)
        {
            try
            {
                // Le pasamos el paquete armado al cocinero
                var productoCreado = await _productoService.CrearProductoAsync(nuevoProducto);

                // Devuelve un código 201 (Creado) y muestra el producto que se guardó
                return Ok(productoCreado);
            }
            catch (ArgumentException ex)
            {
                // Si el servicio tira el error del precio en cero, devolvemos un código 400 (Bad Request)
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "El mate no existe." }); // Devuelve código 404 (No encontrado)

            return Ok(producto); // Devuelve código 200 con los datos del mate
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] Producto productoModificado)
        {
            try
            {
                var productoActualizado = await _productoService.ActualizarProductoAsync(id, productoModificado);
                return Ok(productoActualizado);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var exito = await _productoService.EliminarProductoAsync(id);

            if (!exito)
                return NotFound(new { mensaje = "No se encontró el mate para eliminar." });

            // Devuelve código 204 (No Content). En internet, esto significa: 
            // "La orden se cumplió con éxito (lo borré), pero no tengo nada para mostrarte en pantalla".
            return NoContent();
        }
    }
}