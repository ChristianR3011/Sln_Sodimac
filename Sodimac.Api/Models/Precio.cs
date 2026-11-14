using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Precio vigente de un producto en una zona: precio normal, de oferta, con tarjeta CMR, % de descuento, etiqueta del evento y precio por volumen.
/// </summary>
[Table("precio", Schema = "busqueda")]
[Index("IdCupon", Name = "IX_precio_id_cupon")]
[Index("IdZona", Name = "IX_precio_id_zona")]
[Index("Sku", Name = "IX_precio_sku")]
[Index("Sku", "IdZona", "VigenteDesde", Name = "IX_precio_vigencia")]
public partial class Precio
{
    [Key]
    [Column("id_precio")]
    public int IdPrecio { get; set; }

    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("id_zona")]
    public int IdZona { get; set; }

    [Column("id_cupon")]
    public int? IdCupon { get; set; }

    [Column("precio_normal", TypeName = "decimal(10, 2)")]
    public decimal PrecioNormal { get; set; }

    [Column("precio_oferta", TypeName = "decimal(10, 2)")]
    public decimal? PrecioOferta { get; set; }

    [Column("precio_cmr", TypeName = "decimal(10, 2)")]
    public decimal? PrecioCmr { get; set; }

    [Column("porcentaje_descuento", TypeName = "decimal(10, 2)")]
    public decimal? PorcentajeDescuento { get; set; }

    [Column("etiqueta_evento")]
    [StringLength(60)]
    public string? EtiquetaEvento { get; set; }

    [Column("precio_volumen", TypeName = "decimal(10, 2)")]
    public decimal? PrecioVolumen { get; set; }

    [Column("cantidad_min_volumen")]
    public int? CantidadMinVolumen { get; set; }

    [Column("vigente_desde")]
    public DateOnly VigenteDesde { get; set; }

    [Column("vigente_hasta")]
    public DateOnly? VigenteHasta { get; set; }

    [ForeignKey("IdCupon")]
    [InverseProperty("Precios")]
    public virtual Cupon? IdCuponNavigation { get; set; }

    [ForeignKey("IdZona")]
    [InverseProperty("Precios")]
    public virtual Zona IdZonaNavigation { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("Precios")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
