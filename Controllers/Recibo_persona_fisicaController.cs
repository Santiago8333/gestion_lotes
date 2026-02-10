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
public async Task<IActionResult> ObtenerReciboF(int pagina = 1, string? apellido = null, string? numeroLote = null)
{
    int registrosPorPagina = 5;

    var queryBase = repo.ObtenerTodosyFormasPagosyLotes(apellido, numeroLote);

    var totalDeRegistros = await queryBase.CountAsync();

    var reciboPaginados = await queryBase
                                .OrderBy(u => u.fecha_creacion)
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

// [ValidateAntiForgeryToken]
[HttpPost]
[Route("/api/recibos")]
public async Task<IActionResult> Agregar([FromBody] CrearReciboRequest datos)
{
    ModelState.Remove("creado_por");
    ModelState.Remove("fecha_creacion");
     if (!ModelState.IsValid)
    {
        var errores = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
        return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
    }

    try
    {
        bool LoteExiste = await repo.ExisteReciboEnLote(datos.id_lote);

        if (LoteExiste)
        {
            return Conflict(new { mensaje = $"El lote {datos.id_lote} ya tiene un recibo asociado y no puede duplicarse." });
        }
        string usuario = User.Identity?.Name ?? "Sistema";
        var recibo = await repo.CrearReciboConPagos(datos,usuario);
        
        return Ok(new { mensaje = "Recibo agregado"});
    }
    catch (Exception ex)
    {
         return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
[HttpDelete]
[Route("api/recibos/{id}")]
public async Task<IActionResult> EliminarRecibo(int id)
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
[HttpPut]
[Route("/api/recibosmd")]
public async Task<IActionResult> Modificar([FromBody] CrearReciboRequestMd datos)
{
    ModelState.Remove("creado_por");
    ModelState.Remove("fecha_creacion");
     if (!ModelState.IsValid)
    {
        var errores = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
        return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
    }

    try
    {
        bool loteDuplicado = await repo.ExisteLoteDuplicado(
            datos.id_lote, 
            idReciboFisicaExcluir: datos.id_recibo_persona_fisica, 
            idReciboJuridicaExcluir: null
        );

        if (loteDuplicado)
        {
            return Conflict(new { mensaje = $"El lote {datos.n_lote} ya tiene un recibo asociado y no puede duplicarse." });
        }
        string usuario = User.Identity?.Name ?? "Sistema";
        var recibo = await repo.ModificarReciboConPagos(datos.id_recibo_persona_fisica,datos,usuario);
        
        return Ok(new { mensaje = "Recibo modificado"});
    }
    catch (Exception ex)
    {
         return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
}
