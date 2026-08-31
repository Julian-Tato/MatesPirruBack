using MatesPirru.Backend.Models;
using MatesPirru.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatesPirru.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todo este controlador requiere la pulsera VIP
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // POST: api/pedidos
        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] CrearPedidoDTO pedidoData)
        {
            try
            {
                // Leemos quién es el cliente mirando su token
                var idClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int usuarioId))
                    return Unauthorized(new { mensaje = "Token inválido." });

                // Mandamos a procesar la compra
                var nuevoPedido = await _pedidoService.CrearPedidoAsync(usuarioId, pedidoData);

                return Ok(new { mensaje = "¡Compra realizada con éxito!", pedidoId = nuevoPedido.Id, total = nuevoPedido.Total });
            }
            catch (Exception ex)
            {
                // Si alguien intenta comprar sin stock, el Service lanza el error y lo atajamos acá
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/pedidos/mis-pedidos
        [HttpGet("mis-pedidos")]
        public async Task<IActionResult> ObtenerMisPedidos()
        {
            var idClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int usuarioId))
                return Unauthorized(new { mensaje = "Token inválido." });

            var misPedidos = await _pedidoService.ObtenerPedidosPorUsuarioAsync(usuarioId);
            return Ok(misPedidos);
        }
    }
}