namespace gestion_lotes.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Forma_Pagos
{
    [Key]
    public int id_forma_pago { get; set; }
    [ForeignKey("Recibo")]
    public int id_recibo_persona_fisica { get; set; }
    public string destinatario { get; set; } = "";
    public decimal efectivo { get; set; }
    public decimal transferencia { get; set; }
    public decimal dolar_monto { get; set; }
    public decimal dolar_cotizacion { get; set; }
    public decimal euro_monto { get; set; }
    public decimal euro_cotizacion { get; set; }
}