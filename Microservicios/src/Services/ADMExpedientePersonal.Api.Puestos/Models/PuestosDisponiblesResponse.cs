namespace ADMExpedientePersonal.Api.Puestos.Models
{
    public class PuestosDisponiblesResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }

        public List<PuestoDto> Puestos { get; set; } = new();

        public int Pagina { get; set; }

        public int TamanoPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas { get; set; }
    }
}