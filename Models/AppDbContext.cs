// AppDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace gestion_lotes.Models
{
    public class AppDbContext : DbContext
    {
        // El constructor es VITAL para la inyección de dependencias.
        // Le permite a tu aplicación (desde Program.cs) pasarle la configuración,
        // como la cadena de conexión.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Define tus tablas aquí ---
        // Por cada clase/entidad que quieras que se convierta en una tabla,
        // agrega una propiedad DbSet.

        public DbSet<Usuarios> Usuarios { get; set; }

        // Opcional: Puedes agregar configuraciones adicionales aquí si es necesario
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     // Por ejemplo, para configurar relaciones complejas, índices, etc.
        // }
    }
}