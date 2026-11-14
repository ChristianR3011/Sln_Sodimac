using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Models;

/// <summary>
/// Artículo que se vende, identificado por su SKU. Es la clase central de la búsqueda: autocompletado, productos populares, búsqueda por palabra clave, listado por categoría, filtros, orden, ficha de detalle y productos relacionados.
/// </summary>
[Table("producto", Schema = "busqueda")]
[Index("IdCategoria", Name = "IX_producto_id_categoria")]
[Index("IdMarca", Name = "IX_producto_id_marca")]
[Index("IdVendedor", Name = "IX_producto_id_vendedor")]
[Index("Nombre", Name = "IX_producto_nombre")]
public partial class Producto
{
    [Key]
    [Column("sku")]
    [StringLength(20)]
    [Unicode(false)]
    public string Sku { get; set; } = null!;

    [Column("id_marca")]
    public int IdMarca { get; set; }

    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [Column("id_vendedor")]
    public int IdVendedor { get; set; }

    [Column("nombre")]
    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [Column("modelo")]
    [StringLength(60)]
    public string? Modelo { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("especificaciones")]
    public string? Especificaciones { get; set; }

    [Column("imagen")]
    [StringLength(255)]
    public string? Imagen { get; set; }

    [Column("calificacion_promedio", TypeName = "decimal(10, 2)")]
    public decimal CalificacionPromedio { get; set; }

    [Column("total_resenas")]
    public int TotalResenas { get; set; }

    [Column("unidades_vendidas")]
    public int UnidadesVendidas { get; set; }

    [Column("visitas")]
    public int Visitas { get; set; }

    [Column("cantidad_maxima_compra")]
    public int CantidadMaximaCompra { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<AtributoProducto> AtributoProductos { get; set; } = new List<AtributoProducto>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<EntregaProductoZona> EntregaProductoZonas { get; set; } = new List<EntregaProductoZona>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("Productos")]
    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    [ForeignKey("IdMarca")]
    [InverseProperty("Productos")]
    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    [ForeignKey("IdVendedor")]
    [InverseProperty("Productos")]
    public virtual Vendedor IdVendedorNavigation { get; set; } = null!;

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<ItemCarro> ItemCarros { get; set; } = new List<ItemCarro>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<Precio> Precios { get; set; } = new List<Precio>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<ProductoPatrocinado> ProductoPatrocinados { get; set; } = new List<ProductoPatrocinado>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<ProductoRelacionado> ProductoRelacionadoSkuNavigations { get; set; } = new List<ProductoRelacionado>();

    [InverseProperty("SkuRelacionadoNavigation")]
    public virtual ICollection<ProductoRelacionado> ProductoRelacionadoSkuRelacionadoNavigations { get; set; } = new List<ProductoRelacionado>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<ProductoServicio> ProductoServicios { get; set; } = new List<ProductoServicio>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    [InverseProperty("SkuNavigation")]
    public virtual ICollection<StockTienda> StockTienda { get; set; } = new List<StockTienda>();

    [ForeignKey("Sku")]
    [InverseProperty("Skus")]
    public virtual ICollection<Campana> IdCampanas { get; set; } = new List<Campana>();

    [ForeignKey("Sku")]
    [InverseProperty("Skus")]
    public virtual ICollection<Seleccion> IdSeleccions { get; set; } = new List<Seleccion>();
}
