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
    }
}