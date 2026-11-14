using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Tercer nivel de la ubicación; pertenece a una provincia y ubica zonas, tiendas y direcciones.
/// </summary>
[Table("distrito", Schema = "busqueda")]
[Index("IdProvincia", Name = "IX_distrito_id_provincia")]
public partial class Distrito
{
    [Key]
    [Column("id_distrito")]
    public int IdDistrito { get; set; }

    [Column("id_provincia")]
    public int IdProvincia { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdDistritoNavigation")]
    public virtual ICollection<Direccion> Direccions { get; set; } = new List<Direccion>();

    [ForeignKey("IdProvincia")]
    [InverseProperty("Distritos")]
    public virtual Provincia IdProvinciaNavigation { get; set; } = null!;

    [InverseProperty("IdDistritoNavigation")]
    public virtual ICollection<Tienda> Tienda { get; set; } = new List<Tienda>();

    [InverseProperty("IdDistritoNavigation")]
    public virtual Zona? Zona { get; set; }
}
