using Sodimac.Api.Dtos;
using Sodimac.Api.Models;

namespace Sodimac.Api.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task<List<MedioPago>> GetAllMediosPagoAsync();
        Task<Pago> CrearPagoAsync(Pago pago);
        Task<PagoTarjeta> CrearPagoTarjetaAsync(PagoTarjeta pagoTarjeta);
        Task<PagoEfectivo> CrearPagoEfectivoAsync(PagoEfectivo pagoEfectivo);
        Task<Pago?> GetPagoByIdAsync(int idTransaccion);
        Task ActualizarEstadoPagoAsync(int idTransaccion, string estado);
    }
}
