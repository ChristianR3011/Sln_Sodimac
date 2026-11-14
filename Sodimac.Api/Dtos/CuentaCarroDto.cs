namespace Sodimac.Api.Dtos
{
    public class ClienteDetalleDto
    {
        public string TipoDocumento { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
    }

    public class ActualizarClienteRequest
    {
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Celular { get; set; }
        public string? ContrasenaActual { get; set; }
        public string? NuevaContrasena { get; set; }
    }

    public class ActualizarClienteResponse
    {
        public string Mensaje { get; set; } = string.Empty;
    }

    public class DireccionClienteDto
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

    public class RegistrarDireccionRequest
    {
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Referencia { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
    }

    public class ActualizarDireccionRequest
    {
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Referencia { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
        public bool? Principal { get; set; }
    }

    public class ProductoCarroRequest
    {
        public string? Sku { get; set; }
        public int Cantidad { get; set; }
        public string IdZona { get; set; } = "Lima";
    }

    public class ProductoCarroResponse
    {
        public string IdCarro { get; set; } = string.Empty;
        public int CantidadTotal { get; set; }
    }

    public class ServicioCarroDto
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }

    public class ItemCarroResumenDto
    {
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Imagen { get; set; }
        public string Vendedor { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public List<ServicioCarroDto> Servicios { get; set; } = new();
    }

    public class ResumenCarroDto
    {
        public Dictionary<string, List<ItemCarroResumenDto>> ItemsPorVendedor { get; set; } = new();
        public int CantidadTotal { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuentos { get; set; }
        public decimal Total { get; set; }
    }

    public class ActualizarCantidadItemRequest
    {
        public int Cantidad { get; set; }
    }

    public class AgregarServicioRequest
    {
        public string IdServicio { get; set; } = string.Empty;
    }

    public class CuponRequest
    {
        public string CodigoCupon { get; set; } = string.Empty;
        public string? MedioPago { get; set; }
    }

    public class CuponResponse
    {
        public decimal DescuentoCupon { get; set; }
        public decimal Total { get; set; }
    }

    public class FusionCarroRequest
    {
        public string IdCarroInvitado { get; set; } = string.Empty;
    }

    public class FusionCarroResponse
    {
        public string IdCarro { get; set; } = string.Empty;
    }
}
