# Sodimac API y MongoDB

## Conexión local

La API usa MongoDB en `mongodb://localhost:27017` y la base `ecommerce_sodimac`.
Puedes cambiar esos valores en `appsettings.json` o con las variables de entorno
`MongoDB__ConnectionString` y `MongoDB__DatabaseName`.

En ambiente Development se insertan datos demo si faltan. Los documentos con esos
identificadores no se sobrescriben. El carro demo es `1`; también se usan la
dirección `1`, la zona `1`, la tienda `1` y los medios de pago `1` y `2`.
En otros ambientes la carga demo está desactivada.

## Probar

Desde la carpeta del proyecto:

```powershell
dotnet run --launch-profile http
```

Rutas de prueba:

- `GET /api/checkout/resumen?idCarro=1`
- `GET /api/checkout/medios-pago`
- `GET /api/entrega/fechas-horarios?idCarro=1&idDireccion=1`
- `GET /api/entrega/tiendas-retiro?idZona=1&idCarro=1`

El archivo SQL proporcionado contiene la definición de tablas, pero no filas
para importar. Los datos demo cubren las rutas actuales de checkout, pagos y
entregas; no representan una migración de registros SQL existentes. Las rutas
de cuenta y carrito implementadas por `ClienteCarroService` siguen usando su
almacenamiento en memoria.
