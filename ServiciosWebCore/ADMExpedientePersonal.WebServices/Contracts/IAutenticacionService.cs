using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ADMExpedientePersonal.WebServices.Contracts
{
    [ServiceContract]
    public interface IAutenticacionService
    {
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/autenticar",
            BodyStyle = WebMessageBodyStyle.Bare
        )]
        AutenticacionResponse Autenticar(AutenticacionRequest request);
    }
}