using MatesPirru.Backend.Models;
using MatesPirru.Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace MatesPirru.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria == null)
                return NotFound(new { mensaje = "La categoría no existe." });
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] Categoria nuevaCategoria)
        {
            try
            {
                var categoriaCreada = await _categoriaService.CrearCategoriaAsync(nuevaCategoria);
                return Ok(categoriaCreada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCategoria(int id, [FromBody] Categoria categoriaModificada)
        {
            try
            {
                var categoriaActualizada = await _categoriaService.ActualizarCategoriaAsync(id, categoriaModificada);
                return Ok(categoriaActualizada);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var exito = await _categoriaService.EliminarCategoriaAsync(id);
            if (!exito)
                return NotFound(new { mensaje = "No se encontró la categoría para eliminar." });
            return NoContent();
        }
    }
}