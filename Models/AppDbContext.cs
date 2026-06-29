// AppDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace gestion_lotes.Models
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Lotes> Lotes { get; set; }
        public DbSet<Recibo_persona_fisica> Recibo_persona_fisica { get; set; }
        public DbSet<Forma_Pagos> Forma_Pagos { get; set; }
        public DbSet<Recibo_persona_juridica> Recibo_persona_juridica { get; set; }
        public DbSet<Parametros> Parametros { get; set; }
        
    }
}