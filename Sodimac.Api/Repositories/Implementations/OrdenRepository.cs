using Microsoft.EntityFrameworkCore;
using Sodimac.Api.Models;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly SodimacContext _context;

        public OrdenRepository(SodimacContext context)
        {
            _context = context;
        }

        public async Task<string> GenerarNumeroOrdenAsync()
        {
            var ultimaOrden = await _context.Ordens
                .Where(o => o.NroOrden.StartsWith("ORD-"))
                .OrderByDescending(o => o.NroOrden)
                .FirstOrDefaultAsync();

            int siguienteNumero = 1;

            if (ultimaOrden != null && int.TryParse(ultimaOrden.NroOrden.Replace("ORD-", ""), out int ultimoNumero))
            {
                siguienteNumero = ultimoNumero + 1;
            }

            return $"ORD-{siguienteNumero:D8}";
        }

        public async Task<Orden> CrearAsync(Orden orden)
        {
            _context.Ordens.Add(orden);
            await _context.SaveChangesAsync();
            return orden;
        }

        public async Task<Orden?> GetByNroOrdenAsync(string nroOrden)
        {
            return await _context.Ordens.FindAsync(nroOrden);
        }

        public async Task ActualizarEstadoAsync(string nroOrden, string estado)
        {
            var orden = await _context.Ordens.FindAsync(nroOrden);
            if (orden != null)
            {
                orden.Estado = estado;
                await _context.SaveChangesAsync();
            }
        }
    }
}
