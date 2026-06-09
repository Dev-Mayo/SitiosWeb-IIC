using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities
{
    public class Puesto
    {
        public int PuestoId { get; set; }
        public string Nombre { get; set; }
        public decimal Salario { get; set; }
        public int? JefePuestoId { get; set; }
    }
}
