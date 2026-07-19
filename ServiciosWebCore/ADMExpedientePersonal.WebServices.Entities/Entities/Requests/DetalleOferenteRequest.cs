using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Requests
{
    [DataContract]
    public class DetalleOferenteRequest
    {
        [DataMember]
        public string Identificacion { get; set; }
    }
}
