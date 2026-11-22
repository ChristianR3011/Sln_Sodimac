using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Sodimac.Api.Models;

namespace Sodimac.Api.Data;

public sealed class MongoDataContext
{
    private readonly IMongoCollection<BsonDocument> _sequences;

    public MongoDataContext(IConfiguration configuration)
    {
        RegisterDocumentMappings();

        var connectionString = configuration["MongoDB:ConnectionString"]
            ?? throw new InvalidOperationException("Falta configurar MongoDB:ConnectionString.");
        var databaseName = configuration["MongoDB:DatabaseName"]
            ?? throw new InvalidOperationException("Falta configurar MongoDB:DatabaseName.");

        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
        _sequences = Database.GetCollection<BsonDocument>("_sequences");
    }

    public IMongoDatabase Database { get; }

    public IMongoCollection<T> Collection<T>(string name) where T : class =>
        Database.GetCollection<T>(name);

    public Task PingAsync() =>
        Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));

    public async Task<int> NextIdAsync(string sequenceName)
    {
        var result = await _sequences.FindOneAndUpdateAsync(
            Builders<BsonDocument>.Filter.Eq("_id", sequenceName),
            Builders<BsonDocument>.Update.Inc("value", 1),
            new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            });

        return result["value"].ToInt32();
    }

    public async Task InitializeSequenceAsync(string sequenceName, int initialValue)
    {
        await _sequences.UpdateOneAsync(
            Builders<BsonDocument>.Filter.Eq("_id", sequenceName),
            Builders<BsonDocument>.Update.Max("value", initialValue),
            new UpdateOptions { IsUpsert = true });
    }

    private static void RegisterDocumentMappings()
    {
        BsonSerializer.RegisterSerializer(new DateOnlySerializer(BsonType.String));

        RegisterEntity<Carro>(nameof(Carro.IdCarro));
        RegisterEntity<ItemCarro>(nameof(ItemCarro.IdItem));
        RegisterEntity<ItemServicio>(nameof(ItemServicio.IdItemServicio));
        RegisterEntity<ServicioAdicional>(nameof(ServicioAdicional.IdServicio));
        RegisterEntity<MedioPago>(nameof(MedioPago.IdMedio));
        RegisterEntity<Pago>(nameof(Pago.IdTransaccion));
        RegisterEntity<PagoTarjeta>(nameof(PagoTarjeta.IdTransaccion));
        RegisterEntity<PagoEfectivo>(nameof(PagoEfectivo.IdTransaccion));
        RegisterEntity<Orden>(nameof(Orden.NroOrden));
        RegisterEntity<Comprobante>(nameof(Comprobante.IdComprobante));
        RegisterEntity<NotificacionCorreo>(nameof(NotificacionCorreo.IdNotificacion));
        RegisterEntity<Cliente>(nameof(Cliente.IdCliente));
        RegisterEntity<Sesion>(nameof(Sesion.IdSesion));
        RegisterEntity<Direccion>(nameof(Direccion.IdDireccion));
        RegisterEntity<Zona>(nameof(Zona.IdZona));
        RegisterEntity<HorarioDespacho>(nameof(HorarioDespacho.IdHorario));
        RegisterEntity<Entrega>(nameof(Entrega.IdEntrega));
        RegisterEntity<Tienda>(nameof(Tienda.IdTienda));
        RegisterEntity<Distrito>(nameof(Distrito.IdDistrito));
        RegisterEntity<StockTienda>(null);
    }

    private static void RegisterEntity<T>(string? idMember) where T : class
    {
        var type = typeof(T);
        if (BsonClassMap.IsClassMapRegistered(type))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<T>(classMap =>
        {
            classMap.AutoMap();
            classMap.SetIgnoreExtraElements(true);
            if (idMember is not null)
            {
                var idProperty = type.GetProperty(idMember)
                    ?? throw new InvalidOperationException($"No se encontró la propiedad {type.Name}.{idMember}.");
                classMap.MapIdMember(idProperty);
            }

            var nonScalarMembers = classMap.AllMemberMaps
                .Where(member => member.MemberInfo is PropertyInfo property && !IsScalar(property.PropertyType))
                .Select(member => member.MemberInfo)
                .ToArray();

            foreach (var member in nonScalarMembers)
            {
                classMap.UnmapMember(member);
            }
        });
    }

    private static bool IsScalar(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateOnly)
            || type == typeof(TimeOnly)
            || type == typeof(Guid)
            || type == typeof(byte[]);
    }
}
