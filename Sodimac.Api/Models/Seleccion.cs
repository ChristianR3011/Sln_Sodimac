using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Colección de productos armada por el área comercial (ej. «Lo más vendido», «Precio x volumen»).
/// </summary>
[Table("seleccion", Schema = "busqueda")]
[Index("Nombre", Name = "UQ_seleccion_nombre", IsUnique = true)]
public partial class Seleccion
{
    [Key]
    [Column("id_seleccion")]
    public int IdSeleccion { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("activa")]
    public bool Activa { get; set; }

    [ForeignKey("IdSeleccion")]
    [InverseProperty("IdSeleccions")]
    public virtual ICollection<Producto> Skus { get; set; } = new List<Producto>();
}
