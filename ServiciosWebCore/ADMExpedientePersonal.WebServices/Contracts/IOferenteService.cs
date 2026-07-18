using ADMExpedientePersonal.WebServices.Entities.Entities.Requests;
using ADMExpedientePersonal.WebServices.Entities.Entities.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ADMExpedientePersonal.WebServices.Contracts
{
    [ServiceContract]
    public interface IOferenteService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "obtenerPorPuesto",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        OferentesPorPuestoResponse ObtenerOferentesPorPuesto(OferentesPorPuestoRequest request);
    }
}