using MatesPirru.Backend.Data;
using MatesPirru.Backend.Models;
using MatesPirru.Backend.Service;
using Microsoft.EntityFrameworkCore;

namespace MatesPirru.Backend.Service
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> ObtenerTodasAsync()
        {
            return await _context.Categorias.Where(c => c.Estado == true).ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        public async Task<Categoria> CrearCategoriaAsync(Categoria nuevaCategoria)
        {
            if (string.IsNullOrWhiteSpace(nuevaCategoria.Descripcion))
            {
                throw new ArgumentException("La descripción de la categoría no puede estar vacía.");
            }

            _context.Categorias.Add(nuevaCategoria);
            await _context.SaveChangesAsync();
            return nuevaCategoria;
        }

        public async Task<Categoria> ActualizarCategoriaAsync(int id, Categoria categoriaModificada)
        {
            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if (categoriaExistente == null)
                throw new Exception("La categoría que intentás modificar no existe.");

            categoriaExistente.Descripcion = categoriaModificada.Descripcion;
            categoriaExistente.Estado = categoriaModificada.Estado;

            await _context.SaveChangesAsync();
            return categoriaExistente;
        }

        public async Task<bool> EliminarCategoriaAsync(int id)
        {
            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if (categoriaExistente == null) return false;

            _context.Categorias.Remove(categoriaExistente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}