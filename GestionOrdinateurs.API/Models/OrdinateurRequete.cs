using System.ComponentModel.DataAnnotations;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Models;

public class OrdinateurRequete
{
    [Required]
    public string Nom { get; set; } = "";

    [Required]
    public string Marque { get; set; } = "";

    [Range(1, 128)]
    public int RAM { get; set; }

    [Required]
    public string Processeur { get; set; } = "";

    [EnumDataType(typeof(EtatOrdinateur))]
    public EtatOrdinateur Etat { get; set; } = EtatOrdinateur.EnService;

    [Range(0, int.MaxValue)]
    public int Stock { get; set; } = 0;

    [Range(0, int.MaxValue)]
    public int Prix { get; set; } = 0;

    [Range(1, int.MaxValue)]
    public int StockageGo { get; set; }

    [EnumDataType(typeof(TypeOrdinateur))]
    public TypeOrdinateur TypeOrdi { get; set; }
}