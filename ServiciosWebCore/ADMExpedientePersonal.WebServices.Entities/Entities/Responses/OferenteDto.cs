using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Entities.Entities.Responses
{
    [DataContract]
    public class OferenteDto
    {
        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }
    }
}
