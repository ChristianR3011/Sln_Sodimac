using Sodimac.Api.Models;

namespace Sodimac.Api.Repositories.Interfaces
{
    public interface IOrdenRepository
    {
        Task<string> GenerarNumeroOrdenAsync();
        Task<Orden> CrearAsync(Orden orden);
        Task<Orden?> GetByNroOrdenAsync(string nroOrden);
        Task ActualizarEstadoAsync(string nroOrden, string estado);
    }
}
