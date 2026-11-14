using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Campaña u oferta destacada de la página de inicio (ej. «Día del Shopping»). Solo se listan las vigentes; tipoDestino e idDestino indican si al hacer clic se abre una categoría o una selección.
/// </summary>
[Table("campana", Schema = "busqueda")]
[Index("FechaInicio", "FechaFin", Name = "IX_campana_vigencia")]
public partial class Campana
{
    [Key]
    [Column("id_campana")]
    public int IdCampana { get; set; }

    [Column("titulo")]
    [StringLength(100)]
    public string Titulo { get; set; } = null!;

    [Column("descuento", TypeName = "decimal(10, 2)")]
    public decimal? Descuento { get; set; }

    [Column("imagen")]
    [StringLength(255)]
    public string? Imagen { get; set; }

    [Column("tipo_destino")]
    [StringLength(20)]
    public string TipoDestino { get; set; } = null!;

    [Column("id_destino")]
    [StringLength(30)]
    [Unicode(false)]
    public string IdDestino { get; set; } = null!;

    [Column("fecha_inicio")]
    public DateOnly FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateOnly FechaFin { get; set; }

    [ForeignKey("IdCampana")]
    [InverseProperty("IdCampanas")]
    public virtual ICollection<Producto> Skus { get; set; } = new List<Producto>();
}
