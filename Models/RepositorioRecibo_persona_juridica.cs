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
    }
}