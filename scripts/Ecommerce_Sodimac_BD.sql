/* ==========================================================================
   Sistema de E-Commerce de Sodimac – Script de creación de la base de datos
   Motor: Microsoft SQL Server 2016 o superior (usa DROP ... IF EXISTS)
   Base de datos: ecommerce_sodimac
   Contenido: 5 esquemas (uno por proceso), 45 tablas, 63 claves foráneas,
              claves primarias, únicas, CHECK, valores por defecto, índices y descripciones.
   Fuente: Diagrama ER del Capítulo II (62 servicios del inventario).
   Grupo 8 – Arquitectura Orientada al Servicio
   ========================================================================== */

SET NOCOUNT ON;
GO

/* 1. Base de datos ------------------------------------------------------- */
IF DB_ID(N'ecommerce_sodimac') IS NULL
    CREATE DATABASE ecommerce_sodimac COLLATE Modern_Spanish_CI_AS;
GO
USE ecommerce_sodimac;
GO

/* 2. Limpieza (permite volver a ejecutar el script) ----------------------- */
DROP TABLE IF EXISTS compra.comprobante;
DROP TABLE IF EXISTS busqueda.entrega_producto_zona;
DROP TABLE IF EXISTS busqueda.stock_tienda;
DROP TABLE IF EXISTS busqueda.resena;
DROP TABLE IF EXISTS busqueda.precio;
DROP TABLE IF EXISTS busqueda.seleccion_producto;
DROP TABLE IF EXISTS busqueda.campana_producto;
DROP TABLE IF EXISTS pago.notificacion_correo;
DROP TABLE IF EXISTS pago.orden;
DROP TABLE IF EXISTS pago.pago_efectivo;
DROP TABLE IF EXISTS pago.pago_tarjeta;
DROP TABLE IF EXISTS pago.pago;
DROP TABLE IF EXISTS compra.medio_pago;
DROP TABLE IF EXISTS compra.entrega;
DROP TABLE IF EXISTS compra.comprador;
DROP TABLE IF EXISTS carro.item_servicio;
DROP TABLE IF EXISTS carro.producto_servicio;
DROP TABLE IF EXISTS carro.servicio_adicional;
DROP TABLE IF EXISTS carro.item_carro;
DROP TABLE IF EXISTS carro.carro;
DROP TABLE IF EXISTS carro.cupon;
DROP TABLE IF EXISTS cuenta.direccion;
DROP TABLE IF EXISTS cuenta.token_recuperacion;
DROP TABLE IF EXISTS cuenta.sesion;
DROP TABLE IF EXISTS cuenta.cliente;
DROP TABLE IF EXISTS compra.horario_despacho;
DROP TABLE IF EXISTS compra.tipo_entrega;
DROP TABLE IF EXISTS compra.tienda;
DROP TABLE IF EXISTS busqueda.zona;
DROP TABLE IF EXISTS busqueda.distrito;
DROP TABLE IF EXISTS busqueda.provincia;
DROP TABLE IF EXISTS busqueda.departamento;
DROP TABLE IF EXISTS busqueda.producto_patrocinado;
DROP TABLE IF EXISTS busqueda.producto_relacionado;
DROP TABLE IF EXISTS busqueda.atributo_producto;
DROP TABLE IF EXISTS busqueda.producto;
DROP TABLE IF EXISTS busqueda.vendedor;
DROP TABLE IF EXISTS busqueda.seleccion;
DROP TABLE IF EXISTS busqueda.campana;
DROP TABLE IF EXISTS busqueda.opcion_asistente;
DROP TABLE IF EXISTS busqueda.pregunta_asistente;
DROP TABLE IF EXISTS busqueda.pregunta_frecuente;
DROP TABLE IF EXISTS busqueda.marca_categoria;
DROP TABLE IF EXISTS busqueda.marca;
DROP TABLE IF EXISTS busqueda.categoria;
GO

/* 3. Esquemas: uno por proceso del inventario ---------------------------- */
IF SCHEMA_ID(N'busqueda') IS NULL EXEC (N'CREATE SCHEMA busqueda AUTHORIZATION dbo');   -- Búsqueda de Productos (servicios 1–30)
IF SCHEMA_ID(N'cuenta') IS NULL EXEC (N'CREATE SCHEMA cuenta AUTHORIZATION dbo');   -- Gestión de Cuenta de Cliente (servicios 31–41)
IF SCHEMA_ID(N'carro') IS NULL EXEC (N'CREATE SCHEMA carro AUTHORIZATION dbo');   -- Carro de Compras (servicios 42–48)
IF SCHEMA_ID(N'compra') IS NULL EXEC (N'CREATE SCHEMA compra AUTHORIZATION dbo');   -- Proceso de Compra (servicios 49–57)
IF SCHEMA_ID(N'pago') IS NULL EXEC (N'CREATE SCHEMA pago AUTHORIZATION dbo');   -- Pago (servicios 58–62)
GO

/* 4. Tablas -------------------------------------------------------------- */

/* ---- Búsqueda de Productos (esquema busqueda) ---- */

-- Agrupa los productos del catálogo en una jerarquía de categorías y subcategorías (ej. Herramientas > Herramientas eléctricas > Taladros). Atiende el menú «Categorías», la ruta de navegación, las categorías relacionadas y el contenido informativo de cada categoría.
CREATE TABLE busqueda.categoria (
    id_categoria               INT             IDENTITY(1,1) NOT NULL,
    id_categoria_padre         INT             NULL,
    nombre                     NVARCHAR(80)    NOT NULL,
    orden                      INT             NOT NULL,
    activa                     BIT             NOT NULL CONSTRAINT DF_categoria_activa DEFAULT (1),
    descripcion_informativa    NVARCHAR(MAX)   NULL,
    CONSTRAINT PK_categoria PRIMARY KEY CLUSTERED (id_categoria),
    CONSTRAINT CK_categoria_padre_distinto CHECK (id_categoria_padre IS NULL OR id_categoria_padre <> id_categoria)   -- servicio 2, 10
);

-- Marca o fabricante de los productos (ej. Bauker, Bosch). Lista las marcas destacadas de una categoría en el menú y los productos de una marca.
CREATE TABLE busqueda.marca (
    id_marca                   INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(60)    NOT NULL,
    CONSTRAINT PK_marca PRIMARY KEY CLUSTERED (id_marca),
    CONSTRAINT UQ_marca_nombre UNIQUE (nombre)   -- servicio 25
);

-- Tabla intermedia que resuelve la relación muchos a muchos entre marca y categoría: qué marcas se destacan en cada categoría del menú.
CREATE TABLE busqueda.marca_categoria (
    id_marca                   INT             NOT NULL,
    id_categoria               INT             NOT NULL,
    CONSTRAINT PK_marca_categoria PRIMARY KEY CLUSTERED (id_marca, id_categoria)
);

-- Pregunta y respuesta que forma parte del contenido informativo al final del listado de una categoría.
CREATE TABLE busqueda.pregunta_frecuente (
    id_pregunta                INT             IDENTITY(1,1) NOT NULL,
    id_categoria               INT             NOT NULL,
    pregunta                   NVARCHAR(MAX)   NOT NULL,
    respuesta                  NVARCHAR(MAX)   NOT NULL,
    CONSTRAINT PK_pregunta_frecuente PRIMARY KEY CLUSTERED (id_pregunta)
);

-- Pregunta del asistente de búsqueda guiada de una categoría (ej. «¿Qué tipo de taladro prefieres?»).
CREATE TABLE busqueda.pregunta_asistente (
    id_pregunta_asistente      INT             IDENTITY(1,1) NOT NULL,
    id_categoria               INT             NOT NULL,
    pregunta                   NVARCHAR(150)   NOT NULL,
    CONSTRAINT PK_pregunta_asistente PRIMARY KEY CLUSTERED (id_pregunta_asistente)
);

-- Opción de respuesta de una pregunta del asistente; cada opción tiene el filtro que se aplica al elegirla.
CREATE TABLE busqueda.opcion_asistente (
    id_opcion                  INT             IDENTITY(1,1) NOT NULL,
    id_pregunta_asistente      INT             NOT NULL,
    etiqueta                   NVARCHAR(80)    NOT NULL,
    filtro_asociado            NVARCHAR(100)   NOT NULL,
    CONSTRAINT PK_opcion_asistente PRIMARY KEY CLUSTERED (id_opcion)
);

