namespace gestion_lotes.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
public class Usuarios
{
    // 1. Clave Primaria
    [Key]
    public int id_usuario { get; set; }

    // 2. Campo requerido y con longitud máxima
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50)]
    public string apellido { get; set; } = "";

    [Required]
    [EmailAddress] // 3. Validación de formato de email
    [StringLength(100)]
    public string email { get; set; } = "";
    
    // 4. Convertido a propiedad y nulable
    [StringLength(255)] // Longitud para una URL
    public string? avatarUrl { get; set; }

    [NotMapped]
    public IFormFile? avatarFile { get; set; }

    [Required]
    public int rol { get; set; }

    [Required]
    [StringLength(255)] // Una longitud apropiada para un hash de contraseña
    public string clave { get; set; } = "";

    [NotMapped]
    public string? oldClave { get; set; } = "";

    public bool estado { get; set; }

    // 5. Nombre de columna explícito (opcional)
    [Column("fecha_creacion")]
    public DateTime fecha_creacion { get; set; }
}