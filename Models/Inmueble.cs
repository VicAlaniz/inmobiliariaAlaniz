using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlaniz.Models;

public class Inmueble {
    public int Id {get; set;}
    [Display(Name = "Dirección")]
    public string Direccion {get; set;}
    public string Uso {get; set;}
    public string Tipo {get; set;}
    [Display(Name = "Cantidad de Ambientes")]
    public int CantAmbientes {get; set;}
    public string Coordenadas {get; set;}
    public decimal Precio {get; set;}
    [Display(Name = "Dueño")]
    public int IdPropietario { get; set; }
    [ForeignKey(nameof(IdPropietario))]
    public Propietario Duenio { get; set; }
  
}