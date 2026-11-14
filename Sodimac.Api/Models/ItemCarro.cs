using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Línea del carro: un producto con su cantidad, precio unitario y subtotal. Permite actualizar la cantidad o eliminar el producto.
/// </summary>
[Table("item_carro", Schema = "carro")]
[Index("IdCarro", Name = "IX_item_carro_id_carro")]
[Index("Sku", Name = "IX_item_carro_sku")]
public partial class ItemCarro
{
    [Key]
    [Column("id_item")]
    public int IdItem { get; set; }

    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Column("precio_unitario", TypeName = "decimal(10, 2)")]
    public decimal PrecioUnitario { get; set; }

    [Column("subtotal", TypeName = "decimal(10, 2)")]
    public decimal Subtotal { get; set; }

    [ForeignKey("IdCarro")]
    [InverseProperty("ItemCarros")]
    public virtual Carro IdCarroNavigation { get; set; } = null!;

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<ItemServicio> ItemServicios { get; set; } = new List<ItemServicio>();

    [ForeignKey("Sku")]
    [InverseProperty("ItemCarros")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
