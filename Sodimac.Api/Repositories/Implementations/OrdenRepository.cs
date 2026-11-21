using MongoDB.Driver;
using Sodimac.Api.Data;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly MongoDataContext _mongo;

        public OrdenRepository(MongoDataContext mongo)
        {
            _mongo = mongo;
        }

        public async Task<string> GenerarNumeroOrdenAsync() =>
            $"ORD-{await _mongo.NextIdAsync("ordenes"):D8}";

        public async Task<Orden> CrearAsync(Orden orden)
        {
            await _mongo.Collection<Orden>("ordenes").InsertOneAsync(orden);
            return orden;
        }

        public async Task<Orden?> GetByNroOrdenAsync(string nroOrden)
        {
            return await _mongo.Collection<Orden>("ordenes")
                .Find(order => order.NroOrden == nroOrden)
                .FirstOrDefaultAsync();
        }

        public async Task ActualizarEstadoAsync(string nroOrden, string estado)
        {
            await _mongo.Collection<Orden>("ordenes").UpdateOneAsync(
                order => order.NroOrden == nroOrden,
                Builders<Orden>.Update.Set(order => order.Estado, estado));
        }
    }
}
