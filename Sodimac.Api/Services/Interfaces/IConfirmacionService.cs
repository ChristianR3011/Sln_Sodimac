using Sodimac.Api.Dtos;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Interfaces
{
    public interface IConfirmacionService
    {
        Task<ApiResponse<RegistrarOrdenResponse>> RegistrarOrdenAsync(RegistrarOrdenRequest request);
        Task<ApiResponse<EnviarConfirmacionResponse>> EnviarConfirmacionAsync(EnviarConfirmacionRequest request);
        Task<ApiResponse<EmitirComprobanteResponse>> EmitirComprobanteAsync(EmitirComprobanteRequest request);
    }
}
