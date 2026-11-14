using Sodimac.Api.Dtos;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Interfaces
{
    public interface IPagoService
    {
        Task<ApiResponse<PagarConTarjetaResponse>> PagarConTarjetaAsync(PagarConTarjetaRequest request);
        Task<ApiResponse<GenerarCodigoPagoEfectivoResponse>> GenerarPagoEfectivoAsync(GenerarCodigoPagoEfectivoRequest request);
    }
}
