using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Responses
{
    [DataContract]
    public class EmpleadoResponse
    {
        [DataMember]
        public bool Exito { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public int EmpleadoId { get; set; }
    }
}

//con esta y la de request tenemos las clases que van a viajar entre PHP → WCF y WCF → PHP.