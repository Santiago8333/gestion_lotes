namespace gestion_lotes.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Parametros
{
    [Key]
    public int id_parametros { get; set; }

    [Required(ErrorMessage = "El porcentaje de honorarios es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100 (ej. 10 = 10%).")]
    public decimal honorarios { get; set; }

    [Required(ErrorMessage = "El porcentaje de sellado es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100 (ej. 0.6 = 0.6%).")]
    public decimal sellado { get; set; }
}
