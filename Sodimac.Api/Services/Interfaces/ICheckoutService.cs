using Sodimac.Api.Dtos;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<ApiResponse<RegistrarComprobanteResponse>> RegistrarComprobanteAsync(RegistrarComprobanteRequest request);
        Task<ApiResponse<ResumenCompraDto>> GetResumenCompraAsync(ObtenerResumenRequest request);
        Task<ApiResponse<ListaMediosPagoResponse>> GetMediosPagoAsync(ListaMediosPagoRequest request);
    }
}
