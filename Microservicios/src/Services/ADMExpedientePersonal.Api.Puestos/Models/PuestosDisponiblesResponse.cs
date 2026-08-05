using ADMExpedientePersonal.Api.Puestos.Models;

namespace ADMExpedientePersonal.Api.Puestos.Models
{
    public class PuestosDisponiblesResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }
        public List<PuestoDto> Puestos { get; set; }
    }
}
