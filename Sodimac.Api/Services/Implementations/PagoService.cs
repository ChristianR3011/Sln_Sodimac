using Sodimac.Api.Dtos;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;
using Sodimac.Api.Services.Interfaces;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Implementations
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepo;
        private readonly ICarroRepository _carroRepo;

        public PagoService(IPagoRepository pagoRepo, ICarroRepository carroRepo)
        {
            _pagoRepo = pagoRepo;
            _carroRepo = carroRepo;
        }

        // CASO 58: Pagar con tarjeta de crédito o débito
        public async Task<ApiResponse<PagarConTarjetaResponse>> PagarConTarjetaAsync(PagarConTarjetaRequest request)
        {
            try
            {
                var carro = await _carroRepo.GetByIdAsync(request.IdCarro);
                if (carro == null)
                    return ApiResponse<PagarConTarjetaResponse>.Fail("Carro no encontrado");

                if (request.Monto <= 0)
                    return ApiResponse<PagarConTarjetaResponse>.Fail("El monto debe ser mayor a cero");

                // Validación de tipo de tarjeta según CK_pago_tarjeta_tipo_tarjeta
                if (!new[] { "CREDITO", "DEBITO", "CMR" }.Contains(request.TipoTarjeta.ToUpper()))
                    return ApiResponse<PagarConTarjetaResponse>.Fail("Tipo de tarjeta inválido");

                // Validación de cuotas según CK_pago_tarjeta_cuotas
                if (request.Cuotas < 1 || request.Cuotas > 36)
                    return ApiResponse<PagarConTarjetaResponse>.Fail("Las cuotas deben estar entre 1 y 36");

                // Simulación de pasarela de pagos
                var aprobado = true; // Aquí iría la llamada real a la pasarela

                var pago = new Pago
                {
                    IdCarro = request.IdCarro,
                    IdMedio = request.IdMedio,
                    Monto = request.Monto,
                    Estado = aprobado ? "APROBADO" : "RECHAZADO",
                    Fecha = DateTime.Now
                };

                var pagoCreado = await _pagoRepo.CrearPagoAsync(pago);

                var pagoTarjeta = new PagoTarjeta
                {
                    IdTransaccion = pagoCreado.IdTransaccion,
                    Cuotas = request.Cuotas,
                    TipoTarjeta = request.TipoTarjeta.ToUpper(),
                    UltimosDigitos = request.UltimosDigitos,
                    CodigoAutorizacion = aprobado ? Guid.NewGuid().ToString().Substring(0, 8).ToUpper() : null
                };

                await _pagoRepo.CrearPagoTarjetaAsync(pagoTarjeta);

                if (!aprobado)
                    return ApiResponse<PagarConTarjetaResponse>.Fail("Pago rechazado. Intente con otro medio de pago.");

                return ApiResponse<PagarConTarjetaResponse>.Ok(new PagarConTarjetaResponse
                {
                    IdTransaccion = pagoCreado.IdTransaccion,
                    Estado = pagoCreado.Estado,
                    CodigoAutorizacion = pagoTarjeta.CodigoAutorizacion
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<PagarConTarjetaResponse>.Fail($"Error al procesar pago: {ex.Message}");
            }
        }

        // CASO 59: Generar código de pago en efectivo
        public async Task<ApiResponse<GenerarCodigoPagoEfectivoResponse>> GenerarPagoEfectivoAsync(GenerarCodigoPagoEfectivoRequest request)
        {
            try
            {
                var carro = await _carroRepo.GetByIdAsync(request.IdCarro);
                if (carro == null)
                    return ApiResponse<GenerarCodigoPagoEfectivoResponse>.Fail("Carro no encontrado");

                if (request.Monto <= 0)
                    return ApiResponse<GenerarCodigoPagoEfectivoResponse>.Fail("El monto debe ser mayor a cero");

                var codigoPago = $"PE-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

                var pago = new Pago
                {
                    IdCarro = request.IdCarro,
                    IdMedio = request.IdMedio,
                    Monto = request.Monto,
                    Estado = "PENDIENTE",
                    Fecha = DateTime.Now
                };

                var pagoCreado = await _pagoRepo.CrearPagoAsync(pago);

                var pagoEfectivo = new PagoEfectivo
                {
                    IdTransaccion = pagoCreado.IdTransaccion,
                    CodigoPago = codigoPago,
                    FechaVencimiento = DateTime.Now.AddHours(24)
                };

                await _pagoRepo.CrearPagoEfectivoAsync(pagoEfectivo);

                return ApiResponse<GenerarCodigoPagoEfectivoResponse>.Ok(new GenerarCodigoPagoEfectivoResponse
                {
                    IdTransaccion = pagoCreado.IdTransaccion,
                    CodigoPago = codigoPago,
                    Monto = request.Monto,
                    FechaVencimiento = pagoEfectivo.FechaVencimiento
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<GenerarCodigoPagoEfectivoResponse>.Fail($"Error al generar código: {ex.Message}");
            }
        }
    }
}