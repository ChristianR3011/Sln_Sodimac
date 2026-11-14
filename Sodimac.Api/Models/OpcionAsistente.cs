using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Opción de respuesta de una pregunta del asistente; cada opción tiene el filtro que se aplica al elegirla.
/// </summary>
[Table("opcion_asistente", Schema = "busqueda")]
[Index("IdPreguntaAsistente", Name = "IX_opcion_asistente_id_pregunta_asistente")]
public partial class OpcionAsistente
{
    [Key]
    [Column("id_opcion")]
    public int IdOpcion { get; set; }

    [Column("id_pregunta_asistente")]
    public int IdPreguntaAsistente { get; set; }

    [Column("etiqueta")]
    [StringLength(80)]
    public string Etiqueta { get; set; } = null!;

    [Column("filtro_asociado")]
    [StringLength(100)]
    public string FiltroAsociado { get; set; } = null!;

    [ForeignKey("IdPreguntaAsistente")]
    [InverseProperty("OpcionAsistentes")]
    public virtual PreguntaAsistente IdPreguntaAsistenteNavigation { get; set; } = null!;
}
