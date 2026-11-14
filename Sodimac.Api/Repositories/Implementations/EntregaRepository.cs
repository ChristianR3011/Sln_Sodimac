using Microsoft.EntityFrameworkCore;
using Sodimac.Api.Models;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class EntregaRepository : IEntregaRepository
    {
        private readonly SodimacContext _context;

        public EntregaRepository(SodimacContext context)
        {
            _context = context;
        }

        public async Task<int> GetIdZonaByDireccionAsync(int idDireccion)
        {
            var direccion = await _context.Direccions.FirstOrDefaultAsync(d => d.IdDireccion == idDireccion);

            if (direccion == null) return 0;

            var zona = await _context.Zonas
                .FirstOrDefaultAsync(z => z.IdDistrito == direccion.IdDistrito);

            return zona?.IdZona ?? 0;
        }

        public async Task<List<HorarioDespacho>> GetHorariosDisponiblesAsync(int idZona)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            return await _context.HorarioDespachos
                .Where(h => h.IdZona == idZona && h.Fecha >= hoy && h.Capacidad > 0)
                .OrderBy(h => h.Fecha)
                .ThenBy(h => h.RangoHorario)
                .ToListAsync();
        }

        public async Task<bool> VerificarCoberturaAsync(int idZona)
        {
            var zona = await _context.Zonas.FindAsync(idZona);
            return zona?.TieneCobertura ?? false;
        }

        public async Task<Entrega> CrearEntregaAsync(Entrega entrega)
        {
            _context.Entregas.Add(entrega);
            await _context.SaveChangesAsync();
            return entrega;
        }

        public async Task<List<Tienda>> GetTiendasByZonaAsync(int idZona)
        {
            return await (from t in _context.Tiendas
                          join d in _context.Distritos on t.IdDistrito equals d.IdDistrito
                          join z in _context.Zonas on d.IdDistrito equals z.IdDistrito
                          where z.IdZona == idZona
                          orderby t.Nombre
                          select t)
                         .ToListAsync();
        }

        public async Task<List<StockTienda>> GetStockTiendasAsync(int idCarro, List<int> idsTienda)
        {
            var skus = await _context.ItemCarros
                .Where(i => i.IdCarro == idCarro)
                .Select(i => i.Sku)
                .Distinct()
                .ToListAsync();

            if (!skus.Any()) return new List<StockTienda>();

            return await _context.StockTiendas
                .Where(st => skus.Contains(st.Sku) && idsTienda.Contains(st.IdTienda))
                .ToListAsync();
        }
    }
}
