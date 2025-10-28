using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using gestion_lotes.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
public IActionResult Index(int pagina = 1)
{
    // --- NUEVO CÓDIGO EFICIENTE ---
    int registrosPorPagina = 10;
    
    // Obtener la consulta IQueryable sin ejecutar
    var consultaUsuarios = repo.ObtenerTodos(); 

    // 1. Conteo total (EF ejecuta SELECT COUNT(*))
    // Esta es una consulta SQL muy rápida.
    var totalDeRegistros = consultaUsuarios.Count(); 

    // 2. Paginación (EF ejecuta SELECT ... LIMIT X, Y)
    // Se ejecuta una sola consulta que solo trae los 10 registros necesarios.
    var usuariosPaginados = consultaUsuarios
                            .Skip((pagina - 1) * registrosPorPagina)
                            .Take(registrosPorPagina)
                            .ToList(); // AQUÍ y SOLO AQUÍ se ejecuta la consulta

    // Asignación de ViewBag y retorno a la vista...
    ViewBag.PaginaActual = pagina;
    ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalDeRegistros / registrosPorPagina);
    ViewBag.Registros = totalDeRegistros > 0;
    
    return View(usuariosPaginados);
}

}