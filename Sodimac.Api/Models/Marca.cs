using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Marca o fabricante de los productos (ej. Bauker, Bosch). Lista las marcas destacadas de una categoría en el menú y los productos de una marca.
/// </summary>
[Table("marca", Schema = "busqueda")]
[Index("Nombre", Name = "UQ_marca_nombre", IsUnique = true)]
public partial class Marca
{
    [Key]
    [Column("id_marca")]
    public int IdMarca { get; set; }

    [Column("nombre")]
    [StringLength(60)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdMarcaNavigation")]
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    [ForeignKey("IdMarca")]
    [InverseProperty("IdMarcas")]
    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
}
