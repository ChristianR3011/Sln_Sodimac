using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Cupón de descuento con sus condiciones: monto mínimo, medio de pago requerido y vigencia (ej. CMR10).
/// </summary>
[Table("cupon", Schema = "carro")]
[Index("Codigo", Name = "UQ_cupon_codigo", IsUnique = true)]
public partial class Cupon
{
    [Key]
    [Column("id_cupon")]
    public int IdCupon { get; set; }

    [Column("codigo")]
    [StringLength(20)]
    [Unicode(false)]
    public string Codigo { get; set; } = null!;

    [Column("descuento", TypeName = "decimal(10, 2)")]
    public decimal Descuento { get; set; }

    [Column("monto_minimo", TypeName = "decimal(10, 2)")]
    public decimal? MontoMinimo { get; set; }

    [Column("medio_pago_requerido")]
    [StringLength(30)]
    public string? MedioPagoRequerido { get; set; }

    [Column("vigente_desde")]
    public DateOnly VigenteDesde { get; set; }

    [Column("vigente_hasta")]
    public DateOnly VigenteHasta { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [InverseProperty("IdCuponNavigation")]
    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    [InverseProperty("IdCuponNavigation")]
    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();
}
