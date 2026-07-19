using System.ServiceModel;
using System.ServiceModel.Web;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;

namespace ADMExpedientePersonal.WebServices.Contracts
{
    [ServiceContract]
    public interface IEmpleadoService
    {
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "/registrar-empleado",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare
        )]
        EmpleadoResponse RegistrarEmpleado(
            EmpleadoRequest request
        );

        [OperationContract]
        [WebGet(
            UriTemplate = "/probar-conexion",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare
        )]
        int ProbarConexionEMP();
    }
}
//Este es el contrato WCF por eso que va aquí en el COntracts del proyecto, NO en el proyecto Services, porque este es el que expone el servicio y el otro es el que implementa la lógica de negocio.