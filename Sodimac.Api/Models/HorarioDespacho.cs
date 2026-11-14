using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Fecha y rango horario de despacho con capacidad disponible para una zona.
/// </summary>
[Table("horario_despacho", Schema = "compra")]
[Index("IdZona", Name = "IX_horario_despacho_id_zona")]
[Index("IdZona", "Fecha", Name = "IX_horario_zona_fecha")]
public partial class HorarioDespacho
{
    [Key]
    [Column("id_horario")]
    public int IdHorario { get; set; }

    [Column("id_zona")]
    public int IdZona { get; set; }

    [Column("fecha")]
    public DateOnly Fecha { get; set; }

    [Column("rango_horario")]
    [StringLength(20)]
    public string RangoHorario { get; set; } = null!;

    [Column("capacidad")]
    public int Capacidad { get; set; }

    [Column("costo", TypeName = "decimal(10, 2)")]
    public decimal Costo { get; set; }

    [InverseProperty("IdHorarioNavigation")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    [ForeignKey("IdZona")]
    [InverseProperty("HorarioDespachos")]
    public virtual Zona IdZonaNavigation { get; set; } = null!;
}
