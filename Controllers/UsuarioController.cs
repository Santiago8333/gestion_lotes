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
using Google.Protobuf.WellKnownTypes;
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
            const string defaultAvatarName = "default-avatar.png";
            bool isDefaultAvatar = true;
            if (!string.IsNullOrEmpty(usuario.avatarUrl))
            {
                isDefaultAvatar = usuario.avatarUrl.EndsWith(defaultAvatarName);
            }
            if (!isDefaultAvatar && !string.IsNullOrEmpty(usuario.avatarUrl))
            {
                try
                {
                    var relativePath = usuario.avatarUrl.TrimStart('/');
                    
                    relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

                    var physicalPath = Path.Combine(_environment.WebRootPath, relativePath);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al borrar avatar: {ex.Message}");
                }
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
    [Route("/api/usuarios")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar([Bind("nombre,apellido,email,clave,rol")] Usuarios usuario, IFormFile? avatarFile)
    {
        // Verifica si los datos recibidos del formulario son válidos
        if (!ModelState.IsValid)
        {
            // Devolvemos un error 400 (Bad Request) con los mensajes
            var errores = ModelState.Values.SelectMany(v => v.Errors)
                                         .Select(e => e.ErrorMessage);
            return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
        }
        var usuarioExistente = await repo.ObtenerPorEmailAsync(usuario.email);

        if (usuarioExistente != null)
        {
            return BadRequest(new { mensaje = "El email ingresado ya está registrado." });
        }
        try
        {
            // Lógica para manejar el archivo de avatar (si se subió uno)
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_environment.WebRootPath, "images", "avatars");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                string filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }
                // Guarda la ruta del archivo en el objeto usuario
                usuario.avatarUrl = $"/images/avatars/{fileName}";
            }
            else
            {
                usuario.avatarUrl = "/images/avatars/default-avatar.png";
            }
            // Hashea la contraseña antes de guardarla
            var saltString = config["Salt"];
            if (string.IsNullOrEmpty(saltString))
            {
                throw new InvalidOperationException("El valor 'Salt' no está configurado en el archivo de configuración.");
            }
            //Console.WriteLine("dentro: " + nuevoUsuario);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuario.clave,
                salt: System.Text.Encoding.ASCII.GetBytes(saltString),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            usuario.clave = hashed;
            usuario.fecha_creacion = DateTime.Now;
            // Llama al método del repositorio para agregar el usuario
            await repo.Agregar(usuario);

            return Ok(new { mensaje = "¡Usuario agregado exitosamente!" });
        }
        catch (Exception ex)
        {
            // 7. Devolver respuesta JSON de ERROR
            return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
        }
    }
[HttpPut] 
[Route("/api/usuarios/{id_usuario:int}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Modificar(int id_usuario, 
    [Bind("id_usuario,nombre,apellido,email,clave,rol")] Usuarios usuario, 
    IFormFile? avatarFile)
{
    ModelState.Remove("clave");
    if (id_usuario != usuario.id_usuario)
    {
        return BadRequest(new { mensaje = "Inconsistencia en el ID del usuario." });
    }

   
    if (!ModelState.IsValid)
    {
        var errores = ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage);
        return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
    }

    
    var usuarioExistente = await repo.ObtenerPorEmailAsync(usuario.email);
    if (usuarioExistente != null && usuarioExistente.id_usuario != usuario.id_usuario)
    {
        return BadRequest(new { mensaje = "El email ingresado ya está registrado por otro usuario." });
    }
    var usuarioEnDb = await repo.ObtenerPorId(usuario.id_usuario);
    if (usuarioEnDb == null)
    {
        return NotFound(new { mensaje = "El usuario que intentas modificar no existe." });
    }
    try
    {

        string avatarAntiguoUrl = usuarioEnDb.avatarUrl;

    // 3. Tu código para manejar el NUEVO archivo
    if (avatarFile != null && avatarFile.Length > 0)
    {
        var uploadsDir = Path.Combine(_environment.WebRootPath, "images", "avatars");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
        string filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatarFile.CopyToAsync(stream);
        }
        
        
        usuario.avatarUrl = $"/images/avatars/{fileName}"; 
        
       
        if (!string.IsNullOrEmpty(avatarAntiguoUrl) && 
            avatarAntiguoUrl != "/images/avatars/default-avatar.png") // No borrar el default
        {
            try
            {
                
                string nombreArchivoAntiguo = Path.GetFileName(avatarAntiguoUrl);
                string rutaFisicaAntigua = Path.Combine(uploadsDir, nombreArchivoAntiguo);

                if (System.IO.File.Exists(rutaFisicaAntigua))
                {
                    System.IO.File.Delete(rutaFisicaAntigua);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error al eliminar avatar antiguo: {ex.Message}");
            }
        }
    }
    else
    {
        
        usuario.avatarUrl = avatarAntiguoUrl;
    }

        if (!string.IsNullOrEmpty(usuario.clave))
        {
            var saltString = config["Salt"];
            if (string.IsNullOrEmpty(saltString))
            {
                throw new InvalidOperationException("El valor 'Salt' no está configurado.");
            }
            
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuario.clave,
                salt: System.Text.Encoding.ASCII.GetBytes(saltString),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            usuario.clave = hashed;
            }
            else
            {
                usuario.clave = usuarioEnDb.clave;
            }

        var usuarioModificado = await repo.Modificar(usuario);

        if (usuarioModificado == null)
        {
            return NotFound(new { mensaje = "Usuario no encontrado." });
        }

        return Ok(new { mensaje = "¡Usuario modificado exitosamente!" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
}