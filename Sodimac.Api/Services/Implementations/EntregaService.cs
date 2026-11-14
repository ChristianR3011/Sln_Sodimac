using Sodimac.Api.Dtos;
using Sodimac.Api.Models;
using Sodimac.Api.Repositories.Interfaces;
using Sodimac.Api.Services.Interfaces;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Implementations
{
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _entregaRepo;
        private readonly ICarroRepository _carroRepo;

        public EntregaService(IEntregaRepository entregaRepo, ICarroRepository carroRepo)
        {
            _entregaRepo = entregaRepo;
            _carroRepo = carroRepo;
        }

        // CASO 51: Lista de fechas y horarios de despacho
        public async Task<ApiResponse<ListaFechasHorariosResponse>> GetFechasYHorariosAsync(ListaFechasHorariosRequest request)
        {
            try
            {
                var carro = await _carroRepo.GetByIdAsync(request.IdCarro);
                if (carro == null)
                    return ApiResponse<ListaFechasHorariosResponse>.Fail("Carro no encontrado");

                var idZona = await _entregaRepo.GetIdZonaByDireccionAsync(request.IdDireccion);
                if (idZona == 0)
                    return ApiResponse<ListaFechasHorariosResponse>.Fail("No se encontró zona para la dirección");

                var cobertura = await _entregaRepo.VerificarCoberturaAsync(idZona);
                if (!cobertura)
                    return ApiResponse<ListaFechasHorariosResponse>.Fail("La zona no tiene cobertura de despacho");

                var horarios = await _entregaRepo.GetHorariosDisponiblesAsync(idZona);

                var response = new ListaFechasHorariosResponse
                {
                    FechasDisponibles = horarios
                        .Where(h => h.Capacidad > 0)
                        .GroupBy(h => h.Fecha)
                        .Select(g => new FechaDespachoDto
                        {
                            Fecha = g.Key.ToDateTime(TimeOnly.MinValue),
                            RangosHorarios = g.Select(h => new RangoHorarioDto
                            {
                                IdHorario = h.IdHorario,
                                RangoHorario = h.RangoHorario,
                                Costo = h.Costo,
                                CapacidadDisponible = h.Capacidad
                            }).ToList()
                        }).OrderBy(f => f.Fecha).ToList()
                };

                return ApiResponse<ListaFechasHorariosResponse>.Ok(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ListaFechasHorariosResponse>.Fail($"Error al obtener fechas: {ex.Message}");
            }
        }

        // CASO 52: Seleccionar despacho a domicilio
        public async Task<ApiResponse<SeleccionarDespachoResponse>> SeleccionarDespachoDomicilioAsync(SeleccionarDespachoRequest request)
        {
            try
            {
                var idZona = await _entregaRepo.GetIdZonaByDireccionAsync(request.IdDireccion);
                var cobertura = await _entregaRepo.VerificarCoberturaAsync(idZona);
                if (!cobertura)
                    return ApiResponse<SeleccionarDespachoResponse>.Fail("La dirección no está dentro de la zona de cobertura");

                var entrega = new Entrega
                {
                    IdCarro = request.IdCarro,
                    IdTipoEntrega = 1, // Domicilio
                    IdDireccion = request.IdDireccion,
                    IdHorario = request.IdHorario,
                    CostoEnvio = 0 // Se calcula en repositorio
                };

                var entregaCreada = await _entregaRepo.CrearEntregaAsync(entrega);

                return ApiResponse<SeleccionarDespachoResponse>.Ok(new SeleccionarDespachoResponse
                {
                    IdEntrega = entregaCreada.IdEntrega,
                    CostoEnvio = entregaCreada.CostoEnvio,
                    FechaEstimada = entregaCreada.FechaEstimada?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddDays(2)
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<SeleccionarDespachoResponse>.Fail($"Error al seleccionar despacho: {ex.Message}");
            }
        }

        // CASO 53: Lista de tiendas para retiro
        public async Task<ApiResponse<ListaTiendasRetiroResponse>> GetTiendasRetiroAsync(ListaTiendasRetiroRequest request)
        {
            try
            {
                var tiendas = await _entregaRepo.GetTiendasByZonaAsync(request.IdZona);
                var stockTiendas = await _entregaRepo.GetStockTiendasAsync(request.IdCarro, tiendas.Select(t => t.IdTienda).ToList());

                var response = new ListaTiendasRetiroResponse
                {
                    Tiendas = tiendas.Select(t =>
                    {
                        var stock = stockTiendas.FirstOrDefault(s => s.IdTienda == t.IdTienda);
                        var tieneStock = stock?.Disponible == true;
                        return new TiendaRetiroDto
                        {
                            IdTienda = t.IdTienda,
                            Nombre = t.Nombre,
                            Tipo = t.Tipo,
                            Direccion = t.Direccion,
                            Horario = t.Horario,
                            TiempoRetiro = tieneStock ? "Retira desde 120 min" : "2-3 días",
                            TieneStock = tieneStock
                        };
                    }).ToList()
                };

                return ApiResponse<ListaTiendasRetiroResponse>.Ok(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ListaTiendasRetiroResponse>.Fail($"Error al obtener tiendas: {ex.Message}");
            }
        }

        // CASO 54: Seleccionar retiro en tienda
        public async Task<ApiResponse<SeleccionarRetiroTiendaResponse>> SeleccionarRetiroTiendaAsync(SeleccionarRetiroTiendaRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NombreRetira) || string.IsNullOrWhiteSpace(request.NroDocRetira))
                    return ApiResponse<SeleccionarRetiroTiendaResponse>.Fail("Nombre y documento de quien retira son obligatorios");

                var entrega = new Entrega
                {
                    IdCarro = request.IdCarro,
                    IdTipoEntrega = 3, // Retiro en tienda
                    IdTienda = request.IdTienda,
                    NombreRetira = request.NombreRetira,
                    NroDocRetira = request.NroDocRetira,
                    CostoEnvio = 0
                };

                var entregaCreada = await _entregaRepo.CrearEntregaAsync(entrega);

                return ApiResponse<SeleccionarRetiroTiendaResponse>.Ok(new SeleccionarRetiroTiendaResponse
                {
                    IdEntrega = entregaCreada.IdEntrega,
                    FechaEstimadaRetiro = entregaCreada.FechaEstimada?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now.AddMinutes(120)
                });
            }
            catch (Exception ex)
            {
                return ApiResponse<SeleccionarRetiroTiendaResponse>.Fail($"Error al seleccionar retiro: {ex.Message}");
            }
        }
    }
}