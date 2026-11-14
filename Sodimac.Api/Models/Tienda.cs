using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Tienda Sodimac o Maestro donde el cliente puede retirar su pedido.
/// </summary>
[Table("tienda", Schema = "compra")]
[Index("IdDistrito", Name = "IX_tienda_id_distrito")]
public partial class Tienda
{
    [Key]
    [Column("id_tienda")]
    public int IdTienda { get; set; }

    [Column("id_distrito")]
    public int IdDistrito { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("tipo")]
    [StringLength(20)]
    public string Tipo { get; set; } = null!;

    [Column("direccion")]
    [StringLength(150)]
    public string Direccion { get; set; } = null!;

    [Column("horario")]
    [StringLength(80)]
    public string? Horario { get; set; }

    [InverseProperty("IdTiendaNavigation")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    [ForeignKey("IdDistrito")]
    [InverseProperty("Tienda")]
    public virtual Distrito IdDistritoNavigation { get; set; } = null!;

    [InverseProperty("IdTiendaNavigation")]
    public virtual ICollection<StockTienda> StockTienda { get; set; } = new List<StockTienda>();
}
