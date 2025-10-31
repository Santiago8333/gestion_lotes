using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gestion_lotes.Models
{
    public interface IUsuarioRepositorio
    {
        IQueryable<Usuarios> ObtenerTodos();
        Task<Usuarios?> ObtenerPorId(int id); // Usamos '?' porque puede devolver null
        Task<int> EliminarDirecto(int id);
        Task<Usuarios> Agregar(Usuarios usuario);
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

        public async Task<Usuarios?> ObtenerPorId(int id)
        {
            // Usamos Find() porque es la forma más eficiente de buscar por ID.
            return _context.Usuarios.Find(id);
        }
        public async Task<int> EliminarDirecto(int id)
        {
            // Crea una consulta para encontrar al usuario y luego ejecuta el DELETE.
            return await _context.Usuarios
                .Where(u => u.id_usuario == id)
                .ExecuteDeleteAsync();
        }
        public async Task<Usuarios> Agregar(Usuarios usuario)
        {

            _context.Usuarios.Add(usuario);


            await _context.SaveChangesAsync();

            return usuario;
        }
    }
}