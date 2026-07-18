using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Contracts;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;

// Alias para evitar conflicto de nombres
using DetalleOferenteBusinessService =
    ADMExpedientePersonal.WebServices.Services.DetalleOferenteService;

namespace ADMExpedientePersonal.WebServices
{
    public class DetalleOferenteService : IDetalleOferenteService
    {
        private readonly DetalleOferenteBusinessService _service;

        public DetalleOferenteService()
        {
            var repository =
                RepositoryFactory.CrearDetalleOferenteRepository();

            _service =
                new DetalleOferenteBusinessService(repository);
        }

        public DetalleOferenteResponse ObtenerDetalleOferente(
            DetalleOferenteRequest request)
        {
            return _service.ObtenerDetalleOferente(request);
        }
    }
}