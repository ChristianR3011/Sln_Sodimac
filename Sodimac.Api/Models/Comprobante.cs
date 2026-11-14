using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Boleta o factura electrónica: se registra el tipo en el checkout y se emite cuando se crea la orden.
/// </summary>
[Table("comprobante", Schema = "compra")]
[Index("IdCarro", Name = "UQ_comprobante_carro", IsUnique = true)]
public partial class Comprobante
{
    [Key]
    [Column("id_comprobante")]
    public int IdComprobante { get; set; }

    [Column("id_carro")]
    public int IdCarro { get; set; }

    [Column("nro_orden")]
    [StringLength(20)]
    [Unicode(false)]
    public string? NroOrden { get; set; }

    [Column("tipo")]
    [StringLength(10)]
    public string Tipo { get; set; } = null!;

    [Column("nro_documento_ruc")]
    [StringLength(15)]
    [Unicode(false)]
    public string NroDocumentoRuc { get; set; } = null!;

    [Column("razon_social")]
    [StringLength(120)]
    public string? RazonSocial { get; set; }

    [Column("direccion_fiscal")]
    [StringLength(150)]
    public string? DireccionFiscal { get; set; }

    [Column("serie")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Serie { get; set; }

    [Column("numero")]
    [StringLength(15)]
    [Unicode(false)]
    public string? Numero { get; set; }

    [Column("archivo_pdf")]
    [StringLength(255)]
    public string? ArchivoPdf { get; set; }

    [Column("fecha_emision")]
    [Precision(0)]
    public DateTime? FechaEmision { get; set; }

    [ForeignKey("IdCarro")]
    [InverseProperty("Comprobante")]
    public virtual Carro IdCarroNavigation { get; set; } = null!;

    [ForeignKey("NroOrden")]
    [InverseProperty("Comprobante")]
    public virtual Orden? NroOrdenNavigation { get; set; }
}
