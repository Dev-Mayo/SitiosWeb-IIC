using System;
using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Responses
{
    [DataContract]
    public class ConcursoResponse
    {
        [DataMember]
        public int CodigoConcurso { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public DateTime FechaFin { get; set; }

        [DataMember]
        public string Estado { get; set; }
    }
}