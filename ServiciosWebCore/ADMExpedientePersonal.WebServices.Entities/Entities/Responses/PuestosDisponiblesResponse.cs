using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ADMExpedientePersonal.WebServices.Entities.Entities.Models;

namespace ADMExpedientePersonal.WebServices.Entities.Entities.Responses
{
    [DataContract]
    public class PuestosDisponiblesResponse
    {
        [DataMember] public bool Success { get; set; }
        [DataMember] public string Mensaje { get; set; }
        [DataMember] public List<PuestoDto> Puestos { get; set; }
    }
}