-- Campaña u oferta destacada de la página de inicio (ej. «Día del Shopping»). Solo se listan las vigentes; tipoDestino e idDestino indican si al hacer clic se abre una categoría o una selección.
CREATE TABLE busqueda.campana (
    id_campana                 INT             IDENTITY(1,1) NOT NULL,
    titulo                     NVARCHAR(100)   NOT NULL,
    descuento                  DECIMAL(10,2)   NULL,
    imagen                     NVARCHAR(255)   NULL,
    tipo_destino               NVARCHAR(20)    NOT NULL,
    id_destino                 VARCHAR(30)     NOT NULL,
    fecha_inicio               DATE            NOT NULL,
    fecha_fin                  DATE            NOT NULL,
    CONSTRAINT PK_campana PRIMARY KEY CLUSTERED (id_campana),
    CONSTRAINT CK_campana_tipo_destino CHECK (tipo_destino IN (N'CATEGORIA', N'SELECCION', N'MARCA')),   -- servicio 5
    CONSTRAINT CK_campana_fechas CHECK (fecha_fin >= fecha_inicio),   -- servicio 5
    CONSTRAINT CK_campana_descuento CHECK (descuento IS NULL OR descuento BETWEEN 0 AND 100)   -- servicio 5
);

-- Colección de productos armada por el área comercial (ej. «Lo más vendido», «Precio x volumen»).
CREATE TABLE busqueda.seleccion (
    id_seleccion               INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(80)    NOT NULL,
    activa                     BIT             NOT NULL CONSTRAINT DF_seleccion_activa DEFAULT (1),
    CONSTRAINT PK_seleccion PRIMARY KEY CLUSTERED (id_seleccion),
    CONSTRAINT UQ_seleccion_nombre UNIQUE (nombre)   -- servicio 26
);

-- Quién vende el producto: Sodimac o un vendedor tercero del marketplace. Se usa en las tarjetas de producto y en el filtro «Vendido por».
CREATE TABLE busqueda.vendedor (
    id_vendedor                INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(80)    NOT NULL,
    tipo                       NVARCHAR(20)    NOT NULL,
    CONSTRAINT PK_vendedor PRIMARY KEY CLUSTERED (id_vendedor),
    CONSTRAINT CK_vendedor_tipo CHECK (tipo IN (N'SODIMAC', N'TERCERO'))   -- servicio 19
);

-- Artículo que se vende, identificado por su SKU. Es la clase central de la búsqueda: autocompletado, productos populares, búsqueda por palabra clave, listado por categoría, filtros, orden, ficha de detalle y productos relacionados.
CREATE TABLE busqueda.producto (
    sku                        VARCHAR(20)     NOT NULL,
    id_marca                   INT             NOT NULL,
    id_categoria               INT             NOT NULL,
    id_vendedor                INT             NOT NULL,
    nombre                     NVARCHAR(150)   NOT NULL,
    modelo                     NVARCHAR(60)    NULL,
    descripcion                NVARCHAR(MAX)   NULL,
    especificaciones           NVARCHAR(MAX)   NULL,
    imagen                     NVARCHAR(255)   NULL,
    calificacion_promedio      DECIMAL(10,2)   NOT NULL CONSTRAINT DF_producto_calificacion_promedio DEFAULT (0),
    total_resenas              INT             NOT NULL CONSTRAINT DF_producto_total_resenas DEFAULT (0),
    unidades_vendidas          INT             NOT NULL CONSTRAINT DF_producto_unidades_vendidas DEFAULT (0),
    visitas                    INT             NOT NULL CONSTRAINT DF_producto_visitas DEFAULT (0),
    cantidad_maxima_compra     INT             NOT NULL CONSTRAINT DF_producto_cantidad_maxima_compra DEFAULT (10),
    activo                     BIT             NOT NULL CONSTRAINT DF_producto_activo DEFAULT (1),
    CONSTRAINT PK_producto PRIMARY KEY CLUSTERED (sku),
    CONSTRAINT CK_producto_calificacion CHECK (calificacion_promedio BETWEEN 0 AND 5),   -- servicio 20
    CONSTRAINT CK_producto_contadores CHECK (total_resenas >= 0 AND unidades_vendidas >= 0 AND visitas >= 0),   -- servicio 7
    CONSTRAINT CK_producto_cantidad_max CHECK (cantidad_maxima_compra > 0)   -- servicio 42
);

-- Atributo técnico de un producto (ej. Potencia, Voltaje). Forma las especificaciones de la ficha y los filtros por atributos.
CREATE TABLE busqueda.atributo_producto (
    id_atributo                INT             IDENTITY(1,1) NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    nombre                     NVARCHAR(60)    NOT NULL,
    valor                      NVARCHAR(80)    NOT NULL,
    CONSTRAINT PK_atributo_producto PRIMARY KEY CLUSTERED (id_atributo)
);

-- Tabla intermedia que relaciona un producto consigo mismo: para cada SKU, los SKU similares o complementarios (tipo).
CREATE TABLE busqueda.producto_relacionado (
    sku                        VARCHAR(20)     NOT NULL,
    sku_relacionado            VARCHAR(20)     NOT NULL,
    tipo                       NVARCHAR(20)    NOT NULL,
    CONSTRAINT PK_producto_relacionado PRIMARY KEY CLUSTERED (sku, sku_relacionado),
    CONSTRAINT CK_producto_relacionado_distinto CHECK (sku <> sku_relacionado),   -- servicio 30
    CONSTRAINT CK_producto_relacionado_tipo CHECK (tipo IN (N'SIMILAR', N'COMPLEMENTARIO'))   -- servicio 30
);

-- Producto promocionado por un anunciante que se intercala en los resultados con la etiqueta «Patrocinado».
CREATE TABLE busqueda.producto_patrocinado (
    id_patrocinio              INT             IDENTITY(1,1) NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    id_categoria               INT             NULL,
    anunciante                 NVARCHAR(80)    NOT NULL,
    fecha_inicio               DATE            NOT NULL,
    fecha_fin                  DATE            NOT NULL,
    CONSTRAINT PK_producto_patrocinado PRIMARY KEY CLUSTERED (id_patrocinio),
    CONSTRAINT CK_producto_patrocinado_fechas CHECK (fecha_fin >= fecha_inicio)   -- servicio 22
);

-- Primer nivel de la ubicación geográfica (ubigeo).
CREATE TABLE busqueda.departamento (
    id_departamento            INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(50)    NOT NULL,
    CONSTRAINT PK_departamento PRIMARY KEY CLUSTERED (id_departamento)
);

-- Segundo nivel de la ubicación; pertenece a un departamento.
CREATE TABLE busqueda.provincia (
    id_provincia               INT             IDENTITY(1,1) NOT NULL,
    id_departamento            INT             NOT NULL,
    nombre                     NVARCHAR(50)    NOT NULL,
    CONSTRAINT PK_provincia PRIMARY KEY CLUSTERED (id_provincia)
);

-- Tercer nivel de la ubicación; pertenece a una provincia y ubica zonas, tiendas y direcciones.
CREATE TABLE busqueda.distrito (
    id_distrito                INT             IDENTITY(1,1) NOT NULL,
    id_provincia               INT             NOT NULL,
    nombre                     NVARCHAR(50)    NOT NULL,
    CONSTRAINT PK_distrito PRIMARY KEY CLUSTERED (id_distrito)
);

-- Zona de cobertura de un distrito. Se obtiene al registrar la ubicación de entrega y condiciona precios, entregas, horarios y servicios.
CREATE TABLE busqueda.zona (
    id_zona                    INT             IDENTITY(1,1) NOT NULL,
    id_distrito                INT             NOT NULL,
    nombre                     NVARCHAR(60)    NOT NULL,
    tiene_cobertura            BIT             NOT NULL,
    CONSTRAINT PK_zona PRIMARY KEY CLUSTERED (id_zona),
    CONSTRAINT UQ_zona_distrito UNIQUE (id_distrito)   -- servicio 6: una zona por distrito
);

/* ---- Proceso de Compra (esquema compra) ---- */

-- Tienda Sodimac o Maestro donde el cliente puede retirar su pedido.
CREATE TABLE compra.tienda (
    id_tienda                  INT             IDENTITY(1,1) NOT NULL,
    id_distrito                INT             NOT NULL,
    nombre                     NVARCHAR(80)    NOT NULL,
    tipo                       NVARCHAR(20)    NOT NULL CONSTRAINT DF_tienda_tipo DEFAULT (N'SODIMAC'),
    direccion                  NVARCHAR(150)   NOT NULL,
    horario                    NVARCHAR(80)    NULL,
    CONSTRAINT PK_tienda PRIMARY KEY CLUSTERED (id_tienda),
    CONSTRAINT CK_tienda_tipo CHECK (tipo IN (N'SODIMAC', N'MAESTRO'))   -- servicio 17, 53
);

