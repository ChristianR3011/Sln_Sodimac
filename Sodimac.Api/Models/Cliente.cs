using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Persona registrada en la tienda. Registra su cuenta, inicia sesión y consulta o actualiza sus datos. Guarda los intentos fallidos para el bloqueo temporal.
/// </summary>
[Table("cliente", Schema = "cuenta")]
[Index("Correo", Name = "UQ_cliente_correo", IsUnique = true)]
[Index("NroDocumento", Name = "UQ_cliente_nro_documento", IsUnique = true)]
public partial class Cliente
{
    [Key]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

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

    [Column("correo")]
    [StringLength(100)]
    public string Correo { get; set; } = null!;

    [Column("celular")]
    [StringLength(15)]
    [Unicode(false)]
    public string Celular { get; set; } = null!;

    [Column("contrasena_hash")]
    [StringLength(255)]
    [Unicode(false)]
    public string ContrasenaHash { get; set; } = null!;

    [Column("acepto_terminos")]
    public bool AceptoTerminos { get; set; }

    [Column("intentos_fallidos")]
    public int IntentosFallidos { get; set; }

    [Column("bloqueado_hasta")]
    [Precision(0)]
    public DateTime? BloqueadoHasta { get; set; }

    [Column("fecha_registro")]
    [Precision(0)]
    public DateTime FechaRegistro { get; set; }

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Comprador> Compradors { get; set; } = new List<Comprador>();

    [InverseProperty("IdClienteNavigation")]
    public virtual Direccion? Direccion { get; set; }

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Sesion> Sesions { get; set; } = new List<Sesion>();

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<TokenRecuperacion> TokenRecuperacions { get; set; } = new List<TokenRecuperacion>();
}
