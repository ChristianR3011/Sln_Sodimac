using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Quién vende el producto: Sodimac o un vendedor tercero del marketplace. Se usa en las tarjetas de producto y en el filtro «Vendido por».
/// </summary>
[Table("vendedor", Schema = "busqueda")]
public partial class Vendedor
{
    [Key]
    [Column("id_vendedor")]
    public int IdVendedor { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("tipo")]
    [StringLength(20)]
    public string Tipo { get; set; } = null!;

    [InverseProperty("IdVendedorNavigation")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
