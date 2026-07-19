using System.ServiceModel;
using System.ServiceModel.Web;
using ADMExpedientePersonal.WebServices.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Responses;

namespace ADMExpedientePersonal.WebServices.Contracts
{
    [ServiceContract]
    public interface IDetalleOferenteService
    {
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "/obtener-detalle",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare
        )]
        DetalleOferenteResponse ObtenerDetalleOferente(
            DetalleOferenteRequest request
        );
    }
}