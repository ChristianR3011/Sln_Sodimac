using Sodimac.Api.Models;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Sodimac.Api.Repositories.Implementations
{
    public class PagoRepository : IPagoRepository
    {
        private readonly SodimacContext _context;

        public PagoRepository(SodimacContext context)
        {
            _context = context;
        }

        public async Task<List<MedioPago>> GetAllMediosPagoAsync()
        {
            return await _context.MedioPagos
                .OrderBy(m => m.IdMedio)
                .ToListAsync();
        }

        public async Task<Pago> CrearPagoAsync(Pago pago)
        {
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<PagoTarjeta> CrearPagoTarjetaAsync(PagoTarjeta pagoTarjeta)
        {
            _context.PagoTarjetas.Add(pagoTarjeta);
            await _context.SaveChangesAsync();
            return pagoTarjeta;
        }

        public async Task<PagoEfectivo> CrearPagoEfectivoAsync(PagoEfectivo pagoEfectivo)
        {
            _context.PagoEfectivos.Add(pagoEfectivo);
            await _context.SaveChangesAsync();
            return pagoEfectivo;
        }

        public async Task<Pago?> GetPagoByIdAsync(int idTransaccion)
        {
            return await _context.Pagos.FindAsync(idTransaccion);
        }

        public async Task ActualizarEstadoPagoAsync(int idTransaccion, string estado)
        {
            var pago = await _context.Pagos.FindAsync(idTransaccion);
            if (pago != null)
            {
                pago.Estado = estado;
                await _context.SaveChangesAsync();
            }
        }
    }
}
