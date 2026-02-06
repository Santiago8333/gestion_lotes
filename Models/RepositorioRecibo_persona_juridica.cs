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
        Task<Recibo_persona_juridica> CrearReciboConPagos(CrearReciboRequestJuridica request, string usuarioCreador);
        Task<bool> ExisteReciboEnLote(int id);
        Task<int> EliminarDirecto(int id);
        Task<Recibo_persona_juridica?> ObtenerPorId(int id);
        Task<bool> ExisteLoteDuplicado(int idLote, int? idReciboFisicaExcluir = null, int? idReciboJuridicaExcluir = null);
        Task<Recibo_persona_juridica> ModificarReciboConPagos(int idRecibo, CrearReciboRequestJMd request, string usuarioModificador);
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
        public async Task<Recibo_persona_juridica> CrearReciboConPagos(CrearReciboRequestJuridica request, string usuarioCreador)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var nuevoRecibo = new Recibo_persona_juridica
                {
                    id_lote = request.id_lote,
                    razon_social = request.razon_social,
                    apoderado_socio = request.apoderado_socio,
                    numero = request.numero,
                    tipo = request.tipo,
                    telefono = request.telefono,
                    email = request.email,
                    domicilio = request.domicilio,
                    codigo_postal = request.codigo_postal,
                    provincia = request.provincia,
                    precio_subastado = request.precio_subastado,
                    pago_lote = request.pago_lote,
                    
                    // Campos automáticos
                    fecha_creacion = DateTime.Now,
                    estado = true,
                    creado_por = usuarioCreador
                };

                _context.Recibo_persona_juridica.Add(nuevoRecibo);
                
                // GUARDAMOS CAMBIOS: Esto genera el ID del recibo en la BD
                await _context.SaveChangesAsync(); 

                // 2. Procesar la lista de pagos (Detalle)
                if (request.lista_pagos != null && request.lista_pagos.Count > 0)
                {
                    var pagosEntidad = new List<Forma_Pagos>();

                    foreach (var item in request.lista_pagos)
                    {
                        var nuevoPago = new Forma_Pagos
                        {
                            id_recibo_persona_juridica = nuevoRecibo.id_recibo_persona_juridica, 
                            destinatario = item.destinatario,
                            efectivo = item.efectivo,
                            transferencia = item.transferencia,
                            dolar_monto = item.dolar_monto,
                            dolar_cotizacion = item.dolar_cotizacion,
                            euro_monto = item.euro_monto,
                            euro_cotizacion = item.euro_cotizacion
                        };
                        pagosEntidad.Add(nuevoPago);
                    }

                    // Agregamos todos los pagos de una sola vez
                    _context.Forma_Pagos.AddRange(pagosEntidad);
                    await _context.SaveChangesAsync();
                }

                // 3. Confirmar transacción
                await transaction.CommitAsync();
                return nuevoRecibo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=================================");
                System.Diagnostics.Debug.WriteLine("ERROR CRÍTICO: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("=================================");
                
                // Si usas 'dotnet run' en consola, usa este:
                Console.WriteLine("ERROR CRÍTICO: " + ex.ToString());

                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> ExisteReciboEnLote(int id)
        {
            return await _context.Recibo_persona_fisica.AnyAsync(r => r.id_lote == id) 
                || await _context.Recibo_persona_juridica.AnyAsync(r => r.id_lote == id);
        }
        public async Task<int> EliminarDirecto(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                await _context.Forma_Pagos
                    .Where(f => f.id_recibo_persona_juridica == id)
                    .ExecuteDeleteAsync();

                int filasAfectadas = await _context.Recibo_persona_juridica
                    .Where(u => u.id_recibo_persona_juridica == id)
                    .ExecuteDeleteAsync();

                await transaction.CommitAsync();
                
                return filasAfectadas;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<Recibo_persona_juridica?> ObtenerPorId(int id)
        {
            return await _context.Recibo_persona_juridica.FindAsync(id);
        }
        public async Task<Recibo_persona_juridica> ModificarReciboConPagos(int idRecibo, CrearReciboRequestJMd request, string usuarioModificador)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var reciboExistente = await _context.Recibo_persona_juridica
                    .FirstOrDefaultAsync(x => x.id_recibo_persona_juridica == idRecibo);

                if (reciboExistente == null)
                {
                    throw new Exception($"No se encontró el recibo con ID {idRecibo}");
                }

                
                reciboExistente.id_lote = request.id_lote;
                reciboExistente.razon_social = request.razon_social;
                reciboExistente.apoderado_socio = request.apoderado_socio;
                reciboExistente.numero = request.numero;
                reciboExistente.tipo = request.tipo;
                reciboExistente.telefono = request.telefono;
                reciboExistente.email = request.email;
                reciboExistente.domicilio = request.domicilio;
                reciboExistente.codigo_postal = request.codigo_postal;
                reciboExistente.provincia = request.provincia;
                reciboExistente.precio_subastado = request.precio_subastado;
                reciboExistente.pago_lote = request.pago_lote;

                
                // reciboExistente.fecha_modificacion = DateTime.Now; 
                // reciboExistente.modificado_por = usuarioModificador;
                var pagosAnteriores = await _context.Forma_Pagos
                    .Where(x => x.id_recibo_persona_fisica == idRecibo)
                    .ToListAsync();

                
                if (pagosAnteriores.Any())
                {
                    _context.Forma_Pagos.RemoveRange(pagosAnteriores);
                }

                
                if (request.lista_pagos != null && request.lista_pagos.Count > 0)
                {
                    var nuevosPagosEntidad = new List<Forma_Pagos>();

                    foreach (var item in request.lista_pagos)
                    {
                        var nuevoPago = new Forma_Pagos
                        {
                            id_recibo_persona_juridica = reciboExistente.id_recibo_persona_juridica,
                            destinatario = item.destinatario,
                            efectivo = item.efectivo,
                            transferencia = item.transferencia,
                            dolar_monto = item.dolar_monto,
                            dolar_cotizacion = item.dolar_cotizacion,
                            euro_monto = item.euro_monto,
                            euro_cotizacion = item.euro_cotizacion
                        };
                        nuevosPagosEntidad.Add(nuevoPago);
                    }

                    _context.Forma_Pagos.AddRange(nuevosPagosEntidad);
                }

                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return reciboExistente;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=================================");
                System.Diagnostics.Debug.WriteLine("ERROR AL MODIFICAR: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("=================================");
                Console.WriteLine("ERROR AL MODIFICAR: " + ex.ToString());

                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> ExisteLoteDuplicado(int idLote, int? idReciboFisicaExcluir = null, int? idReciboJuridicaExcluir = null)
        {
            
            bool existeEnFisica = await _context.Recibo_persona_fisica
                .AnyAsync(r => r.id_lote == idLote && 
                        (!idReciboFisicaExcluir.HasValue || r.id_recibo_persona_fisica != idReciboFisicaExcluir));

            if (existeEnFisica) return true;

            bool existeEnJuridica = await _context.Recibo_persona_juridica
                .AnyAsync(r => r.id_lote == idLote && 
                        (!idReciboJuridicaExcluir.HasValue || r.id_recibo_persona_juridica != idReciboJuridicaExcluir));

            return existeEnJuridica;
        }
    }
}