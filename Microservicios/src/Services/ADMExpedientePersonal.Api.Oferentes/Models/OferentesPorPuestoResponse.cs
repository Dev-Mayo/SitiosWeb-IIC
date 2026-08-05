using ADMExpedientePersonal.Api.Oferentes.Models;

namespace ADMExpedientePersonal.Api.Oferentes.Models
{
    public class OferentesPorPuestoResponse
    {
        public bool Success { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public IEnumerable<OferenteDto> Oferentes { get; set; } = [];

        public int Pagina { get; set; }

        public int TamanoPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas { get; set; }
        public int CodigoEstado { get; set; }
    }
}
