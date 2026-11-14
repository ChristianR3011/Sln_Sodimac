using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Sesión iniciada por un cliente, identificada por un token con fecha de expiración. Permite cerrar sesión.
/// </summary>
[Table("sesion", Schema = "cuenta")]
[Index("IdCliente", Name = "IX_sesion_id_cliente")]
[Index("Token", Name = "UQ_sesion_token", IsUnique = true)]
public partial class Sesion
{
    [Key]
    [Column("id_sesion")]
    public int IdSesion { get; set; }

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("token")]
    [StringLength(255)]
    [Unicode(false)]
    public string Token { get; set; } = null!;

    [Column("fecha_emision")]
    [Precision(0)]
    public DateTime FechaEmision { get; set; }

    [Column("fecha_expiracion")]
    [Precision(0)]
    public DateTime FechaExpiracion { get; set; }

    [Column("activa")]
    public bool Activa { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("Sesions")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
