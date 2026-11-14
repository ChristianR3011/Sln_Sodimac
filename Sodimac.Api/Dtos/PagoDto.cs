namespace Sodimac.Api.Dtos
{

    // CASO 55
    public class RegistrarComprobanteRequest
    {
        public int IdCarro { get; set; }
        public string Tipo { get; set; } = default!; // BOLETA o FACTURA
        public string NroDocumentoRuc { get; set; } = default!;
        public string? RazonSocial { get; set; }
        public string? DireccionFiscal { get; set; }
    }

    public class RegistrarComprobanteResponse
    {
        public int IdComprobante { get; set; }
        public string MensajeConfirmacion { get; set; } = default!;
    }

    // CASO 56
    public class ObtenerResumenRequest
    {
        public int IdCarro { get; set; }
    }

    public class ItemResumenDto
    {
        public string Sku { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Imagen { get; set; } = default!;
        public string Vendedor { get; set; } = default!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public List<ServicioAdicionalDto> Servicios { get; set; } = new();
    }

    public class ServicioAdicionalDto
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = default!;
        public decimal Precio { get; set; }
    }

    public class ResumenCompraDto
    {
        public List<ItemResumenDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Descuentos { get; set; }
        public decimal Total { get; set; }
    }

    // CASO 57
    public class ListaMediosPagoRequest
    {
        public int IdCarro { get; set; }
    }

    public class MedioPagoDto
    {
        public int IdMedio { get; set; }
        public string Nombre { get; set; } = default!;
        public List<int> CuotasDisponibles { get; set; } = new();
    }

    public class ListaMediosPagoResponse
    {
        public List<MedioPagoDto> MediosDePago { get; set; } = new();
    }

    // CASO 58
    public class PagarConTarjetaRequest
    {
        public int IdCarro { get; set; }
        public int IdMedio { get; set; }
        public string TokenTarjeta { get; set; } = default!;
        public string TipoTarjeta { get; set; } = default!; // CREDITO, DEBITO, CMR
        public string UltimosDigitos { get; set; } = default!;
        public int Cuotas { get; set; }
        public decimal Monto { get; set; }
    }

    public class PagarConTarjetaResponse
    {
        public int IdTransaccion { get; set; }
        public string Estado { get; set; } = default!;
        public string? CodigoAutorizacion { get; set; }
    }

    // CASO 59
    public class GenerarCodigoPagoEfectivoRequest
    {
        public int IdCarro { get; set; }
        public int IdMedio { get; set; }
        public decimal Monto { get; set; }
    }

    public class GenerarCodigoPagoEfectivoResponse
    {
        public int IdTransaccion { get; set; }
        public string CodigoPago { get; set; } = default!;
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}