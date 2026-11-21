using MongoDB.Bson;
using MongoDB.Driver;
using Sodimac.Api.Models;

namespace Sodimac.Api.Data;

public static class MongoDemoDataSeeder
{
    public static async Task SeedAsync(MongoDataContext mongo)
    {
        await SeedIfMissingAsync(
            mongo.Collection<Carro>("carros"),
            1,
            new Carro
            {
                IdCarro = 1,
                EsInvitado = true,
                FechaCreacion = DateTime.UtcNow,
                Subtotal = 149.90m,
                Descuentos = 0,
                Total = 149.90m
            });

        await SeedIfMissingAsync(
            mongo.Collection<ItemCarro>("itemsCarro"),
            1,
            new ItemCarro
            {
                IdItem = 1,
                IdCarro = 1,
                Sku = "SKU-DEMO-001",
                Cantidad = 1,
                PrecioUnitario = 149.90m,
                Subtotal = 149.90m
            });

        await SeedIfMissingAsync(
            mongo.Collection<ItemServicio>("itemsServicio"),
            1,
            new ItemServicio { IdItemServicio = 1, IdItem = 1, IdServicio = 1, Precio = 49.90m });
        await SeedIfMissingAsync(
            mongo.Collection<ServicioAdicional>("serviciosAdicionales"),
            1,
            new ServicioAdicional { IdServicio = 1, Nombre = "Armado de producto", Precio = 49.90m });

        await SeedIfMissingAsync(
            mongo.Collection<MedioPago>("mediosPago"),
            1,
            new MedioPago { IdMedio = 1, Nombre = "Tarjeta de crédito", CuotasDisponibles = "1,3,6,12" });
        await SeedIfMissingAsync(
            mongo.Collection<MedioPago>("mediosPago"),
            2,
            new MedioPago { IdMedio = 2, Nombre = "Efectivo", CuotasDisponibles = null });

        await SeedIfMissingAsync(
            mongo.Collection<Distrito>("distritos"),
            1,
            new Distrito { IdDistrito = 1, IdProvincia = 1, Nombre = "Miraflores" });
        await SeedIfMissingAsync(
            mongo.Collection<Zona>("zonas"),
            1,
            new Zona { IdZona = 1, IdDistrito = 1, Nombre = "Lima Centro", TieneCobertura = true });
        await SeedIfMissingAsync(
            mongo.Collection<Direccion>("direcciones"),
            1,
            new Direccion
            {
                IdDireccion = 1,
                IdCliente = 1,
                IdDistrito = 1,
                Nombre = "Casa",
                Direccion1 = "Av. Demo 123",
                Referencia = "Dato de prueba",
                Principal = true
            });
        await SeedIfMissingAsync(
            mongo.Collection<HorarioDespacho>("horariosDespacho"),
            1,
            new HorarioDespacho
            {
                IdHorario = 1,
                IdZona = 1,
                Fecha = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
                RangoHorario = "09:00 - 13:00",
                Capacidad = 10,
                Costo = 0
            });
        await SeedIfMissingAsync(
            mongo.Collection<Tienda>("tiendas"),
            1,
            new Tienda
            {
                IdTienda = 1,
                IdDistrito = 1,
                Nombre = "Sodimac Demo Miraflores",
                Tipo = "SODIMAC",
                Direccion = "Av. Demo 123",
                Horario = "Lunes a sábado, 9:00 - 21:00"
            });

        var stock = mongo.Collection<StockTienda>("stockTiendas");
        var stockFilter = Builders<StockTienda>.Filter.Eq(item => item.Sku, "SKU-DEMO-001")
            & Builders<StockTienda>.Filter.Eq(item => item.IdTienda, 1);
        if (!await stock.Find(stockFilter).AnyAsync())
        {
            await stock.InsertOneAsync(new StockTienda
            {
                Sku = "SKU-DEMO-001",
                IdTienda = 1,
                Disponible = true,
                Cantidad = 5,
                TiempoRetiroMin = 120
            });
        }

        await mongo.InitializeSequenceAsync("pagos", 0);
        await mongo.InitializeSequenceAsync("entregas", 0);
        await mongo.InitializeSequenceAsync("comprobantes", 0);
        await mongo.InitializeSequenceAsync("notificaciones", 0);
        await mongo.InitializeSequenceAsync("ordenes", 0);
    }

    private static async Task SeedIfMissingAsync<T>(
        IMongoCollection<T> collection,
        object id,
        T document) where T : class
    {
        var filter = Builders<T>.Filter.Eq("_id", id);
        if (!await collection.Find(filter).AnyAsync())
        {
            await collection.InsertOneAsync(document);
        }
    }
}
