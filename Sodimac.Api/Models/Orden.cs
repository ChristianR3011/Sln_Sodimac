using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Orden de compra creada cuando el pago se aprueba (o queda pendiente si es en efectivo). Registra los CMR Puntos acumulados.
/// </summary>
[Table("orden", Schema = "pago")]
[Index("IdComprador", Name = "IX_orden_id_comprador")]
[Index("IdCarro", Name = "UQ_orden_carro", IsUnique = true)]
[Index("IdTransaccion", Name = "UQ_orden_transaccion", IsUnique = true)]
public partial class Orden
{
    [Key]
    [Column("nro_orden")]
    [StringLength(20)]
    [Unicode(false)]
    public string NroOrden { get; set; } = null!;

    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("id_comprador")]
    public int IdComprador { get; set; }

    [Column("id_transaccion")]
    public int IdTransaccion { get; set; }

    [Column("estado")]
    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [Column("fecha_creacion")]
    [Precision(0)]
    public DateTime FechaCreacion { get; set; }

    [Column("total", TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [Column("puntos_acumulados")]
    public int PuntosAcumulados { get; set; }

    [InverseProperty("NroOrdenNavigation")]
    public virtual Comprobante? Comprobante { get; set; }

    [ForeignKey("IdCarro")]
    [InverseProperty("Orden")]
    public virtual Carro IdCarroNavigation { get; set; } = null!;

    [ForeignKey("IdComprador")]
    [InverseProperty("Ordens")]
    public virtual Comprador IdCompradorNavigation { get; set; } = null!;

    [ForeignKey("IdTransaccion")]
    [InverseProperty("Orden")]
    public virtual Pago IdTransaccionNavigation { get; set; } = null!;

    [InverseProperty("NroOrdenNavigation")]
    public virtual ICollection<NotificacionCorreo> NotificacionCorreos { get; set; } = new List<NotificacionCorreo>();
}
