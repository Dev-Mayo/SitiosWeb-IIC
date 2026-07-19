using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Responses
{
    [DataContract]
    public class DetalleOferenteResponse
    {
        [DataMember]
        public bool Exito { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string TipoIdentificacion { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public DateTime FechaNacimiento { get; set; }

        [DataMember]
        public bool Contratado { get; set; }

        [DataMember]
        public List<string> Correos { get; set; }

        [DataMember]
        public List<string> Telefonos { get; set; }

        [DataMember]
        public List<PreparacionAcademicaResponse> PreparacionAcademica { get; set; }

        [DataMember]
        public List<ExperienciaLaboralResponse> ExperienciaLaboral { get; set; }

        [DataMember]
        public List<ConcursoResponse> Concursos { get; set; }
    }
}