using Microsoft.AspNetCore.Mvc;
using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;

namespace Sodimac.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteCarroService _clienteCarroService;

        public ClienteController(IClienteCarroService clienteCarroService)
        {
            _clienteCarroService = clienteCarroService;
        }

        [HttpGet("clientes/{idCliente}")]
        public IActionResult GetCliente(string idCliente)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.GetCliente(idCliente);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPatch("clientes/{idCliente}")]
        public IActionResult ActualizarCliente(string idCliente, [FromBody] ActualizarClienteRequest request)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.ActualizarCliente(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("clientes/{idCliente}/direcciones")]
        public IActionResult GetDirecciones(string idCliente)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.GetDirecciones(idCliente);
            return Ok(response);
        }

        [HttpPost("clientes/{idCliente}/direcciones")]
        public IActionResult RegistrarDireccion(string idCliente, [FromBody] RegistrarDireccionRequest request)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.RegistrarDireccion(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("clientes/{idCliente}/direcciones/{idDireccion}")]
        public IActionResult ActualizarDireccion(string idCliente, string idDireccion, [FromBody] ActualizarDireccionRequest request)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.ActualizarDireccion(idCliente, idDireccion, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("clientes/{idCliente}/direcciones/{idDireccion}")]
        public IActionResult EliminarDireccion(string idCliente, string idDireccion)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.EliminarDireccion(idCliente, idDireccion);
            if (!response.Success) return NotFound(response);
            return NoContent();
        }

        [HttpPost("sesiones")]
        public IActionResult CrearSesion([FromBody] Dictionary<string, string> payload)
        {
            var idCliente = payload.ContainsKey("idCliente") ? payload["idCliente"] : "1";
            var response = _clienteCarroService.CrearSesion(idCliente);
            if (!response.Success) return BadRequest(response);
            return Ok(new { token = response.Data });
        }
    }
}
