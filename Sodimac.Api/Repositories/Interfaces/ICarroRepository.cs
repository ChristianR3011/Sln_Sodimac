using Sodimac.Api.Models;

namespace Sodimac.Api.Repositories.Interfaces
{
    public interface ICarroRepository
    {
        Task<Carro?> GetByIdAsync(int idCarro);
        Task<List<ItemCarro>> GetItemsAsync(int idCarro);
        Task<List<(ItemCarro Item, List<ItemServicio> Servicios)>> GetItemsConServiciosAsync(int idCarro);
    }
}
