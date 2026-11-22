using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Dirección de entrega registrada por un cliente; una de ellas es la principal.
/// </summary>
[Table("direccion", Schema = "cuenta")]
[Index("IdDistrito", Name = "IX_direccion_id_distrito")]
public partial class Direccion
{
    [Key]
    [Column("id_direccion")]
    public int IdDireccion { get; set; }

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("id_distrito")]
    public int IdDistrito { get; set; }

    [Column("nombre")]
    [StringLength(60)]
    public string Nombre { get; set; } = null!;

    [Column("direccion")]
    [StringLength(150)]
    public string Direccion1 { get; set; } = null!;

    public string? DepartamentoNombre { get; set; }

    public string? ProvinciaNombre { get; set; }

    public string? DistritoNombre { get; set; }

    [Column("referencia")]
    [StringLength(150)]
    public string? Referencia { get; set; }

    [Column("principal")]
    public bool Principal { get; set; }

    [InverseProperty("IdDireccionNavigation")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    [ForeignKey("IdCliente")]
    [InverseProperty("Direccion")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    [ForeignKey("IdDistrito")]
    [InverseProperty("Direccions")]
    public virtual Distrito IdDistritoNavigation { get; set; } = null!;
}
