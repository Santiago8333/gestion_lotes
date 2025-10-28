using System.Collections.Generic;
using System.Linq;

namespace gestion_lotes.Models
{
    public interface IUsuarioRepositorio
    {
        IQueryable<Usuarios> ObtenerTodos();
        Usuarios? ObtenerPorId(int id); // Usamos '?' porque puede devolver null
    }

    public class RepositorioUsuario : IUsuarioRepositorio
    {
        private readonly AppDbContext _context;

        // Recibimos el DbContext, no la configuración
        public RepositorioUsuario(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Usuarios> ObtenerTodos()
        {
            // Solo devolvemos el DbSet, sin ejecutar ToList()
            return _context.Usuarios;
        }
        
        public Usuarios? ObtenerPorId(int id)
        {
            // Usamos Find() porque es la forma más eficiente de buscar por ID.
            return _context.Usuarios.Find(id);
        }
    }
}