-- Catálogo de modalidades de entrega: envío a domicilio, envío express y retiro en tienda.
CREATE TABLE compra.tipo_entrega (
    id_tipo_entrega            INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(40)    NOT NULL,
    CONSTRAINT PK_tipo_entrega PRIMARY KEY CLUSTERED (id_tipo_entrega)
);

-- Fecha y rango horario de despacho con capacidad disponible para una zona.
CREATE TABLE compra.horario_despacho (
    id_horario                 INT             IDENTITY(1,1) NOT NULL,
    id_zona                    INT             NOT NULL,
    fecha                      DATE            NOT NULL,
    rango_horario              NVARCHAR(20)    NOT NULL,
    capacidad                  INT             NOT NULL,
    costo                      DECIMAL(10,2)   NOT NULL,
    CONSTRAINT PK_horario_despacho PRIMARY KEY CLUSTERED (id_horario),
    CONSTRAINT CK_horario_despacho_capacidad CHECK (capacidad >= 0 AND costo >= 0)   -- servicio 51
);

/* ---- Gestión de Cuenta de Cliente (esquema cuenta) ---- */

-- Persona registrada en la tienda. Registra su cuenta, inicia sesión y consulta o actualiza sus datos. Guarda los intentos fallidos para el bloqueo temporal.
CREATE TABLE cuenta.cliente (
    id_cliente                 INT             IDENTITY(1,1) NOT NULL,
    tipo_documento             VARCHAR(3)      NOT NULL,
    nro_documento              VARCHAR(15)     NOT NULL,
    nombres                    NVARCHAR(60)    NOT NULL,
    apellidos                  NVARCHAR(60)    NOT NULL,
    correo                     NVARCHAR(100)   NOT NULL,
    celular                    VARCHAR(15)     NOT NULL,
    contrasena_hash            VARCHAR(255)    NOT NULL,
    acepto_terminos            BIT             NOT NULL,
    intentos_fallidos          INT             NOT NULL CONSTRAINT DF_cliente_intentos_fallidos DEFAULT (0),
    bloqueado_hasta            DATETIME2(0)    NULL,
    fecha_registro             DATETIME2(0)    NOT NULL CONSTRAINT DF_cliente_fecha_registro DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_cliente PRIMARY KEY CLUSTERED (id_cliente),
    CONSTRAINT UQ_cliente_nro_documento UNIQUE (nro_documento),
    CONSTRAINT UQ_cliente_correo UNIQUE (correo),
    CONSTRAINT CK_cliente_tipo_documento CHECK (tipo_documento IN ('DNI', 'CE', 'PAS')),   -- servicio 31
    CONSTRAINT CK_cliente_correo CHECK (correo LIKE N'%_@_%._%'),   -- servicio 31
    CONSTRAINT CK_cliente_terminos CHECK (acepto_terminos = 1),   -- servicio 31
    CONSTRAINT CK_cliente_intentos CHECK (intentos_fallidos >= 0)   -- servicio 32
);

-- Sesión iniciada por un cliente, identificada por un token con fecha de expiración. Permite cerrar sesión.
CREATE TABLE cuenta.sesion (
    id_sesion                  INT             IDENTITY(1,1) NOT NULL,
    id_cliente                 INT             NOT NULL,
    token                      VARCHAR(255)    NOT NULL,
    fecha_emision              DATETIME2(0)    NOT NULL CONSTRAINT DF_sesion_fecha_emision DEFAULT (SYSDATETIME()),
    fecha_expiracion           DATETIME2(0)    NOT NULL,
    activa                     BIT             NOT NULL CONSTRAINT DF_sesion_activa DEFAULT (1),
    CONSTRAINT PK_sesion PRIMARY KEY CLUSTERED (id_sesion),
    CONSTRAINT UQ_sesion_token UNIQUE (token),   -- servicio 33
    CONSTRAINT CK_sesion_expiracion CHECK (fecha_expiracion > fecha_emision)   -- servicio 33
);

-- Token temporal enviado al correo para recuperar y restablecer la contraseña; tiene vigencia y no puede reutilizarse.
CREATE TABLE cuenta.token_recuperacion (
    id_token                   INT             IDENTITY(1,1) NOT NULL,
    id_cliente                 INT             NOT NULL,
    token                      VARCHAR(255)    NOT NULL,
    vigente_hasta              DATETIME2(0)    NOT NULL,
    usado                      BIT             NOT NULL CONSTRAINT DF_token_recuperacion_usado DEFAULT (0),
    CONSTRAINT PK_token_recuperacion PRIMARY KEY CLUSTERED (id_token),
    CONSTRAINT UQ_token_recuperacion_token UNIQUE (token)   -- servicio 35
);

-- Dirección de entrega registrada por un cliente; una de ellas es la principal.
CREATE TABLE cuenta.direccion (
    id_direccion               INT             IDENTITY(1,1) NOT NULL,
    id_cliente                 INT             NOT NULL,
    id_distrito                INT             NOT NULL,
    nombre                     NVARCHAR(60)    NOT NULL,
    direccion                  NVARCHAR(150)   NOT NULL,
    referencia                 NVARCHAR(150)   NULL,
    principal                  BIT             NOT NULL CONSTRAINT DF_direccion_principal DEFAULT (0),
    CONSTRAINT PK_direccion PRIMARY KEY CLUSTERED (id_direccion)
);

/* ---- Carro de Compras (esquema carro) ---- */

-- Cupón de descuento con sus condiciones: monto mínimo, medio de pago requerido y vigencia (ej. CMR10).
CREATE TABLE carro.cupon (
    id_cupon                   INT             IDENTITY(1,1) NOT NULL,
    codigo                     VARCHAR(20)     NOT NULL,
    descuento                  DECIMAL(10,2)   NOT NULL,
    monto_minimo               DECIMAL(10,2)   NULL,
    medio_pago_requerido       NVARCHAR(30)    NULL,
    vigente_desde              DATE            NOT NULL,
    vigente_hasta              DATE            NOT NULL,
    activo                     BIT             NOT NULL CONSTRAINT DF_cupon_activo DEFAULT (1),
    CONSTRAINT PK_cupon PRIMARY KEY CLUSTERED (id_cupon),
    CONSTRAINT UQ_cupon_codigo UNIQUE (codigo),
    CONSTRAINT CK_cupon_descuento CHECK (descuento > 0),   -- servicio 47
    CONSTRAINT CK_cupon_monto_minimo CHECK (monto_minimo IS NULL OR monto_minimo >= 0),   -- servicio 47
    CONSTRAINT CK_cupon_vigencia CHECK (vigente_hasta >= vigente_desde)   -- servicio 47
);

-- Carro de compras de un cliente o de un invitado. Agrega productos, muestra el carro, une el carro de invitado al iniciar sesión y entrega el resumen de compra.
CREATE TABLE carro.carro (
    id_carro                   INT             IDENTITY(1,1) NOT NULL,
    id_cliente                 INT             NULL,
    id_cupon                   INT             NULL,
    es_invitado                BIT             NOT NULL CONSTRAINT DF_carro_es_invitado DEFAULT (1),
    fecha_creacion             DATETIME2(0)    NOT NULL CONSTRAINT DF_carro_fecha_creacion DEFAULT (SYSDATETIME()),
    subtotal                   DECIMAL(10,2)   NOT NULL CONSTRAINT DF_carro_subtotal DEFAULT (0),
    descuentos                 DECIMAL(10,2)   NOT NULL CONSTRAINT DF_carro_descuentos DEFAULT (0),
    total                      DECIMAL(10,2)   NOT NULL CONSTRAINT DF_carro_total DEFAULT (0),
    CONSTRAINT PK_carro PRIMARY KEY CLUSTERED (id_carro),
    CONSTRAINT CK_carro_montos CHECK (subtotal >= 0 AND descuentos >= 0 AND total >= 0),   -- servicio 43
    CONSTRAINT CK_carro_invitado CHECK ((es_invitado = 1 AND id_cliente IS NULL) OR (es_invitado = 0 AND id_cliente IS NOT NULL))   -- servicio 48
);

-- Línea del carro: un producto con su cantidad, precio unitario y subtotal. Permite actualizar la cantidad o eliminar el producto.
CREATE TABLE carro.item_carro (
    id_item                    INT             IDENTITY(1,1) NOT NULL,
    id_carro                   INT             NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    cantidad                   INT             NOT NULL,
    precio_unitario            DECIMAL(10,2)   NOT NULL,
    subtotal                   DECIMAL(10,2)   NOT NULL,
    CONSTRAINT PK_item_carro PRIMARY KEY CLUSTERED (id_item),
    CONSTRAINT CK_item_carro_cantidad CHECK (cantidad > 0),   -- servicio 42, 44
    CONSTRAINT CK_item_carro_montos CHECK (precio_unitario >= 0 AND subtotal >= 0)   -- servicio 43
);

