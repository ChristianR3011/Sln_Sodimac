using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Tabla intermedia que relaciona un producto consigo mismo: para cada SKU, los SKU similares o complementarios (tipo).
/// </summary>
[PrimaryKey("Sku", "SkuRelacionado")]
[Table("producto_relacionado", Schema = "busqueda")]
[Index("SkuRelacionado", Name = "IX_producto_relacionado_sku_relacionado")]
public partial class ProductoRelacionado
{
    [Key]
    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Key]
    [Column("sku_relacionado")]
    [StringLength(20)]
    [Unicode(false)]
    public string SkuRelacionado { get; set; } = null!;

    [Column("tipo")]
    [StringLength(20)]
    public string Tipo { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("ProductoRelacionadoSkuNavigations")]
    public virtual Producto SkuNavigation { get; set; } = null!;

    [ForeignKey("SkuRelacionado")]
    [InverseProperty("ProductoRelacionadoSkuRelacionadoNavigations")]
    public virtual Producto SkuRelacionadoNavigation { get; set; } = null!;
}
