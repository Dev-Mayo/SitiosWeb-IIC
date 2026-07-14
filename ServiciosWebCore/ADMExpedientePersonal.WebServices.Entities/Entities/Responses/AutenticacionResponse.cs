using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Responses
{
    [DataContract]
    public class AutenticacionResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public int IdUsuario { get; set; }
    }
}