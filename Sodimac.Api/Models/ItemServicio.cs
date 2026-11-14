using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Servicio adicional agregado a una línea del carro. Si se elimina el producto, se elimina también el servicio.
/// </summary>
[Table("item_servicio", Schema = "carro")]
[Index("IdItem", Name = "IX_item_servicio_id_item")]
[Index("IdServicio", Name = "IX_item_servicio_id_servicio")]
public partial class ItemServicio
{
    [Key]
    [Column("id_item_servicio")]
    public int IdItemServicio { get; set; }

    [Column("id_item")]
    public int IdItem { get; set; }

    [Column("id_servicio")]
    public int IdServicio { get; set; }

    [Column("precio", TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("ItemServicios")]
    public virtual ItemCarro IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdServicio")]
    [InverseProperty("ItemServicios")]
    public virtual ServicioAdicional IdServicioNavigation { get; set; } = null!;
}
