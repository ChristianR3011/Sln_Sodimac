using Microsoft.AspNetCore.Mvc;
using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;

namespace Sodimac.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        public PagoController(IPagoService pagoService) => _pagoService = pagoService;

        [HttpPost("tarjeta")]
        public async Task<IActionResult> PagarConTarjeta([FromBody] PagarConTarjetaRequest request)
            => Ok(await _pagoService.PagarConTarjetaAsync(request));

        [HttpPost("efectivo")]
        public async Task<IActionResult> PagoEfectivo([FromBody] GenerarCodigoPagoEfectivoRequest request)
            => Ok(await _pagoService.GenerarPagoEfectivoAsync(request));
    }
}
