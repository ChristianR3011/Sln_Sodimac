using Microsoft.AspNetCore.Mvc;
using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;

namespace Sodimac.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CarroController : ControllerBase
    {
        private readonly IClienteCarroService _clienteCarroService;

        public CarroController(IClienteCarroService clienteCarroService)
        {
            _clienteCarroService = clienteCarroService;
        }

        // Agrega un producto al carro de compras identificado por el ID recibido.
        [HttpPost("carros/{idCarro}/items")]
        public async Task<IActionResult> AgregarProducto(string idCarro, [FromBody] ProductoCarroRequest request)
        {
            var response = await _clienteCarroService.AgregarProductoAsync(idCarro, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        // Consulta los datos y productos del carro, opcionalmente usando una zona de entrega.
        [HttpGet("carros/{idCarro}")]
        public async Task<IActionResult> GetCarro(string idCarro, [FromQuery] string? idZona = "Lima")
        {
            var response = await _clienteCarroService.GetCarroAsync(idCarro, idZona);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        // Actualiza la cantidad de un producto existente dentro del carro.
        [HttpPatch("carros/{idCarro}/items/{sku}")]
        public async Task<IActionResult> ActualizarCantidad(string idCarro, string sku, [FromBody] ActualizarCantidadItemRequest request)
        {
            var response = await _clienteCarroService.ActualizarCantidadAsync(idCarro, sku, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Elimina un producto del carro según el ID del carro y el SKU recibido.
        [HttpDelete("carros/{idCarro}/items/{sku}")]
        public async Task<IActionResult> EliminarProducto(string idCarro, string sku)
        {
            var response = await _clienteCarroService.EliminarProductoAsync(idCarro, sku);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        // Agrega un servicio adicional a un producto específico del carro.
        [HttpPost("carros/{idCarro}/items/{sku}/servicios")]
        public async Task<IActionResult> AgregarServicio(string idCarro, string sku, [FromBody] AgregarServicioRequest request)
        {
            var response = await _clienteCarroService.AgregarServicioAsync(idCarro, sku, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        // Valida y aplica un cupón de descuento al carro de compras.
        [HttpPost("carros/{idCarro}/cupones")]
        public async Task<IActionResult> AplicarCupon(string idCarro, [FromBody] CuponRequest request)
        {
            var response = await _clienteCarroService.AplicarCuponAsync(idCarro, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Fusiona el carro de invitado con el carro del cliente después de validar su sesión.
        [HttpPost("carros/fusion")]
        public async Task<IActionResult> FusionarCarro([FromBody] FusionCarroRequest request)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var idCliente = Request.Headers["X-Cliente-Id"].ToString();
            if (string.IsNullOrWhiteSpace(idCliente))
            {
                return Unauthorized(new { message = "Debe indicar el cliente en el header X-Cliente-Id" });
            }

            if (!await _clienteCarroService.TieneSesionValidaAsync(authHeader, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.FusionarCarroAsync(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}
