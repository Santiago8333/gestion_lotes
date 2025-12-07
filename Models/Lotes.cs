namespace gestion_lotes.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
public class Lotes
{
  
    [Key]
    public int id_lote { get; set; }
    [Required(ErrorMessage = "El numero de lote es obligatorio.")]
    public int n_lote { get; set; }

    [Required(ErrorMessage = "El marca es obligatorio.")]
    [StringLength(50)]
    public string marca { get; set; } = "";

    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(50)]
    public string modelo { get; set; } = "";
    [Required(ErrorMessage = "El dominio es obligatorio.")]
    [StringLength(50)]
    public string dominio { get; set; } = "";
    [Required(ErrorMessage = "El anio es obligatorio.")]
    [StringLength(50)]
    public string anio { get; set; } = "";
    [Required(ErrorMessage = "El base es obligatorio.")]
    public decimal @base { get; set; }
    [Required(ErrorMessage = "El creado_por es obligatorio.")]
    [StringLength(50)]
    public string creado_por { get; set; } = "";
    [Column("fecha_creacion")]
    public DateTime fecha_creacion { get; set; }
    public bool estado { get; set; }
}