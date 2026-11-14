using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Entrega elegida para el carro: despacho a domicilio (dirección, fecha y horario) o retiro en tienda (tienda y persona que retira). Un carro puede tener varias entregas si los productos llegan en envíos separados.
/// </summary>
[Table("entrega", Schema = "compra")]
[Index("IdCarro", Name = "IX_entrega_id_carro")]
[Index("IdDireccion", Name = "IX_entrega_id_direccion")]
[Index("IdHorario", Name = "IX_entrega_id_horario")]
[Index("IdTienda", Name = "IX_entrega_id_tienda")]
[Index("IdTipoEntrega", Name = "IX_entrega_id_tipo_entrega")]
public partial class Entrega
{
    [Key]
    [Column("id_entrega")]
    public int IdEntrega { get; set; }

    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("id_tipo_entrega")]
    public int IdTipoEntrega { get; set; }

    [Column("id_direccion")]
    public int? IdDireccion { get; set; }

    [Column("id_tienda")]
    public int? IdTienda { get; set; }

    [Column("id_horario")]
    public int? IdHorario { get; set; }

    [Column("fecha_estimada")]
    public DateOnly? FechaEstimada { get; set; }

    [Column("costo_envio", TypeName = "decimal(10, 2)")]
    public decimal CostoEnvio { get; set; }

    [Column("nombre_retira")]
    [StringLength(100)]
    public string? NombreRetira { get; set; }

    [Column("nro_doc_retira")]
    [StringLength(15)]
    [Unicode(false)]
    public string? NroDocRetira { get; set; }

    [ForeignKey("IdCarro")]
    [InverseProperty("Entregas")]
    public virtual Carro IdCarroNavigation { get; set; } = null!;

    [ForeignKey("IdDireccion")]
    [InverseProperty("Entregas")]
    public virtual Direccion? IdDireccionNavigation { get; set; }

    [ForeignKey("IdHorario")]
    [InverseProperty("Entregas")]
    public virtual HorarioDespacho? IdHorarioNavigation { get; set; }

    [ForeignKey("IdTienda")]
    [InverseProperty("Entregas")]
    public virtual Tienda? IdTiendaNavigation { get; set; }

    [ForeignKey("IdTipoEntrega")]
    [InverseProperty("Entregas")]
    public virtual TipoEntrega IdTipoEntregaNavigation { get; set; } = null!;
}
