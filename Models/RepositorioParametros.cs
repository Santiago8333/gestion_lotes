using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace gestion_lotes.Models
{
    public interface IParametrosRepositorio
    {
        Task<Parametros> ObtenerVigentes();
        Task<Parametros?> Modificar(Parametros parametros);
    }

    public class RepositorioParametros : IParametrosRepositorio
    {
        private readonly AppDbContext _context;

        public RepositorioParametros(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<Parametros> ObtenerVigentes()
        {
            var parametros = await _context.Parametros
                .OrderBy(p => p.id_parametros)
                .FirstOrDefaultAsync();

            if (parametros == null)
            {
                parametros = new Parametros { honorarios = 10m, sellado = 0.6m };
                _context.Parametros.Add(parametros);
                await _context.SaveChangesAsync();
            }

            return parametros;
        }

        public async Task<Parametros?> Modificar(Parametros parametros)
        {
            var enDb = await _context.Parametros
                .OrderBy(p => p.id_parametros)
                .FirstOrDefaultAsync();

            if (enDb == null)
            {
                enDb = new Parametros();
                _context.Parametros.Add(enDb);
            }

            enDb.honorarios = parametros.honorarios;
            enDb.sellado = parametros.sellado;

            await _context.SaveChangesAsync();

            return enDb;
        }
    }
}
