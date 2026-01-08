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
        Task<Recibo_persona_fisica> CrearReciboConPagos(CrearReciboRequest request,string usuarioCreador);
        Task<bool> ExisteReciboEnLote(int id);
        IQueryable<Recibo_persona_fisica> ObtenerTodosyFormasPagosyLotes();
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
        public IQueryable<Recibo_persona_fisica> ObtenerTodosyFormasPagosyLotes()
        {
            return _context.Recibo_persona_fisica
                        .Include(r => r.FormasDePago)
                        .Include(r => r.Lote); 
        }
        public async Task<Recibo_persona_fisica?> ObtenerPorId(int id)
        {
            return _context.Recibo_persona_fisica.Find(id);
        }
        public async Task<bool> ExisteReciboEnLote(int id)
        {
            return await _context.Recibo_persona_fisica
                .AnyAsync(r => r.id_lote == id);
        }
        public async Task<Recibo_persona_fisica> Agregar(Recibo_persona_fisica recibo)
        {
            _context.Recibo_persona_fisica.Add(recibo);
            await _context.SaveChangesAsync();
            return recibo;
        }
      public async Task<Recibo_persona_fisica> CrearReciboConPagos(CrearReciboRequest request, string usuarioCreador)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var nuevoRecibo = new Recibo_persona_fisica
                {
                    id_lote = request.id_lote,
                    nombre = request.nombre,
                    apellido = request.apellido,
                    dni = request.dni,
                    tipo_dni = request.tipo_dni,
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

                _context.Recibo_persona_fisica.Add(nuevoRecibo);
                
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
                            id_recibo_persona_fisica = nuevoRecibo.id_recibo_persona_fisica, 
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
        public async Task<int> EliminarDirecto(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                await _context.Forma_Pagos
                    .Where(f => f.id_recibo_persona_fisica == id)
                    .ExecuteDeleteAsync();

                int filasAfectadas = await _context.Recibo_persona_fisica
                    .Where(u => u.id_recibo_persona_fisica == id)
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
        public async Task<Recibo_persona_fisica?> ObtenerPorIdyForma_pago(int id)
        {
            return await _context.Recibo_persona_fisica
                .Include(r => r.FormasDePago)
                .FirstOrDefaultAsync(r => r.id_recibo_persona_fisica == id);
        }
    }
    
}