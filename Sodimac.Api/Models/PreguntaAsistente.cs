using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Pregunta del asistente de búsqueda guiada de una categoría (ej. «¿Qué tipo de taladro prefieres?»).
/// </summary>
[Table("pregunta_asistente", Schema = "busqueda")]
[Index("IdCategoria", Name = "IX_pregunta_asistente_id_categoria")]
public partial class PreguntaAsistente
{
    [Key]
    [Column("id_pregunta_asistente")]
    public int IdPreguntaAsistente { get; set; }

    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [Column("pregunta")]
    [StringLength(150)]
    public string Pregunta { get; set; } = null!;

    [ForeignKey("IdCategoria")]
    [InverseProperty("PreguntaAsistentes")]
    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    [InverseProperty("IdPreguntaAsistenteNavigation")]
    public virtual ICollection<OpcionAsistente> OpcionAsistentes { get; set; } = new List<OpcionAsistente>();
}
