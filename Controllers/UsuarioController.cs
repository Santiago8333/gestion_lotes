using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using gestion_lotes.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace gestion_lotes.Controllers;



public class UsuarioController : Controller
{
    private readonly IUsuarioRepositorio repo;
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment _environment;
   
    public UsuarioController(IUsuarioRepositorio repositorio, IConfiguration config, IWebHostEnvironment environment)
    {
        this.repo = repositorio;
        this.config = config;
        _environment = environment;
       
    }
public IActionResult Index()
{
    return View(); 
}


    [HttpGet]
    [Route("api/usuarios")]
    public async Task<IActionResult> ObtenerUsuarios(int pagina = 1)
    {
        int registrosPorPagina = 10;

        var consultaUsuarios = repo.ObtenerTodos().OrderBy(u => u.nombre);
        var totalDeRegistros = await consultaUsuarios.CountAsync();

        var usuariosPaginados = await consultaUsuarios
                                        .Skip((pagina - 1) * registrosPorPagina)
                                        .Take(registrosPorPagina)
                                        .ToListAsync();

        var resultado = new
        {
            PaginaActual = pagina,
            TotalPaginas = (int)Math.Ceiling((double)totalDeRegistros / registrosPorPagina),
            Usuarios = usuariosPaginados
        };

        return Ok(resultado);
    }
    [HttpDelete]
    [Route("api/usuarios/{id}")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        try
        {

            var usuario = await repo.ObtenerPorId(id);
            if (usuario == null)
            {

                return NotFound(new { mensaje = "El usuario no fue encontrado." });
            }


            await repo.EliminarDirecto(id);

            return Ok(new { mensaje = "Usuario eliminado exitosamente." });
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { mensaje = "Ocurrió un error interno al intentar eliminar el usuario." });
        }
    }
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Agregar([Bind("Nombre,Apellido,Email,Clave,Rol")] Usuarios usuario, IFormFile? AvatarFile)
{
    // Verifica si los datos recibidos del formulario son válidos
    if (ModelState.IsValid)
    {
        // Lógica para manejar el archivo de avatar (si se subió uno)
        if (AvatarFile != null && AvatarFile.Length > 0)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(AvatarFile.FileName);
            string filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await AvatarFile.CopyToAsync(stream);
            }
            // Guarda la ruta del archivo en el objeto usuario
            usuario.avatarUrl = $"/uploads/avatars/{fileName}";
        }

        // Llama al método del repositorio para agregar el usuario
        await repo.Agregar(usuario);

        // Redirige al usuario a la página principal de usuarios
        return RedirectToAction(nameof(Index));
    }

    // Si el modelo no es válido, vuelve a mostrar la vista del formulario
    // con los datos que el usuario ya había ingresado.
    return View(usuario);
}
}