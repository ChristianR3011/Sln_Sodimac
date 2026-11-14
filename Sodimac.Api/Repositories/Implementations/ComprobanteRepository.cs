using Microsoft.EntityFrameworkCore;
using Sodimac.Api.Models;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class ComprobanteRepository : IComprobanteRepository
    {
        private readonly SodimacContext _context;

        public ComprobanteRepository(SodimacContext context)
        {
            _context = context;
        }

        public async Task<Comprobante> CrearAsync(Comprobante comprobante)
        {
            _context.Comprobantes.Add(comprobante);
            await _context.SaveChangesAsync();
            return comprobante;
        }

        public async Task<Comprobante?> GetByIdCarroAsync(int idCarro)
        {
            return await _context.Comprobantes.FirstOrDefaultAsync(c => c.IdCarro == idCarro);
        }

        public async Task<Comprobante?> GetByNroOrdenAsync(string nroOrden)
        {
            return await _context.Comprobantes
                .FirstOrDefaultAsync(c => c.NroOrden == nroOrden);
        }

        public async Task ActualizarAsync(Comprobante comprobante)
        {
            _context.Comprobantes.Update(comprobante);
            await _context.SaveChangesAsync();
        }
    }
}
