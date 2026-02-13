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


[Authorize]
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
[Authorize(Policy = "Administrador")]
public IActionResult Index()
{
    return View(); 
}
public IActionResult Perfil()
{
    return View(); 
}
[HttpGet]
[Route("api/miperfil")] 
public async Task<IActionResult> ObtenerMiPerfil()
{
    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; 

    if (idClaim == null) return Unauthorized();

    int idUsuario = int.Parse(idClaim);

    var usuario = await repo.ObtenerPorId(idUsuario);
    if (usuario == null)
    {
        return NotFound(new { mensaje = "Usuario no encontrado." });
    }
    var perfilDto = new PerfilDto 
    {
        nombre = usuario.nombre,
        apellido = usuario.apellido,
        email = usuario.email,
        id_usuario = usuario.id_usuario
    };

    return Ok(perfilDto);
}
[HttpPut]
[Route("api/actualizarperfil")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ModificarPerfil([FromBody] PerfilDto usuario)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState); 
    }

    try
    {
        var perfilActualizado = await repo.ModificarPerfil(usuario);

        if (perfilActualizado == null)
        {
            return NotFound(new { mensaje = "No se encontró el usuario especificado." });
        }
        var rolActual = User.FindFirst(ClaimTypes.Role)?.Value ?? "Usuario";
        var avatarActual = User.FindFirst("AvatarUrl")?.Value ?? "";

        var nuevoAvatar = !string.IsNullOrEmpty(usuario.avatarUrl) ? usuario.avatarUrl : avatarActual;

       
        var nuevasClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, perfilActualizado.id_usuario.ToString()),
            new Claim(ClaimTypes.Name, perfilActualizado.email ?? ""),
            new Claim("FullName", perfilActualizado.nombre + " " + perfilActualizado.apellido),
            new Claim("AvatarUrl", nuevoAvatar), 
            new Claim(ClaimTypes.Role, rolActual)
        };

        var claimsIdentity = new ClaimsIdentity(nuevasClaims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties 
            { 
                IsPersistent = true 
            }
        );
        return Ok(new { 
            mensaje = "Perfil actualizado correctamente", 
            datos = perfilActualizado 
        });
    }
    catch (Exception ex)
    {
        
        return StatusCode(StatusCodes.Status500InternalServerError, new { 
            mensaje = "Error interno del servidor al actualizar el perfil.",
            error = ex.Message
        });
    }
}
[AllowAnonymous]
public IActionResult Login()
{
    return View();
}
public async Task<ActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Login", "Usuario");
}

    [HttpGet]
    [Route("api/usuarios")]
    public async Task<IActionResult> ObtenerUsuarios(int pagina = 1,string? email = null)
    {
        int registrosPorPagina = 10;

        var consultaUsuarios = repo.ObtenerTodos(email).OrderBy(u => u.nombre);
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
    [Authorize(Policy = "Administrador")]
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
            return StatusCode(500, new { mensaje = "Ocurrió un error interno al intentar eliminar el usuario."+ex });
        }
    }
    [Authorize(Policy = "Administrador")]
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
    [Authorize(Policy = "Administrador")]
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

            string avatarAntiguoUrl = usuarioEnDb.avatarUrl ?? "";

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
[AllowAnonymous]
[HttpPost]
public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new { success = false, message = "Datos de entrada inválidos." });
    }

    var saltString = config["Salt"];
    if (string.IsNullOrEmpty(saltString))
    {
        Console.WriteLine("Error crítico: 'Salt' no está configurado.");
        return StatusCode(500, new { success = false, message = "Error interno del servidor." });
    }
    
    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: loginModel.clave, 
        salt: System.Text.Encoding.ASCII.GetBytes(saltString),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 1000,
        numBytesRequested: 256 / 8));

    var e = await repo.ObtenerPorEmailAsync(loginModel.email); 

    if (e == null || e.clave != hashed)
    {
        return Unauthorized(new { success = false, message = "Email o contraseña incorrectos." });
    }

    var AvatarUrl = e.avatarUrl;
    if (string.IsNullOrEmpty(AvatarUrl))
    {
        AvatarUrl = "/uploads/avatars/default.png"; 
    }
    string rolNombre = "Empleado"; 
    if (e.rol == 1) 
    {
        rolNombre = "Administrador";
    }
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, e.id_usuario.ToString()),
        new Claim(ClaimTypes.Name, e.email),
        new Claim("FullName", e.nombre + " " + e.apellido),
        new Claim("AvatarUrl", AvatarUrl), 
        new Claim(ClaimTypes.Role, rolNombre),
    };
    
    var claimsIdentity = new ClaimsIdentity(
        claims, CookieAuthenticationDefaults.AuthenticationScheme);

    
    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity));

    
    return Ok(new 
    { 
        success = true, 
        message = "Login exitoso",
        redirectUrl = Url.Action("Index", "Home") 
    });
}
}