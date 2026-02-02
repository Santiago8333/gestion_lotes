using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using gestion_lotes.Models; 

namespace gestion_lotes.Models
{
    
public interface IRecibo_persona_juridicaRepositorio
    {
        IQueryable<Recibo_persona_juridica> ObtenerTodos();
        IQueryable<Recibo_persona_juridica> ObtenerTodosyFormasPagosyLotes(string? razon_social = null, string? numeroLote = null);
    }

public class RepositorioRecibo_persona_juridica : IRecibo_persona_juridicaRepositorio
    {
         private readonly AppDbContext _context;

        public RepositorioRecibo_persona_juridica(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Recibo_persona_juridica> ObtenerTodos()
        {
            return _context.Recibo_persona_juridica;
        }
        public IQueryable<Recibo_persona_juridica> ObtenerTodosyFormasPagosyLotes(string? razon_social = null, string? numeroLote = null)
        {
            var query = _context.Recibo_persona_juridica
                .AsNoTracking()
                .Include(r => r.FormasDePago)
                .Include(r => r.Lote)
                .AsQueryable();

            if (!string.IsNullOrEmpty(razon_social))
            {
                query = query.Where(r => r.razon_social.Contains(razon_social.Trim()));
            }

            if (!string.IsNullOrEmpty(numeroLote))
            {
                query = query.Where(r => r.Lote != null && r.Lote.n_lote.ToString().Contains(numeroLote.Trim()));
            }

            return query;
        }
    }
}