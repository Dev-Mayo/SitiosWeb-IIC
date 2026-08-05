using ADMExpedientePersonal.Api.Oferentes.Models;

namespace ADMExpedientePersonal.Api.Oferentes.Models
{
    public class OferentesPorPuestoResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }
        public List<OferenteDto> Oferentes { get; set; }
    }
}
