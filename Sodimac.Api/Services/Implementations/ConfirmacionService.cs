using Sodimac.Api.Dtos;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;
using Sodimac.Api.Services.Interfaces;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Implementations
{
    public class ConfirmacionService : IConfirmacionService
    {
        private readonly IOrdenRepository _ordenRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly IComprobanteRepository _comprobanteRepo;
        private readonly INotificacionRepository _notificacionRepo;

        public ConfirmacionService(
            IOrdenRepository ordenRepo,
            IPagoRepository pagoRepo,
            IComprobanteRepository comprobanteRepo,
            INotificacionRepository notificacionRepo)
        {
            _ordenRepo = ordenRepo;
            _pagoRepo = pagoRepo;
            _comprobanteRepo = comprobanteRepo;
            _notificacionRepo = notificacionRepo;
        }

        // CASO 60: Registrar orden de compra
        public async Task<ApiResponse<RegistrarOrdenResponse>> RegistrarOrdenAsync(RegistrarOrdenRequest request)
        {
            try
            {
                var pago = await _pagoRepo.GetPagoByIdAsync(request.IdTransaccion);
                if (pago == null)
                    return ApiResponse<RegistrarOrdenResponse>.Fail("Transacción de pago no encontrada");

                if (pago.Estado != "APROBADO" && pago.Estado != "PENDIENTE")
                    return ApiResponse<RegistrarOrdenResponse>.Fail("El pago no está aprobado o pendiente");

                var nroOrden = await _ordenRepo.GenerarNumeroOrdenAsync();

                var orden = new Orden
                {
                    NroOrden = nroOrden,
                    IdCarro = pago.IdCarro,
                    IdComprador = request.IdComprador,
                    IdTransaccion = request.IdTransaccion,
                    Estado = pago.Estado == "APROBADO" ? "PAGADA" : "PENDIENTE_PAGO",
                    FechaCreacion = DateTime.Now,
                    Total = pago.Monto,
                    PuntosAcumulados = (int)(pago.Monto * 0.01m) // 1% en puntos CMR
                };

                var ordenCreada = await _ordenRepo.CrearAsync(orden);

                return ApiResponse<RegistrarOrdenResponse>.Ok(new RegistrarOrdenResponse
                {
                    NroOrden = ordenCreada.NroOrden,
                    Estado = ordenCreada.Estado,
                    Total = ordenCreada.Total,
                    PuntosAcumulados = ordenCreada.PuntosAcumulados
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<RegistrarOrdenResponse>.Fail($"Error al registrar orden: {ex.Message}");
            }
        }

        // CASO 61: Enviar confirmación de compra
        public async Task<ApiResponse<EnviarConfirmacionResponse>> EnviarConfirmacionAsync(EnviarConfirmacionRequest request)
        {
            try
            {
                var orden = await _ordenRepo.GetByNroOrdenAsync(request.NroOrden);
                if (orden == null)
                    return ApiResponse<EnviarConfirmacionResponse>.Fail("Orden no encontrada");

                var asunto = $"Confirmación de compra - Orden {request.NroOrden}";
                var cuerpo = $"Su orden {request.NroOrden} ha sido confirmada. Total: S/ {orden.Total:F2}";

                var enviado = await _notificacionRepo.EnviarCorreoAsync(request.Correo, asunto, cuerpo);

                if (!enviado)
                    return ApiResponse<EnviarConfirmacionResponse>.Fail("No se pudo enviar el correo de confirmación");

                var notificacion = new NotificacionCorreo
                {
                    NroOrden = request.NroOrden,
                    Correo = request.Correo,
                    Asunto = asunto,
                    FechaEnvio = DateTime.Now,
                    Estado = "ENVIADO"
                };

                var registrada = await _notificacionRepo.RegistrarAsync(notificacion);

                return ApiResponse<EnviarConfirmacionResponse>.Ok(new EnviarConfirmacionResponse
                {
                    IdNotificacion = registrada.IdNotificacion,
                    Mensaje = $"Confirmación enviada a {request.Correo}"
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<EnviarConfirmacionResponse>.Fail($"Error al enviar confirmación: {ex.Message}");
            }
        }

        // CASO 62: Emitir comprobante electrónico
        public async Task<ApiResponse<EmitirComprobanteResponse>> EmitirComprobanteAsync(EmitirComprobanteRequest request)
        {
            try
            {
                var orden = await _ordenRepo.GetByNroOrdenAsync(request.NroOrden);
                if (orden == null)
                    return ApiResponse<EmitirComprobanteResponse>.Fail("Orden no encontrada");

                var comprobante = await _comprobanteRepo.GetByNroOrdenAsync(request.NroOrden);
                if (comprobante == null)
                    return ApiResponse<EmitirComprobanteResponse>.Fail("Comprobante no encontrado para esta orden");

                // Simulación de emisión de comprobante electrónico (SUNAT)
                var serie = comprobante.Tipo == "FACTURA" ? "F001" : "B001";
                var numero = DateTime.Now.ToString("yyyyMMddHHmmss");
                var archivoPdf = $"comprobantes/{serie}-{numero}.pdf";

                comprobante.Serie = serie;
                comprobante.Numero = numero;
                comprobante.ArchivoPdf = archivoPdf;
                comprobante.FechaEmision = DateTime.Now;
                comprobante.NroOrden = request.NroOrden;

                await _comprobanteRepo.ActualizarAsync(comprobante);

                return ApiResponse<EmitirComprobanteResponse>.Ok(new EmitirComprobanteResponse
                {
                    IdComprobante = comprobante.IdComprobante,
                    Tipo = comprobante.Tipo,
                    Serie = serie,
                    Numero = numero,
                    ArchivoPdf = archivoPdf,
                    FechaEmision = comprobante.FechaEmision.Value
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<EmitirComprobanteResponse>.Fail($"Error al emitir comprobante: {ex.Message}");
            }
        }
    }
}