-- Servicio que se puede contratar con un producto (ej. armado, instalación, garantía extendida).
CREATE TABLE carro.servicio_adicional (
    id_servicio                INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(80)    NOT NULL,
    precio                     DECIMAL(10,2)   NOT NULL,
    CONSTRAINT PK_servicio_adicional PRIMARY KEY CLUSTERED (id_servicio),
    CONSTRAINT CK_servicio_adicional_precio CHECK (precio >= 0)   -- servicio 46
);

-- Clase de asociación que indica si un servicio adicional está disponible para un producto en una zona.
CREATE TABLE carro.producto_servicio (
    sku                        VARCHAR(20)     NOT NULL,
    id_servicio                INT             NOT NULL,
    id_zona                    INT             NOT NULL,
    disponible                 BIT             NOT NULL CONSTRAINT DF_producto_servicio_disponible DEFAULT (1),
    CONSTRAINT PK_producto_servicio PRIMARY KEY CLUSTERED (sku, id_servicio, id_zona)
);

-- Servicio adicional agregado a una línea del carro. Si se elimina el producto, se elimina también el servicio.
CREATE TABLE carro.item_servicio (
    id_item_servicio           INT             IDENTITY(1,1) NOT NULL,
    id_item                    INT             NOT NULL,
    id_servicio                INT             NOT NULL,
    precio                     DECIMAL(10,2)   NOT NULL,
    CONSTRAINT PK_item_servicio PRIMARY KEY CLUSTERED (id_item_servicio),
    CONSTRAINT CK_item_servicio_precio CHECK (precio >= 0)   -- servicio 46
);

/* ---- Proceso de Compra (esquema compra) ---- */

-- Datos de quien compra (cliente registrado o invitado) registrados en el checkout.
CREATE TABLE compra.comprador (
    id_comprador               INT             IDENTITY(1,1) NOT NULL,
    id_carro                   INT             NOT NULL,
    id_cliente                 INT             NULL,
    correo                     NVARCHAR(100)   NOT NULL,
    tipo_documento             VARCHAR(3)      NOT NULL,
    nro_documento              VARCHAR(15)     NOT NULL,
    nombres                    NVARCHAR(60)    NOT NULL,
    apellidos                  NVARCHAR(60)    NOT NULL,
    celular                    VARCHAR(15)     NOT NULL,
    CONSTRAINT PK_comprador PRIMARY KEY CLUSTERED (id_comprador),
    CONSTRAINT UQ_comprador_carro UNIQUE (id_carro),   -- servicio 49: un carro tiene como máximo un comprador
    CONSTRAINT CK_comprador_tipo_documento CHECK (tipo_documento IN ('DNI', 'CE', 'PAS', 'RUC')),   -- servicio 49
    CONSTRAINT CK_comprador_correo CHECK (correo LIKE N'%_@_%._%')   -- servicio 49
);

-- Entrega elegida para el carro: despacho a domicilio (dirección, fecha y horario) o retiro en tienda (tienda y persona que retira). Un carro puede tener varias entregas si los productos llegan en envíos separados.
CREATE TABLE compra.entrega (
    id_entrega                 INT             IDENTITY(1,1) NOT NULL,
    id_carro                   INT             NOT NULL,
    id_tipo_entrega            INT             NOT NULL,
    id_direccion               INT             NULL,
    id_tienda                  INT             NULL,
    id_horario                 INT             NULL,
    fecha_estimada             DATE            NULL,
    costo_envio                DECIMAL(10,2)   NOT NULL CONSTRAINT DF_entrega_costo_envio DEFAULT (0),
    nombre_retira              NVARCHAR(100)   NULL,
    nro_doc_retira             VARCHAR(15)     NULL,
    CONSTRAINT PK_entrega PRIMARY KEY CLUSTERED (id_entrega),
    CONSTRAINT CK_entrega_modalidad CHECK (id_direccion IS NOT NULL OR id_tienda IS NOT NULL),   -- servicio 52, 54
    CONSTRAINT CK_entrega_retiro CHECK (id_tienda IS NULL OR (nombre_retira IS NOT NULL AND nro_doc_retira IS NOT NULL)),   -- servicio 54
    CONSTRAINT CK_entrega_costo CHECK (costo_envio >= 0)   -- servicio 52
);

-- Medio de pago aceptado (tarjeta CMR, crédito, débito, pago en efectivo, Gift Card) con las cuotas disponibles.
CREATE TABLE compra.medio_pago (
    id_medio                   INT             IDENTITY(1,1) NOT NULL,
    nombre                     NVARCHAR(40)    NOT NULL,
    cuotas_disponibles         NVARCHAR(30)    NULL,
    CONSTRAINT PK_medio_pago PRIMARY KEY CLUSTERED (id_medio)
);

/* ---- Pago (esquema pago) ---- */

-- Clase abstracta con los datos comunes de una transacción de pago: monto, estado y fecha.
CREATE TABLE pago.pago (
    id_transaccion             INT             IDENTITY(1,1) NOT NULL,
    id_carro                   INT             NOT NULL,
    id_medio                   INT             NOT NULL,
    monto                      DECIMAL(10,2)   NOT NULL,
    estado                     NVARCHAR(20)    NOT NULL CONSTRAINT DF_pago_estado DEFAULT (N'PENDIENTE'),
    fecha                      DATETIME2(0)    NOT NULL CONSTRAINT DF_pago_fecha DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_pago PRIMARY KEY CLUSTERED (id_transaccion),
    CONSTRAINT CK_pago_monto CHECK (monto > 0),   -- servicio 58, 59
    CONSTRAINT CK_pago_estado CHECK (estado IN (N'PENDIENTE', N'APROBADO', N'RECHAZADO', N'ANULADO'))   -- servicio 58, 59
);

-- Pago con tarjeta de crédito o débito autorizado por la pasarela; guarda solo los últimos dígitos, nunca la tarjeta completa.
CREATE TABLE pago.pago_tarjeta (
    id_transaccion             INT             NOT NULL,
    cuotas                     INT             NOT NULL CONSTRAINT DF_pago_tarjeta_cuotas DEFAULT (1),
    tipo_tarjeta               NVARCHAR(10)    NOT NULL,
    ultimos_digitos            VARCHAR(4)      NOT NULL,
    codigo_autorizacion        VARCHAR(20)     NULL,
    CONSTRAINT PK_pago_tarjeta PRIMARY KEY CLUSTERED (id_transaccion),
    CONSTRAINT CK_pago_tarjeta_cuotas CHECK (cuotas BETWEEN 1 AND 36),   -- servicio 58
    CONSTRAINT CK_pago_tarjeta_tipo_tarjeta CHECK (tipo_tarjeta IN (N'CREDITO', N'DEBITO', N'CMR')),   -- servicio 58
    CONSTRAINT CK_pago_tarjeta_digitos CHECK (ultimos_digitos LIKE '[0-9][0-9][0-9][0-9]')   -- servicio 58
);

-- Pago en efectivo mediante un código de pago con fecha de vencimiento.
CREATE TABLE pago.pago_efectivo (
    id_transaccion             INT             NOT NULL,
    codigo_pago                VARCHAR(20)     NOT NULL,
    fecha_vencimiento          DATETIME2(0)    NOT NULL,
    CONSTRAINT PK_pago_efectivo PRIMARY KEY CLUSTERED (id_transaccion),
    CONSTRAINT UQ_pago_efectivo_codigo UNIQUE (codigo_pago)   -- servicio 59
);

-- Orden de compra creada cuando el pago se aprueba (o queda pendiente si es en efectivo). Registra los CMR Puntos acumulados.
CREATE TABLE pago.orden (
    nro_orden                  VARCHAR(20)     NOT NULL,
    id_carro                   INT             NOT NULL,
    id_comprador               INT             NOT NULL,
    id_transaccion             INT             NOT NULL,
    estado                     NVARCHAR(20)    NOT NULL CONSTRAINT DF_orden_estado DEFAULT (N'REGISTRADA'),
    fecha_creacion             DATETIME2(0)    NOT NULL CONSTRAINT DF_orden_fecha_creacion DEFAULT (SYSDATETIME()),
    total                      DECIMAL(10,2)   NOT NULL,
    puntos_acumulados          INT             NOT NULL CONSTRAINT DF_orden_puntos_acumulados DEFAULT (0),
    CONSTRAINT PK_orden PRIMARY KEY CLUSTERED (nro_orden),
    CONSTRAINT UQ_orden_carro UNIQUE (id_carro),   -- servicio 60: un carro origina como máximo una orden
    CONSTRAINT UQ_orden_transaccion UNIQUE (id_transaccion),   -- servicio 60: un pago confirma como máximo una orden
    CONSTRAINT CK_orden_estado CHECK (estado IN (N'REGISTRADA', N'PENDIENTE_PAGO', N'PAGADA', N'ANULADA')),   -- servicio 59, 60
    CONSTRAINT CK_orden_montos CHECK (total >= 0 AND puntos_acumulados >= 0)   -- servicio 60
);

