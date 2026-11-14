using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Modalidad de entrega disponible para un producto en una zona, con su plazo y costo (ej. «Llega hoy», «Retira desde 120 min»).
/// </summary>
[Table("entrega_producto_zona", Schema = "busqueda")]
[Index("IdTipoEntrega", Name = "IX_entrega_producto_zona_id_tipo_entrega")]
[Index("IdZona", Name = "IX_entrega_producto_zona_id_zona")]
[Index("Sku", "IdZona", "IdTipoEntrega", Name = "UQ_entrega_producto_zona_producto_zona_tipo", IsUnique = true)]
public partial class EntregaProductoZona
{
    [Key]
    [Column("id_entrega_zona")]
    public int IdEntregaZona { get; set; }

    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("id_zona")]
    public int IdZona { get; set; }

    [Column("id_tipo_entrega")]
    public int IdTipoEntrega { get; set; }

    [Column("plazo")]
    [StringLength(40)]
    public string Plazo { get; set; } = null!;

    [Column("costo", TypeName = "decimal(10, 2)")]
    public decimal Costo { get; set; }

    [ForeignKey("IdTipoEntrega")]
    [InverseProperty("EntregaProductoZonas")]
    public virtual TipoEntrega IdTipoEntregaNavigation { get; set; } = null!;

    [ForeignKey("IdZona")]
    [InverseProperty("EntregaProductoZonas")]
    public virtual Zona IdZonaNavigation { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("EntregaProductoZonas")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
