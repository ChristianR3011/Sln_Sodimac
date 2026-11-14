using Microsoft.AspNetCore.Mvc;
using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;

namespace Sodimac.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        public CheckoutController(ICheckoutService checkoutService) => _checkoutService = checkoutService;

        [HttpPost("comprobante")]
        public async Task<IActionResult> RegistrarComprobante([FromBody] RegistrarComprobanteRequest request)
            => Ok(await _checkoutService.RegistrarComprobanteAsync(request));

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen([FromQuery] ObtenerResumenRequest request)
            => Ok(await _checkoutService.GetResumenCompraAsync(request));

        [HttpGet("medios-pago")]
        public async Task<IActionResult> GetMediosPago([FromQuery] ListaMediosPagoRequest request)
            => Ok(await _checkoutService.GetMediosPagoAsync(request));
    }
}
