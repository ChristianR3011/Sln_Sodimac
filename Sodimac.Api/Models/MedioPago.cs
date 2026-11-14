using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Medio de pago aceptado (tarjeta CMR, crédito, débito, pago en efectivo, Gift Card) con las cuotas disponibles.
/// </summary>
[Table("medio_pago", Schema = "compra")]
public partial class MedioPago
{
    [Key]
    [Column("id_medio")]
    public int IdMedio { get; set; }

    [Column("nombre")]
    [StringLength(40)]
    public string Nombre { get; set; } = null!;

    [Column("cuotas_disponibles")]
    [StringLength(30)]
    public string? CuotasDisponibles { get; set; }

    [InverseProperty("IdMedioNavigation")]
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
