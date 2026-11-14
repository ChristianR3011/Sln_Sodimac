using Microsoft.AspNetCore.Mvc;
using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;

namespace Sodimac.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfirmacionController : ControllerBase
    {
        private readonly IConfirmacionService _confirmacionService;
        public ConfirmacionController(IConfirmacionService confirmacionService) => _confirmacionService = confirmacionService;

        [HttpPost("orden")]
        public async Task<IActionResult> RegistrarOrden([FromBody] RegistrarOrdenRequest request)
            => Ok(await _confirmacionService.RegistrarOrdenAsync(request));

        [HttpPost("enviar-confirmacion")]
        public async Task<IActionResult> EnviarConfirmacion([FromBody] EnviarConfirmacionRequest request)
            => Ok(await _confirmacionService.EnviarConfirmacionAsync(request));

        [HttpGet("comprobante")]
        public async Task<IActionResult> EmitirComprobante([FromQuery] EmitirComprobanteRequest request)
        {
            var response = await _confirmacionService.EmitirComprobanteAsync(request);
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
    }
}
