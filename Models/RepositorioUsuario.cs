using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gestion_lotes.Models
{
    public interface IUsuarioRepositorio
    {
        IQueryable<Usuarios> ObtenerTodos(string? email = null);
        Task<Usuarios?> ObtenerPorId(int id); // Usamos '?' porque puede devolver null
        Task<int> EliminarDirecto(int id);
        Task<Usuarios> Agregar(Usuarios usuario);
        Task<Usuarios?> ObtenerPorEmailAsync(string email);
        Task<Usuarios?> Modificar(Usuarios usuario);
        Task<PerfilDto?> ModificarPerfil(PerfilDto usuario);
    }

    public class RepositorioUsuario : IUsuarioRepositorio
    {
        private readonly AppDbContext _context;

        // Recibimos el DbContext, no la configuración
        public RepositorioUsuario(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Usuarios> ObtenerTodos(string? email = null)
        {

            var query = _context.Usuarios.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(r => r.email.Contains(email.Trim()));
            }
            return query;
        }

        public async Task<Usuarios?> ObtenerPorId(int id)
        {
            return await _context.Usuarios.FindAsync(id);
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
        public async Task<Usuarios?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.email.ToLower() == email.ToLower());
        }
        public async Task<Usuarios?> Modificar(Usuarios usuario)
    {

        var usuarioEnDb = await _context.Usuarios.FindAsync(usuario.id_usuario);

        if (usuarioEnDb == null)
        {
            return null;
        }

        usuarioEnDb.nombre = usuario.nombre; 
        usuarioEnDb.apellido = usuario.apellido; 
        usuarioEnDb.email = usuario.email; 
        usuarioEnDb.rol = usuario.rol;


        if (!string.IsNullOrEmpty(usuario.clave))
        {
            usuarioEnDb.clave = usuario.clave; 
        }

        if (!string.IsNullOrEmpty(usuario.avatarUrl))
        {
            usuarioEnDb.avatarUrl = usuario.avatarUrl; 
        }
        
        await _context.SaveChangesAsync();

        return usuarioEnDb;
    }
    public async Task<PerfilDto?> ModificarPerfil(PerfilDto usuario)
    {
    var usuarioEnDb = await _context.Usuarios.FindAsync(usuario.id_usuario);

    if (usuarioEnDb == null)
    {
        return null;
    }

    usuarioEnDb.nombre = usuario.nombre ?? usuarioEnDb.nombre; 
    usuarioEnDb.apellido = usuario.apellido ?? usuarioEnDb.apellido; 
    usuarioEnDb.email = usuario.email ?? usuarioEnDb.email; 
    
    await _context.SaveChangesAsync();

    return new PerfilDto
    {
        id_usuario = usuarioEnDb.id_usuario,
        nombre = usuarioEnDb.nombre,
        apellido = usuarioEnDb.apellido,
        email = usuarioEnDb.email,
    };
    }
    }
}