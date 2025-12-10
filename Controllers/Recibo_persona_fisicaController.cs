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
}
