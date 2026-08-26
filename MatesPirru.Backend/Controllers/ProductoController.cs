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
    }
}