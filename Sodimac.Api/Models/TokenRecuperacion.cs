using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Token temporal enviado al correo para recuperar y restablecer la contraseña; tiene vigencia y no puede reutilizarse.
/// </summary>
[Table("token_recuperacion", Schema = "cuenta")]
[Index("IdCliente", Name = "IX_token_recuperacion_id_cliente")]
[Index("Token", Name = "UQ_token_recuperacion_token", IsUnique = true)]
public partial class TokenRecuperacion
{
    [Key]
    [Column("id_token")]
    public int IdToken { get; set; }

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("token")]
    [StringLength(255)]
    [Unicode(false)]
    public string Token { get; set; } = null!;

    [Column("vigente_hasta")]
    [Precision(0)]
    public DateTime VigenteHasta { get; set; }

    [Column("usado")]
    public bool Usado { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("TokenRecuperacions")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
