using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Producto promocionado por un anunciante que se intercala en los resultados con la etiqueta «Patrocinado».
/// </summary>
[Table("producto_patrocinado", Schema = "busqueda")]
[Index("IdCategoria", Name = "IX_producto_patrocinado_id_categoria")]
[Index("Sku", Name = "IX_producto_patrocinado_sku")]
public partial class ProductoPatrocinado
{
    [Key]
    [Column("id_patrocinio")]
    public int IdPatrocinio { get; set; }

    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("id_categoria")]
    public int? IdCategoria { get; set; }

    [Column("anunciante")]
    [StringLength(80)]
    public string Anunciante { get; set; } = null!;

    [Column("fecha_inicio")]
    public DateOnly FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateOnly FechaFin { get; set; }

    [ForeignKey("IdCategoria")]
    [InverseProperty("ProductoPatrocinados")]
    public virtual Categoria? IdCategoriaNavigation { get; set; }

    [ForeignKey("Sku")]
    [InverseProperty("ProductoPatrocinados")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
