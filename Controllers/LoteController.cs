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
public async Task<IActionResult> ObtenerLotes(int pagina = 1)
{
    int registrosPorPagina = 10;

    var consultaLotes = repo.ObtenerTodos().OrderBy(u => u.n_lote);
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
[HttpPost]
[Route("/api/lotes")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Agregar([Bind("n_lote,marca,modelo,dominio,año,base")] Lotes lote)
{
    // Verifica si los datos recibidos del formulario son válidos
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
       
        
        await repo.Agregar(lote);

        return Ok(new { mensaje = "¡Lote agregado exitosamente!" });
    }
    catch (Exception ex)
    {
        // 7. Devolver respuesta JSON de ERROR
        return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
    }
}
}