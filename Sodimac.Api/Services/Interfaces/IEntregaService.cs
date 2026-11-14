using Sodimac.Api.Dtos;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Interfaces
{
    public interface IEntregaService
    {
        Task<ApiResponse<ListaFechasHorariosResponse>> GetFechasYHorariosAsync(ListaFechasHorariosRequest request);
        Task<ApiResponse<SeleccionarDespachoResponse>> SeleccionarDespachoDomicilioAsync(SeleccionarDespachoRequest request);
        Task<ApiResponse<ListaTiendasRetiroResponse>> GetTiendasRetiroAsync(ListaTiendasRetiroRequest request);
        Task<ApiResponse<SeleccionarRetiroTiendaResponse>> SeleccionarRetiroTiendaAsync(SeleccionarRetiroTiendaRequest request);
    }
}
