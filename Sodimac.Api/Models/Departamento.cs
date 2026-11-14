using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Primer nivel de la ubicación geográfica (ubigeo).
/// </summary>
[Table("departamento", Schema = "busqueda")]
public partial class Departamento
{
    [Key]
    [Column("id_departamento")]
    public int IdDepartamento { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdDepartamentoNavigation")]
    public virtual ICollection<Provincia> Provincia { get; set; } = new List<Provincia>();
}
