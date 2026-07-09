using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Requests
{
    [DataContract]
    public class EmpleadoRequest
    {
        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string TipoIdentificacion { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public DateTime FechaNacimiento { get; set; }

        [DataMember]
        public int PuestoId { get; set; }

        [DataMember]
        public List<string> Correos { get; set; }

        [DataMember]
        public List<string> Telefonos { get; set; }
    }
}