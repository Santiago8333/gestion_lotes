using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using gestion_lotes.Models; 

namespace gestion_lotes.Models
{
    public interface IRecibo_persona_fisica
    {
        IQueryable<Recibo_persona_fisica> ObtenerTodos();
        Task<Recibo_persona_fisica?> ObtenerPorId(int id);
    }
    public class RepositorioRecibo_persona_fisica : IRecibo_persona_fisica
    {
         private readonly AppDbContext _context;

        public RepositorioRecibo_persona_fisica(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Recibo_persona_fisica> ObtenerTodos()
        {
            return _context.Recibo_persona_fisica;
        }
        public async Task<Recibo_persona_fisica?> ObtenerPorId(int id)
        {
            return _context.Recibo_persona_fisica.Find(id);
        }
    }
}