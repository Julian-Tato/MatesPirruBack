using MatesPirru.Backend.Models;

namespace MatesPirru.Backend.Service
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> ObtenerTodasAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task<Categoria> CrearCategoriaAsync(Categoria nuevaCategoria);
        Task<Categoria> ActualizarCategoriaAsync(int id, Categoria categoriaModificada);
        Task<bool> EliminarCategoriaAsync(int id);
    }
}