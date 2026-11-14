using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Stock de un producto en una tienda y tiempo estimado de retiro. Responde «Consultar stock por tienda» y el filtro por stock.
/// </summary>
[PrimaryKey("Sku", "IdTienda")]
[Table("stock_tienda", Schema = "busqueda")]
[Index("IdTienda", Name = "IX_stock_tienda_id_tienda")]
public partial class StockTienda
{
    [Key]
    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Key]
    [Column("id_tienda")]
    public int IdTienda { get; set; }

    [Column("disponible")]
    public bool Disponible { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Column("tiempo_retiro_min")]
    public int TiempoRetiroMin { get; set; }

    [ForeignKey("IdTienda")]
    [InverseProperty("StockTienda")]
    public virtual Tienda IdTiendaNavigation { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("StockTienda")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
