using Sodimac.Api.Dtos;
using Sodimac.Api.Data;
using Sodimac.Api.Models;
using Sodimac.Api.Services.Interfaces;
using Sodimac.Api.Tools;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Sodimac.Api.Services.Implementations
{
    public class ClienteCarroService : IClienteCarroService
    {
        private readonly MongoDataContext _mongo;

        public ClienteCarroService(MongoDataContext mongo)
        {
            _mongo = mongo;
        }

        private static readonly HashSet<string> ZonasCobertura = new(StringComparer.OrdinalIgnoreCase)
        {
            "Lima",
            "Callao"
        };

        private static readonly Dictionary<string, ProductoDemo> Productos = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SKU-1001"] = new ProductoDemo
            {
                Sku = "SKU-1001",
                Nombre = "Taladro percutor 800W",
                Vendedor = "Sodimac",
                Precio = 259.90m,
                StockPorZona = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Lima"] = 50,
                    ["Callao"] = 30
                },
                Imagen = ""
            },
            ["SKU-2002"] = new ProductoDemo
            {
                Sku = "SKU-2002",
                Nombre = "Pintura látex 1 gal",
                Vendedor = "Sodimac",
                Precio = 74.50m,
                StockPorZona = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Lima"] = 120,
                    ["Callao"] = 80
                },
                Imagen = ""
            }
        };

        private static readonly Dictionary<string, ServicioDemo> Servicios = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SRV-ARMADO"] = new ServicioDemo { IdServicio = "SRV-ARMADO", Nombre = "Armado a domicilio", Precio = 40m },
        };

        private static readonly Dictionary<string, CuponDemo> Cupones = new(StringComparer.OrdinalIgnoreCase)
        {
            ["CMR10"] = new CuponDemo { CodigoCupon = "CMR10", Porcentaje = 0.10m, MedioPago = "cmr", MontoMinimo = 0m }
        };

        public async Task<ApiResponse<ClienteDetalleDto>> GetClienteAsync(string idCliente)
        {
            if (!int.TryParse(idCliente, out var clienteId))
                return ApiResponse<ClienteDetalleDto>.Fail("Cliente no encontrado");

            var cliente = await _mongo.Collection<Cliente>("clientes")
                .Find(c => c.IdCliente == clienteId)
                .FirstOrDefaultAsync();
            if (cliente is null)
                return ApiResponse<ClienteDetalleDto>.Fail("Cliente no encontrado");

            return ApiResponse<ClienteDetalleDto>.Ok(new ClienteDetalleDto
            {
                TipoDocumento = cliente.TipoDocumento,
                NumeroDocumento = cliente.NroDocumento,
                Nombres = cliente.Nombres,
                Apellidos = cliente.Apellidos,
                Correo = cliente.Correo,
                Celular = cliente.Celular
            });
        }

        public async Task<ApiResponse<ActualizarClienteResponse>> ActualizarClienteAsync(string idCliente, ActualizarClienteRequest request)
        {
            if (!int.TryParse(idCliente, out var clienteId))
                return ApiResponse<ActualizarClienteResponse>.Fail("Cliente no encontrado");

            var collection = _mongo.Collection<Cliente>("clientes");
            var cliente = await collection.Find(c => c.IdCliente == clienteId).FirstOrDefaultAsync();
            if (cliente is null)
                return ApiResponse<ActualizarClienteResponse>.Fail("Cliente no encontrado");

            var updates = new List<UpdateDefinition<Cliente>>();
            if (request.Nombres is not null)
                updates.Add(Builders<Cliente>.Update.Set(c => c.Nombres, request.Nombres));
            if (request.Apellidos is not null)
                updates.Add(Builders<Cliente>.Update.Set(c => c.Apellidos, request.Apellidos));
            if (request.Celular is not null)
                updates.Add(Builders<Cliente>.Update.Set(c => c.Celular, request.Celular));

            if (!string.IsNullOrWhiteSpace(request.NuevaContrasena))
            {
                if (string.IsNullOrWhiteSpace(request.ContrasenaActual) || !VerifyPassword(request.ContrasenaActual, cliente.ContrasenaHash))
                    return ApiResponse<ActualizarClienteResponse>.Fail("La contraseña actual es incorrecta");

                if (!ValidarPassword(request.NuevaContrasena))
                    return ApiResponse<ActualizarClienteResponse>.Fail("La nueva contraseña no cumple los requisitos de seguridad");

                updates.Add(Builders<Cliente>.Update.Set(
                    c => c.ContrasenaHash,
                    HashPassword(request.NuevaContrasena)));
            }

            if (updates.Count > 0)
                await collection.UpdateOneAsync(
                    c => c.IdCliente == clienteId,
                    Builders<Cliente>.Update.Combine(updates));

            return ApiResponse<ActualizarClienteResponse>.Ok(new ActualizarClienteResponse
            {
                Mensaje = "Datos actualizados correctamente"
            });
        }

        public async Task<ApiResponse<List<DireccionClienteDto>>> GetDireccionesAsync(string idCliente)
        {
            if (!int.TryParse(idCliente, out var clienteId))
                return ApiResponse<List<DireccionClienteDto>>.Fail("El ID del cliente no es válido");

            var collection = _mongo.Collection<BsonDocument>("direcciones");
            var direcciones = await collection.Find(BuildClientAddressFilter(clienteId))
                .ToListAsync();

            return ApiResponse<List<DireccionClienteDto>>.Ok(
                direcciones.Select(MapDireccion)
                    .OrderByDescending(d => d.Principal)
                    .ThenBy(d => int.TryParse(d.IdDireccion, out var id) ? id : int.MaxValue)
                    .ToList());
        }

        public async Task<ApiResponse<DireccionClienteDto>> RegistrarDireccionAsync(string idCliente, RegistrarDireccionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Direccion) || string.IsNullOrWhiteSpace(request.Departamento) || string.IsNullOrWhiteSpace(request.Provincia) || string.IsNullOrWhiteSpace(request.Distrito))
                return ApiResponse<DireccionClienteDto>.Fail("Faltan datos obligatorios de la dirección");

            if (!int.TryParse(idCliente, out var clienteId))
                return ApiResponse<DireccionClienteDto>.Fail("El ID del cliente no es válido");

            var clienteExiste = await _mongo.Collection<Cliente>("clientes")
                .Find(c => c.IdCliente == clienteId)
                .AnyAsync();
            if (!clienteExiste)
                return ApiResponse<DireccionClienteDto>.Fail("Cliente no encontrado");

            var collection = _mongo.Collection<BsonDocument>("direcciones");
            var distrito = await _mongo.Collection<Distrito>("distritos")
                .Find(d => d.Nombre == request.Distrito)
                .FirstOrDefaultAsync();
            var zonaTieneCobertura = distrito is not null
                && await _mongo.Collection<Zona>("zonas")
                    .Find(z => z.IdDistrito == distrito.IdDistrito && z.TieneCobertura)
                    .AnyAsync();
            if (!zonaTieneCobertura && !ZonasCobertura.Contains(request.Distrito))
                return ApiResponse<DireccionClienteDto>.Fail("El distrito no tiene cobertura de despacho");

            var yaTiene = await collection.Find(BuildClientAddressFilter(clienteId)).AnyAsync();
            var lastDocument = await collection.Find(Builders<BsonDocument>.Filter.Empty)
                .Sort(new BsonDocument("_id", -1))
                .FirstOrDefaultAsync();
            var maxId = lastDocument is null ? 0 : GetInt32(lastDocument, "_id", "IdDireccion", "idDireccion");
            await _mongo.InitializeSequenceAsync("direcciones", maxId);

            var direccion = new BsonDocument
            {
                ["_id"] = await _mongo.NextIdAsync("direcciones"),
                ["IdCliente"] = clienteId,
                ["IdDistrito"] = distrito?.IdDistrito ?? 0,
                ["Nombre"] = request.Nombre ?? "Casa",
                ["Direccion1"] = request.Direccion,
                ["Referencia"] = request.Referencia is null ? BsonNull.Value : new BsonString(request.Referencia),
                ["DepartamentoNombre"] = request.Departamento,
                ["ProvinciaNombre"] = request.Provincia,
                ["DistritoNombre"] = request.Distrito,
                ["Principal"] = !yaTiene
            };

            await collection.InsertOneAsync(direccion);
            return ApiResponse<DireccionClienteDto>.Ok(MapDireccion(direccion));
        }

        public async Task<ApiResponse<DireccionClienteDto>> ActualizarDireccionAsync(string idCliente, string idDireccion, ActualizarDireccionRequest request)
        {
            if (!int.TryParse(idCliente, out var clienteId) || !int.TryParse(idDireccion, out var direccionId))
                return ApiResponse<DireccionClienteDto>.Fail("El ID del cliente o de la dirección no es válido");

            var collection = _mongo.Collection<BsonDocument>("direcciones");
            var filter = BuildClientAddressFilter(clienteId) & BuildAddressIdFilter(direccionId);
            var direccion = await collection.Find(filter)
                .FirstOrDefaultAsync();
            if (direccion is null)
                return ApiResponse<DireccionClienteDto>.Fail("Dirección no encontrada");

            var updates = new List<UpdateDefinition<BsonDocument>>();
            if (!string.IsNullOrWhiteSpace(request.Nombre))
                AddAddressUpdate(updates, direccion, request.Nombre, "Nombre", "tipo");
            if (!string.IsNullOrWhiteSpace(request.Direccion))
                AddAddressUpdate(updates, direccion, request.Direccion, "Direccion1", "direccion");
            if (request.Referencia is not null)
                AddAddressUpdate(updates, direccion, request.Referencia, "Referencia", "referencia");
            if (!string.IsNullOrWhiteSpace(request.Departamento))
                AddAddressUpdate(updates, direccion, request.Departamento, "DepartamentoNombre", "departamento");
            if (!string.IsNullOrWhiteSpace(request.Provincia))
                AddAddressUpdate(updates, direccion, request.Provincia, "ProvinciaNombre", "provincia");
            if (!string.IsNullOrWhiteSpace(request.Distrito))
            {
                var distrito = await _mongo.Collection<Distrito>("distritos")
                    .Find(d => d.Nombre == request.Distrito)
                    .FirstOrDefaultAsync();
                var zonaTieneCobertura = distrito is not null
                    && await _mongo.Collection<Zona>("zonas")
                        .Find(z => z.IdDistrito == distrito.IdDistrito && z.TieneCobertura)
                        .AnyAsync();
                if (!zonaTieneCobertura && !ZonasCobertura.Contains(request.Distrito))
                    return ApiResponse<DireccionClienteDto>.Fail("El distrito no tiene cobertura de despacho");

                AddAddressUpdate(updates, direccion, request.Distrito, "DistritoNombre", "distrito");
                updates.Add(Builders<BsonDocument>.Update.Set("IdDistrito", distrito?.IdDistrito ?? 0));
            }

            if (request.Principal == true)
            {
                await collection.UpdateManyAsync(
                    BuildClientAddressFilter(clienteId),
                    Builders<BsonDocument>.Update.Set("Principal", false));
            }

            if (request.Principal.HasValue)
                updates.Add(Builders<BsonDocument>.Update.Set("Principal", request.Principal.Value));

            if (updates.Count > 0)
            {
                await collection.UpdateOneAsync(filter, Builders<BsonDocument>.Update.Combine(updates));
                direccion = await collection.Find(filter)
                    .FirstOrDefaultAsync();
            }

            return ApiResponse<DireccionClienteDto>.Ok(MapDireccion(direccion));
        }

        public async Task<ApiResponse<bool>> EliminarDireccionAsync(string idCliente, string idDireccion)
        {
            if (!int.TryParse(idCliente, out var clienteId) || !int.TryParse(idDireccion, out var direccionId))
                return ApiResponse<bool>.Fail("Dirección no encontrada");

            var result = await _mongo.Collection<BsonDocument>("direcciones")
                .DeleteOneAsync(BuildClientAddressFilter(clienteId) & BuildAddressIdFilter(direccionId));
            if (result.DeletedCount == 0)
                return ApiResponse<bool>.Fail("Dirección no encontrada");

            return ApiResponse<bool>.Ok(true, "Dirección eliminada correctamente");
        }

        public async Task<ApiResponse<ProductoCarroResponse>> AgregarProductoAsync(string idCarro, ProductoCarroRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku) || !Productos.TryGetValue(request.Sku, out var producto))
                return ApiResponse<ProductoCarroResponse>.Fail("Producto no encontrado");

            if (!int.TryParse(idCarro, out _))
                return ApiResponse<ProductoCarroResponse>.Fail("El ID del carro debe ser numérico");

            if (request.Cantidad < 1 || request.Cantidad > 10)
                return ApiResponse<ProductoCarroResponse>.Fail("Cantidad inválida");

            var carro = await GetCarroAsync(idCarro)
                ?? new CarroDemo { IdCarro = idCarro, EsInvitado = true };

            var zona = string.IsNullOrWhiteSpace(request.IdZona) ? "Lima" : request.IdZona;
            var actual = carro.Items.TryGetValue(request.Sku, out var itemActual) ? itemActual.Cantidad : 0;
            var nuevaCantidad = actual + request.Cantidad;
            if (nuevaCantidad > 10)
                return ApiResponse<ProductoCarroResponse>.Fail("Máximo 10 unidades por producto");

            if (nuevaCantidad > (producto.StockPorZona.TryGetValue(zona, out var stock) ? stock : 0))
                return ApiResponse<ProductoCarroResponse>.Fail("Stock insuficiente para la zona");

            carro.Items[request.Sku] = new ItemCarroDemo
            {
                Sku = request.Sku,
                Cantidad = nuevaCantidad,
                Servicios = itemActual?.Servicios ?? new List<string>()
            };

            await SaveCarroAsync(carro);
            return ApiResponse<ProductoCarroResponse>.Ok(new ProductoCarroResponse
            {
                IdCarro = idCarro,
                CantidadTotal = carro.Items.Values.Sum(i => i.Cantidad)
            });
        }

        public async Task<ApiResponse<ResumenCarroDto>> GetCarroAsync(string idCarro, string? idZona = "Lima")
        {
            var carro = await GetCarroAsync(idCarro);
            if (carro is null)
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            var resumen = await CalcularResumenAsync(carro, idZona ?? "Lima");
            return ApiResponse<ResumenCarroDto>.Ok(resumen);
        }

        public async Task<ApiResponse<ResumenCarroDto>> ActualizarCantidadAsync(string idCarro, string sku, ActualizarCantidadItemRequest request)
        {
            var carro = await GetCarroAsync(idCarro);
            if (carro is null)
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            if (!carro.Items.TryGetValue(sku, out var item))
                return ApiResponse<ResumenCarroDto>.Fail("El producto no está en el carro");

            if (request.Cantidad < 0 || request.Cantidad > 10)
                return ApiResponse<ResumenCarroDto>.Fail("Cantidad inválida");

            if (request.Cantidad == 0)
            {
                carro.Items.Remove(sku);
            }
            else
            {
                var producto = Productos[sku];
                if (request.Cantidad > (producto.StockPorZona.TryGetValue("Lima", out var stock) ? stock : 0))
                    return ApiResponse<ResumenCarroDto>.Fail("Stock insuficiente");

                item.Cantidad = request.Cantidad;
            }

            await SaveCarroAsync(carro);
            return ApiResponse<ResumenCarroDto>.Ok(await CalcularResumenAsync(carro, "Lima"));
        }

        public async Task<ApiResponse<ResumenCarroDto>> EliminarProductoAsync(string idCarro, string sku)
        {
            var carro = await GetCarroAsync(idCarro);
            if (carro is null)
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            if (!carro.Items.Remove(sku))
                return ApiResponse<ResumenCarroDto>.Fail("El producto no está en el carro");

            await SaveCarroAsync(carro);
            return ApiResponse<ResumenCarroDto>.Ok(await CalcularResumenAsync(carro, "Lima"));
        }

        public async Task<ApiResponse<ResumenCarroDto>> AgregarServicioAsync(string idCarro, string sku, AgregarServicioRequest request)
        {
            var carro = await GetCarroAsync(idCarro);
            if (carro is null)
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            if (!carro.Items.TryGetValue(sku, out var item))
                return ApiResponse<ResumenCarroDto>.Fail("El producto no está en el carro");

            var servicio = await ObtenerServicioAsync(request.IdServicio);
            if (servicio is null)
                return ApiResponse<ResumenCarroDto>.Fail("Servicio no encontrado");

            var idServicio = servicio.IdServicio.StartsWith("SRV-", StringComparison.OrdinalIgnoreCase)
                ? servicio.IdServicio
                : $"SRV-{servicio.IdServicio}";
            carro.ServiciosCatalogo[idServicio] = servicio;
            if (!item.Servicios.Contains(idServicio, StringComparer.OrdinalIgnoreCase))
                item.Servicios.Add(idServicio);

            await SaveCarroAsync(carro);
            return ApiResponse<ResumenCarroDto>.Ok(await CalcularResumenAsync(carro, "Lima"));
        }

        public async Task<ApiResponse<CuponResponse>> AplicarCuponAsync(string idCarro, CuponRequest request)
        {
            var carro = await GetCarroAsync(idCarro);
            if (carro is null)
                return ApiResponse<CuponResponse>.Fail("Carro no encontrado");

            var codigoCupon = request.CodigoCupon?.Trim();
            if (string.IsNullOrWhiteSpace(codigoCupon))
                return ApiResponse<CuponResponse>.Fail("Cupón inexistente o vencido");

            var cupon = await _mongo.Collection<BsonDocument>("cupones")
                .Find(Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("Codigo", codigoCupon),
                    Builders<BsonDocument>.Filter.Eq("codigo", codigoCupon)))
                .FirstOrDefaultAsync();
            if (cupon is null)
                return ApiResponse<CuponResponse>.Fail("Cupón inexistente o vencido");
            if (cupon.TryGetValue("Activo", out var activo) && !activo.ToBoolean())
                return ApiResponse<CuponResponse>.Fail("Cupón inexistente o vencido");

            var medioPago = GetString(cupon, "MedioPagoRequerido", "MedioPago");
            if (!string.IsNullOrWhiteSpace(medioPago)
                && !string.Equals(medioPago, request.MedioPago, StringComparison.OrdinalIgnoreCase))
                return ApiResponse<CuponResponse>.Fail("El cupón no aplica con el medio de pago seleccionado");

            var resumen = await CalcularResumenAsync(carro, "Lima");
            var montoMinimo = GetDecimal(cupon, "MontoMinimo", "montoMinimo");
            if (resumen.Subtotal < montoMinimo)
                return ApiResponse<CuponResponse>.Fail("No cumple el monto mínimo");

            if (!int.TryParse(GetString(cupon, "_id", "IdCupon", "idCupon"), out var cuponId))
                return ApiResponse<CuponResponse>.Fail("El cupón no tiene un identificador válido");

            carro.IdCupon = cuponId;
            await SaveCarroAsync(carro);
            var descuento = await ObtenerDescuentoCuponAsync(cupon, resumen.Subtotal);
            return ApiResponse<CuponResponse>.Ok(new CuponResponse
            {
                DescuentoCupon = descuento,
                Total = Math.Round(resumen.Subtotal - resumen.Descuentos - descuento, 2)
            });
        }

        public async Task<ApiResponse<FusionCarroResponse>> FusionarCarroAsync(string idCliente, FusionCarroRequest request)
        {
            if (!int.TryParse(idCliente, out var clienteId))
                return ApiResponse<FusionCarroResponse>.Fail("El ID del cliente no es válido");

            var invitado = await GetCarroAsync(request.IdCarroInvitado);
            if (invitado is null)
                return ApiResponse<FusionCarroResponse>.Fail("Carro no encontrado");

            var carros = _mongo.Collection<Carro>("carros");
            var destinoDb = await carros.Find(c => c.IdCliente == clienteId && !c.EsInvitado)
                .FirstOrDefaultAsync();
            var destinoId = destinoDb?.IdCarro;
            if (destinoId is null)
            {
                var maxId = await carros.Find(Builders<Carro>.Filter.Empty)
                    .SortByDescending(c => c.IdCarro)
                    .Project(c => c.IdCarro)
                    .FirstOrDefaultAsync();
                await _mongo.InitializeSequenceAsync("carros", maxId);
                destinoId = await _mongo.NextIdAsync("carros");
            }

            var idCarroCliente = destinoId.Value.ToString();
            var destino = await GetCarroAsync(idCarroCliente)
                ?? new CarroDemo { IdCarro = idCarroCliente, IdCliente = clienteId, EsInvitado = false };
            foreach (var item in invitado.Items)
            {
                if (!destino.Items.TryGetValue(item.Key, out var actual))
                {
                    destino.Items[item.Key] = new ItemCarroDemo
                    {
                        Sku = item.Key,
                        Cantidad = item.Value.Cantidad,
                        Servicios = new List<string>(item.Value.Servicios)
                    };
                    continue;
                }

                actual.Cantidad = Math.Min(10, actual.Cantidad + item.Value.Cantidad);
                actual.Servicios = actual.Servicios.Concat(item.Value.Servicios).Distinct().ToList();
            }

            destino.IdCliente = clienteId;
            destino.EsInvitado = false;
            await SaveCarroAsync(destino);

            if (!string.Equals(invitado.IdCarro, destino.IdCarro, StringComparison.Ordinal))
                await EliminarCarroAsync(invitado.IdCarro);

            return ApiResponse<FusionCarroResponse>.Ok(new FusionCarroResponse { IdCarro = idCarroCliente });
        }

        public async Task<bool> TieneSesionValidaAsync(string? authorizationHeader, string idCliente)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return true;

            var token = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader[7..].Trim()
                : authorizationHeader.Trim();

            if (string.IsNullOrWhiteSpace(token))
                return true;

            if (!int.TryParse(idCliente, out var clienteId))
                return false;

            var sesion = await _mongo.Collection<Sesion>("sesiones")
                .Find(s => s.Token == token && s.Activa && s.IdCliente == clienteId && s.FechaExpiracion > DateTime.UtcNow)
                .FirstOrDefaultAsync();
            return sesion is not null;
        }

        public async Task<ApiResponse<string>> CrearSesionAsync(string idCliente)
        {
            if (!int.TryParse(idCliente, out var clienteId)
                || !await _mongo.Collection<Cliente>("clientes").Find(c => c.IdCliente == clienteId).AnyAsync())
                return ApiResponse<string>.Fail("Cliente no encontrado");

            var token = Guid.NewGuid().ToString("N");
            var collection = _mongo.Collection<Sesion>("sesiones");
            var maxId = await collection.Find(Builders<Sesion>.Filter.Empty)
                .SortByDescending(s => s.IdSesion)
                .Project(s => s.IdSesion)
                .FirstOrDefaultAsync();
            await _mongo.InitializeSequenceAsync("sesiones", maxId);
            await collection.InsertOneAsync(new Sesion
            {
                IdSesion = await _mongo.NextIdAsync("sesiones"),
                IdCliente = clienteId,
                Token = token,
                FechaEmision = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddHours(8),
                Activa = true
            });
            return ApiResponse<string>.Ok(token);
        }

        private async Task<CarroDemo?> GetCarroAsync(string idCarro)
        {
            if (!int.TryParse(idCarro, out var carroId))
                return null;

            var carroDb = await _mongo.Collection<Carro>("carros")
                .Find(c => c.IdCarro == carroId)
                .FirstOrDefaultAsync();
            if (carroDb is null)
                return null;

            var lineas = await _mongo.Collection<ItemCarro>("itemsCarro")
                .Find(i => i.IdCarro == carroId)
                .ToListAsync();
            var itemIds = lineas.Select(i => i.IdItem).ToList();
            var servicios = itemIds.Count == 0
                ? new List<ItemServicio>()
                : await _mongo.Collection<ItemServicio>("itemsServicio")
                    .Find(s => itemIds.Contains(s.IdItem))
                    .ToListAsync();
            var catalogoServicios = servicios.Count == 0
                ? new List<ServicioAdicional>()
                : await _mongo.Collection<ServicioAdicional>("serviciosAdicionales")
                    .Find(s => servicios.Select(link => link.IdServicio).Distinct().Contains(s.IdServicio))
                    .ToListAsync();

            var carro = new CarroDemo
            {
                IdCarro = carroId.ToString(),
                IdCliente = carroDb.IdCliente,
                IdCupon = carroDb.IdCupon,
                EsInvitado = carroDb.EsInvitado
            };

            foreach (var linea in lineas)
            {
                if (!Productos.ContainsKey(linea.Sku))
                {
                    carro.ProductosCatalogo[linea.Sku] = new ProductoDemo
                    {
                        Sku = linea.Sku,
                        Nombre = linea.Sku,
                        Vendedor = "Sodimac",
                        Precio = linea.PrecioUnitario,
                        StockPorZona = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    };
                }

                var item = new ItemCarroDemo { Sku = linea.Sku, Cantidad = linea.Cantidad };
                foreach (var servicio in servicios.Where(s => s.IdItem == linea.IdItem))
                {
                    var serviceKey = $"SRV-{servicio.IdServicio}";
                    item.Servicios.Add(serviceKey);
                    var serviceDb = catalogoServicios.FirstOrDefault(s => s.IdServicio == servicio.IdServicio);
                    if (serviceDb is not null)
                    {
                        carro.ServiciosCatalogo[serviceKey] = new ServicioDemo
                        {
                            IdServicio = serviceKey,
                            Nombre = serviceDb.Nombre,
                            Precio = servicio.Precio
                        };
                    }
                }

                carro.Items[linea.Sku] = item;
            }

            return carro;
        }

        private async Task SaveCarroAsync(CarroDemo carro)
        {
            if (!int.TryParse(carro.IdCarro, out var carroId))
                throw new InvalidOperationException("El ID del carro debe ser numérico para guardarlo en MongoDB.");

            var carros = _mongo.Collection<Carro>("carros");
            var carroDb = await carros.Find(c => c.IdCarro == carroId).FirstOrDefaultAsync()
                ?? new Carro
                {
                    IdCarro = carroId,
                    FechaCreacion = DateTime.UtcNow,
                    EsInvitado = carro.EsInvitado
                };
            carroDb.IdCliente = carro.IdCliente ?? carroDb.IdCliente;
            carroDb.EsInvitado = carro.IdCliente.HasValue ? false : carro.EsInvitado;
            carroDb.IdCupon = carro.IdCupon ?? carroDb.IdCupon;

            var lineas = _mongo.Collection<ItemCarro>("itemsCarro");
            var currentLines = await lineas.Find(i => i.IdCarro == carroId).ToListAsync();
            var skuActuales = carro.Items.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var removidas = currentLines.Where(i => !skuActuales.Contains(i.Sku)).ToList();
            if (removidas.Count > 0)
            {
                var removedIds = removidas.Select(i => i.IdItem).ToList();
                await _mongo.Collection<ItemServicio>("itemsServicio")
                    .DeleteManyAsync(s => removedIds.Contains(s.IdItem));
                await lineas.DeleteManyAsync(i => removedIds.Contains(i.IdItem));
            }

            var maxItemId = await lineas.Find(Builders<ItemCarro>.Filter.Empty)
                .SortByDescending(i => i.IdItem)
                .Project(i => i.IdItem)
                .FirstOrDefaultAsync();
            await _mongo.InitializeSequenceAsync("itemsCarro", maxItemId);

            foreach (var item in carro.Items.Values)
            {
                var existing = currentLines.FirstOrDefault(i =>
                    string.Equals(i.Sku, item.Sku, StringComparison.OrdinalIgnoreCase));
                var product = Productos.TryGetValue(item.Sku, out var knownProduct)
                    ? knownProduct
                    : carro.ProductosCatalogo.GetValueOrDefault(item.Sku);
                var unitPrice = product?.Precio ?? existing?.PrecioUnitario ?? 0m;
                var line = existing ?? new ItemCarro
                {
                    IdItem = await _mongo.NextIdAsync("itemsCarro"),
                    IdCarro = carroId,
                    Sku = item.Sku
                };
                line.Cantidad = item.Cantidad;
                line.PrecioUnitario = unitPrice;
                line.Subtotal = unitPrice * item.Cantidad;

                if (existing is null)
                    await lineas.InsertOneAsync(line);
                else
                    await lineas.ReplaceOneAsync(i => i.IdItem == line.IdItem, line);

                var serviceLinks = _mongo.Collection<ItemServicio>("itemsServicio");
                await serviceLinks.DeleteManyAsync(s => s.IdItem == line.IdItem);
                foreach (var serviceKey in item.Servicios.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var service = await ObtenerServicioAsync(serviceKey);
                    if (service is null)
                        continue;

                    carro.ServiciosCatalogo[$"SRV-{service.IdServicio}"] = service;
                    var maxServiceLinkId = await serviceLinks.Find(Builders<ItemServicio>.Filter.Empty)
                        .SortByDescending(s => s.IdItemServicio)
                        .Project(s => s.IdItemServicio)
                        .FirstOrDefaultAsync();
                    await _mongo.InitializeSequenceAsync("itemsServicio", maxServiceLinkId);
                    await serviceLinks.InsertOneAsync(new ItemServicio
                    {
                        IdItemServicio = await _mongo.NextIdAsync("itemsServicio"),
                        IdItem = line.IdItem,
                        IdServicio = int.Parse(service.IdServicio.Replace("SRV-", string.Empty)),
                        Precio = service.Precio
                    });
                }
            }

            var summary = await CalcularResumenAsync(carro, "Lima");
            carroDb.Subtotal = summary.Subtotal;
            carroDb.Descuentos = summary.Descuentos;
            carroDb.Total = summary.Total;

            var existingCart = await carros.Find(c => c.IdCarro == carroId).AnyAsync();
            if (existingCart)
                await carros.ReplaceOneAsync(c => c.IdCarro == carroId, carroDb);
            else
                await carros.InsertOneAsync(carroDb);
        }

        private async Task<ResumenCarroDto> CalcularResumenAsync(CarroDemo carro, string idZona)
        {
            var summary = CalcularResumen(carro, idZona);
            if (carro.IdCupon is not int couponId)
                return summary;

            var coupon = await _mongo.Collection<BsonDocument>("cupones")
                .Find(Builders<BsonDocument>.Filter.Eq("_id", couponId))
                .FirstOrDefaultAsync();
            if (coupon is null)
                return summary;

            var couponDiscount = await ObtenerDescuentoCuponAsync(coupon, summary.Subtotal);
            summary.Descuentos += couponDiscount;
            summary.Total = Math.Max(0, Math.Round(summary.Subtotal - summary.Descuentos, 2));
            return summary;
        }

        private Task<decimal> ObtenerDescuentoCuponAsync(BsonDocument coupon, decimal subtotal)
        {
            var value = GetDecimal(coupon, "Valor", "Descuento", "descuento");
            var type = GetString(coupon, "TipoDescuento", "tipoDescuento");
            if (type.Contains("PORCENTAJE", StringComparison.OrdinalIgnoreCase)
                || type.Contains("PERCENT", StringComparison.OrdinalIgnoreCase)
                || type == "%")
                value = subtotal * value / 100m;
            else if (type.Length == 0 && value <= 1m)
                value *= subtotal;

            return Task.FromResult(Math.Round(Math.Clamp(value, 0m, subtotal), 2));
        }

        private async Task<ServicioDemo?> ObtenerServicioAsync(string idServicio)
        {
            var idText = idServicio.StartsWith("SRV-", StringComparison.OrdinalIgnoreCase)
                ? idServicio[4..]
                : idServicio;
            if (!int.TryParse(idText, out var serviceId))
                return Servicios.TryGetValue(idServicio, out var demoService) ? demoService : null;

            var service = await _mongo.Collection<ServicioAdicional>("serviciosAdicionales")
                .Find(s => s.IdServicio == serviceId)
                .FirstOrDefaultAsync();
            if (service is not null)
            {
                return new ServicioDemo
                {
                    IdServicio = $"SRV-{service.IdServicio}",
                    Nombre = service.Nombre,
                    Precio = service.Precio
                };
            }

            return Servicios.TryGetValue($"SRV-{serviceId}", out var fallback)
                ? fallback
                : null;
        }

        private async Task EliminarCarroAsync(string idCarro)
        {
            if (!int.TryParse(idCarro, out var carroId))
                return;

            var lineas = _mongo.Collection<ItemCarro>("itemsCarro");
            var itemIds = await lineas.Find(i => i.IdCarro == carroId)
                .Project(i => i.IdItem)
                .ToListAsync();
            if (itemIds.Count > 0)
            {
                await _mongo.Collection<ItemServicio>("itemsServicio")
                    .DeleteManyAsync(s => itemIds.Contains(s.IdItem));
                await lineas.DeleteManyAsync(i => i.IdCarro == carroId);
            }

            await _mongo.Collection<Carro>("carros").DeleteOneAsync(c => c.IdCarro == carroId);
        }

        private static decimal GetDecimal(BsonDocument document, params string[] fieldNames)
        {
            var value = GetValue(document, fieldNames);
            return value.IsBsonNull
                ? 0m
                : decimal.TryParse(value.ToString(), System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out var amount) ? amount : 0m;
        }

        private static FilterDefinition<BsonDocument> BuildClientAddressFilter(int clienteId) =>
            Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("IdCliente", clienteId),
                Builders<BsonDocument>.Filter.Eq("clienteId", clienteId),
                Builders<BsonDocument>.Filter.Eq("idCliente", clienteId),
                Builders<BsonDocument>.Filter.Eq("id_cliente", clienteId));

        private static FilterDefinition<BsonDocument> BuildAddressIdFilter(int direccionId) =>
            Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("_id", direccionId),
                Builders<BsonDocument>.Filter.Eq("IdDireccion", direccionId),
                Builders<BsonDocument>.Filter.Eq("idDireccion", direccionId),
                Builders<BsonDocument>.Filter.Eq("id_direccion", direccionId));

        private static void AddAddressUpdate(
            ICollection<UpdateDefinition<BsonDocument>> updates,
            BsonDocument document,
            string value,
            params string[] fieldNames)
        {
            updates.Add(Builders<BsonDocument>.Update.Set(GetFieldName(document, fieldNames), value));
        }

        private static string GetFieldName(BsonDocument document, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                if (document.Contains(fieldName))
                    return fieldName;
            }

            return fieldNames.Length > 0
                ? fieldNames[0]
                : throw new ArgumentException("Debe indicar al menos un nombre de campo.", nameof(fieldNames));
        }

        private static int GetInt32(BsonDocument document, params string[] fieldNames)
        {
            var value = GetValue(document, fieldNames);
            return value.IsBsonNull ? 0 : value.ToInt32();
        }

        private static BsonValue GetValue(BsonDocument document, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                if (document.TryGetValue(fieldName, out var value) && !value.IsBsonNull)
                    return value;
            }

            return BsonNull.Value;
        }

        private static string GetString(BsonDocument document, params string[] fieldNames)
        {
            var value = GetValue(document, fieldNames);
            return value.IsBsonNull ? string.Empty : value.ToString() ?? string.Empty;
        }

        private static DireccionClienteDto MapDireccion(BsonDocument direccion)
        {
            return new DireccionClienteDto
            {
                IdDireccion = GetString(direccion, "_id", "IdDireccion", "idDireccion", "id_direccion"),
                IdCliente = GetString(direccion, "IdCliente", "clienteId", "idCliente", "id_cliente"),
                Nombre = GetString(direccion, "Nombre", "tipo"),
                Direccion = GetString(direccion, "Direccion1", "direccion"),
                Referencia = GetString(direccion, "Referencia", "referencia"),
                Departamento = GetString(direccion, "DepartamentoNombre", "departamento"),
                Provincia = GetString(direccion, "ProvinciaNombre", "provincia"),
                Distrito = GetString(direccion, "DistritoNombre", "distrito"),
                Principal = !GetValue(direccion, "Principal", "principal").IsBsonNull
                    && GetValue(direccion, "Principal", "principal").ToBoolean()
            };
        }

        private static ResumenCarroDto CalcularResumen(CarroDemo carro, string idZona)
        {
            var subtotal = 0m;
            var descuentos = 0m;
            var cantidadTotal = 0;
            var itemsPorVendedor = new Dictionary<string, List<ItemCarroResumenDto>>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in carro.Items.Values)
            {
                if (!Productos.TryGetValue(item.Sku, out var producto)
                    && !carro.ProductosCatalogo.TryGetValue(item.Sku, out producto))
                    continue;

                var bruto = producto.Precio * item.Cantidad;
                var dto = item.Cantidad >= 5 ? bruto * 0.10m : 0m;
                var servicios = item.Servicios
                    .Select(s => carro.ServiciosCatalogo.TryGetValue(s, out var servicio)
                        ? servicio
                        : Servicios.TryGetValue(s, out var fallback) ? fallback : null)
                    .Where(s => s is not null)
                    .Select(s => new ServicioCarroDto
                    {
                        IdServicio = int.TryParse(s!.IdServicio.Replace("SRV-", string.Empty), out var id) ? id : 0,
                        Nombre = s.Nombre,
                        Precio = s.Precio
                    })
                    .ToList();

                var subtotalItem = bruto + servicios.Sum(s => s.Precio);
                subtotal += subtotalItem;
                descuentos += dto;
                cantidadTotal += item.Cantidad;

                var itemResumen = new ItemCarroResumenDto
                {
                    Sku = item.Sku,
                    Nombre = producto.Nombre,
                    Imagen = producto.Imagen,
                    Vendedor = producto.Vendedor,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = subtotalItem,
                    Servicios = servicios
                };

                if (!itemsPorVendedor.TryGetValue(producto.Vendedor, out var lista))
                {
                    lista = new List<ItemCarroResumenDto>();
                    itemsPorVendedor[producto.Vendedor] = lista;
                }

                lista.Add(itemResumen);
            }

            return new ResumenCarroDto
            {
                ItemsPorVendedor = itemsPorVendedor,
                CantidadTotal = cantidadTotal,
                Subtotal = Math.Round(subtotal, 2),
                Descuentos = Math.Round(descuentos, 2),
                Total = Math.Round(subtotal - descuentos, 2)
            };
        }

        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password + ":sodimac")));
        }

        private static bool VerifyPassword(string inputPassword, string storedHash)
        {
            return HashPassword(inputPassword) == storedHash;
        }

        private static bool ValidarPassword(string password)
        {
            return password.Length >= 8 && password.Any(char.IsDigit) && password.Any(char.IsLetter);
        }

        private sealed class CarroDemo
        {
            public string IdCarro { get; set; } = string.Empty;
            public Dictionary<string, ItemCarroDemo> Items { get; set; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, ProductoDemo> ProductosCatalogo { get; set; } = new(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, ServicioDemo> ServiciosCatalogo { get; set; } = new(StringComparer.OrdinalIgnoreCase);
            public int? IdCliente { get; set; }
            public int? IdCupon { get; set; }
            public bool EsInvitado { get; set; } = true;
        }

        private sealed class ItemCarroDemo
        {
            public string Sku { get; set; } = string.Empty;
            public int Cantidad { get; set; }
            public List<string> Servicios { get; set; } = new();
        }

        private sealed class ProductoDemo
        {
            public string Sku { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string Vendedor { get; set; } = string.Empty;
            public decimal Precio { get; set; }
            public Dictionary<string, int> StockPorZona { get; set; } = new(StringComparer.OrdinalIgnoreCase);
            public string Imagen { get; set; } = string.Empty;
        }

        private sealed class ServicioDemo
        {
            public string IdServicio { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public decimal Precio { get; set; }
        }

        private sealed class CuponDemo
        {
            public string CodigoCupon { get; set; } = string.Empty;
            public decimal Porcentaje { get; set; }
            public string? MedioPago { get; set; }
            public decimal MontoMinimo { get; set; }
        }
    }
}