-- Correo enviado al comprador con la confirmación de su compra.
CREATE TABLE pago.notificacion_correo (
    id_notificacion            INT             IDENTITY(1,1) NOT NULL,
    nro_orden                  VARCHAR(20)     NOT NULL,
    correo                     NVARCHAR(100)   NOT NULL,
    asunto                     NVARCHAR(150)   NOT NULL,
    fecha_envio                DATETIME2(0)    NOT NULL CONSTRAINT DF_notificacion_correo_fecha_envio DEFAULT (SYSDATETIME()),
    estado                     NVARCHAR(20)    NOT NULL CONSTRAINT DF_notificacion_correo_estado DEFAULT (N'ENVIADO'),
    CONSTRAINT PK_notificacion_correo PRIMARY KEY CLUSTERED (id_notificacion),
    CONSTRAINT CK_notificacion_correo_estado CHECK (estado IN (N'ENVIADO', N'ERROR'))   -- servicio 61
);

/* ---- Búsqueda de Productos (esquema busqueda) ---- */

-- Tabla intermedia entre campaña y producto: qué productos promociona cada campaña (el servicio 5 devuelve datos de esos productos).
CREATE TABLE busqueda.campana_producto (
    id_campana                 INT             NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    CONSTRAINT PK_campana_producto PRIMARY KEY CLUSTERED (id_campana, sku)
);

-- Tabla intermedia entre selección y producto: qué productos forman cada selección o colección.
CREATE TABLE busqueda.seleccion_producto (
    id_seleccion               INT             NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    CONSTRAINT PK_seleccion_producto PRIMARY KEY CLUSTERED (id_seleccion, sku)
);

-- Precio vigente de un producto en una zona: precio normal, de oferta, con tarjeta CMR, % de descuento, etiqueta del evento y precio por volumen.
CREATE TABLE busqueda.precio (
    id_precio                  INT             IDENTITY(1,1) NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    id_zona                    INT             NOT NULL,
    id_cupon                   INT             NULL,
    precio_normal              DECIMAL(10,2)   NOT NULL,
    precio_oferta              DECIMAL(10,2)   NULL,
    precio_cmr                 DECIMAL(10,2)   NULL,
    porcentaje_descuento       DECIMAL(10,2)   NULL,
    etiqueta_evento            NVARCHAR(60)    NULL,
    precio_volumen             DECIMAL(10,2)   NULL,
    cantidad_min_volumen       INT             NULL,
    vigente_desde              DATE            NOT NULL,
    vigente_hasta              DATE            NULL,
    CONSTRAINT PK_precio PRIMARY KEY CLUSTERED (id_precio),
    CONSTRAINT CK_precio_montos CHECK (precio_normal >= 0 AND (precio_oferta IS NULL OR precio_oferta BETWEEN 0 AND precio_normal) AND (precio_cmr IS NULL OR precio_cmr >= 0)),   -- servicio 23
    CONSTRAINT CK_precio_descuento CHECK (porcentaje_descuento IS NULL OR porcentaje_descuento BETWEEN 0 AND 100),   -- servicio 23
    CONSTRAINT CK_precio_volumen CHECK ((precio_volumen IS NULL AND cantidad_min_volumen IS NULL) OR (precio_volumen >= 0 AND cantidad_min_volumen > 1)),   -- servicio 43
    CONSTRAINT CK_precio_vigencia CHECK (vigente_hasta IS NULL OR vigente_hasta >= vigente_desde)   -- servicio 23
);

-- Opinión publicada por un cliente sobre un producto, con su calificación y fecha.
CREATE TABLE busqueda.resena (
    id_resena                  INT             IDENTITY(1,1) NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    id_cliente                 INT             NOT NULL,
    calificacion               INT             NOT NULL,
    comentario                 NVARCHAR(MAX)   NULL,
    fecha                      DATETIME2(0)    NOT NULL CONSTRAINT DF_resena_fecha DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_resena PRIMARY KEY CLUSTERED (id_resena),
    CONSTRAINT CK_resena_calificacion CHECK (calificacion BETWEEN 1 AND 5)   -- servicio 29
);

-- Stock de un producto en una tienda y tiempo estimado de retiro. Responde «Consultar stock por tienda» y el filtro por stock.
CREATE TABLE busqueda.stock_tienda (
    sku                        VARCHAR(20)     NOT NULL,
    id_tienda                  INT             NOT NULL,
    disponible                 BIT             NOT NULL CONSTRAINT DF_stock_tienda_disponible DEFAULT (1),
    cantidad                   INT             NOT NULL CONSTRAINT DF_stock_tienda_cantidad DEFAULT (0),
    tiempo_retiro_min          INT             NOT NULL,
    CONSTRAINT PK_stock_tienda PRIMARY KEY CLUSTERED (sku, id_tienda),
    CONSTRAINT CK_stock_tienda_cantidad CHECK (cantidad >= 0 AND tiempo_retiro_min >= 0)   -- servicio 28
);

-- Modalidad de entrega disponible para un producto en una zona, con su plazo y costo (ej. «Llega hoy», «Retira desde 120 min»).
CREATE TABLE busqueda.entrega_producto_zona (
    id_entrega_zona            INT             IDENTITY(1,1) NOT NULL,
    sku                        VARCHAR(20)     NOT NULL,
    id_zona                    INT             NOT NULL,
    id_tipo_entrega            INT             NOT NULL,
    plazo                      NVARCHAR(40)    NOT NULL,
    costo                      DECIMAL(10,2)   NOT NULL,
    CONSTRAINT PK_entrega_producto_zona PRIMARY KEY CLUSTERED (id_entrega_zona),
    CONSTRAINT UQ_entrega_producto_zona_producto_zona_tipo UNIQUE (sku, id_zona, id_tipo_entrega),   -- servicio 24
    CONSTRAINT CK_entrega_producto_zona_costo CHECK (costo >= 0)   -- servicio 24
);

/* ---- Proceso de Compra (esquema compra) ---- */

-- Boleta o factura electrónica: se registra el tipo en el checkout y se emite cuando se crea la orden.
CREATE TABLE compra.comprobante (
    id_comprobante             INT             IDENTITY(1,1) NOT NULL,
    id_carro                   INT             NOT NULL,
    nro_orden                  VARCHAR(20)     NULL,
    tipo                       NVARCHAR(10)    NOT NULL,
    nro_documento_ruc          VARCHAR(15)     NOT NULL,
    razon_social               NVARCHAR(120)   NULL,
    direccion_fiscal           NVARCHAR(150)   NULL,
    serie                      VARCHAR(10)     NULL,
    numero                     VARCHAR(15)     NULL,
    archivo_pdf                NVARCHAR(255)   NULL,
    fecha_emision              DATETIME2(0)    NULL,
    CONSTRAINT PK_comprobante PRIMARY KEY CLUSTERED (id_comprobante),
    CONSTRAINT UQ_comprobante_carro UNIQUE (id_carro),   -- servicio 55: un comprobante por carro
    CONSTRAINT CK_comprobante_tipo CHECK (tipo IN (N'BOLETA', N'FACTURA')),   -- servicio 55
    CONSTRAINT CK_comprobante_factura CHECK (tipo = N'BOLETA' OR (LEN(nro_documento_ruc) = 11 AND nro_documento_ruc NOT LIKE '%[^0-9]%' AND razon_social IS NOT NULL AND direccion_fiscal IS NOT NULL))   -- servicio 55
);
GO

