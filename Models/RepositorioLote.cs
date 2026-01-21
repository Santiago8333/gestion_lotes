using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using gestion_lotes.Models; 

namespace gestion_lotes.Models
{

    public interface ILoteRepositorio
    {
        IQueryable<Lotes> ObtenerTodos();
        Task<Lotes?> ObtenerPorId(int id);
        Task<Lotes?> ObtenerPorNloteAsync(int n_lote);
        Task<Lotes> Agregar(Lotes lote);
        Task<int> EliminarDirecto(int id);
        Task<Lotes?> Modificar(Lotes lote);
        Task<IEnumerable<Lotes>> BuscarLotesGeneralAsync(string terminoBusqueda);
    }

    public class RepositorioLote : ILoteRepositorio
    {
        private readonly AppDbContext _context;

        public RepositorioLote(AppDbContext context)
        {
            _context = context;
        }

        
        public IQueryable<Lotes> ObtenerTodos()
        {
            return _context.Lotes;
        }
        public async Task<Lotes?> ObtenerPorId(int id)
        {
            return _context.Lotes.Find(id);
        }
        public async Task<Lotes?> ObtenerPorNloteAsync(int n_lote)
        {
            return await _context.Lotes
                .FirstOrDefaultAsync(u => u.n_lote == n_lote);
        }
        public async Task<Lotes> Agregar(Lotes lote)
        {
            _context.Lotes.Add(lote);
            await _context.SaveChangesAsync();
            return lote;
        }
    public async Task<int> EliminarDirecto(int idLote)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            
            await _context.Forma_Pagos
            .Where(fp => _context.Recibo_persona_fisica
                .Where(r => r.id_lote == idLote)
                .Select(r => r.id_recibo_persona_fisica)
                .Contains(fp.id_recibo_persona_fisica))
            .ExecuteDeleteAsync();

           await _context.Recibo_persona_fisica
            .Where(r => r.id_lote == idLote)
            .ExecuteDeleteAsync();

            var lotesEliminados = await _context.Lotes
                .Where(u => u.id_lote == idLote)
                .ExecuteDeleteAsync();

            await transaction.CommitAsync();

            return lotesEliminados;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<Lotes?> Modificar(Lotes lote)
    {

        var loteEnDb = await _context.Lotes.FindAsync(lote.id_lote);

        if (loteEnDb == null)
        {
            return null;
        }

        loteEnDb.n_lote = lote.n_lote; 
        loteEnDb.marca = lote.marca; 
        loteEnDb.modelo = lote.modelo; 
        loteEnDb.dominio = lote.dominio;
        loteEnDb.anio = lote.anio;
        loteEnDb.@base = lote.@base;
        
        await _context.SaveChangesAsync();

        return loteEnDb;
    }
    public async Task<IEnumerable<Lotes>> BuscarLotesGeneralAsync(string terminoBusqueda)
    {
        
        if (string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            return new List<Lotes>();
        }

       
        bool esNumero = int.TryParse(terminoBusqueda, out int numeroLote);

        var query = _context.Lotes.AsQueryable();

        if (esNumero)
        {
            query = query.Where(l => l.n_lote == numeroLote || l.marca.Contains(terminoBusqueda));
        }
        else
        {
            
            query = query.Where(l => l.marca.Contains(terminoBusqueda));
            
        }

        return await query.ToListAsync();
    }
    
    }
}