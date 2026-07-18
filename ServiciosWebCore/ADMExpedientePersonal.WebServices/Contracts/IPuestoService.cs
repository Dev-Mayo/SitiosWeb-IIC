using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace ADMExpedientePersonal.WebServices.Contracts
{
    [ServiceContract]
    public interface IPuestoService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "listarDisponibles",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        PuestosDisponiblesResponse ListarPuestosDisponibles();
    }
}