using Microsoft.AspNetCore.Mvc;
using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;

namespace Sodimac.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaService _entregaService;
        public EntregaController(IEntregaService entregaService) => _entregaService = entregaService;

        [HttpGet("fechas-horarios")]
        public async Task<IActionResult> GetFechasHorarios([FromQuery] ListaFechasHorariosRequest request)
            => Ok(await _entregaService.GetFechasYHorariosAsync(request));

        [HttpPost("despacho-domicilio")]
        public async Task<IActionResult> SeleccionarDespacho([FromBody] SeleccionarDespachoRequest request)
            => Ok(await _entregaService.SeleccionarDespachoDomicilioAsync(request));

        [HttpGet("tiendas-retiro")]
        public async Task<IActionResult> GetTiendasRetiro([FromQuery] ListaTiendasRetiroRequest request)
            => Ok(await _entregaService.GetTiendasRetiroAsync(request));

        [HttpPost("retiro-tienda")]
        public async Task<IActionResult> SeleccionarRetiroTienda([FromBody] SeleccionarRetiroTiendaRequest request)
            => Ok(await _entregaService.SeleccionarRetiroTiendaAsync(request));
    }
}
