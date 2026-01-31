namespace gestion_lotes.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

public class Recibo_persona_fisica
{
    [Key]
    public int id_recibo_persona_fisica { get; set; }
    public int id_lote { get; set; }
    [Required(ErrorMessage = "El nombre de recibo es obligatorio.")]
    public string nombre {get;set;} = "";
    [Required(ErrorMessage = "El apellido de recibo es obligatorio.")]
    public string apellido {get;set;} = "";
    [Required(ErrorMessage = "El tipo dni de recibo es obligatorio.")]
    public string tipo_dni {get;set;} = "";
    [Required(ErrorMessage = "El dni de recibo es obligatorio.")]
    public string dni {get;set;} = "";
    [Required(ErrorMessage = "El telefono de recibo es obligatorio.")]
    public string telefono {get;set;} = "";
    [Required(ErrorMessage = "El codigo postal de recibo es obligatorio.")]
    public string codigo_postal {get;set;} = "";
    [Required(ErrorMessage = "El email de recibo es obligatorio.")]
    public string email {get;set;} = "";
    public string creado_por {get;set;} = "";
    [Required(ErrorMessage = "El domicilio de recibo es obligatorio.")]
    public string domicilio {get;set;} = "";
    [Column("fecha_creacion")]
    public DateTime fecha_creacion { get; set; }
    [Required(ErrorMessage = "El provincia de recibo es obligatorio.")]
    public string provincia {get;set;} = "";
    [Required(ErrorMessage = "El % de pago de recibo es obligatorio.")]
    [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
    public decimal pago_lote { get; set; }
    [Required(ErrorMessage = "El precio subastado de recibo es obligatorio.")]
    public decimal precio_subastado { get; set; }
    public bool estado { get; set; }
    
    [ForeignKey("id_lote")]
    public virtual Lotes? Lote { get; set; }
    public virtual ICollection<Forma_Pagos>? FormasDePago { get; set; }
    
}
public class CrearReciboRequest
{
    public int id_lote { get; set; }
    public string nombre { get; set; } = "";
    public string apellido { get; set; } = "";
    public string tipo_dni { get; set; } = "";
    public string dni { get; set; } = "";
    public string telefono { get; set; } = "";
    public string email { get; set; } = "";
    public string domicilio { get; set; } = "";
    public string codigo_postal { get; set; } = "";
    public string provincia { get; set; } = "";
    public decimal precio_subastado { get; set; }
    public decimal pago_lote { get; set; }
    public List<Forma_Pagos> lista_pagos { get; set; } = new List<Forma_Pagos>();
}

public class CrearReciboRequestMd
{
    public int id_lote { get; set; }
    public int id_recibo_persona_fisica {get;set;}
    public string nombre { get; set; } = "";
    public string apellido { get; set; } = "";
    public string tipo_dni { get; set; } = "";
    public string dni { get; set; } = "";
    public string telefono { get; set; } = "";
    public string email { get; set; } = "";
    public string domicilio { get; set; } = "";
    public string codigo_postal { get; set; } = "";
    public string provincia { get; set; } = "";
    public decimal precio_subastado { get; set; }
    public decimal pago_lote { get; set; }
    public List<Forma_Pagos> lista_pagos { get; set; } = new List<Forma_Pagos>();
}