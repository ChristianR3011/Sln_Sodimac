using Sodimac.Api.Models;

namespace Sodimac.Api.Repositories.Interfaces
{
    public interface IComprobanteRepository
    {
        Task<Comprobante> CrearAsync(Comprobante comprobante);
        Task<Comprobante?> GetByIdCarroAsync(int idCarro);
        Task<Comprobante?> GetByNroOrdenAsync(string nroOrden);
        Task ActualizarAsync(Comprobante comprobante);
    }
}
