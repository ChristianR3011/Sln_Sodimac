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

        [HttpPost("carros/{idCarro}/items")]
        public IActionResult AgregarProducto(string idCarro, [FromBody] ProductoCarroRequest request)
        {
            var response = _clienteCarroService.AgregarProducto(idCarro, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet("carros/{idCarro}")]
        public IActionResult GetCarro(string idCarro, [FromQuery] string? idZona = "Lima")
        {
            var response = _clienteCarroService.GetCarro(idCarro, idZona);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPatch("carros/{idCarro}/items/{sku}")]
        public IActionResult ActualizarCantidad(string idCarro, string sku, [FromBody] ActualizarCantidadItemRequest request)
        {
            var response = _clienteCarroService.ActualizarCantidad(idCarro, sku, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("carros/{idCarro}/items/{sku}")]
        public IActionResult EliminarProducto(string idCarro, string sku)
        {
            var response = _clienteCarroService.EliminarProducto(idCarro, sku);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPost("carros/{idCarro}/items/{sku}/servicios")]
        public IActionResult AgregarServicio(string idCarro, string sku, [FromBody] AgregarServicioRequest request)
        {
            var response = _clienteCarroService.AgregarServicio(idCarro, sku, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("carros/{idCarro}/cupones")]
        public IActionResult AplicarCupon(string idCarro, [FromBody] CuponRequest request)
        {
            var response = _clienteCarroService.AplicarCupon(idCarro, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("carros/fusion")]
        public IActionResult FusionarCarro([FromBody] FusionCarroRequest request)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var idCliente = Request.Headers["X-Cliente-Id"].ToString();
            if (string.IsNullOrWhiteSpace(idCliente))
            {
                return Unauthorized(new { message = "Debe indicar el cliente en el header X-Cliente-Id" });
            }

            if (!_clienteCarroService.TieneSesionValida(authHeader, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.FusionarCarro(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}
