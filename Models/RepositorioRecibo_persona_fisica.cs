using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using gestion_lotes.Models; 

namespace gestion_lotes.Models
{
    public interface IRecibo_persona_fisicaRepositorio
    {
        IQueryable<Recibo_persona_fisica> ObtenerTodos();
        Task<Recibo_persona_fisica?> ObtenerPorId(int id);
        Task<Recibo_persona_fisica> Agregar(Recibo_persona_fisica recibo);
        Task<int> EliminarDirecto(int id);

    }
    public class RepositorioRecibo_persona_fisica : IRecibo_persona_fisicaRepositorio
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
        public async Task<Recibo_persona_fisica> Agregar(Recibo_persona_fisica recibo)
        {
            _context.Recibo_persona_fisica.Add(recibo);
            await _context.SaveChangesAsync();
            return recibo;
        }
        public async Task<int> EliminarDirecto(int id)
        {
            return await _context.Recibo_persona_fisica
                .Where(u => u.id_recibo_persona_fisica == id)
                .ExecuteDeleteAsync();
        }
        public async Task<Recibo_persona_fisica?> Modificar(Recibo_persona_fisica recibo)
        {

            var reciboEnDb = await _context.Recibo_persona_fisica.FindAsync(recibo.id_recibo_persona_fisica);

            if (reciboEnDb == null)
            {
                return null;
            }

            reciboEnDb.nombre = recibo.nombre;
            reciboEnDb.apellido = recibo.apellido;
            reciboEnDb.tipo_dni = recibo.tipo_dni;
            reciboEnDb.dni = recibo.dni;
            reciboEnDb.telefono = recibo.telefono;
            reciboEnDb.codigo_postal = recibo.codigo_postal;
            reciboEnDb.email = recibo.email;
            reciboEnDb.creado_por = recibo.creado_por;
            reciboEnDb.domicilio = recibo.domicilio;
            reciboEnDb.provincia = recibo.provincia;

            
            await _context.SaveChangesAsync();

            return reciboEnDb;
        }
    }
}