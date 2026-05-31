using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.Entities
{
    public class AccionPersonal
    {
        public int accion_id { get; set; }
        public int codigo_accion { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }

        public int empleado_id { get; set; }
        public int jefatura_id { get; set; }

        public string nombre_empleado { get; set; }
        public string nombre_jefatura { get; set; }
    }
}
