using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using gestion_lotes.Models; 

namespace gestion_lotes.Models
{
    
public interface IRecibo_persona_juridicaRepositorio
    {
        IQueryable<Recibo_persona_juridica> ObtenerTodos();
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
    }
}