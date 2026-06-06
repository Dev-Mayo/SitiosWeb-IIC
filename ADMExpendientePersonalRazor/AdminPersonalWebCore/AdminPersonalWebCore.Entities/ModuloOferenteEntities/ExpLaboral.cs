using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Entities.ModuloOferenteEntities
{
    public class ExpLaboral
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string OferenteId { get; set; }
        public string Puesto { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

    }
}
