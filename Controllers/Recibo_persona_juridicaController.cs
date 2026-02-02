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
public class Recibo_persona_juridicaController : Controller
{
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment _environment;

    public Recibo_persona_juridicaController(IConfiguration config, IWebHostEnvironment environment)
    {
        this.config = config;
        _environment = environment;

    }
public IActionResult Index()
{
    return View(); 
}
}