using MongoDB.Driver;
using Sodimac.Api.Data;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class EntregaRepository : IEntregaRepository
    {
        private readonly MongoDataContext _mongo;

        public EntregaRepository(MongoDataContext mongo)
        {
            _mongo = mongo;
        }

        public async Task<int> GetIdZonaByDireccionAsync(int idDireccion)
        {
            var direccion = await _mongo.Collection<Direccion>("direcciones")
                .Find(d => d.IdDireccion == idDireccion)
                .FirstOrDefaultAsync();

            if (direccion == null) return 0;

            var zona = await _mongo.Collection<Zona>("zonas")
                .Find(z => z.IdDistrito == direccion.IdDistrito)
                .FirstOrDefaultAsync();

            return zona?.IdZona ?? 0;
        }

        public async Task<List<HorarioDespacho>> GetHorariosDisponiblesAsync(int idZona)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            return await _mongo.Collection<HorarioDespacho>("horariosDespacho")
                .Find(h => h.IdZona == idZona && h.Fecha >= hoy && h.Capacidad > 0)
                .SortBy(h => h.Fecha)
                .ThenBy(h => h.RangoHorario)
                .ToListAsync();
        }

        public async Task<bool> VerificarCoberturaAsync(int idZona)
        {
            var zona = await _mongo.Collection<Zona>("zonas")
                .Find(z => z.IdZona == idZona)
                .FirstOrDefaultAsync();
            return zona?.TieneCobertura ?? false;
        }

        public async Task<Entrega> CrearEntregaAsync(Entrega entrega)
        {
            entrega.IdEntrega = await _mongo.NextIdAsync("entregas");
            await _mongo.Collection<Entrega>("entregas").InsertOneAsync(entrega);
            return entrega;
        }

        public async Task<List<Tienda>> GetTiendasByZonaAsync(int idZona)
        {
            var zona = await _mongo.Collection<Zona>("zonas")
                .Find(z => z.IdZona == idZona)
                .FirstOrDefaultAsync();

            if (zona == null) return new List<Tienda>();

            return await _mongo.Collection<Tienda>("tiendas")
                .Find(store => store.IdDistrito == zona.IdDistrito)
                .SortBy(store => store.Nombre)
                .ToListAsync();
        }

        public async Task<List<StockTienda>> GetStockTiendasAsync(int idCarro, List<int> idsTienda)
        {
            var skus = await _mongo.Collection<ItemCarro>("itemsCarro")
                .Find(item => item.IdCarro == idCarro)
                .Project(item => item.Sku)
                .ToListAsync();
            skus = skus.Distinct().ToList();

            if (!skus.Any()) return new List<StockTienda>();

            return await _mongo.Collection<StockTienda>("stockTiendas")
                .Find(stock => skus.Contains(stock.Sku) && idsTienda.Contains(stock.IdTienda))
                .ToListAsync();
        }
    }
}