/* 5. Claves foráneas ----------------------------------------------------- */
ALTER TABLE busqueda.categoria ADD CONSTRAINT FK_categoria_categoria FOREIGN KEY (id_categoria_padre) REFERENCES busqueda.categoria (id_categoria);
ALTER TABLE busqueda.marca_categoria ADD CONSTRAINT FK_marca_categoria_marca FOREIGN KEY (id_marca) REFERENCES busqueda.marca (id_marca);
ALTER TABLE busqueda.marca_categoria ADD CONSTRAINT FK_marca_categoria_categoria FOREIGN KEY (id_categoria) REFERENCES busqueda.categoria (id_categoria);
ALTER TABLE busqueda.pregunta_frecuente ADD CONSTRAINT FK_pregunta_frecuente_categoria FOREIGN KEY (id_categoria) REFERENCES busqueda.categoria (id_categoria);
ALTER TABLE busqueda.pregunta_asistente ADD CONSTRAINT FK_pregunta_asistente_categoria FOREIGN KEY (id_categoria) REFERENCES busqueda.categoria (id_categoria);
ALTER TABLE busqueda.opcion_asistente ADD CONSTRAINT FK_opcion_asistente_pregunta_asistente FOREIGN KEY (id_pregunta_asistente) REFERENCES busqueda.pregunta_asistente (id_pregunta_asistente) ON DELETE CASCADE;
ALTER TABLE busqueda.campana_producto ADD CONSTRAINT FK_campana_producto_campana FOREIGN KEY (id_campana) REFERENCES busqueda.campana (id_campana);
ALTER TABLE busqueda.campana_producto ADD CONSTRAINT FK_campana_producto_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.seleccion_producto ADD CONSTRAINT FK_seleccion_producto_seleccion FOREIGN KEY (id_seleccion) REFERENCES busqueda.seleccion (id_seleccion);
ALTER TABLE busqueda.seleccion_producto ADD CONSTRAINT FK_seleccion_producto_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.producto ADD CONSTRAINT FK_producto_marca FOREIGN KEY (id_marca) REFERENCES busqueda.marca (id_marca);
ALTER TABLE busqueda.producto ADD CONSTRAINT FK_producto_categoria FOREIGN KEY (id_categoria) REFERENCES busqueda.categoria (id_categoria);
ALTER TABLE busqueda.producto ADD CONSTRAINT FK_producto_vendedor FOREIGN KEY (id_vendedor) REFERENCES busqueda.vendedor (id_vendedor);
ALTER TABLE busqueda.precio ADD CONSTRAINT FK_precio_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.precio ADD CONSTRAINT FK_precio_zona FOREIGN KEY (id_zona) REFERENCES busqueda.zona (id_zona);
ALTER TABLE busqueda.precio ADD CONSTRAINT FK_precio_cupon FOREIGN KEY (id_cupon) REFERENCES carro.cupon (id_cupon);
ALTER TABLE busqueda.atributo_producto ADD CONSTRAINT FK_atributo_producto_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.resena ADD CONSTRAINT FK_resena_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.resena ADD CONSTRAINT FK_resena_cliente FOREIGN KEY (id_cliente) REFERENCES cuenta.cliente (id_cliente);
ALTER TABLE busqueda.producto_relacionado ADD CONSTRAINT FK_producto_relacionado_producto_sku FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.producto_relacionado ADD CONSTRAINT FK_producto_relacionado_producto_sku_relacionado FOREIGN KEY (sku_relacionado) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.producto_patrocinado ADD CONSTRAINT FK_producto_patrocinado_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.producto_patrocinado ADD CONSTRAINT FK_producto_patrocinado_categoria FOREIGN KEY (id_categoria) REFERENCES busqueda.categoria (id_categoria);
ALTER TABLE busqueda.stock_tienda ADD CONSTRAINT FK_stock_tienda_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.stock_tienda ADD CONSTRAINT FK_stock_tienda_tienda FOREIGN KEY (id_tienda) REFERENCES compra.tienda (id_tienda);
ALTER TABLE busqueda.entrega_producto_zona ADD CONSTRAINT FK_entrega_producto_zona_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE busqueda.entrega_producto_zona ADD CONSTRAINT FK_entrega_producto_zona_zona FOREIGN KEY (id_zona) REFERENCES busqueda.zona (id_zona);
ALTER TABLE busqueda.entrega_producto_zona ADD CONSTRAINT FK_entrega_producto_zona_tipo_entrega FOREIGN KEY (id_tipo_entrega) REFERENCES compra.tipo_entrega (id_tipo_entrega);
ALTER TABLE busqueda.provincia ADD CONSTRAINT FK_provincia_departamento FOREIGN KEY (id_departamento) REFERENCES busqueda.departamento (id_departamento);
ALTER TABLE busqueda.distrito ADD CONSTRAINT FK_distrito_provincia FOREIGN KEY (id_provincia) REFERENCES busqueda.provincia (id_provincia);
ALTER TABLE busqueda.zona ADD CONSTRAINT FK_zona_distrito FOREIGN KEY (id_distrito) REFERENCES busqueda.distrito (id_distrito);
ALTER TABLE compra.tienda ADD CONSTRAINT FK_tienda_distrito FOREIGN KEY (id_distrito) REFERENCES busqueda.distrito (id_distrito);
ALTER TABLE compra.horario_despacho ADD CONSTRAINT FK_horario_despacho_zona FOREIGN KEY (id_zona) REFERENCES busqueda.zona (id_zona);
ALTER TABLE cuenta.sesion ADD CONSTRAINT FK_sesion_cliente FOREIGN KEY (id_cliente) REFERENCES cuenta.cliente (id_cliente);
ALTER TABLE cuenta.token_recuperacion ADD CONSTRAINT FK_token_recuperacion_cliente FOREIGN KEY (id_cliente) REFERENCES cuenta.cliente (id_cliente);
ALTER TABLE cuenta.direccion ADD CONSTRAINT FK_direccion_cliente FOREIGN KEY (id_cliente) REFERENCES cuenta.cliente (id_cliente);
ALTER TABLE cuenta.direccion ADD CONSTRAINT FK_direccion_distrito FOREIGN KEY (id_distrito) REFERENCES busqueda.distrito (id_distrito);
ALTER TABLE carro.carro ADD CONSTRAINT FK_carro_cliente FOREIGN KEY (id_cliente) REFERENCES cuenta.cliente (id_cliente);
ALTER TABLE carro.carro ADD CONSTRAINT FK_carro_cupon FOREIGN KEY (id_cupon) REFERENCES carro.cupon (id_cupon);
ALTER TABLE carro.item_carro ADD CONSTRAINT FK_item_carro_carro FOREIGN KEY (id_carro) REFERENCES carro.carro (id_carro) ON DELETE CASCADE;
ALTER TABLE carro.item_carro ADD CONSTRAINT FK_item_carro_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE carro.producto_servicio ADD CONSTRAINT FK_producto_servicio_producto FOREIGN KEY (sku) REFERENCES busqueda.producto (sku);
ALTER TABLE carro.producto_servicio ADD CONSTRAINT FK_producto_servicio_servicio_adicional FOREIGN KEY (id_servicio) REFERENCES carro.servicio_adicional (id_servicio);
ALTER TABLE carro.producto_servicio ADD CONSTRAINT FK_producto_servicio_zona FOREIGN KEY (id_zona) REFERENCES busqueda.zona (id_zona);
ALTER TABLE carro.item_servicio ADD CONSTRAINT FK_item_servicio_item_carro FOREIGN KEY (id_item) REFERENCES carro.item_carro (id_item) ON DELETE CASCADE;
ALTER TABLE carro.item_servicio ADD CONSTRAINT FK_item_servicio_servicio_adicional FOREIGN KEY (id_servicio) REFERENCES carro.servicio_adicional (id_servicio);
ALTER TABLE compra.comprador ADD CONSTRAINT FK_comprador_carro FOREIGN KEY (id_carro) REFERENCES carro.carro (id_carro);
ALTER TABLE compra.comprador ADD CONSTRAINT FK_comprador_cliente FOREIGN KEY (id_cliente) REFERENCES cuenta.cliente (id_cliente);
ALTER TABLE compra.entrega ADD CONSTRAINT FK_entrega_carro FOREIGN KEY (id_carro) REFERENCES carro.carro (id_carro);
ALTER TABLE compra.entrega ADD CONSTRAINT FK_entrega_tipo_entrega FOREIGN KEY (id_tipo_entrega) REFERENCES compra.tipo_entrega (id_tipo_entrega);
ALTER TABLE compra.entrega ADD CONSTRAINT FK_entrega_direccion FOREIGN KEY (id_direccion) REFERENCES cuenta.direccion (id_direccion) ON DELETE SET NULL;
ALTER TABLE compra.entrega ADD CONSTRAINT FK_entrega_tienda FOREIGN KEY (id_tienda) REFERENCES compra.tienda (id_tienda);
ALTER TABLE compra.entrega ADD CONSTRAINT FK_entrega_horario_despacho FOREIGN KEY (id_horario) REFERENCES compra.horario_despacho (id_horario);
ALTER TABLE compra.comprobante ADD CONSTRAINT FK_comprobante_carro FOREIGN KEY (id_carro) REFERENCES carro.carro (id_carro);
ALTER TABLE compra.comprobante ADD CONSTRAINT FK_comprobante_orden FOREIGN KEY (nro_orden) REFERENCES pago.orden (nro_orden);
ALTER TABLE pago.pago ADD CONSTRAINT FK_pago_carro FOREIGN KEY (id_carro) REFERENCES carro.carro (id_carro);
ALTER TABLE pago.pago ADD CONSTRAINT FK_pago_medio_pago FOREIGN KEY (id_medio) REFERENCES compra.medio_pago (id_medio);
ALTER TABLE pago.pago_tarjeta ADD CONSTRAINT FK_pago_tarjeta_pago FOREIGN KEY (id_transaccion) REFERENCES pago.pago (id_transaccion) ON DELETE CASCADE;
ALTER TABLE pago.pago_efectivo ADD CONSTRAINT FK_pago_efectivo_pago FOREIGN KEY (id_transaccion) REFERENCES pago.pago (id_transaccion) ON DELETE CASCADE;
ALTER TABLE pago.orden ADD CONSTRAINT FK_orden_carro FOREIGN KEY (id_carro) REFERENCES carro.carro (id_carro);
ALTER TABLE pago.orden ADD CONSTRAINT FK_orden_comprador FOREIGN KEY (id_comprador) REFERENCES compra.comprador (id_comprador);
ALTER TABLE pago.orden ADD CONSTRAINT FK_orden_pago FOREIGN KEY (id_transaccion) REFERENCES pago.pago (id_transaccion);
ALTER TABLE pago.notificacion_correo ADD CONSTRAINT FK_notificacion_correo_orden FOREIGN KEY (nro_orden) REFERENCES pago.orden (nro_orden);
GO

