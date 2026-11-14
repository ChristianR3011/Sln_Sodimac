namespace Sodimac.Api.Dtos
{
    // CASO 51
    public class ListaFechasHorariosRequest
    {
        public int IdCarro { get; set; }
        public int IdDireccion { get; set; }
        public int IdTipoEntrega { get; set; }
    }

    public class RangoHorarioDto
    {
        public int IdHorario { get; set; }
        public string RangoHorario { get; set; } = default!;
        public decimal Costo { get; set; }
        public int CapacidadDisponible { get; set; }
    }

    public class FechaDespachoDto
    {
        public DateTime Fecha { get; set; }
        public List<RangoHorarioDto> RangosHorarios { get; set; } = new();
    }

    public class ListaFechasHorariosResponse
    {
        public List<FechaDespachoDto> FechasDisponibles { get; set; } = new();
    }

    // CASO 52
    public class SeleccionarDespachoRequest
    {
        public int IdCarro { get; set; }
        public int IdDireccion { get; set; }
        public int IdHorario { get; set; }
    }

    public class SeleccionarDespachoResponse
    {
        public int IdEntrega { get; set; }
        public decimal CostoEnvio { get; set; }
        public DateTime FechaEstimada { get; set; }
    }

    // CASO 53
    public class ListaTiendasRetiroRequest
    {
        public int IdCarro { get; set; }
        public int IdZona { get; set; }
    }

    public class TiendaRetiroDto
    {
        public int IdTienda { get; set; }
        public string Nombre { get; set; } = default!;
        public string Tipo { get; set; } = default!;
        public string Direccion { get; set; } = default!;
        public string? Horario { get; set; }
        public string TiempoRetiro { get; set; } = default!;
        public bool TieneStock { get; set; }
    }

    public class ListaTiendasRetiroResponse
    {
        public List<TiendaRetiroDto> Tiendas { get; set; } = new();
    }

    // CASO 54
    public class SeleccionarRetiroTiendaRequest
    {
        public int IdCarro { get; set; }
        public int IdTienda { get; set; }
        public string NombreRetira { get; set; } = default!;
        public string NroDocRetira { get; set; } = default!;
    }

    public class SeleccionarRetiroTiendaResponse
    {
        public int IdEntrega { get; set; }
        public DateTime FechaEstimadaRetiro { get; set; }
    }
}