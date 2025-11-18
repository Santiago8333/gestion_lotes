using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gestion_lotes.Models
{
     public interface ILoteRepositorio
    {
        IQueryable<Usuarios> ObtenerTodos();
    }

    public class RepositorioLote : ILoteRepositorio
        {
            private readonly AppDbContext _context;

            // Recibimos el DbContext, no la configuración
            public RepositorioLote(AppDbContext context)
            {
                _context = context;
            }

            public IQueryable<Usuarios> ObtenerTodos()
            {
                // Solo devolvemos el DbSet, sin ejecutar ToList()
                return _context.Usuarios;
            }
        }

}