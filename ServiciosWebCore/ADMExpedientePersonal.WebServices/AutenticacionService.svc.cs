using ADMExpedientePersonal.WebServices.Common;
using ADMExpedientePersonal.WebServices.Contracts;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Services;

namespace ADMExpedientePersonal.WebServices
{
    public class AutenticacionServiceHost : IAutenticacionService
    {
        private readonly AutenticacionService _service;

        public AutenticacionServiceHost()
        {
            _service = ServiceFactory.CrearAutenticacionService();
        }

        public AutenticacionResponse Autenticar(AutenticacionRequest request)
        {
            return _service.Autenticar(request);
        }
    }
}