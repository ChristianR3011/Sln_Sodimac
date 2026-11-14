using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Clase abstracta con los datos comunes de una transacción de pago: monto, estado y fecha.
/// </summary>
[Table("pago", Schema = "pago")]
[Index("IdCarro", Name = "IX_pago_id_carro")]
[Index("IdMedio", Name = "IX_pago_id_medio")]
public partial class Pago
{
    [Key]
    [Column("id_transaccion")]
    public int IdTransaccion { get; set; }

    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("id_medio")]
    public int IdMedio { get; set; }

    [Column("monto", TypeName = "decimal(10, 2)")]
    public decimal Monto { get; set; }

    [Column("estado")]
    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [Column("fecha")]
    [Precision(0)]
    public DateTime Fecha { get; set; }

    [ForeignKey("IdCarro")]
    [InverseProperty("Pagos")]
    public virtual Carro IdCarroNavigation { get; set; } = null!;

    [ForeignKey("IdMedio")]
    [InverseProperty("Pagos")]
    public virtual MedioPago IdMedioNavigation { get; set; } = null!;

    [InverseProperty("IdTransaccionNavigation")]
    public virtual Orden? Orden { get; set; }

    [InverseProperty("IdTransaccionNavigation")]
    public virtual PagoEfectivo? PagoEfectivo { get; set; }

    [InverseProperty("IdTransaccionNavigation")]
    public virtual PagoTarjeta? PagoTarjetum { get; set; }
}
