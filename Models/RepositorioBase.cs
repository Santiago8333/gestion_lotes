// Ya no necesitas IConfiguration aquí
using gestion_lotes.Models; // O donde sea que esté tu DbContext

namespace gestion_lotes.Models
{
    public abstract class RepositorioBase<T> where T : class
    {
        // El DbContext es ahora la pieza central
        protected readonly AppDbContext _context;

        // El constructor recibe el DbContext a través de inyección de dependencias
        protected RepositorioBase(AppDbContext context)
        {
            _context = context;
        }

        // Puedes agregar métodos comunes aquí si quieres
        public virtual void GuardarCambios()
        {
            _context.SaveChanges();
        }
    }
}