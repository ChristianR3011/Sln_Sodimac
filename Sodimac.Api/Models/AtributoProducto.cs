using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Atributo técnico de un producto (ej. Potencia, Voltaje). Forma las especificaciones de la ficha y los filtros por atributos.
/// </summary>
[Table("atributo_producto", Schema = "busqueda")]
[Index("Sku", Name = "IX_atributo_producto_sku")]
public partial class AtributoProducto
{
    [Key]
    [Column("id_atributo")]
    public int IdAtributo { get; set; }

    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("nombre")]
    [StringLength(60)]
    public string Nombre { get; set; } = null!;

    [Column("valor")]
    [StringLength(80)]
    public string Valor { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("AtributoProductos")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
