using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.Entities
{
    public class Entrevista
    {
        public int EntrevistaId { get; set; }
        public string OferenteIdentificacion { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaEntrevista { get; set; }
        public string Estado { get; set; } // Pendiente, Realizada, Eliminada
    }
}
