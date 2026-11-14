using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Zona de cobertura de un distrito. Se obtiene al registrar la ubicación de entrega y condiciona precios, entregas, horarios y servicios.
/// </summary>
[Table("zona", Schema = "busqueda")]
[Index("IdDistrito", Name = "UQ_zona_distrito", IsUnique = true)]
public partial class Zona
{
    [Key]
    [Column("id_zona")]
    public int IdZona { get; set; }

    [Column("id_distrito")]
    public int IdDistrito { get; set; }

    [Column("nombre")]
    [StringLength(60)]
    public string Nombre { get; set; } = null!;

    [Column("tiene_cobertura")]
    public bool TieneCobertura { get; set; }

    [InverseProperty("IdZonaNavigation")]
    public virtual ICollection<EntregaProductoZona> EntregaProductoZonas { get; set; } = new List<EntregaProductoZona>();

    [InverseProperty("IdZonaNavigation")]
    public virtual ICollection<HorarioDespacho> HorarioDespachos { get; set; } = new List<HorarioDespacho>();

    [ForeignKey("IdDistrito")]
    [InverseProperty("Zona")]
    public virtual Distrito IdDistritoNavigation { get; set; } = null!;

    [InverseProperty("IdZonaNavigation")]
    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();

    [InverseProperty("IdZonaNavigation")]
    public virtual ICollection<ProductoServicio> ProductoServicios { get; set; } = new List<ProductoServicio>();
}
