using Sodimac.Api.Data;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace Sodimac.Api.Repositories.Implementations
{
    public class CarroRepository : ICarroRepository
    {
        private readonly MongoDataContext _mongo;

        public CarroRepository(MongoDataContext mongo)
        {
            _mongo = mongo;
        }

        public async Task<Carro?> GetByIdAsync(int idCarro)
        {
            return await _mongo.Collection<Carro>("carros")
                .Find(c => c.IdCarro == idCarro)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ItemCarro>> GetItemsAsync(int idCarro)
        {
            return await _mongo.Collection<ItemCarro>("itemsCarro")
                .Find(i => i.IdCarro == idCarro)
                .ToListAsync();
        }

        public async Task<List<(ItemCarro Item, List<ItemServicio> Servicios)>> GetItemsConServiciosAsync(int idCarro)
        {
            var items = await GetItemsAsync(idCarro);

            if (!items.Any())
                return new List<(ItemCarro, List<ItemServicio>)>();

            var itemIds = items.Select(i => i.IdItem).ToList();
            var relaciones = await _mongo.Collection<ItemServicio>("itemsServicio")
                .Find(link => itemIds.Contains(link.IdItem))
                .ToListAsync();
            var servicioIds = relaciones.Select(link => link.IdServicio).Distinct().ToList();
            var servicios = await _mongo.Collection<ServicioAdicional>("serviciosAdicionales")
                .Find(service => servicioIds.Contains(service.IdServicio))
                .ToListAsync();

            return items.Select(item => (
                item,
                relaciones
                    .Where(link => link.IdItem == item.IdItem)
                    .Select(link =>
                    {
                        return new ItemServicio
                        {
                            IdItemServicio = link.IdItemServicio,
                            IdItem = link.IdItem,
                            IdServicio = link.IdServicio,
                            Precio = link.Precio
                        };
                    }).ToList()
            )).ToList();
        }
    }
}
