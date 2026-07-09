using System.ServiceModel;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using ADMExpedientePersonal.WebServices.Entities.Requests;

namespace ADMExpedientePersonal.WebServices.Contracts
{
    [ServiceContract]
    public interface IEmpleadoService
    {
        [OperationContract]
        EmpleadoResponse RegistrarEmpleado(EmpleadoRequest request);
    }
}