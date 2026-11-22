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
        public async Task<IActionResult> GetCliente(string idCliente)
        {
            if (!await _clienteCarroService.TieneSesionValidaAsync(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.GetClienteAsync(idCliente);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        // Actualiza los datos personales del cliente después de validar su sesión.
        [HttpPatch("clientes/{idCliente}")]
        public async Task<IActionResult> ActualizarCliente(string idCliente, [FromBody] ActualizarClienteRequest request)
        {
            if (!await _clienteCarroService.TieneSesionValidaAsync(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.ActualizarClienteAsync(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Consulta todas las direcciones registradas para el cliente.
        [HttpGet("clientes/{idCliente}/direcciones")]
        public async Task<IActionResult> GetDirecciones(string idCliente)
        {
            if (!await _clienteCarroService.TieneSesionValidaAsync(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.GetDireccionesAsync(idCliente);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Registra una nueva dirección de entrega para el cliente.
        [HttpPost("clientes/{idCliente}/direcciones")]
        public async Task<IActionResult> RegistrarDireccion(string idCliente, [FromBody] RegistrarDireccionRequest request)
        {
            if (!await _clienteCarroService.TieneSesionValidaAsync(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.RegistrarDireccionAsync(idCliente, request);
            if (!response.Success) return BadRequest(response);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        // Modifica una dirección existente del cliente.
        [HttpPut("clientes/{idCliente}/direcciones/{idDireccion}")]
        public async Task<IActionResult> ActualizarDireccion(string idCliente, string idDireccion, [FromBody] ActualizarDireccionRequest request)
        {
            if (!await _clienteCarroService.TieneSesionValidaAsync(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.ActualizarDireccionAsync(idCliente, idDireccion, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // Elimina una dirección registrada según el ID del cliente y el ID de la dirección.
        [HttpDelete("clientes/{idCliente}/direcciones/{idDireccion}")]
        public async Task<IActionResult> EliminarDireccion(string idCliente, string idDireccion)
        {
            if (!await _clienteCarroService.TieneSesionValidaAsync(Request.Headers.Authorization, idCliente))
                return Unauthorized(new { message = "Sesión no iniciada o token inválido" });

            var response = await _clienteCarroService.EliminarDireccionAsync(idCliente, idDireccion);
            if (!response.Success) return NotFound(response);
            return NoContent();
        }

        // Crea una sesión para el cliente y devuelve el token de autorización generado.
        [HttpPost("sesiones")]
        public async Task<IActionResult> CrearSesion([FromBody] Dictionary<string, string> payload)
        {
            var idCliente = payload.ContainsKey("idCliente") ? payload["idCliente"] : "1";
            var response = await _clienteCarroService.CrearSesionAsync(idCliente);
            if (!response.Success) return BadRequest(response);
            return Ok(new { token = response.Data });
        }
    }
}
