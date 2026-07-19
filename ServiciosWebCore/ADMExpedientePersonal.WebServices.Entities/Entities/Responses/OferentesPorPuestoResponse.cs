using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Entities.Entities.Responses
{
    [DataContract]
    public class OferentesPorPuestoResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public List<OferenteDto> Oferentes { get; set; }
    }
}
