using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Correo enviado al comprador con la confirmación de su compra.
/// </summary>
[Table("notificacion_correo", Schema = "pago")]
[Index("NroOrden", Name = "IX_notificacion_correo_nro_orden")]
public partial class NotificacionCorreo
{
    [Key]
    [Column("id_notificacion")]
    public int IdNotificacion { get; set; }

    [Column("nro_orden")]
    [StringLength(20)]
    [Unicode(false)]
    public string NroOrden { get; set; } = null!;

    [Column("correo")]
    [StringLength(100)]
    public string Correo { get; set; } = null!;

    [Column("asunto")]
    [StringLength(150)]
    public string Asunto { get; set; } = null!;

    [Column("fecha_envio")]
    [Precision(0)]
    public DateTime FechaEnvio { get; set; }

    [Column("estado")]
    [StringLength(20)]
    public string Estado { get; set; } = null!;

    [ForeignKey("NroOrden")]
    [InverseProperty("NotificacionCorreos")]
    public virtual Orden NroOrdenNavigation { get; set; } = null!;
}
