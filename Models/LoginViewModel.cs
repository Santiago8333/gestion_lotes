namespace gestion_lotes.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
public class LoginViewModel
{
    [Required(ErrorMessage = "El Email es obligatorio.")]
    [EmailAddress]
    public string email { get; set; } = "";

    [Required(ErrorMessage = "La Clave es obligatoria.")]
    public string clave { get; set; } = "";
}