using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Agrupa los productos del catálogo en una jerarquía de categorías y subcategorías (ej. Herramientas &gt; Herramientas eléctricas &gt; Taladros). Atiende el menú «Categorías», la ruta de navegación, las categorías relacionadas y el contenido informativo de cada categoría.
/// </summary>
[Table("categoria", Schema = "busqueda")]
[Index("IdCategoriaPadre", Name = "IX_categoria_id_categoria_padre")]
public partial class Categoria
{
    [Key]
    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [Column("id_categoria_padre")]
    public int? IdCategoriaPadre { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("orden")]
    public int Orden { get; set; }

    [Column("activa")]
    public bool Activa { get; set; }

    [Column("descripcion_informativa")]
    public string? DescripcionInformativa { get; set; }

    [ForeignKey("IdCategoriaPadre")]
    [InverseProperty("InverseIdCategoriaPadreNavigation")]
    public virtual Categoria? IdCategoriaPadreNavigation { get; set; }

    [InverseProperty("IdCategoriaPadreNavigation")]
    public virtual ICollection<Categoria> InverseIdCategoriaPadreNavigation { get; set; } = new List<Categoria>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<PreguntaAsistente> PreguntaAsistentes { get; set; } = new List<PreguntaAsistente>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<PreguntaFrecuente> PreguntaFrecuentes { get; set; } = new List<PreguntaFrecuente>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<ProductoPatrocinado> ProductoPatrocinados { get; set; } = new List<ProductoPatrocinado>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("IdCategoria")]
    public virtual ICollection<Marca> IdMarcas { get; set; } = new List<Marca>();
}
