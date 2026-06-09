using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.Entities
{
    public class Puesto
    {
        public int puesto_id { get; set; }
        public string nombre { get; set; }
        public decimal salario { get; set; }
        public int? jefe_puesto_id { get; set; }
    }
}