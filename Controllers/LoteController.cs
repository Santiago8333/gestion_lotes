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
}