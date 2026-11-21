using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;
using Sodimac.Api.Data;
using MongoDB.Driver;

namespace Sodimac.Api.Repositories.Implementations
{
    public class PagoRepository : IPagoRepository
    {
        private readonly MongoDataContext _mongo;

        public PagoRepository(MongoDataContext mongo)
        {
            _mongo = mongo;
        }

        public async Task<List<MedioPago>> GetAllMediosPagoAsync()
        {
            return await _mongo.Collection<MedioPago>("mediosPago")
                .Find(_ => true)
                .SortBy(m => m.IdMedio)
                .ToListAsync();
        }

        public async Task<Pago> CrearPagoAsync(Pago pago)
        {
            pago.IdTransaccion = await _mongo.NextIdAsync("pagos");
            await _mongo.Collection<Pago>("pagos").InsertOneAsync(pago);
            return pago;
        }

        public async Task<PagoTarjeta> CrearPagoTarjetaAsync(PagoTarjeta pagoTarjeta)
        {
            await _mongo.Collection<PagoTarjeta>("pagosTarjeta").InsertOneAsync(pagoTarjeta);
            return pagoTarjeta;
        }

        public async Task<PagoEfectivo> CrearPagoEfectivoAsync(PagoEfectivo pagoEfectivo)
        {
            await _mongo.Collection<PagoEfectivo>("pagosEfectivo").InsertOneAsync(pagoEfectivo);
            return pagoEfectivo;
        }

        public async Task<Pago?> GetPagoByIdAsync(int idTransaccion)
        {
            return await _mongo.Collection<Pago>("pagos")
                .Find(payment => payment.IdTransaccion == idTransaccion)
                .FirstOrDefaultAsync();
        }

        public async Task ActualizarEstadoPagoAsync(int idTransaccion, string estado)
        {
            await _mongo.Collection<Pago>("pagos").UpdateOneAsync(
                payment => payment.IdTransaccion == idTransaccion,
                Builders<Pago>.Update.Set(payment => payment.Estado, estado));
        }
    }
}
