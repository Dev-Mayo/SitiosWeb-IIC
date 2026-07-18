using System;
using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Responses
{
    [DataContract]
    public class ExperienciaLaboralResponse
    {
        [DataMember]
        public string Empresa { get; set; }

        [DataMember]
        public string Puesto { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public DateTime FechaFin { get; set; }
    }
}