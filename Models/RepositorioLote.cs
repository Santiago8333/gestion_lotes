using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
// Asegúrate de importar donde tienes definida la clase Lote
using gestion_lotes.Models; 

namespace gestion_lotes.Models
{
    // Cambiamos <Lotes> por <Lote> (el tipo de objeto)
    public interface ILoteRepositorio
    {
        IQueryable<Lotes> ObtenerTodos();
        Task<Lotes?> ObtenerPorId(int id);
        Task<Lotes?> ObtenerPorNloteAsync(int n_lote);
        Task<Lotes> Agregar(Lotes lote);
    }

    public class RepositorioLote : ILoteRepositorio
    {
        private readonly AppDbContext _context;

        public RepositorioLote(AppDbContext context)
        {
            _context = context;
        }

        // Cambiamos <Lotes> por <Lote> aquí también
        public IQueryable<Lotes> ObtenerTodos()
        {
            // _context.Lotes es correcto si así se llama tu DbSet en el AppDbContext
            return _context.Lotes;
        }
        public async Task<Lotes?> ObtenerPorId(int id)
        {
            // Usamos Find() porque es la forma más eficiente de buscar por ID.
            return _context.Lotes.Find(id);
        }
        public async Task<Lotes?> ObtenerPorNloteAsync(int n_lote)
        {
            return await _context.Lotes
                .FirstOrDefaultAsync(u => u.n_lote == n_lote);
        }
        public async Task<Lotes> Agregar(Lotes lote)
        {
            _context.Lotes.Add(lote);
            await _context.SaveChangesAsync();
            return lote;
        }
    }
}