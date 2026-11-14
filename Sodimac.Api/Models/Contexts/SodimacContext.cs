using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sodimac.Api.Models;

namespace Sodimac.Api.Models.Contexts;

public partial class SodimacContext : DbContext
{
    public SodimacContext(DbContextOptions<SodimacContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AtributoProducto> AtributoProductos { get; set; }

    public virtual DbSet<Campana> Campanas { get; set; }

    public virtual DbSet<Carro> Carros { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Comprador> Compradors { get; set; }

    public virtual DbSet<Comprobante> Comprobantes { get; set; }

    public virtual DbSet<Cupon> Cupons { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Direccion> Direccions { get; set; }

    public virtual DbSet<Distrito> Distritos { get; set; }

    public virtual DbSet<Entrega> Entregas { get; set; }

    public virtual DbSet<EntregaProductoZona> EntregaProductoZonas { get; set; }

    public virtual DbSet<HorarioDespacho> HorarioDespachos { get; set; }

    public virtual DbSet<ItemCarro> ItemCarros { get; set; }

    public virtual DbSet<ItemServicio> ItemServicios { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<MedioPago> MedioPagos { get; set; }

    public virtual DbSet<NotificacionCorreo> NotificacionCorreos { get; set; }

    public virtual DbSet<OpcionAsistente> OpcionAsistentes { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PagoEfectivo> PagoEfectivos { get; set; }

    public virtual DbSet<PagoTarjeta> PagoTarjetas { get; set; }

    public virtual DbSet<Precio> Precios { get; set; }

    public virtual DbSet<PreguntaAsistente> PreguntaAsistentes { get; set; }

    public virtual DbSet<PreguntaFrecuente> PreguntaFrecuentes { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoPatrocinado> ProductoPatrocinados { get; set; }

    public virtual DbSet<ProductoRelacionado> ProductoRelacionados { get; set; }

    public virtual DbSet<ProductoServicio> ProductoServicios { get; set; }

    public virtual DbSet<Provincia> Provincias { get; set; }

    public virtual DbSet<Resena> Resenas { get; set; }

    public virtual DbSet<Seleccion> Seleccions { get; set; }

    public virtual DbSet<ServicioAdicional> ServicioAdicionals { get; set; }

    public virtual DbSet<Sesion> Sesions { get; set; }

    public virtual DbSet<StockTienda> StockTiendas { get; set; }

    public virtual DbSet<Tienda> Tiendas { get; set; }

    public virtual DbSet<TipoEntrega> TipoEntregas { get; set; }

    public virtual DbSet<TokenRecuperacion> TokenRecuperacions { get; set; }

    public virtual DbSet<Vendedor> Vendedors { get; set; }

    public virtual DbSet<Zona> Zonas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AtributoProducto>(entity =>
        {
            entity.ToTable("atributo_producto", "busqueda", tb => tb.HasComment("Atributo técnico de un producto (ej. Potencia, Voltaje). Forma las especificaciones de la ficha y los filtros por atributos."));

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.AtributoProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_atributo_producto_producto");
        });

        modelBuilder.Entity<Campana>(entity =>
        {
            entity.ToTable("campana", "busqueda", tb => tb.HasComment("Campaña u oferta destacada de la página de inicio (ej. «Día del Shopping»). Solo se listan las vigentes; tipoDestino e idDestino indican si al hacer clic se abre una categoría o una selección."));

            entity.HasMany(d => d.Skus).WithMany(p => p.IdCampanas)
                .UsingEntity<Dictionary<string, object>>(
                    "CampanaProducto",
                    r => r.HasOne<Producto>().WithMany()
                        .HasForeignKey("Sku")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_campana_producto_producto"),
                    l => l.HasOne<Campana>().WithMany()
                        .HasForeignKey("IdCampana")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_campana_producto_campana"),
                    j =>
                    {
                        j.HasKey("IdCampana", "Sku");
                        j.ToTable("campana_producto", "busqueda", tb => tb.HasComment("Tabla intermedia entre campaña y producto: qué productos promociona cada campaña (el servicio 5 devuelve datos de esos productos)."));
                        j.HasIndex(new[] { "Sku" }, "IX_campana_producto_sku");
                        j.IndexerProperty<int>("IdCampana").HasColumnName("id_campana");
                        j.IndexerProperty<string>("Sku")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("sku");
                    });
        });

        modelBuilder.Entity<Carro>(entity =>
        {
            entity.ToTable("carro", "carro", tb => tb.HasComment("Carro de compras de un cliente o de un invitado. Agrega productos, muestra el carro, une el carro de invitado al iniciar sesión y entrega el resumen de compra."));

            entity.Property(e => e.EsInvitado).HasDefaultValue(true, "DF_carro_es_invitado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())", "DF_carro_fecha_creacion");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Carros).HasConstraintName("FK_carro_cliente");

            entity.HasOne(d => d.IdCuponNavigation).WithMany(p => p.Carros).HasConstraintName("FK_carro_cupon");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categoria", "busqueda", tb => tb.HasComment("Agrupa los productos del catálogo en una jerarquía de categorías y subcategorías (ej. Herramientas > Herramientas eléctricas > Taladros). Atiende el menú «Categorías», la ruta de navegación, las categorías relacionadas y el contenido informativo de cada categoría."));

            entity.Property(e => e.Activa).HasDefaultValue(true, "DF_categoria_activa");

            entity.HasOne(d => d.IdCategoriaPadreNavigation).WithMany(p => p.InverseIdCategoriaPadreNavigation).HasConstraintName("FK_categoria_categoria");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("cliente", "cuenta", tb => tb.HasComment("Persona registrada en la tienda. Registra su cuenta, inicia sesión y consulta o actualiza sus datos. Guarda los intentos fallidos para el bloqueo temporal."));

            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())", "DF_cliente_fecha_registro");
        });

        modelBuilder.Entity<Comprador>(entity =>
        {
            entity.ToTable("comprador", "compra", tb => tb.HasComment("Datos de quien compra (cliente registrado o invitado) registrados en el checkout."));

            entity.HasOne(d => d.IdCarroNavigation).WithOne(p => p.Comprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_comprador_carro");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Compradors).HasConstraintName("FK_comprador_cliente");
        });

        modelBuilder.Entity<Comprobante>(entity =>
        {
            entity.ToTable("comprobante", "compra", tb => tb.HasComment("Boleta o factura electrónica: se registra el tipo en el checkout y se emite cuando se crea la orden."));

            entity.HasIndex(e => e.NroOrden, "UX_comprobante_orden")
                .IsUnique()
                .HasFilter("([nro_orden] IS NOT NULL)");

            entity.HasIndex(e => new { e.Serie, e.Numero }, "UX_comprobante_serie_numero")
                .IsUnique()
                .HasFilter("([serie] IS NOT NULL AND [numero] IS NOT NULL)");

            entity.HasOne(d => d.IdCarroNavigation).WithOne(p => p.Comprobante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_comprobante_carro");

            entity.HasOne(d => d.NroOrdenNavigation).WithOne(p => p.Comprobante).HasConstraintName("FK_comprobante_orden");
        });

        modelBuilder.Entity<Cupon>(entity =>
        {
            entity.ToTable("cupon", "carro", tb => tb.HasComment("Cupón de descuento con sus condiciones: monto mínimo, medio de pago requerido y vigencia (ej. CMR10)."));

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_cupon_activo");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.ToTable("departamento", "busqueda", tb => tb.HasComment("Primer nivel de la ubicación geográfica (ubigeo)."));
        });

        modelBuilder.Entity<Direccion>(entity =>
        {
            entity.ToTable("direccion", "cuenta", tb => tb.HasComment("Dirección de entrega registrada por un cliente; una de ellas es la principal."));

            entity.HasIndex(e => e.IdCliente, "UX_direccion_principal")
                .IsUnique()
                .HasFilter("([principal]=(1))");

            entity.HasOne(d => d.IdClienteNavigation).WithOne(p => p.Direccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_direccion_cliente");

            entity.HasOne(d => d.IdDistritoNavigation).WithMany(p => p.Direccions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_direccion_distrito");
        });

        modelBuilder.Entity<Distrito>(entity =>
        {
            entity.ToTable("distrito", "busqueda", tb => tb.HasComment("Tercer nivel de la ubicación; pertenece a una provincia y ubica zonas, tiendas y direcciones."));

            entity.HasOne(d => d.IdProvinciaNavigation).WithMany(p => p.Distritos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_distrito_provincia");
        });

        modelBuilder.Entity<Entrega>(entity =>
        {
            entity.ToTable("entrega", "compra", tb => tb.HasComment("Entrega elegida para el carro: despacho a domicilio (dirección, fecha y horario) o retiro en tienda (tienda y persona que retira). Un carro puede tener varias entregas si los productos llegan en envíos separados."));

            entity.HasOne(d => d.IdCarroNavigation).WithMany(p => p.Entregas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entrega_carro");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Entregas)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_entrega_direccion");

            entity.HasOne(d => d.IdHorarioNavigation).WithMany(p => p.Entregas).HasConstraintName("FK_entrega_horario_despacho");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Entregas).HasConstraintName("FK_entrega_tienda");

            entity.HasOne(d => d.IdTipoEntregaNavigation).WithMany(p => p.Entregas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entrega_tipo_entrega");
        });

        modelBuilder.Entity<EntregaProductoZona>(entity =>
        {
            entity.ToTable("entrega_producto_zona", "busqueda", tb => tb.HasComment("Modalidad de entrega disponible para un producto en una zona, con su plazo y costo (ej. «Llega hoy», «Retira desde 120 min»)."));

            entity.HasOne(d => d.IdTipoEntregaNavigation).WithMany(p => p.EntregaProductoZonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entrega_producto_zona_tipo_entrega");

            entity.HasOne(d => d.IdZonaNavigation).WithMany(p => p.EntregaProductoZonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entrega_producto_zona_zona");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.EntregaProductoZonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entrega_producto_zona_producto");
        });

        modelBuilder.Entity<HorarioDespacho>(entity =>
        {
            entity.ToTable("horario_despacho", "compra", tb => tb.HasComment("Fecha y rango horario de despacho con capacidad disponible para una zona."));

            entity.HasOne(d => d.IdZonaNavigation).WithMany(p => p.HorarioDespachos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_horario_despacho_zona");
        });

        modelBuilder.Entity<ItemCarro>(entity =>
        {
            entity.ToTable("item_carro", "carro", tb => tb.HasComment("Línea del carro: un producto con su cantidad, precio unitario y subtotal. Permite actualizar la cantidad o eliminar el producto."));

            entity.HasOne(d => d.IdCarroNavigation).WithMany(p => p.ItemCarros).HasConstraintName("FK_item_carro_carro");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.ItemCarros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_carro_producto");
        });

        modelBuilder.Entity<ItemServicio>(entity =>
        {
            entity.ToTable("item_servicio", "carro", tb => tb.HasComment("Servicio adicional agregado a una línea del carro. Si se elimina el producto, se elimina también el servicio."));

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.ItemServicios).HasConstraintName("FK_item_servicio_item_carro");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ItemServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_item_servicio_servicio_adicional");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.ToTable("marca", "busqueda", tb => tb.HasComment("Marca o fabricante de los productos (ej. Bauker, Bosch). Lista las marcas destacadas de una categoría en el menú y los productos de una marca."));

            entity.HasMany(d => d.IdCategoria).WithMany(p => p.IdMarcas)
                .UsingEntity<Dictionary<string, object>>(
                    "MarcaCategorium",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_marca_categoria_categoria"),
                    l => l.HasOne<Marca>().WithMany()
                        .HasForeignKey("IdMarca")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_marca_categoria_marca"),
                    j =>
                    {
                        j.HasKey("IdMarca", "IdCategoria");
                        j.ToTable("marca_categoria", "busqueda", tb => tb.HasComment("Tabla intermedia que resuelve la relación muchos a muchos entre marca y categoría: qué marcas se destacan en cada categoría del menú."));
                        j.HasIndex(new[] { "IdCategoria" }, "IX_marca_categoria_id_categoria");
                        j.IndexerProperty<int>("IdMarca").HasColumnName("id_marca");
                        j.IndexerProperty<int>("IdCategoria").HasColumnName("id_categoria");
                    });
        });

        modelBuilder.Entity<MedioPago>(entity =>
        {
            entity.ToTable("medio_pago", "compra", tb => tb.HasComment("Medio de pago aceptado (tarjeta CMR, crédito, débito, pago en efectivo, Gift Card) con las cuotas disponibles."));
        });

        modelBuilder.Entity<NotificacionCorreo>(entity =>
        {
            entity.ToTable("notificacion_correo", "pago", tb => tb.HasComment("Correo enviado al comprador con la confirmación de su compra."));

            entity.Property(e => e.Estado).HasDefaultValue("ENVIADO", "DF_notificacion_correo_estado");
            entity.Property(e => e.FechaEnvio).HasDefaultValueSql("(sysdatetime())", "DF_notificacion_correo_fecha_envio");

            entity.HasOne(d => d.NroOrdenNavigation).WithMany(p => p.NotificacionCorreos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_notificacion_correo_orden");
        });

        modelBuilder.Entity<OpcionAsistente>(entity =>
        {
            entity.ToTable("opcion_asistente", "busqueda", tb => tb.HasComment("Opción de respuesta de una pregunta del asistente; cada opción tiene el filtro que se aplica al elegirla."));

            entity.HasOne(d => d.IdPreguntaAsistenteNavigation).WithMany(p => p.OpcionAsistentes).HasConstraintName("FK_opcion_asistente_pregunta_asistente");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.ToTable("orden", "pago", tb => tb.HasComment("Orden de compra creada cuando el pago se aprueba (o queda pendiente si es en efectivo). Registra los CMR Puntos acumulados."));

            entity.Property(e => e.Estado).HasDefaultValue("REGISTRADA", "DF_orden_estado");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())", "DF_orden_fecha_creacion");

            entity.HasOne(d => d.IdCarroNavigation).WithOne(p => p.Orden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orden_carro");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Ordens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orden_comprador");

            entity.HasOne(d => d.IdTransaccionNavigation).WithOne(p => p.Orden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orden_pago");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("pago", "pago", tb => tb.HasComment("Clase abstracta con los datos comunes de una transacción de pago: monto, estado y fecha."));

            entity.Property(e => e.Estado).HasDefaultValue("PENDIENTE", "DF_pago_estado");
            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysdatetime())", "DF_pago_fecha");

            entity.HasOne(d => d.IdCarroNavigation).WithMany(p => p.Pagos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_carro");

            entity.HasOne(d => d.IdMedioNavigation).WithMany(p => p.Pagos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_medio_pago");
        });

        modelBuilder.Entity<PagoEfectivo>(entity =>
        {
            entity.ToTable("pago_efectivo", "pago", tb => tb.HasComment("Pago en efectivo mediante un código de pago con fecha de vencimiento."));

            entity.Property(e => e.IdTransaccion).ValueGeneratedNever();

            entity.HasOne(d => d.IdTransaccionNavigation).WithOne(p => p.PagoEfectivo).HasConstraintName("FK_pago_efectivo_pago");
        });

        modelBuilder.Entity<PagoTarjeta>(entity =>
        {
            entity.ToTable("pago_tarjeta", "pago", tb => tb.HasComment("Pago con tarjeta de crédito o débito autorizado por la pasarela; guarda solo los últimos dígitos, nunca la tarjeta completa."));

            entity.Property(e => e.IdTransaccion).ValueGeneratedNever();
            entity.Property(e => e.Cuotas).HasDefaultValue(1, "DF_pago_tarjeta_cuotas");

            entity.HasOne(d => d.IdTransaccionNavigation).WithOne(p => p.PagoTarjetum).HasConstraintName("FK_pago_tarjeta_pago");
        });

        modelBuilder.Entity<Precio>(entity =>
        {
            entity.ToTable("precio", "busqueda", tb => tb.HasComment("Precio vigente de un producto en una zona: precio normal, de oferta, con tarjeta CMR, % de descuento, etiqueta del evento y precio por volumen."));

            entity.HasOne(d => d.IdCuponNavigation).WithMany(p => p.Precios).HasConstraintName("FK_precio_cupon");

            entity.HasOne(d => d.IdZonaNavigation).WithMany(p => p.Precios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_precio_zona");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.Precios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_precio_producto");
        });

        modelBuilder.Entity<PreguntaAsistente>(entity =>
        {
            entity.ToTable("pregunta_asistente", "busqueda", tb => tb.HasComment("Pregunta del asistente de búsqueda guiada de una categoría (ej. «¿Qué tipo de taladro prefieres?»)."));

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.PreguntaAsistentes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pregunta_asistente_categoria");
        });

        modelBuilder.Entity<PreguntaFrecuente>(entity =>
        {
            entity.ToTable("pregunta_frecuente", "busqueda", tb => tb.HasComment("Pregunta y respuesta que forma parte del contenido informativo al final del listado de una categoría."));

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.PreguntaFrecuentes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pregunta_frecuente_categoria");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("producto", "busqueda", tb => tb.HasComment("Artículo que se vende, identificado por su SKU. Es la clase central de la búsqueda: autocompletado, productos populares, búsqueda por palabra clave, listado por categoría, filtros, orden, ficha de detalle y productos relacionados."));

            entity.HasIndex(e => new { e.UnidadesVendidas, e.Visitas }, "IX_producto_populares")
                .IsDescending()
                .HasFilter("([activo]=(1))");

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_producto_activo");
            entity.Property(e => e.CantidadMaximaCompra).HasDefaultValue(10, "DF_producto_cantidad_maxima_compra");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_categoria");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_marca");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Productos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_vendedor");
        });

        modelBuilder.Entity<ProductoPatrocinado>(entity =>
        {
            entity.ToTable("producto_patrocinado", "busqueda", tb => tb.HasComment("Producto promocionado por un anunciante que se intercala en los resultados con la etiqueta «Patrocinado»."));

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.ProductoPatrocinados).HasConstraintName("FK_producto_patrocinado_categoria");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.ProductoPatrocinados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_patrocinado_producto");
        });

        modelBuilder.Entity<ProductoRelacionado>(entity =>
        {
            entity.ToTable("producto_relacionado", "busqueda", tb => tb.HasComment("Tabla intermedia que relaciona un producto consigo mismo: para cada SKU, los SKU similares o complementarios (tipo)."));

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.ProductoRelacionadoSkuNavigations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SkuRelacionadoNavigation).WithMany(p => p.ProductoRelacionadoSkuRelacionadoNavigations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProductoServicio>(entity =>
        {
            entity.ToTable("producto_servicio", "carro", tb => tb.HasComment("Clase de asociación que indica si un servicio adicional está disponible para un producto en una zona."));

            entity.Property(e => e.Disponible).HasDefaultValue(true, "DF_producto_servicio_disponible");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ProductoServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_servicio_servicio_adicional");

            entity.HasOne(d => d.IdZonaNavigation).WithMany(p => p.ProductoServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_servicio_zona");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.ProductoServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_servicio_producto");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.ToTable("provincia", "busqueda", tb => tb.HasComment("Segundo nivel de la ubicación; pertenece a un departamento."));

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Provincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_provincia_departamento");
        });

        modelBuilder.Entity<Resena>(entity =>
        {
            entity.ToTable("resena", "busqueda", tb => tb.HasComment("Opinión publicada por un cliente sobre un producto, con su calificación y fecha."));

            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysdatetime())", "DF_resena_fecha");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Resenas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resena_cliente");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.Resenas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_resena_producto");
        });

        modelBuilder.Entity<Seleccion>(entity =>
        {
            entity.ToTable("seleccion", "busqueda", tb => tb.HasComment("Colección de productos armada por el área comercial (ej. «Lo más vendido», «Precio x volumen»)."));

            entity.Property(e => e.Activa).HasDefaultValue(true, "DF_seleccion_activa");

            entity.HasMany(d => d.Skus).WithMany(p => p.IdSeleccions)
                .UsingEntity<Dictionary<string, object>>(
                    "SeleccionProducto",
                    r => r.HasOne<Producto>().WithMany()
                        .HasForeignKey("Sku")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_seleccion_producto_producto"),
                    l => l.HasOne<Seleccion>().WithMany()
                        .HasForeignKey("IdSeleccion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_seleccion_producto_seleccion"),
                    j =>
                    {
                        j.HasKey("IdSeleccion", "Sku");
                        j.ToTable("seleccion_producto", "busqueda", tb => tb.HasComment("Tabla intermedia entre selección y producto: qué productos forman cada selección o colección."));
                        j.HasIndex(new[] { "Sku" }, "IX_seleccion_producto_sku");
                        j.IndexerProperty<int>("IdSeleccion").HasColumnName("id_seleccion");
                        j.IndexerProperty<string>("Sku")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .HasColumnName("sku");
                    });
        });

        modelBuilder.Entity<ServicioAdicional>(entity =>
        {
            entity.ToTable("servicio_adicional", "carro", tb => tb.HasComment("Servicio que se puede contratar con un producto (ej. armado, instalación, garantía extendida)."));
        });

        modelBuilder.Entity<Sesion>(entity =>
        {
            entity.ToTable("sesion", "cuenta", tb => tb.HasComment("Sesión iniciada por un cliente, identificada por un token con fecha de expiración. Permite cerrar sesión."));

            entity.Property(e => e.Activa).HasDefaultValue(true, "DF_sesion_activa");
            entity.Property(e => e.FechaEmision).HasDefaultValueSql("(sysdatetime())", "DF_sesion_fecha_emision");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Sesions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sesion_cliente");
        });

        modelBuilder.Entity<StockTienda>(entity =>
        {
            entity.ToTable("stock_tienda", "busqueda", tb => tb.HasComment("Stock de un producto en una tienda y tiempo estimado de retiro. Responde «Consultar stock por tienda» y el filtro por stock."));

            entity.Property(e => e.Disponible).HasDefaultValue(true, "DF_stock_tienda_disponible");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.StockTienda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_tienda_tienda");

            entity.HasOne(d => d.SkuNavigation).WithMany(p => p.StockTienda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stock_tienda_producto");
        });

        modelBuilder.Entity<Tienda>(entity =>
        {
            entity.ToTable("tienda", "compra", tb => tb.HasComment("Tienda Sodimac o Maestro donde el cliente puede retirar su pedido."));

            entity.Property(e => e.Tipo).HasDefaultValue("SODIMAC", "DF_tienda_tipo");

            entity.HasOne(d => d.IdDistritoNavigation).WithMany(p => p.Tienda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tienda_distrito");
        });

        modelBuilder.Entity<TipoEntrega>(entity =>
        {
            entity.ToTable("tipo_entrega", "compra", tb => tb.HasComment("Catálogo de modalidades de entrega: envío a domicilio, envío express y retiro en tienda."));
        });

        modelBuilder.Entity<TokenRecuperacion>(entity =>
        {
            entity.ToTable("token_recuperacion", "cuenta", tb => tb.HasComment("Token temporal enviado al correo para recuperar y restablecer la contraseña; tiene vigencia y no puede reutilizarse."));

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.TokenRecuperacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_token_recuperacion_cliente");
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.ToTable("vendedor", "busqueda", tb => tb.HasComment("Quién vende el producto: Sodimac o un vendedor tercero del marketplace. Se usa en las tarjetas de producto y en el filtro «Vendido por»."));
        });

        modelBuilder.Entity<Zona>(entity =>
        {
            entity.ToTable("zona", "busqueda", tb => tb.HasComment("Zona de cobertura de un distrito. Se obtiene al registrar la ubicación de entrega y condiciona precios, entregas, horarios y servicios."));

            entity.HasOne(d => d.IdDistritoNavigation).WithOne(p => p.Zona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_zona_distrito");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
