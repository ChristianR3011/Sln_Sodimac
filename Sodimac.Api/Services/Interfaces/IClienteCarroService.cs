using Sodimac.Api.Dtos;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Interfaces
{
    public interface IClienteCarroService
    {
        ApiResponse<ClienteDetalleDto> GetCliente(string idCliente);
        ApiResponse<ActualizarClienteResponse> ActualizarCliente(string idCliente, ActualizarClienteRequest request);
        ApiResponse<List<DireccionClienteDto>> GetDirecciones(string idCliente);
        ApiResponse<DireccionClienteDto> RegistrarDireccion(string idCliente, RegistrarDireccionRequest request);
        ApiResponse<DireccionClienteDto> ActualizarDireccion(string idCliente, string idDireccion, ActualizarDireccionRequest request);
        ApiResponse<bool> EliminarDireccion(string idCliente, string idDireccion);
        ApiResponse<ProductoCarroResponse> AgregarProducto(string idCarro, ProductoCarroRequest request);
        ApiResponse<ResumenCarroDto> GetCarro(string idCarro, string? idZona = "Lima");
        ApiResponse<ResumenCarroDto> ActualizarCantidad(string idCarro, string sku, ActualizarCantidadItemRequest request);
        ApiResponse<ResumenCarroDto> EliminarProducto(string idCarro, string sku);
        ApiResponse<ResumenCarroDto> AgregarServicio(string idCarro, string sku, AgregarServicioRequest request);
        ApiResponse<CuponResponse> AplicarCupon(string idCarro, CuponRequest request);
        ApiResponse<FusionCarroResponse> FusionarCarro(string idCliente, FusionCarroRequest request);
        bool TieneSesionValida(string? authorizationHeader, string idCliente);
        ApiResponse<string> CrearSesion(string idCliente);
    }
}
