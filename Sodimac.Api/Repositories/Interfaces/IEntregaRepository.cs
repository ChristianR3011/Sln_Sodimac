using Sodimac.Api.Dtos;
using Sodimac.Api.Models;

namespace Sodimac.Api.Repositories.Interfaces
{
    public interface IEntregaRepository
    {
        Task<int> GetIdZonaByDireccionAsync(int idDireccion);
        Task<List<HorarioDespacho>> GetHorariosDisponiblesAsync(int idZona);
        Task<bool> VerificarCoberturaAsync(int idZona);
        Task<Entrega> CrearEntregaAsync(Entrega entrega);
        Task<List<Tienda>> GetTiendasByZonaAsync(int idZona);
        Task<List<StockTienda>> GetStockTiendasAsync(int idCarro, List<int> idsTienda);
    }
}
