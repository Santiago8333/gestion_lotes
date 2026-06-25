using Microsoft.AspNetCore.Mvc;
using gestion_lotes.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace gestion_lotes.Controllers;

[Authorize]
public class ParametroController : Controller
{
    private readonly IParametrosRepositorio repo;

    public ParametroController(IParametrosRepositorio repositorio)
    {
        this.repo = repositorio;
    }

    // La pantalla de configuración solo es accesible para el administrador.
    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("api/parametros")]
    public async Task<IActionResult> Obtener()
    {
        var parametros = await repo.ObtenerVigentes();
        return Ok(parametros);
    }

    // Solo el administrador puede modificar los porcentajes.
    [Authorize(Policy = "Administrador")]
    [HttpPut]
    [Route("api/parametros")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar([FromBody] Parametros parametros)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage);
            return BadRequest(new { mensaje = "Datos inválidos: " + string.Join(", ", errores) });
        }

        try
        {
            var actualizado = await repo.Modificar(parametros);
            return Ok(new { mensaje = "Parámetros actualizados correctamente.", datos = actualizado });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = $"Error interno del servidor: {ex.Message}" });
        }
    }
}
