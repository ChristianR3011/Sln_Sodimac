using Sodimac.Api.Dtos;
using Sodimac.Api.Services.Interfaces;
using Sodimac.Api.Tools;

namespace Sodimac.Api.Services.Implementations
{
    public class ClienteCarroService : IClienteCarroService
    {
        private static readonly HashSet<string> ZonasCobertura = new(StringComparer.OrdinalIgnoreCase)
        {
            "Lima",
            "Callao"
        };

        private static readonly Dictionary<string, ClienteDemo> Clientes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["1"] = new ClienteDemo
            {
                IdCliente = "1",
                TipoDocumento = "DNI",
                NumeroDocumento = "12345678",
                Nombres = "Ana",
                Apellidos = "Pérez",
                Correo = "ana@demo.com",
                Celular = "999999999",
                Password = HashPassword("pass1234")
            }
        };

        private static readonly Dictionary<string, DireccionDemo> Direcciones = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, CarroDemo> Carros = new(StringComparer.OrdinalIgnoreCase);
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

        private static readonly Dictionary<string, string> Sesiones = new(StringComparer.OrdinalIgnoreCase);

        public ApiResponse<ClienteDetalleDto> GetCliente(string idCliente)
        {
            if (!Clientes.TryGetValue(idCliente, out var cliente))
                return ApiResponse<ClienteDetalleDto>.Fail("Cliente no encontrado");

            return ApiResponse<ClienteDetalleDto>.Ok(new ClienteDetalleDto
            {
                TipoDocumento = cliente.TipoDocumento,
                NumeroDocumento = cliente.NumeroDocumento,
                Nombres = cliente.Nombres,
                Apellidos = cliente.Apellidos,
                Correo = cliente.Correo,
                Celular = cliente.Celular
            });
        }

        public ApiResponse<ActualizarClienteResponse> ActualizarCliente(string idCliente, ActualizarClienteRequest request)
        {
            if (!Clientes.TryGetValue(idCliente, out var cliente))
                return ApiResponse<ActualizarClienteResponse>.Fail("Cliente no encontrado");

            if (request.Nombres is not null) cliente.Nombres = request.Nombres;
            if (request.Apellidos is not null) cliente.Apellidos = request.Apellidos;
            if (request.Celular is not null) cliente.Celular = request.Celular;

            if (!string.IsNullOrWhiteSpace(request.NuevaContrasena))
            {
                if (string.IsNullOrWhiteSpace(request.ContrasenaActual) || !VerifyPassword(request.ContrasenaActual, cliente.Password))
                    return ApiResponse<ActualizarClienteResponse>.Fail("La contraseña actual es incorrecta");

                if (!ValidarPassword(request.NuevaContrasena))
                    return ApiResponse<ActualizarClienteResponse>.Fail("La nueva contraseña no cumple los requisitos de seguridad");

                cliente.Password = HashPassword(request.NuevaContrasena);
            }

            return ApiResponse<ActualizarClienteResponse>.Ok(new ActualizarClienteResponse
            {
                Mensaje = "Datos actualizados correctamente"
            });
        }

        public ApiResponse<List<DireccionClienteDto>> GetDirecciones(string idCliente)
        {
            var lista = Direcciones.Values
                .Where(d => d.IdCliente == idCliente)
                .Select(d => MapDireccion(d))
                .OrderByDescending(d => d.Principal)
                .ToList();

            return ApiResponse<List<DireccionClienteDto>>.Ok(lista);
        }