/* 6. Índices ------------------------------------------------------------- */
-- Índices sobre claves foráneas que no encabezan la clave primaria ni un índice único
CREATE NONCLUSTERED INDEX IX_categoria_id_categoria_padre ON busqueda.categoria (id_categoria_padre);
CREATE NONCLUSTERED INDEX IX_marca_categoria_id_categoria ON busqueda.marca_categoria (id_categoria);
CREATE NONCLUSTERED INDEX IX_pregunta_frecuente_id_categoria ON busqueda.pregunta_frecuente (id_categoria);
CREATE NONCLUSTERED INDEX IX_pregunta_asistente_id_categoria ON busqueda.pregunta_asistente (id_categoria);
CREATE NONCLUSTERED INDEX IX_opcion_asistente_id_pregunta_asistente ON busqueda.opcion_asistente (id_pregunta_asistente);
CREATE NONCLUSTERED INDEX IX_campana_producto_sku ON busqueda.campana_producto (sku);
CREATE NONCLUSTERED INDEX IX_seleccion_producto_sku ON busqueda.seleccion_producto (sku);
CREATE NONCLUSTERED INDEX IX_producto_id_marca ON busqueda.producto (id_marca);
CREATE NONCLUSTERED INDEX IX_producto_id_categoria ON busqueda.producto (id_categoria);
CREATE NONCLUSTERED INDEX IX_producto_id_vendedor ON busqueda.producto (id_vendedor);
CREATE NONCLUSTERED INDEX IX_precio_sku ON busqueda.precio (sku);
CREATE NONCLUSTERED INDEX IX_precio_id_zona ON busqueda.precio (id_zona);
CREATE NONCLUSTERED INDEX IX_precio_id_cupon ON busqueda.precio (id_cupon);
CREATE NONCLUSTERED INDEX IX_atributo_producto_sku ON busqueda.atributo_producto (sku);
CREATE NONCLUSTERED INDEX IX_resena_sku ON busqueda.resena (sku);
CREATE NONCLUSTERED INDEX IX_resena_id_cliente ON busqueda.resena (id_cliente);
CREATE NONCLUSTERED INDEX IX_producto_relacionado_sku_relacionado ON busqueda.producto_relacionado (sku_relacionado);
CREATE NONCLUSTERED INDEX IX_producto_patrocinado_sku ON busqueda.producto_patrocinado (sku);
CREATE NONCLUSTERED INDEX IX_producto_patrocinado_id_categoria ON busqueda.producto_patrocinado (id_categoria);
CREATE NONCLUSTERED INDEX IX_stock_tienda_id_tienda ON busqueda.stock_tienda (id_tienda);
CREATE NONCLUSTERED INDEX IX_entrega_producto_zona_id_zona ON busqueda.entrega_producto_zona (id_zona);
CREATE NONCLUSTERED INDEX IX_entrega_producto_zona_id_tipo_entrega ON busqueda.entrega_producto_zona (id_tipo_entrega);
CREATE NONCLUSTERED INDEX IX_provincia_id_departamento ON busqueda.provincia (id_departamento);
CREATE NONCLUSTERED INDEX IX_distrito_id_provincia ON busqueda.distrito (id_provincia);
CREATE NONCLUSTERED INDEX IX_tienda_id_distrito ON compra.tienda (id_distrito);
CREATE NONCLUSTERED INDEX IX_horario_despacho_id_zona ON compra.horario_despacho (id_zona);
CREATE NONCLUSTERED INDEX IX_sesion_id_cliente ON cuenta.sesion (id_cliente);
CREATE NONCLUSTERED INDEX IX_token_recuperacion_id_cliente ON cuenta.token_recuperacion (id_cliente);
CREATE NONCLUSTERED INDEX IX_direccion_id_distrito ON cuenta.direccion (id_distrito);
CREATE NONCLUSTERED INDEX IX_carro_id_cliente ON carro.carro (id_cliente);
CREATE NONCLUSTERED INDEX IX_carro_id_cupon ON carro.carro (id_cupon);
CREATE NONCLUSTERED INDEX IX_item_carro_id_carro ON carro.item_carro (id_carro);
CREATE NONCLUSTERED INDEX IX_item_carro_sku ON carro.item_carro (sku);
CREATE NONCLUSTERED INDEX IX_producto_servicio_id_servicio ON carro.producto_servicio (id_servicio);
CREATE NONCLUSTERED INDEX IX_producto_servicio_id_zona ON carro.producto_servicio (id_zona);
CREATE NONCLUSTERED INDEX IX_item_servicio_id_item ON carro.item_servicio (id_item);
CREATE NONCLUSTERED INDEX IX_item_servicio_id_servicio ON carro.item_servicio (id_servicio);
CREATE NONCLUSTERED INDEX IX_comprador_id_cliente ON compra.comprador (id_cliente);
CREATE NONCLUSTERED INDEX IX_entrega_id_carro ON compra.entrega (id_carro);
CREATE NONCLUSTERED INDEX IX_entrega_id_tipo_entrega ON compra.entrega (id_tipo_entrega);
CREATE NONCLUSTERED INDEX IX_entrega_id_direccion ON compra.entrega (id_direccion);
CREATE NONCLUSTERED INDEX IX_entrega_id_tienda ON compra.entrega (id_tienda);
CREATE NONCLUSTERED INDEX IX_entrega_id_horario ON compra.entrega (id_horario);
CREATE NONCLUSTERED INDEX IX_pago_id_carro ON pago.pago (id_carro);
CREATE NONCLUSTERED INDEX IX_pago_id_medio ON pago.pago (id_medio);
CREATE NONCLUSTERED INDEX IX_orden_id_comprador ON pago.orden (id_comprador);
CREATE NONCLUSTERED INDEX IX_notificacion_correo_nro_orden ON pago.notificacion_correo (nro_orden);

-- Índices únicos filtrados (reglas de negocio condicionales)
CREATE UNIQUE NONCLUSTERED INDEX UX_direccion_principal ON cuenta.direccion (id_cliente) WHERE principal = 1;   -- servicio 40: solo una dirección principal por cliente
CREATE UNIQUE NONCLUSTERED INDEX UX_comprobante_serie_numero ON compra.comprobante (serie, numero) WHERE serie IS NOT NULL AND numero IS NOT NULL;   -- servicio 62
CREATE UNIQUE NONCLUSTERED INDEX UX_comprobante_orden ON compra.comprobante (nro_orden) WHERE nro_orden IS NOT NULL;   -- servicio 62: un comprobante por orden

-- Índices de apoyo a las búsquedas más frecuentes
CREATE NONCLUSTERED INDEX IX_producto_nombre ON busqueda.producto (nombre) INCLUDE (activo);                       -- servicios 4 y 8
CREATE NONCLUSTERED INDEX IX_producto_populares ON busqueda.producto (unidades_vendidas DESC, visitas DESC) WHERE activo = 1;   -- servicio 7
CREATE NONCLUSTERED INDEX IX_campana_vigencia ON busqueda.campana (fecha_inicio, fecha_fin);                         -- servicio 5
CREATE NONCLUSTERED INDEX IX_precio_vigencia ON busqueda.precio (sku, id_zona, vigente_desde);                       -- servicio 23
CREATE NONCLUSTERED INDEX IX_horario_zona_fecha ON compra.horario_despacho (id_zona, fecha);                         -- servicio 51
GO

