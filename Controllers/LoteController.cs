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
public class LoteController : Controller
{
    private readonly ILoteRepositorio repo;
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment _environment;

    public LoteController(ILoteRepositorio repositorio, IConfiguration config, IWebHostEnvironment environment)
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
[Route("api/lotes")]
public async Task<IActionResult> ObtenerLotes(int pagina = 1,string? marca = null, string? numeroLote = null)
{
    int registrosPorPagina = 5;

    var consultaLotes = repo.ObtenerTodos(marca,numeroLote).OrderBy(u => u.n_lote);
    var totalDeRegistros = await consultaLotes.CountAsync();

    var lotesPaginados = await consultaLotes
                                    .Skip((pagina - 1) * registrosPorPagina)
                                    .Take(registrosPorPagina)
                                    .ToListAsync();

    var resultado = new
    {
        PaginaActual = pagina,
        TotalPaginas = (int)Math.Ceiling((double)totalDeRegistros / registrosPorPagina),
        Lotes = lotesPaginados
    };

    return Ok(resultado);
}
[HttpGet]
[Route("api/buscar/lotes/{termino}")]
public async Task<IActionResult> ObtenerLotesBuscar(string termino)
{
    var resultados = await repo.BuscarLotesGeneralAsync(termino);

    if (resultados == null || !resultados.Any())
    {
        return NotFound($"No se encontraron lotes con el criterio: {termino}");
    }

    return Ok(resultados);
}
[HttpPost]
[Route("/api/lotes")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Agregar([Bind("n_lote,marca,modelo,dominio,anio,base")] Lotes lote)
{
    // Verifica si los datos recibidos del formulario son válidos
    ModelState.Remove("creado_por");
    if (!ModelState.IsValid)
    {
        // Devolvemos un error 400 (Bad Request) con los mensajes
        var errores = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
        return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
    }
    var LoteExistente = await repo.ObtenerPorNloteAsync(lote.n_lote);

    if (LoteExistente != null)
    {
        return BadRequest(new { mensaje = "El numero de lote ingresado ya está registrado." });
    }
    try
    {
        lote.fecha_creacion = DateTime.Now;
        lote.estado = true;
        lote.creado_por = User.Identity?.Name ?? "Sistema"; 
        await repo.Agregar(lote);

        return Ok(new { mensaje = "¡Lote agregado exitosamente!" });
    }
    catch (Exception ex)
    {
        // 7. Devolver respuesta JSON de ERROR
        return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
[Authorize(Policy = "Administrador")]
[HttpDelete]
[Route("api/lotes/{id}")]
public async Task<IActionResult> EliminarLote(int id)
{
    try
    {

        var lote = await repo.ObtenerPorNloteAsync(id);
        if (lote == null)
        {
            return NotFound(new { mensaje = "El lote no fue encontrado." });
        }
        
        await repo.EliminarDirecto(id);

        return Ok(new { mensaje = "Lote eliminado exitosamente." });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { mensaje = "Ocurrió un error interno al intentar eliminar el lote."+ex });
    }
}
[HttpPut]
[Route("/api/lotes/{id_lote:int}")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Modificar(int id_lote,[Bind("id_lote,n_lote,marca,modelo,dominio,anio,base")] Lotes lote)
{
    ModelState.Remove("creado_por");
    ModelState.Remove("fecha_creacion");
    if (id_lote != lote.id_lote)
    {
        return BadRequest(new { mensaje = "Inconsistencia en el ID del lote." });
    }


    if (!ModelState.IsValid)
    {
        var errores = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
        return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
    }

    var loteEnDb = await repo.ObtenerPorNloteAsync(lote.n_lote);
    if (loteEnDb == null)
    {
        return NotFound(new { mensaje = "El lote que intentas modificar no existe." });
    }
    try
    {

        var loteModificado = await repo.Modificar(lote);

        if (loteModificado == null)
        {
            return NotFound(new { mensaje = "Lote no encontrado." });
        }

        return Ok(new { mensaje = "¡Lote modificado exitosamente!" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
}