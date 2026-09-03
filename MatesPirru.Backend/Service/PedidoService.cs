using MatesPirru.Backend.Data;
using MatesPirru.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace MatesPirru.Backend.Service
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _context;

        public PedidoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pedido> CrearPedidoAsync(int idUsuario, CrearPedidoDTO pedidoData)
        {
            // 1. Iniciamos el esqueleto del pedido
            var nuevoPedido = new Pedido
            {
                IdUsuario = idUsuario,
                DireccionEnvio = pedidoData.DireccionEnvio,
                Fecha = DateTime.UtcNow,
                Estado = EstadoPedido.PendientePago,
                Total = 0,
                CostoEnvio = 0 // A futuro podés calcularlo según el código postal
            };


            // 2. Analizamos cada renglón del carrito
            foreach (var item in pedidoData.Detalles)
            {
                var producto = await _context.Productos.FindAsync(item.IdProducto);

                if (producto == null)
                    throw new Exception($"El producto con ID {item.IdProducto} no existe.");

                if (!producto.Activo)
                    throw new Exception($"El producto {producto.Nombre} ya no está disponible.");

                if (producto.Stock < item.Cantidad)
                    throw new Exception($"No hay suficiente stock para {producto.Nombre}. Stock disponible: {producto.Stock}");

                // 3. Congelamos el precio oficial y armamos el detalle
                var detalle = new DetallePedido
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio // Sacamos el precio de la BD, no del cliente
                };

                // 4. Descontamos el stock
                producto.Stock -= item.Cantidad;

                nuevoPedido.Detalles.Add(detalle);
                nuevoPedido.Total += (detalle.Cantidad * detalle.PrecioUnitario);
            }

            nuevoPedido.Total += nuevoPedido.CostoEnvio;

            // 5. Guardamos todo junto (Entity Framework maneja la transacción entera automáticamente)
            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            return nuevoPedido;
        }

        public async Task<Pedido> ObtenerPedidoPorId(int id)
        {
            // El .Include va a buscar el objeto entero del Usuario asociado a la clave foránea
            return await _context.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<Pedido>> ObtenerPedidosPorUsuarioAsync(int idUsuario)
        {
            // Traemos los pedidos de la persona, incluyendo los detalles y el nombre de cada mate
            return await _context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(p => p.IdUsuario == idUsuario)
                .OrderByDescending(p => p.Fecha) // Los más recientes primero
                .ToListAsync();
        }

        public async Task<Pedido> ActualizarPedido(int id, Pedido pedidoActualizado)
        {
            var pedidoExistente = await _context.Pedidos.FindAsync(id);

            if (pedidoExistente == null) return null;

            // Actualizamos la dirección solo si el usuario nos mandó un texto válido
            if (!string.IsNullOrWhiteSpace(pedidoActualizado.DireccionEnvio))
            {
                pedidoExistente.DireccionEnvio = pedidoActualizado.DireccionEnvio;
            }

            // Si el día de mañana querés que también puedan cambiar el estado desde otra pantalla, 
            // podés ir sumando reglas acá, por ejemplo:
            // pedidoExistente.Estado = pedidoActualizado.Estado;

            // Guardamos los cambios. EF Core es inteligente y solo va a hacer un UPDATE 
            // en la base de datos de la columna "DireccionEnvio".
            await _context.SaveChangesAsync();

            return pedidoExistente;
        }

        public async Task<bool> EliminarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null) return false;

            // Borrado lógico: simplemente le cambiamos el estado al pedido
            pedido.Estado = EstadoPedido.Cancelado;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}