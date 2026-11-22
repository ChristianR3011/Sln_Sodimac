using Sodimac.Api.Dtos;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Interfaces
{
    public interface IClienteCarroService
    {
        Task<ApiResponse<ClienteDetalleDto>> GetClienteAsync(string idCliente);
        Task<ApiResponse<ActualizarClienteResponse>> ActualizarClienteAsync(string idCliente, ActualizarClienteRequest request);
        Task<ApiResponse<List<DireccionClienteDto>>> GetDireccionesAsync(string idCliente);
        Task<ApiResponse<DireccionClienteDto>> RegistrarDireccionAsync(string idCliente, RegistrarDireccionRequest request);
        Task<ApiResponse<DireccionClienteDto>> ActualizarDireccionAsync(string idCliente, string idDireccion, ActualizarDireccionRequest request);
        Task<ApiResponse<bool>> EliminarDireccionAsync(string idCliente, string idDireccion);
        Task<ApiResponse<ProductoCarroResponse>> AgregarProductoAsync(string idCarro, ProductoCarroRequest request);
        Task<ApiResponse<ResumenCarroDto>> GetCarroAsync(string idCarro, string? idZona = "Lima");
        Task<ApiResponse<ResumenCarroDto>> ActualizarCantidadAsync(string idCarro, string sku, ActualizarCantidadItemRequest request);
        Task<ApiResponse<ResumenCarroDto>> EliminarProductoAsync(string idCarro, string sku);
        Task<ApiResponse<ResumenCarroDto>> AgregarServicioAsync(string idCarro, string sku, AgregarServicioRequest request);
        Task<ApiResponse<CuponResponse>> AplicarCuponAsync(string idCarro, CuponRequest request);
        Task<ApiResponse<FusionCarroResponse>> FusionarCarroAsync(string idCliente, FusionCarroRequest request);
        Task<bool> TieneSesionValidaAsync(string? authorizationHeader, string idCliente);
        Task<ApiResponse<string>> CrearSesionAsync(string idCliente);
    }
}
