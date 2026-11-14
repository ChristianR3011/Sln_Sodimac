using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Datos de quien compra (cliente registrado o invitado) registrados en el checkout.
/// </summary>
[Table("comprador", Schema = "compra")]
[Index("IdCliente", Name = "IX_comprador_id_cliente")]
[Index("IdCarro", Name = "UQ_comprador_carro", IsUnique = true)]
public partial class Comprador
{
    [Key]
    [Column("id_comprador")]
    public int IdComprador { get; set; }

    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("id_cliente")]
    public int? IdCliente { get; set; }

    [Column("correo")]
    [StringLength(100)]
    public string Correo { get; set; } = null!;

    [Column("tipo_documento")]
    [StringLength(3)]
    [Unicode(false)]
    public string TipoDocumento { get; set; } = null!;

    [Column("nro_documento")]
    [StringLength(15)]
    [Unicode(false)]
    public string NroDocumento { get; set; } = null!;

    [Column("nombres")]
    [StringLength(60)]
    public string Nombres { get; set; } = null!;

    [Column("apellidos")]
    [StringLength(60)]
    public string Apellidos { get; set; } = null!;

    [Column("celular")]
    [StringLength(15)]
    [Unicode(false)]
    public string Celular { get; set; } = null!;

    [ForeignKey("IdCarro")]
    [InverseProperty("Comprador")]
    public virtual Carro IdCarroNavigation { get; set; } = null!;

    [ForeignKey("IdCliente")]
    [InverseProperty("Compradors")]
    public virtual Cliente? IdClienteNavigation { get; set; }

    [InverseProperty("IdCompradorNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
