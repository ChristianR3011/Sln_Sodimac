using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Catálogo de modalidades de entrega: envío a domicilio, envío express y retiro en tienda.
/// </summary>
[Table("tipo_entrega", Schema = "compra")]
public partial class TipoEntrega
{
    [Key]
    [Column("id_tipo_entrega")]
    public int IdTipoEntrega { get; set; }

    [Column("nombre")]
    [StringLength(40)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdTipoEntregaNavigation")]
    public virtual ICollection<EntregaProductoZona> EntregaProductoZonas { get; set; } = new List<EntregaProductoZona>();

    [InverseProperty("IdTipoEntregaNavigation")]
    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
