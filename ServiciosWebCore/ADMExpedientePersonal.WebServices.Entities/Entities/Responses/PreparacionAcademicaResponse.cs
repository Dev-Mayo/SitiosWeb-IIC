using System;
using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Responses
{
    [DataContract]
    public class PreparacionAcademicaResponse
    {
        [DataMember]
        public string CodigoInstitucion { get; set; }

        [DataMember]
        public string Titulo { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public DateTime FechaFin { get; set; }
    }
}