using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Segundo nivel de la ubicación; pertenece a un departamento.
/// </summary>
[Table("provincia", Schema = "busqueda")]
[Index("IdDepartamento", Name = "IX_provincia_id_departamento")]
public partial class Provincia
{
    [Key]
    [Column("id_provincia")]
    public int IdProvincia { get; set; }

    [Column("id_departamento")]
    public int IdDepartamento { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdProvinciaNavigation")]
    public virtual ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();

    [ForeignKey("IdDepartamento")]
    [InverseProperty("Provincia")]
    public virtual Departamento IdDepartamentoNavigation { get; set; } = null!;
}
