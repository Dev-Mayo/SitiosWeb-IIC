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


        [OperationContract]
        int ProbarConexionEMP();
    }


}
//Este es el contrato WCF por eso que va aquí en el COntracts del proyecto, NO en el proyecto Services, porque este es el que expone el servicio y el otro es el que implementa la lógica de negocio.