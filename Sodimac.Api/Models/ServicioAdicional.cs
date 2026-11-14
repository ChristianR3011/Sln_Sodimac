using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Servicio que se puede contratar con un producto (ej. armado, instalación, garantía extendida).
/// </summary>
[Table("servicio_adicional", Schema = "carro")]
public partial class ServicioAdicional
{
    [Key]
    [Column("id_servicio")]
    public int IdServicio { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("precio", TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<ItemServicio> ItemServicios { get; set; } = new List<ItemServicio>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<ProductoServicio> ProductoServicios { get; set; } = new List<ProductoServicio>();
}
