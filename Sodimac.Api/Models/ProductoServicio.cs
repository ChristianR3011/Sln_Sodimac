using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Clase de asociación que indica si un servicio adicional está disponible para un producto en una zona.
/// </summary>
[PrimaryKey("Sku", "IdServicio", "IdZona")]
[Table("producto_servicio", Schema = "carro")]
[Index("IdServicio", Name = "IX_producto_servicio_id_servicio")]
[Index("IdZona", Name = "IX_producto_servicio_id_zona")]
public partial class ProductoServicio
{
    [Key]
    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Key]
    [Column("id_servicio")]
    public int IdServicio { get; set; }

    [Key]
    [Column("id_zona")]
    public int IdZona { get; set; }

    [Column("disponible")]
    public bool Disponible { get; set; }

    [ForeignKey("IdServicio")]
    [InverseProperty("ProductoServicios")]
    public virtual ServicioAdicional IdServicioNavigation { get; set; } = null!;

    [ForeignKey("IdZona")]
    [InverseProperty("ProductoServicios")]
    public virtual Zona IdZonaNavigation { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("ProductoServicios")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
