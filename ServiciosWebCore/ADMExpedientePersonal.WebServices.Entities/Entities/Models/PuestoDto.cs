using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.WebServices.Entities.Entities.Models
{
    [DataContract]
    public class PuestoDto
    {
        [DataMember] public int PuestoId { get; set; }
        [DataMember] public string Nombre { get; set; }
        [DataMember] public decimal Salario { get; set; }
        [DataMember] public string Jefe { get; set; }
        [DataMember] public bool Disponible { get; set; }
    }
}