/* 7. Descripciones (propiedades extendidas MS_Description) --------------- */
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Agrupa los productos del catálogo en una jerarquía de categorías y subcategorías (ej. Herramientas > Herramientas eléctricas > Taladros). Atiende el menú «Categorías», la ruta de navegación, las categorías relacionadas y el contenido informativo de cada categoría.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'categoria';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Marca o fabricante de los productos (ej. Bauker, Bosch). Lista las marcas destacadas de una categoría en el menú y los productos de una marca.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'marca';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Tabla intermedia que resuelve la relación muchos a muchos entre marca y categoría: qué marcas se destacan en cada categoría del menú.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'marca_categoria';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Pregunta y respuesta que forma parte del contenido informativo al final del listado de una categoría.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'pregunta_frecuente';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Pregunta del asistente de búsqueda guiada de una categoría (ej. «¿Qué tipo de taladro prefieres?»).', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'pregunta_asistente';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Opción de respuesta de una pregunta del asistente; cada opción tiene el filtro que se aplica al elegirla.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'opcion_asistente';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Campaña u oferta destacada de la página de inicio (ej. «Día del Shopping»). Solo se listan las vigentes; tipoDestino e idDestino indican si al hacer clic se abre una categoría o una selección.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'campana';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Colección de productos armada por el área comercial (ej. «Lo más vendido», «Precio x volumen»).', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'seleccion';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Quién vende el producto: Sodimac o un vendedor tercero del marketplace. Se usa en las tarjetas de producto y en el filtro «Vendido por».', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'vendedor';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Artículo que se vende, identificado por su SKU. Es la clase central de la búsqueda: autocompletado, productos populares, búsqueda por palabra clave, listado por categoría, filtros, orden, ficha de detalle y productos relacionados.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'producto';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Atributo técnico de un producto (ej. Potencia, Voltaje). Forma las especificaciones de la ficha y los filtros por atributos.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'atributo_producto';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Tabla intermedia que relaciona un producto consigo mismo: para cada SKU, los SKU similares o complementarios (tipo).', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'producto_relacionado';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Producto promocionado por un anunciante que se intercala en los resultados con la etiqueta «Patrocinado».', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'producto_patrocinado';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Primer nivel de la ubicación geográfica (ubigeo).', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'departamento';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Segundo nivel de la ubicación; pertenece a un departamento.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'provincia';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Tercer nivel de la ubicación; pertenece a una provincia y ubica zonas, tiendas y direcciones.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'distrito';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Zona de cobertura de un distrito. Se obtiene al registrar la ubicación de entrega y condiciona precios, entregas, horarios y servicios.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'zona';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Tienda Sodimac o Maestro donde el cliente puede retirar su pedido.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'tienda';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Catálogo de modalidades de entrega: envío a domicilio, envío express y retiro en tienda.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'tipo_entrega';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Fecha y rango horario de despacho con capacidad disponible para una zona.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'horario_despacho';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Persona registrada en la tienda. Registra su cuenta, inicia sesión y consulta o actualiza sus datos. Guarda los intentos fallidos para el bloqueo temporal.', @level0type = N'SCHEMA', @level0name = N'cuenta', @level1type = N'TABLE', @level1name = N'cliente';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Sesión iniciada por un cliente, identificada por un token con fecha de expiración. Permite cerrar sesión.', @level0type = N'SCHEMA', @level0name = N'cuenta', @level1type = N'TABLE', @level1name = N'sesion';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Token temporal enviado al correo para recuperar y restablecer la contraseña; tiene vigencia y no puede reutilizarse.', @level0type = N'SCHEMA', @level0name = N'cuenta', @level1type = N'TABLE', @level1name = N'token_recuperacion';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Dirección de entrega registrada por un cliente; una de ellas es la principal.', @level0type = N'SCHEMA', @level0name = N'cuenta', @level1type = N'TABLE', @level1name = N'direccion';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Cupón de descuento con sus condiciones: monto mínimo, medio de pago requerido y vigencia (ej. CMR10).', @level0type = N'SCHEMA', @level0name = N'carro', @level1type = N'TABLE', @level1name = N'cupon';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Carro de compras de un cliente o de un invitado. Agrega productos, muestra el carro, une el carro de invitado al iniciar sesión y entrega el resumen de compra.', @level0type = N'SCHEMA', @level0name = N'carro', @level1type = N'TABLE', @level1name = N'carro';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Línea del carro: un producto con su cantidad, precio unitario y subtotal. Permite actualizar la cantidad o eliminar el producto.', @level0type = N'SCHEMA', @level0name = N'carro', @level1type = N'TABLE', @level1name = N'item_carro';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Servicio que se puede contratar con un producto (ej. armado, instalación, garantía extendida).', @level0type = N'SCHEMA', @level0name = N'carro', @level1type = N'TABLE', @level1name = N'servicio_adicional';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Clase de asociación que indica si un servicio adicional está disponible para un producto en una zona.', @level0type = N'SCHEMA', @level0name = N'carro', @level1type = N'TABLE', @level1name = N'producto_servicio';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Servicio adicional agregado a una línea del carro. Si se elimina el producto, se elimina también el servicio.', @level0type = N'SCHEMA', @level0name = N'carro', @level1type = N'TABLE', @level1name = N'item_servicio';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Datos de quien compra (cliente registrado o invitado) registrados en el checkout.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'comprador';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Entrega elegida para el carro: despacho a domicilio (dirección, fecha y horario) o retiro en tienda (tienda y persona que retira). Un carro puede tener varias entregas si los productos llegan en envíos separados.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'entrega';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Medio de pago aceptado (tarjeta CMR, crédito, débito, pago en efectivo, Gift Card) con las cuotas disponibles.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'medio_pago';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Clase abstracta con los datos comunes de una transacción de pago: monto, estado y fecha.', @level0type = N'SCHEMA', @level0name = N'pago', @level1type = N'TABLE', @level1name = N'pago';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Pago con tarjeta de crédito o débito autorizado por la pasarela; guarda solo los últimos dígitos, nunca la tarjeta completa.', @level0type = N'SCHEMA', @level0name = N'pago', @level1type = N'TABLE', @level1name = N'pago_tarjeta';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Pago en efectivo mediante un código de pago con fecha de vencimiento.', @level0type = N'SCHEMA', @level0name = N'pago', @level1type = N'TABLE', @level1name = N'pago_efectivo';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Orden de compra creada cuando el pago se aprueba (o queda pendiente si es en efectivo). Registra los CMR Puntos acumulados.', @level0type = N'SCHEMA', @level0name = N'pago', @level1type = N'TABLE', @level1name = N'orden';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Correo enviado al comprador con la confirmación de su compra.', @level0type = N'SCHEMA', @level0name = N'pago', @level1type = N'TABLE', @level1name = N'notificacion_correo';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Tabla intermedia entre campaña y producto: qué productos promociona cada campaña (el servicio 5 devuelve datos de esos productos).', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'campana_producto';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Tabla intermedia entre selección y producto: qué productos forman cada selección o colección.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'seleccion_producto';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Precio vigente de un producto en una zona: precio normal, de oferta, con tarjeta CMR, % de descuento, etiqueta del evento y precio por volumen.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'precio';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Opinión publicada por un cliente sobre un producto, con su calificación y fecha.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'resena';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Stock de un producto en una tienda y tiempo estimado de retiro. Responde «Consultar stock por tienda» y el filtro por stock.', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'stock_tienda';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Modalidad de entrega disponible para un producto en una zona, con su plazo y costo (ej. «Llega hoy», «Retira desde 120 min»).', @level0type = N'SCHEMA', @level0name = N'busqueda', @level1type = N'TABLE', @level1name = N'entrega_producto_zona';
EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'Boleta o factura electrónica: se registra el tipo en el checkout y se emite cuando se crea la orden.', @level0type = N'SCHEMA', @level0name = N'compra', @level1type = N'TABLE', @level1name = N'comprobante';
GO

/* 8. Verificación --------------------------------------------------------- */
SELECT s.name AS esquema, COUNT(*) AS tablas
FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE s.name IN (N'busqueda', N'cuenta', N'carro', N'compra', N'pago')
GROUP BY s.name ORDER BY s.name;
-- Se esperan 45 tablas: busqueda 23, cuenta 4, carro 6, compra 7, pago 5.
SELECT COUNT(*) AS claves_foraneas FROM sys.foreign_keys;   -- Se esperan 63.
GO
