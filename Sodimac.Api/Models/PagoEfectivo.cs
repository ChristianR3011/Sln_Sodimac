using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Pago en efectivo mediante un código de pago con fecha de vencimiento.
/// </summary>
[Table("pago_efectivo", Schema = "pago")]
[Index("CodigoPago", Name = "UQ_pago_efectivo_codigo", IsUnique = true)]
public partial class PagoEfectivo
{
    [Key]
    [Column("id_transaccion")]
    public int IdTransaccion { get; set; }

    [Column("codigo_pago")]
    [StringLength(20)]
    [Unicode(false)]
    public string CodigoPago { get; set; } = null!;

    [Column("fecha_vencimiento")]
    [Precision(0)]
    public DateTime FechaVencimiento { get; set; }

    [ForeignKey("IdTransaccion")]
    [InverseProperty("PagoEfectivo")]
    public virtual Pago IdTransaccionNavigation { get; set; } = null!;
}
