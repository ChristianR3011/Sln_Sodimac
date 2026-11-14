using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Pregunta y respuesta que forma parte del contenido informativo al final del listado de una categoría.
/// </summary>
[Table("pregunta_frecuente", Schema = "busqueda")]
[Index("IdCategoria", Name = "IX_pregunta_frecuente_id_categoria")]
public partial class PreguntaFrecuente
{
    [Key]
    [Column("id_pregunta")]
    public int IdPregunta { get; set; }

    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [Column("pregunta")]
    public string Pregunta { get; set; } = null!;

    [Column("respuesta")]
    public string Respuesta { get; set; } = null!;

    [ForeignKey("IdCategoria")]
    [InverseProperty("PreguntaFrecuentes")]
    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
}
