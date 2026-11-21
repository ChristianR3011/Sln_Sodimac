using MongoDB.Driver;
using Sodimac.Api.Data;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class ComprobanteRepository : IComprobanteRepository
    {
        private readonly MongoDataContext _mongo;

        public ComprobanteRepository(MongoDataContext mongo)
        {
            _mongo = mongo;
        }

        public async Task<Comprobante> CrearAsync(Comprobante comprobante)
        {
            comprobante.IdComprobante = await _mongo.NextIdAsync("comprobantes");
            await _mongo.Collection<Comprobante>("comprobantes").InsertOneAsync(comprobante);
            return comprobante;
        }

        public async Task<Comprobante?> GetByIdCarroAsync(int idCarro)
        {
            return await _mongo.Collection<Comprobante>("comprobantes")
                .Find(certificate => certificate.IdCarro == idCarro)
                .FirstOrDefaultAsync();
        }

        public async Task<Comprobante?> GetByNroOrdenAsync(string nroOrden)
        {
            return await _mongo.Collection<Comprobante>("comprobantes")
                .Find(certificate => certificate.NroOrden == nroOrden)
                .FirstOrDefaultAsync();
        }

        public async Task ActualizarAsync(Comprobante comprobante)
        {
            await _mongo.Collection<Comprobante>("comprobantes")
                .ReplaceOneAsync(certificate => certificate.IdComprobante == comprobante.IdComprobante, comprobante);
        }
    }
}
