using System.Runtime.Serialization;

namespace ADMExpedientePersonal.WebServices.Entities.Requests
{
    [DataContract]
    public class AutenticacionRequest
    {
        [DataMember]
        public string Usuario { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}