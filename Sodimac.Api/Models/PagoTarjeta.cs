using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Pago con tarjeta de crédito o débito autorizado por la pasarela; guarda solo los últimos dígitos, nunca la tarjeta completa.
/// </summary>
[Table("pago_tarjeta", Schema = "pago")]
public partial class PagoTarjeta
{
    [Key]
    [Column("id_transaccion")]
    public int IdTransaccion { get; set; }

    [Column("cuotas")]
    public int Cuotas { get; set; }

    [Column("tipo_tarjeta")]
    [StringLength(10)]
    public string TipoTarjeta { get; set; } = null!;

    [Column("ultimos_digitos")]
    [StringLength(4)]
    [Unicode(false)]
    public string UltimosDigitos { get; set; } = null!;

    [Column("codigo_autorizacion")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CodigoAutorizacion { get; set; }

    [ForeignKey("IdTransaccion")]
    [InverseProperty("PagoTarjetum")]
    public virtual Pago IdTransaccionNavigation { get; set; } = null!;
}
