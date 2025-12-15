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
public class Recibo_persona_fisicaController : Controller
{
    private readonly IRecibo_persona_fisicaRepositorio repo;
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment _environment;

    public Recibo_persona_fisicaController(IRecibo_persona_fisicaRepositorio repositorio, IConfiguration config, IWebHostEnvironment environment)
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
[Route("api/recibof")]
public async Task<IActionResult> ObtenerReciboF(int pagina = 1)
{
    int registrosPorPagina = 5;

    var consultaRecibo = repo.ObtenerTodos().OrderBy(u => u.fecha_creacion);
    var totalDeRegistros = await consultaRecibo.CountAsync();

    var reciboPaginados = await consultaRecibo
                                    .Skip((pagina - 1) * registrosPorPagina)
                                    .Take(registrosPorPagina)
                                    .ToListAsync();

    var resultado = new
    {
        PaginaActual = pagina,
        TotalPaginas = (int)Math.Ceiling((double)totalDeRegistros / registrosPorPagina),
        Recibos = reciboPaginados
    };

    return Ok(resultado);
}
[HttpPost]
[Route("/api/recibos")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Agregar([Bind("id_lote,nombre,apellido,tipo_dni,dni,telefono,codigo_postal,email,domicilio,provincia,pago_lote,precio_subastado")] Recibo_persona_fisica recibo)
{
    // Verifica si los datos recibidos del formulario son válidos
    ModelState.Remove("creado_por");
    ModelState.Remove("estado");
    if (!ModelState.IsValid)
    {
        // Devolvemos un error 400 (Bad Request) con los mensajes
        var errores = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
        return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
    }
    /*
    var LoteExistente = await repo.ObtenerPorNloteAsync(lote.n_lote);

    if (LoteExistente != null)
    {
        return BadRequest(new { mensaje = "El numero de recibo ingresado ya está registrado." });
    }
    */
    try
    {
        recibo.fecha_creacion = DateTime.Now;
        recibo.estado = true;
        recibo.creado_por = User.Identity?.Name ?? "Sistema"; 
        //await repo.Agregar(recibo);

        return Ok(new { mensaje = "¡Recibo agregado exitosamente!" });
    }
    catch (Exception ex)
    {
        // 7. Devolver respuesta JSON de ERROR
        return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
[HttpDelete]
[Route("api/recibos/{id}")]
public async Task<IActionResult> EliminarLote(int id)
{
    try
    {

        var recibo = await repo.ObtenerPorId(id);
        if (recibo == null)
        {
            return NotFound(new { mensaje = "El recibo no fue encontrado." });
        }
        
        await repo.EliminarDirecto(id);

        return Ok(new { mensaje = "recibo eliminado exitosamente." });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { mensaje = "Ocurrió un error interno al intentar eliminar el recibo."+ex });
    }
}
}
