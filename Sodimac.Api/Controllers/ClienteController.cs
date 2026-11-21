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

        // Obtiene los datos del cliente después de validar su sesión.
        [HttpGet("clientes/{idCliente}")]
        public IActionResult GetCliente(string idCliente)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.GetCliente(idCliente);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        // Actualiza los datos personales del cliente después de validar su sesión.
        [HttpPatch("clientes/{idCliente}")]
        public IActionResult ActualizarCliente(string idCliente, [FromBody] ActualizarClienteRequest request)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.ActualizarCliente(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Consulta todas las direcciones registradas para el cliente.
        [HttpGet("clientes/{idCliente}/direcciones")]
        public IActionResult GetDirecciones(string idCliente)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.GetDirecciones(idCliente);
            return Ok(response);
        }

        // Registra una nueva dirección de entrega para el cliente.
        [HttpPost("clientes/{idCliente}/direcciones")]
        public IActionResult RegistrarDireccion(string idCliente, [FromBody] RegistrarDireccionRequest request)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.RegistrarDireccion(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        // Modifica una dirección existente del cliente.
        [HttpPut("clientes/{idCliente}/direcciones/{idDireccion}")]
        public IActionResult ActualizarDireccion(string idCliente, string idDireccion, [FromBody] ActualizarDireccionRequest request)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.ActualizarDireccion(idCliente, idDireccion, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Elimina una dirección registrada según el ID del cliente y el ID de la dirección.
        [HttpDelete("clientes/{idCliente}/direcciones/{idDireccion}")]
        public IActionResult EliminarDireccion(string idCliente, string idDireccion)
        {
            if (!_clienteCarroService.TieneSesionValida(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = _clienteCarroService.EliminarDireccion(idCliente, idDireccion);
            if (!response.Success) return NotFound(response);
            return NoContent();
        }

        // Crea una sesión para el cliente y devuelve el token de autorización generado.
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
