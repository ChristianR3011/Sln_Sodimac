using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Carro de compras de un cliente o de un invitado. Agrega productos, muestra el carro, une el carro de invitado al iniciar sesión y entrega el resumen de compra.
/// </summary>
[Table("carro", Schema = "carro")]
[Index("IdCliente", Name = "IX_carro_id_cliente")]
[Index("IdCupon", Name = "IX_carro_id_cupon")]
public partial class Carro
{
    [Key]
    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("id_cliente")]
    public int? IdCliente { get; set; }

    [Column("id_cupon")]
    public int? IdCupon { get; set; }

    [Column("es_invitado")]
    public bool EsInvitado { get; set; }

    [Column("fecha_creacion")]
    [Precision(0)]
    public DateTime FechaCreacion { get; set; }

    [Column("subtotal", TypeName = "decimal(10, 2)")]
    public decimal Subtotal { get; set; }

    [Column("descuentos", TypeName = "decimal(10, 2)")]
    public decimal Descuentos { get; set; }

    [Column("total", TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [InverseProperty("IdCarroNavigation")]
    public virtual Comprador? Comprador { get; set; }

    [InverseProperty("IdCarroNavigation")]
    public virtual Comprobante? Comprobante { get; set; }

    [InverseProperty("IdCarroNavigation")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    [ForeignKey("IdCliente")]
    [InverseProperty("Carros")]
    public virtual Cliente? IdClienteNavigation { get; set; }

    [ForeignKey("IdCupon")]
    [InverseProperty("Carros")]
    public virtual Cupon? IdCuponNavigation { get; set; }

    [InverseProperty("IdCarroNavigation")]
    public virtual ICollection<ItemCarro> ItemCarros { get; set; } = new List<ItemCarro>();

    [InverseProperty("IdCarroNavigation")]
    public virtual Orden? Orden { get; set; }

    [InverseProperty("IdCarroNavigation")]
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
