using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Opinión publicada por un cliente sobre un producto, con su calificación y fecha.
/// </summary>
[Table("resena", Schema = "busqueda")]
[Index("IdCliente", Name = "IX_resena_id_cliente")]
[Index("Sku", Name = "IX_resena_sku")]
public partial class Resena
{
    [Key]
    [Column("id_resena")]
    public int IdResena { get; set; }

    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("calificacion")]
    public int Calificacion { get; set; }

    [Column("comentario")]
    public string? Comentario { get; set; }

    [Column("fecha")]
    [Precision(0)]
    public DateTime Fecha { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("Resenas")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    [ForeignKey("Sku")]
    [InverseProperty("Resenas")]
    public virtual Producto SkuNavigation { get; set; } = null!;
}
