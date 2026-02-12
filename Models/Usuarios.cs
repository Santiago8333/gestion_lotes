namespace gestion_lotes.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
public class Usuarios
{
    // 1. Clave Primaria
    [Key]
    public int id_usuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string nombre { get; set; } = "";

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50)]
    public string apellido { get; set; } = "";

    [Required]
    [EmailAddress] 
    [StringLength(100)]
    public string email { get; set; } = "";
    

    [StringLength(255)] 
    public string? avatarUrl { get; set; }

    [NotMapped]
    public IFormFile? avatarFile { get; set; }

    [Required]
    public int rol { get; set; }

    [Required]
    [StringLength(255)] 
    public string clave { get; set; } = "";

    [NotMapped]
    public string? oldClave { get; set; } = "";

    public bool estado { get; set; }


    [Column("fecha_creacion")]
    public DateTime fecha_creacion { get; set; }
}

public class PerfilDto
{
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public string? email { get; set; }
    [NotMapped]
    public IFormFile? avatarFile { get; set; }
}