        public ApiResponse<DireccionClienteDto> RegistrarDireccion(string idCliente, RegistrarDireccionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Direccion) || string.IsNullOrWhiteSpace(request.Departamento) || string.IsNullOrWhiteSpace(request.Provincia) || string.IsNullOrWhiteSpace(request.Distrito))
                return ApiResponse<DireccionClienteDto>.Fail("Faltan datos obligatorios de la dirección");

            if (!ZonasCobertura.Contains(request.Distrito))
                return ApiResponse<DireccionClienteDto>.Fail("El distrito no tiene cobertura de despacho");

            var yaTiene = Direcciones.Values.Any(d => d.IdCliente == idCliente);
            var idDireccion = Guid.NewGuid().ToString("N");
            var direccion = new DireccionDemo
            {
                IdDireccion = idDireccion,
                IdCliente = idCliente,
                Nombre = request.Nombre ?? "Casa",
                Direccion = request.Direccion,
                Referencia = request.Referencia,
                Departamento = request.Departamento,
                Provincia = request.Provincia,
                Distrito = request.Distrito,
                Principal = !yaTiene
            };

            Direcciones[idDireccion] = direccion;
            return ApiResponse<DireccionClienteDto>.Ok(MapDireccion(direccion));
        }

        public ApiResponse<DireccionClienteDto> ActualizarDireccion(string idCliente, string idDireccion, ActualizarDireccionRequest request)
        {
            if (!Direcciones.TryGetValue(idDireccion, out var direccion) || direccion.IdCliente != idCliente)
                return ApiResponse<DireccionClienteDto>.Fail("Dirección no encontrada");

            if (!string.IsNullOrWhiteSpace(request.Distrito) && !ZonasCobertura.Contains(request.Distrito))
                return ApiResponse<DireccionClienteDto>.Fail("El distrito no tiene cobertura de despacho");

            if (!string.IsNullOrWhiteSpace(request.Nombre)) direccion.Nombre = request.Nombre;
            if (!string.IsNullOrWhiteSpace(request.Direccion)) direccion.Direccion = request.Direccion;
            if (request.Referencia is not null) direccion.Referencia = request.Referencia;
            if (!string.IsNullOrWhiteSpace(request.Departamento)) direccion.Departamento = request.Departamento;
            if (!string.IsNullOrWhiteSpace(request.Provincia)) direccion.Provincia = request.Provincia;
            if (!string.IsNullOrWhiteSpace(request.Distrito)) direccion.Distrito = request.Distrito;

            if (request.Principal == true)
            {
                foreach (var otra in Direcciones.Values.Where(d => d.IdCliente == idCliente))
                    otra.Principal = false;

                direccion.Principal = true;
            }
            else if (request.Principal == false)
            {
                direccion.Principal = false;
            }

            return ApiResponse<DireccionClienteDto>.Ok(MapDireccion(direccion));
        }

        public ApiResponse<bool> EliminarDireccion(string idCliente, string idDireccion)
        {
            if (!Direcciones.TryGetValue(idDireccion, out var direccion) || direccion.IdCliente != idCliente)
                return ApiResponse<bool>.Fail("Dirección no encontrada");

            Direcciones.Remove(idDireccion);
            return ApiResponse<bool>.Ok(true, "Dirección eliminada correctamente");
        }

        public ApiResponse<ProductoCarroResponse> AgregarProducto(string idCarro, ProductoCarroRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku) || !Productos.TryGetValue(request.Sku, out var producto))
                return ApiResponse<ProductoCarroResponse>.Fail("Producto no encontrado");

            if (request.Cantidad < 1 || request.Cantidad > 10)
                return ApiResponse<ProductoCarroResponse>.Fail("Cantidad inválida");

            if (!Carros.TryGetValue(idCarro, out var carro))
            {
                carro = new CarroDemo { IdCarro = idCarro, Items = new Dictionary<string, ItemCarroDemo>(StringComparer.OrdinalIgnoreCase) };
                Carros[idCarro] = carro;
            }

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

            return ApiResponse<ProductoCarroResponse>.Ok(new ProductoCarroResponse
            {
                IdCarro = idCarro,
                CantidadTotal = carro.Items.Values.Sum(i => i.Cantidad)
            });
        }

        public ApiResponse<ResumenCarroDto> GetCarro(string idCarro, string? idZona = "Lima")
        {
            if (!Carros.TryGetValue(idCarro, out var carro))
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            var resumen = CalcularResumen(carro, idZona ?? "Lima");
            return ApiResponse<ResumenCarroDto>.Ok(resumen);
        }

        public ApiResponse<ResumenCarroDto> ActualizarCantidad(string idCarro, string sku, ActualizarCantidadItemRequest request)
        {
            if (!Carros.TryGetValue(idCarro, out var carro))
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

            return ApiResponse<ResumenCarroDto>.Ok(CalcularResumen(carro, "Lima"));
        }

        public ApiResponse<ResumenCarroDto> EliminarProducto(string idCarro, string sku)
        {
            if (!Carros.TryGetValue(idCarro, out var carro))
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            if (!carro.Items.Remove(sku))
                return ApiResponse<ResumenCarroDto>.Fail("El producto no está en el carro");

            return ApiResponse<ResumenCarroDto>.Ok(CalcularResumen(carro, "Lima"));
        }

        public ApiResponse<ResumenCarroDto> AgregarServicio(string idCarro, string sku, AgregarServicioRequest request)
        {
            if (!Carros.TryGetValue(idCarro, out var carro))
                return ApiResponse<ResumenCarroDto>.Fail("Carro no encontrado");

            if (!carro.Items.TryGetValue(sku, out var item))
                return ApiResponse<ResumenCarroDto>.Fail("El producto no está en el carro");

            if (!Servicios.ContainsKey(request.IdServicio))
                return ApiResponse<ResumenCarroDto>.Fail("Servicio no encontrado");

            if (!item.Servicios.Contains(request.IdServicio))
                item.Servicios.Add(request.IdServicio);

            return ApiResponse<ResumenCarroDto>.Ok(CalcularResumen(carro, "Lima"));
        }

        public ApiResponse<CuponResponse> AplicarCupon(string idCarro, CuponRequest request)
        {
            if (!Carros.TryGetValue(idCarro, out var carro))
                return ApiResponse<CuponResponse>.Fail("Carro no encontrado");

            if (!Cupones.TryGetValue(request.CodigoCupon.Trim(), out var cupon))
                return ApiResponse<CuponResponse>.Fail("Cupón inexistente o vencido");

            if (!string.IsNullOrWhiteSpace(cupon.MedioPago) && !string.Equals(cupon.MedioPago, request.MedioPago, StringComparison.OrdinalIgnoreCase))
                return ApiResponse<CuponResponse>.Fail("El cupón no aplica con el medio de pago seleccionado");

            var resumen = CalcularResumen(carro, "Lima");
            if (resumen.Subtotal < cupon.MontoMinimo)
                return ApiResponse<CuponResponse>.Fail("No cumple el monto mínimo");

            var descuento = Math.Round(resumen.Subtotal * cupon.Porcentaje, 2);
            return ApiResponse<CuponResponse>.Ok(new CuponResponse
            {
                DescuentoCupon = descuento,
                Total = Math.Round(resumen.Total - descuento, 2)
            });
        }

        public ApiResponse<FusionCarroResponse> FusionarCarro(string idCliente, FusionCarroRequest request)
        {
            if (!Carros.TryGetValue(request.IdCarroInvitado, out var invitado))
                return ApiResponse<FusionCarroResponse>.Fail("Carro no encontrado");

            var idCarroCliente = $"cliente-{idCliente}";
            if (!Carros.TryGetValue(idCarroCliente, out var destino))
            {
                destino = new CarroDemo { IdCarro = idCarroCliente, Items = new Dictionary<string, ItemCarroDemo>(StringComparer.OrdinalIgnoreCase) };
                Carros[idCarroCliente] = destino;
            }

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

            Carros.Remove(request.IdCarroInvitado);
            return ApiResponse<FusionCarroResponse>.Ok(new FusionCarroResponse { IdCarro = idCarroCliente });
        }

        public bool TieneSesionValida(string? authorizationHeader, string idCliente)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return true;

            var token = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authorizationHeader[7..].Trim()
                : authorizationHeader.Trim();

            if (string.IsNullOrWhiteSpace(token))
                return true;

            if (!Sesiones.TryGetValue(token, out var clienteId))
                return false;

            return string.Equals(clienteId, idCliente, StringComparison.OrdinalIgnoreCase);
        }

        public ApiResponse<string> CrearSesion(string idCliente)
        {
            if (!Clientes.ContainsKey(idCliente))
                return ApiResponse<string>.Fail("Cliente no encontrado");

            var token = Guid.NewGuid().ToString("N");
            Sesiones[token] = idCliente;
            return ApiResponse<string>.Ok(token);
        }

        private static DireccionClienteDto MapDireccion(DireccionDemo direccion)
        {
            return new DireccionClienteDto
            {
                IdDireccion = direccion.IdDireccion,
                IdCliente = direccion.IdCliente,
                Nombre = direccion.Nombre,
                Direccion = direccion.Direccion,
                Referencia = direccion.Referencia,
                Departamento = direccion.Departamento,
                Provincia = direccion.Provincia,
                Distrito = direccion.Distrito,
                Principal = direccion.Principal
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
                if (!Productos.TryGetValue(item.Sku, out var producto))
                    continue;

                var bruto = producto.Precio * item.Cantidad;
                var dto = item.Cantidad >= 5 ? bruto * 0.10m : 0m;
                var servicios = item.Servicios
                    .Select(s => Servicios.TryGetValue(s, out var servicio) ? servicio : null)
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

        private sealed class ClienteDemo
        {
            public string IdCliente { get; set; } = string.Empty;
            public string TipoDocumento { get; set; } = string.Empty;
            public string NumeroDocumento { get; set; } = string.Empty;
            public string Nombres { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string Celular { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        private sealed class DireccionDemo
        {
            public string IdDireccion { get; set; } = string.Empty;
            public string IdCliente { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string Direccion { get; set; } = string.Empty;
            public string? Referencia { get; set; }
            public string Departamento { get; set; } = string.Empty;
            public string Provincia { get; set; } = string.Empty;
            public string Distrito { get; set; } = string.Empty;
            public bool Principal { get; set; }
        }

        private sealed class CarroDemo
        {
            public string IdCarro { get; set; } = string.Empty;
            public Dictionary<string, ItemCarroDemo> Items { get; set; } = new(StringComparer.OrdinalIgnoreCase);
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
