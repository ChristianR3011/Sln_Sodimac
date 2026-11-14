using Microsoft.EntityFrameworkCore;
using Sodimac.Api.Models;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Interfaces;

namespace Sodimac.Api.Repositories.Implementations
{
    public class CarroRepository : ICarroRepository
    {
        private readonly SodimacContext _context;

        public CarroRepository(SodimacContext context)
        {
            _context = context;
        }

        public async Task<Carro?> GetByIdAsync(int idCarro)
        {
            return await _context.Carros.FindAsync(idCarro);
        }

        public async Task<List<ItemCarro>> GetItemsAsync(int idCarro)
        {
            return await _context.ItemCarros
                .Where(i => i.IdCarro == idCarro)
                .ToListAsync();
        }

        public async Task<List<(ItemCarro Item, List<ItemServicio> Servicios)>> GetItemsConServiciosAsync(int idCarro)
        {
            var items = await _context.ItemCarros
                .Where(i => i.IdCarro == idCarro)
                .ToListAsync();

            if (!items.Any())
                return new List<(ItemCarro, List<ItemServicio>)>();

            var itemIds = items.Select(i => i.IdItem).ToList();

            var serviciosRelacionados = await (from itemServ in _context.ItemServicios
                                               join s in _context.ServicioAdicionals on itemServ.IdServicio equals s.IdServicio
                                               where itemIds.Contains(itemServ.IdItem)
                                               select new { itemServ.IdItem, Servicio = s, itemServ.Precio })
                                               .ToListAsync();

            return items.Select(item => (
                item,
                serviciosRelacionados
                    .Where(sr => sr.IdItem == item.IdItem)
                    .Select(sr => new ItemServicio
                    {
                        IdItemServicio = 0,
                        IdItem = sr.IdItem,
                        IdServicio = sr.Servicio.IdServicio,
                        Precio = sr.Precio
                    }).ToList()
            )).ToList();
        }
    }
}
