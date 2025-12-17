namespace gestion_lotes.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

public class Pagos
{
    [Key]
    public int id_recibo_persona_fisica {get;set;}
    public string destinatario { get; set; } = ""; // "Gobierno", "Cmcpsl", etc.
    public decimal efectivo { get; set; }
    public decimal transferencia { get; set; }
    public decimal dolar_monto { get; set; }
    public decimal dolar_cotizacion { get; set; }
    public decimal euro_monto { get; set; }
    public decimal euro_cotizacion { get; set; }
}