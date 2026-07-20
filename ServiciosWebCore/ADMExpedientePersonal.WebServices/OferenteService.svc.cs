using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Contracts;
using ADMExpedientePersonal.WebServices.Entities.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using ADMExpedientePersonal.WebServices.Services;

namespace ADMExpedientePersonal.WebServices
{
    public class OferenteServiceHost : IOferenteService
    {
        private readonly OferenteService _service;

        public OferenteServiceHost()
        {
            var oferenteRepository =
                RepositoryFactory.CrearOferenteRepository();

            var bitacoraRepository =
                RepositoryFactory.CrearBitacoraRepository();

            _service = new OferenteService(
                oferenteRepository,
                bitacoraRepository
            );
        }

        public OferentesPorPuestoResponse ObtenerOferentesPorPuesto(
            OferentesPorPuestoRequest request)
        {
            return _service.ObtenerOferentesPorPuesto(request);
        }
    }
}