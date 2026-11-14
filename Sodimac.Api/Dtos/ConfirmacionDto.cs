namespace Sodimac.Api.Dtos
{

    // CASO 60
    public class RegistrarOrdenRequest
    {
        public int IdCarro { get; set; }
        public int IdComprador { get; set; }
        public int IdTransaccion { get; set; }
    }

    public class RegistrarOrdenResponse
    {
        public string NroOrden { get; set; } = default!;
        public string Estado { get; set; } = default!;
        public decimal Total { get; set; }
        public int PuntosAcumulados { get; set; }
    }

    // CASO 61
    public class EnviarConfirmacionRequest
    {
        public string NroOrden { get; set; } = default!;
        public string Correo { get; set; } = default!;
    }

    public class EnviarConfirmacionResponse
    {
        public int IdNotificacion { get; set; }
        public string Mensaje { get; set; } = default!;
    }

    // CASO 62
    public class EmitirComprobanteRequest
    {
        public string NroOrden { get; set; } = default!;
    }

    public class EmitirComprobanteResponse
    {
        public int IdComprobante { get; set; }
        public string Tipo { get; set; } = default!;
        public string Serie { get; set; } = default!;
        public string Numero { get; set; } = default!;
        public string ArchivoPdf { get; set; } = default!;
        public DateTime FechaEmision { get; set; }
    }
}