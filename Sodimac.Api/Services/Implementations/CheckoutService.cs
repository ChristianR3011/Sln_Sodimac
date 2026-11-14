using Sodimac.Api.Dtos;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;
using Sodimac.Api.Services.Interfaces;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IComprobanteRepository _comprobanteRepo;
        private readonly ICarroRepository _carroRepo;
        private readonly IPagoRepository _pagoRepo;

        public CheckoutService(IComprobanteRepository comprobanteRepo, ICarroRepository carroRepo, IPagoRepository pagoRepo)
        {
            _comprobanteRepo = comprobanteRepo;
            _carroRepo = carroRepo;
            _pagoRepo = pagoRepo;
        }

        // CASO 55: Registrar tipo de comprobante
        public async Task<ApiResponse<RegistrarComprobanteResponse>> RegistrarComprobanteAsync(RegistrarComprobanteRequest request)
        {
            try
            {
                // Validaciones según CONSTRAINT CK_comprobante_factura
                if (request.Tipo.Equals("FACTURA", StringComparison.OrdinalIgnoreCase))
                {
                    if (request.NroDocumentoRuc.Length != 11 || !request.NroDocumentoRuc.All(char.IsDigit))
                        return ApiResponse<RegistrarComprobanteResponse>.Fail("El RUC debe tener 11 dígitos numéricos");
                    if (string.IsNullOrWhiteSpace(request.RazonSocial))
                        return ApiResponse<RegistrarComprobanteResponse>.Fail("La razón social es obligatoria para factura");
                    if (string.IsNullOrWhiteSpace(request.DireccionFiscal))
                        return ApiResponse<RegistrarComprobanteResponse>.Fail("La dirección fiscal es obligatoria para factura");
                }

                var comprobante = new Comprobante
                {
                    IdCarro = request.IdCarro,
                    Tipo = request.Tipo.ToUpper(),
                    NroDocumentoRuc = request.NroDocumentoRuc,
                    RazonSocial = request.RazonSocial,
                    DireccionFiscal = request.DireccionFiscal
                };

                var creado = await _comprobanteRepo.CrearAsync(comprobante);

                return ApiResponse<RegistrarComprobanteResponse>.Ok(new RegistrarComprobanteResponse
                {
                    IdComprobante = creado.IdComprobante,
                    MensajeConfirmacion = $"Comprobante tipo '{creado.Tipo}' registrado correctamente."
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<RegistrarComprobanteResponse>.Fail($"Error al registrar comprobante: {ex.Message}");
            }
        }

        // CASO 56: Obtener resumen de compra
        public async Task<ApiResponse<ResumenCompraDto>> GetResumenCompraAsync(ObtenerResumenRequest request)
        {
            try
            {
                var carro = await _carroRepo.GetByIdAsync(request.IdCarro);
                if (carro == null)
                    return ApiResponse<ResumenCompraDto>.Fail("Carro no encontrado");

                var itemsConServicios = await _carroRepo.GetItemsConServiciosAsync(request.IdCarro);

                var response = new ResumenCompraDto
                {
                    Items = itemsConServicios.Select(x => new ItemResumenDto
                    {
                        Sku = x.Item.Sku,
                        Nombre = x.Item.Sku, // Se obtendría de tabla producto
                        Imagen = $"https://cdn.sodimac.com/{x.Item.Sku}.jpg",
                        Vendedor = "SODIMAC", // Se obtendría de tabla producto
                        Cantidad = x.Item.Cantidad,
                        PrecioUnitario = x.Item.PrecioUnitario,
                        Subtotal = x.Item.Subtotal,
                        Servicios = x.Servicios.Select(s => new ServicioAdicionalDto
                        {
                            IdServicio = s.IdServicio,
                            Nombre = "Servicio", // Se obtendría de tabla servicio_adicional
                            Precio = s.Precio
                        }).ToList()
                    }).ToList(),
                    Subtotal = carro.Subtotal,
                    Descuentos = carro.Descuentos,
                    Total = carro.Total
                };

                return ApiResponse<ResumenCompraDto>.Ok(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ResumenCompraDto>.Fail($"Error al obtener resumen: {ex.Message}");
            }
        }

        // CASO 57: Lista de medios de pago
        public async Task<ApiResponse<ListaMediosPagoResponse>> GetMediosPagoAsync(ListaMediosPagoRequest request)
        {
            try
            {
                var medios = await _pagoRepo.GetAllMediosPagoAsync();

                var response = new ListaMediosPagoResponse
                {
                    MediosDePago = medios.Select(m => new MedioPagoDto
                    {
                        IdMedio = m.IdMedio,
                        Nombre = m.Nombre,
                        CuotasDisponibles = !string.IsNullOrEmpty(m.CuotasDisponibles)
                            ? m.CuotasDisponibles.Split(',').Select(c => int.Parse(c.Trim())).ToList()
                            : new List<int>()
                    }).ToList()
                };

                return ApiResponse<ListaMediosPagoResponse>.Ok(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ListaMediosPagoResponse>.Fail($"Error al obtener medios de pago: {ex.Message}");
            }
        }
    }
}