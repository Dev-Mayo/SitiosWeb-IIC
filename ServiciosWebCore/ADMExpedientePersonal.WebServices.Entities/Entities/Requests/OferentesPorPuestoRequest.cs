using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Entities.Entities.Requests
{
    [DataContract]
    public class OferentesPorPuestoRequest
    {
        [DataMember]
        public int CodigoPuesto { get; set; }

        [DataMember]
        public string Usuario { get; set; }
    }
}
